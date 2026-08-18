using System;
using System.Windows;
using System.Windows.Controls;
using KleopatraKindomTweaker.Services;

namespace KleopatraKindomTweaker.Pages;

public partial class SystemInfoPage : Page
{
    private SystemInfoSnapshot? _info;

    public SystemInfoPage()
    {
        InitializeComponent();
        Load();

        Loaded += (_, _) => LocalizationService.Instance.PropertyChanged += Loc_Changed;
        Unloaded += (_, _) => LocalizationService.Instance.PropertyChanged -= Loc_Changed;
    }

    private void Loc_Changed(object? sender, System.ComponentModel.PropertyChangedEventArgs e) => Render();

    private void Load()
    {
        var service = new SystemInfoService();
        try
        {
            _info = service.GetSnapshot();
        }
        catch (Exception ex)
        {
            InfoPanel.Children.Clear();
            InfoPanel.Children.Add(Row("Error", ex.Message));
            return;
        }
        Render();
    }

    private void Render()
    {
        if (_info is null) return;
        var loc = LocalizationService.Instance;

        InfoPanel.Children.Clear();
        InfoPanel.Children.Add(Row(loc["Info_Os"], $"{_info.OsName} (build {_info.OsVersion})"));
        InfoPanel.Children.Add(Row(loc["Info_Cpu"], $"{_info.CpuName} — {_info.CpuCores} / {_info.CpuLogicalProcessors}"));
        InfoPanel.Children.Add(Row(loc["Info_Ram"], $"{_info.AvailableRamGb} / {_info.TotalRamGb} GB"));
        InfoPanel.Children.Add(Row(loc["Info_Gpu"], _info.GpuName));
        InfoPanel.Children.Add(Row(loc["Info_Board"], _info.MotherboardModel));

        DisksPanel.Items.Clear();
        foreach (var disk in _info.Disks)
        {
            var used = disk.TotalGb - disk.FreeGb;
            var pct = disk.TotalGb > 0 ? used / disk.TotalGb * 100 : 0;
            DisksPanel.Items.Add(Row(disk.Drive, $"{disk.FreeGb} / {disk.TotalGb} GB ({pct:0}%)"));
        }
    }

    private static UIElement Row(string label, string value)
    {
        var panel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 0, 0, 8) };
        panel.Children.Add(new TextBlock
        {
            Text = label,
            Width = 180,
            FontWeight = FontWeights.SemiBold,
            Foreground = (System.Windows.Media.Brush)Application.Current.Resources["TextSecondaryBrush"]
        });
        panel.Children.Add(new TextBlock
        {
            Text = value,
            TextWrapping = TextWrapping.Wrap,
            Foreground = (System.Windows.Media.Brush)Application.Current.Resources["TextPrimaryBrush"]
        });
        return panel;
    }
}
