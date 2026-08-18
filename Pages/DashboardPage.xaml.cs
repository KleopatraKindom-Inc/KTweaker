using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using KleopatraKindomTweaker.Services;

namespace KleopatraKindomTweaker.Pages;

public partial class DashboardPage : Page
{
    private readonly RegistryTweakService _tweakService = new();
    private readonly SystemInfoService _infoService = new();

    // Stable keys (not display text!) of the tweaks applied by the one-click "recommended" button.
    // Using Title text here would break once the title is localized to a different language.
    private static readonly string[] RecommendedTweakKeys =
    {
        "TwTelemetry", "TwAdId", "TwCortana", "TwBingSearch", "TwAnimations",
    };

    public DashboardPage()
    {
        InitializeComponent();
        LoadSummary();
    }

    private void LoadSummary()
    {
        var loc = LocalizationService.Instance;
        try
        {
            var info = _infoService.GetSnapshot();
            SummaryText.Text = string.Join("\n",
                loc.Format("SummaryOs", info.OsName),
                loc.Format("SummaryCpu", info.CpuName, info.CpuCores, info.CpuLogicalProcessors),
                loc.Format("SummaryRam", info.AvailableRamGb, info.TotalRamGb),
                loc.Format("SummaryGpu", info.GpuName));
        }
        catch (Exception ex)
        {
            SummaryText.Text = loc.Format("SummaryError", ex.Message);
        }
    }

    private void QuickOptimize_Click(object sender, RoutedEventArgs e)
    {
        var loc = LocalizationService.Instance;
        var catalog = _tweakService.BuildCatalog();
        int applied = 0, failed = 0;

        foreach (var tweak in catalog.Where(t => RecommendedTweakKeys.Contains(t.Key)))
        {
            try
            {
                _tweakService.ApplyState(tweak, enable: true);
                applied++;
            }
            catch
            {
                failed++;
            }
        }

        QuickStatusText.Text = failed == 0
            ? loc.Format("QuickStatusOk", applied)
            : loc.Format("QuickStatusPartial", applied, failed);
    }
}
