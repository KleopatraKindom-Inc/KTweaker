using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using KleopatraKindomTweaker.Services;

namespace KleopatraKindomTweaker.Pages;

public class CleanupTargetRow
{
    public required string Key { get; init; }
    public required string Path { get; init; }
    public bool IsSelected { get; set; } = true;
    public string Label { get; set; } = "";
    public string SizeLabel { get; set; } = "";
    public CleanupTarget Target => new(Key, Path);
}

public partial class CleanupPage : Page
{
    private readonly CleanupService _service = new();
    private List<CleanupTargetRow> _rows = new();
    private bool _busy;

    public CleanupPage()
    {
        InitializeComponent();
        _rows = _service.GetTargets().Select(t => new CleanupTargetRow { Key = t.Key, Path = t.Path }).ToList();
        ApplyLocalizedText();
        TargetsControl.ItemsSource = _rows;
        Loaded += (_, _) => LocalizationService.Instance.PropertyChanged += Loc_Changed;
        Unloaded += (_, _) => LocalizationService.Instance.PropertyChanged -= Loc_Changed;
    }

    private void Loc_Changed(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        ApplyLocalizedText();
        RefreshRows();
    }

    private void ApplyLocalizedText()
    {
        var loc = LocalizationService.Instance;
        foreach (var row in _rows)
        {
            row.Label = loc[row.Key];
            if (row.SizeLabel == "") row.SizeLabel = loc["Cleanup_NotScanned"];
        }
    }

    private async void Scan_Click(object sender, RoutedEventArgs e)
    {
        if (_busy) return;
        SetBusy(true);
        var loc = LocalizationService.Instance;
        var progress = new Progress<(int Percent, string Message)>(p =>
        {
            ProgressBar.Value = p.Percent;
            ResultText.Text = p.Message;
        });
        try
        {
            await Task.Run(() =>
            {
                for (int i = 0; i < _rows.Count; i++)
                {
                    var row = _rows[i];
                    long bytes = _service.ScanSizeBytes(row.Target, count =>
                    {
                        int percent = Math.Min(99, (int)(((double)i + count / (count + 250.0)) / _rows.Count * 100));
                        ((IProgress<(int Percent, string Message)>)progress).Report((percent, loc.Format("Cleanup_Progress", percent, loc["Cleanup_Scanning"], count)));
                    });
                    Dispatcher.Invoke(() => row.SizeLabel = FormatSize(bytes));
                    int done = (i + 1) * 100 / _rows.Count;
                    ((IProgress<(int Percent, string Message)>)progress).Report((done, loc.Format("Cleanup_Progress", done, loc["Cleanup_ScanItemDone"], i + 1)));
                }
            });
            ProgressBar.Value = 100;
            ResultText.Text = loc.Format("Cleanup_Progress", 100, loc["Cleanup_ScanComplete"], _rows.Count);
        }
        catch (Exception ex)
        {
            ResultText.Text = loc.Format("Cleanup_OperationError", ex.Message);
        }
        finally
        {
            RefreshRows();
            SetBusy(false);
        }
    }

    private async void Clean_Click(object sender, RoutedEventArgs e)
    {
        if (_busy) return;
        var loc = LocalizationService.Instance;
        var selected = _rows.Where(r => r.IsSelected).ToList();
        if (selected.Count == 0)
        {
            ResultText.Text = loc["Cleanup_NothingSelected"];
            return;
        }
        var confirm = MessageBox.Show(string.Format(loc["Cleanup_ConfirmBody"], selected.Count), loc["Cleanup_ConfirmTitle"], MessageBoxButton.YesNo, MessageBoxImage.Warning);
        if (confirm != MessageBoxResult.Yes) return;

        SetBusy(true);
        long totalFreed = 0;
        int totalFiles = 0;
        int errorCount = 0;
        var progress = new Progress<(int Percent, string Message)>(p =>
        {
            ProgressBar.Value = p.Percent;
            ResultText.Text = p.Message;
        });
        try
        {
            await Task.Run(() =>
            {
                for (int i = 0; i < selected.Count; i++)
                {
                    var row = selected[i];
                    var result = _service.Clean(row.Target, count =>
                    {
                        int percent = Math.Min(99, (int)(((double)i + count / (count + 250.0)) / selected.Count * 100));
                        ((IProgress<(int Percent, string Message)>)progress).Report((percent, loc.Format("Cleanup_Progress", percent, loc["Cleanup_Cleaning"], count)));
                    });
                    totalFreed += result.BytesFreed;
                    totalFiles += result.FilesDeleted;
                    errorCount += result.Errors.Count;
                    Dispatcher.Invoke(() => row.SizeLabel = FormatSize(0));
                    int done = (i + 1) * 100 / selected.Count;
                    ((IProgress<(int Percent, string Message)>)progress).Report((done, loc.Format("Cleanup_Progress", done, loc["Cleanup_CleanItemDone"], i + 1)));
                }
            });
            ProgressBar.Value = 100;
            ResultText.Text = errorCount == 0
                ? loc.Format("CleanupResultOk", totalFiles, FormatSize(totalFreed))
                : loc.Format("CleanupResultPartial", totalFiles, FormatSize(totalFreed), errorCount);
        }
        catch (Exception ex)
        {
            ResultText.Text = loc.Format("Cleanup_OperationError", ex.Message);
        }
        finally
        {
            RefreshRows();
            SetBusy(false);
        }
    }

    private void SetBusy(bool busy)
    {
        _busy = busy;
        ProgressBar.Visibility = busy ? Visibility.Visible : Visibility.Visible;
        ScanButton.IsEnabled = !busy;
        CleanButton.IsEnabled = !busy;
        ProgressBar.Value = 0;
    }

    private void RefreshRows()
    {
        TargetsControl.ItemsSource = null;
        TargetsControl.ItemsSource = _rows;
    }

    private static string FormatSize(long bytes)
    {
        var loc = LocalizationService.Instance;
        double mb = bytes / 1024.0 / 1024.0;
        return mb >= 1024 ? $"{mb / 1024:0.0} {loc["UnitGb"]}" : $"{mb:0.0} {loc["UnitMb"]}";
    }
}
