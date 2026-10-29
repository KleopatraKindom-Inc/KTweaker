using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using KleopatraKindomTweaker.Models;
using KleopatraKindomTweaker.Services;

namespace KleopatraKindomTweaker.Pages;

public partial class SecurityPage : Page
{
    private readonly RegistryTweakService _tweakService = new();
    private readonly SecurityStatusService _statusService = new();
    private List<TweakItem> _tweaks = new();

    public SecurityPage()
    {
        InitializeComponent();
        LoadTweaks();
        _ = LoadDefenderStatusAsync();
    }

    private void LoadTweaks()
    {
        var loc = LocalizationService.Instance;

        _tweaks = _tweakService.BuildCatalog()
            .Where(t => t.Category == loc["CatSecurity"])
            .ToList();

        foreach (var tweak in _tweaks)
        {
            try { tweak.IsEnabled = _tweakService.ReadState(tweak); }
            catch { /* leave unchecked; the per-click handler reports read/write errors */ }
        }

        TweaksControl.ItemsSource = _tweaks;
    }

    private async Task LoadDefenderStatusAsync()
    {
        var loc = LocalizationService.Instance;
        DefenderStatusText.Text = loc["SecurityRefreshing"];
        try
        {
            var status = await Task.Run(_statusService.GetStatus);
            var lines = new List<string>
            {
                loc[status.RealTimeProtectionEnabled ? "SecurityDefenderRealtimeOn" : "SecurityDefenderRealtimeOff"],
                loc[status.AntivirusEnabled ? "SecurityDefenderAvOn" : "SecurityDefenderAvOff"],
            };
            if (status.SignatureAgeDays is int age)
                lines.Add(loc.Format("SecurityDefenderSignatureAge", age));
            DefenderStatusText.Text = string.Join("\n", lines);
        }
        catch (Exception ex)
        {
            // Not an error state worth alarming over - this is expected whenever a third-party
            // antivirus has taken over from Windows Defender.
            DefenderStatusText.Text = loc.Format("SecurityDefenderUnavailable", ex.Message);
        }
    }

    /// <summary>Click-based for the same reason as TweaksPage: Checked/Unchecked also fire from
    /// binding updates and programmatic reverts, which previously caused re-entrant writes.</summary>
    private void Tweak_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not CheckBox { Tag: TweakItem tweak } cb) return;

        bool enable = cb.IsChecked == true;
        if (!TryApply(tweak, enable))
            tweak.IsEnabled = !enable;
    }

    private bool TryApply(TweakItem tweak, bool enable, bool announce = true)
    {
        var loc = LocalizationService.Instance;
        try
        {
            bool alreadyThere;
            try { alreadyThere = _tweakService.ReadState(tweak) == enable; }
            catch { alreadyThere = false; }

            if (!alreadyThere)
                _tweakService.ApplyState(tweak, enable);

            if (announce)
                StatusText.Text = loc.Format("TweakApplied", tweak.Title);
            return true;
        }
        catch (UnauthorizedAccessException)
        {
            StatusText.Text = loc["TweakNoPermission"];
            return false;
        }
        catch (Exception ex)
        {
            StatusText.Text = loc.Format("TweakError", ex.Message);
            return false;
        }
    }

    private void ProtectNow_Click(object sender, RoutedEventArgs e)
    {
        var loc = LocalizationService.Instance;
        int ok = 0, failed = 0;

        foreach (var tweak in _tweaks)
        {
            if (TryApply(tweak, true, announce: false))
            {
                tweak.IsEnabled = true;
                ok++;
            }
            else
            {
                failed++;
            }
        }

        StatusText.Text = failed == 0
            ? loc.Format("QuickStatusOk", ok)
            : loc.Format("QuickStatusPartial", ok, failed);
    }
}
