using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using KleopatraKindomTweaker.Services;

namespace KleopatraKindomTweaker.Pages;

/// <summary>One line of the cleanup list. Notifies the UI so each row can update live.</summary>
public class CleanupTargetRow : INotifyPropertyChanged
{
    public required string Key { get; init; }
    public required string Path { get; init; }

    private bool _isSelected = true;
    public bool IsSelected
    {
        get => _isSelected;
        set { if (_isSelected == value) return; _isSelected = value; Raise(nameof(IsSelected)); }
    }

    private long? _bytes;       // null = not scanned yet
    private bool _scanFailed;

    public void SetSize(long bytes) { _bytes = bytes; _scanFailed = false; Raise(nameof(SizeLabel)); }
    public void SetScanFailed() { _bytes = null; _scanFailed = true; Raise(nameof(SizeLabel)); }

    public string Label => LocalizationService.Instance[Key];

    public string SizeLabel =>
        _scanFailed ? "—"
        : _bytes is long b ? FormatSize(b)
        : LocalizationService.Instance["Cleanup_NotScanned"];

    public CleanupTarget Target => new(Key, Path);

    /// <summary>Re-reads translated text (row name, size units) after a language change.</summary>
    public void RefreshLocalized()
    {
        Raise(nameof(Label));
        Raise(nameof(SizeLabel));
    }

    internal static string FormatSize(long bytes)
    {
        var loc = LocalizationService.Instance;
        double mb = bytes / 1024.0 / 1024.0;
        return mb >= 1024
            ? $"{mb / 1024:0.0} {loc["UnitGb"]}"
            : $"{mb:0.0} {loc["UnitMb"]}";
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    private void Raise(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}

public partial class CleanupPage : Page
{
    private readonly CleanupService _service = new();
    private readonly List<CleanupTargetRow> _rows;
    private bool _busy;

    public CleanupPage()
    {
        InitializeComponent();
        _rows = _service.GetTargets()
            .Select(t => new CleanupTargetRow { Key = t.Key, Path = t.Path })
            .ToList();
        TargetsControl.ItemsSource = _rows;

        Loaded += (_, _) => LocalizationService.Instance.PropertyChanged += Loc_Changed;
        Unloaded += (_, _) => LocalizationService.Instance.PropertyChanged -= Loc_Changed;
    }

    private void Loc_Changed(object? sender, PropertyChangedEventArgs e)
    {
        foreach (var row in _rows) row.RefreshLocalized();
    }

    private void SetBusy(bool busy)
    {
        _busy = busy;
        ScanBtn.IsEnabled = !busy;
        CleanBtn.IsEnabled = !busy;
        BusyBar.Visibility = busy ? Visibility.Visible : Visibility.Collapsed;
    }

    // The folder walks run on a worker thread (Task.Run) so the window stays responsive; each
    // await resumes on the UI thread, which is where the rows and the status line are updated.
    private async void Scan_Click(object sender, RoutedEventArgs e)
    {
        if (_busy) return;
        var loc = LocalizationService.Instance;
        SetBusy(true);
        try
        {
            long total = 0;
            foreach (var row in _rows)
            {
                ResultText.Text = loc.Format("Cleanup_ScanningTarget", row.Label);
                try
                {
                    long bytes = await Task.Run(() => _service.ScanSizeBytes(row.Target));
                    row.SetSize(bytes);
                    total += bytes;
                }
                catch
                {
                    row.SetScanFailed();
                }
            }
            ResultText.Text = loc.Format("Cleanup_ScanDone", CleanupTargetRow.FormatSize(total));
        }
        catch (Exception ex)
        {
            ResultText.Text = loc.Format("Cleanup_Error", ex.Message);
        }
        finally
        {
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

        var confirm = MessageBox.Show(
            string.Format(loc["Cleanup_ConfirmBody"], selected.Count),
            loc["Cleanup_ConfirmTitle"], MessageBoxButton.YesNo, MessageBoxImage.Warning);
        if (confirm != MessageBoxResult.Yes) return;

        SetBusy(true);
        try
        {
            long totalFreed = 0;
            int totalFiles = 0;
            int skipped = 0;

            foreach (var row in selected)
            {
                ResultText.Text = loc.Format("Cleanup_CleaningTarget", row.Label);

                var result = await Task.Run(() => _service.Clean(row.Target));
                totalFreed += result.BytesFreed;
                totalFiles += result.FilesDeleted;
                skipped += result.Skipped;

                // Show what is really left (files in use stay behind) instead of assuming zero.
                long left = await Task.Run(() => _service.ScanSizeBytes(row.Target));
                row.SetSize(left);
            }

            ResultText.Text = skipped == 0
                ? loc.Format("CleanupResultOk", totalFiles, CleanupTargetRow.FormatSize(totalFreed))
                : loc.Format("CleanupResultPartial", totalFiles, CleanupTargetRow.FormatSize(totalFreed), skipped);
        }
        catch (Exception ex)
        {
            ResultText.Text = loc.Format("Cleanup_Error", ex.Message);
        }
        finally
        {
            SetBusy(false);
        }
    }
}
