using System;
using System.Collections.Generic;
using System.Linq;
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

    public CleanupPage()
    {
        InitializeComponent();
        _rows = _service.GetTargets()
            .Select(t => new CleanupTargetRow { Key = t.Key, Path = t.Path })
            .ToList();
        ApplyLocalizedText();
        TargetsControl.ItemsSource = _rows;

        Loaded += (_, _) => LocalizationService.Instance.PropertyChanged += Loc_Changed;
        Unloaded += (_, _) => LocalizationService.Instance.PropertyChanged -= Loc_Changed;
    }

    private void Loc_Changed(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        ApplyLocalizedText();
        TargetsControl.ItemsSource = null;
        TargetsControl.ItemsSource = _rows;
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

    private void Scan_Click(object sender, RoutedEventArgs e)
    {
        var loc = LocalizationService.Instance;
        foreach (var row in _rows)
        {
            try
            {
                long bytes = _service.ScanSizeBytes(row.Target);
                row.SizeLabel = FormatSize(bytes);
            }
            catch (Exception ex)
            {
                row.SizeLabel = "—";
                ResultText.Text = $"{row.Label}: {ex.Message}";
            }
        }
        // Refresh bindings by resetting the ItemsSource.
        TargetsControl.ItemsSource = null;
        TargetsControl.ItemsSource = _rows;
    }

    private void Clean_Click(object sender, RoutedEventArgs e)
    {
        var loc = LocalizationService.Instance;
        var selected = _rows.Where(r => r.IsSelected).ToList();
        if (selected.Count == 0)
        {
            ResultText.Text = loc["Cleanup_NothingSelected"];
            return;
        }

        var confirmBody = string.Format(loc["Cleanup_ConfirmBody"], selected.Count);
        var confirm = MessageBox.Show(
            confirmBody,
            loc["Cleanup_ConfirmTitle"], MessageBoxButton.YesNo, MessageBoxImage.Warning);
        if (confirm != MessageBoxResult.Yes) return;

        long totalFreed = 0;
        int totalFiles = 0;
        int errorCount = 0;

        foreach (var row in selected)
        {
            var result = _service.Clean(row.Target);
            totalFreed += result.BytesFreed;
            totalFiles += result.FilesDeleted;
            errorCount += result.Errors.Count;
            row.SizeLabel = FormatSize(0);
        }

        TargetsControl.ItemsSource = null;
        TargetsControl.ItemsSource = _rows;

        ResultText.Text = errorCount == 0
            ? loc.Format("CleanupResultOk", totalFiles, FormatSize(totalFreed))
            : loc.Format("CleanupResultPartial", totalFiles, FormatSize(totalFreed), errorCount);
    }

    private static string FormatSize(long bytes)
    {
        var loc = LocalizationService.Instance;
        double mb = bytes / 1024.0 / 1024.0;
        return mb >= 1024
            ? $"{mb / 1024:0.0} {loc["UnitGb"]}"
            : $"{mb:0.0} {loc["UnitMb"]}";
    }
}
