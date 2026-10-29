using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using KleopatraKindomTweaker.Models;
using KleopatraKindomTweaker.Services;

namespace KleopatraKindomTweaker.Pages;

public class TweakCategoryGroup
{
    public required string Category { get; init; }
    public required List<TweakItem> Items { get; init; }
}

public partial class TweaksPage : Page
{
    private readonly RegistryTweakService _service = new();
    private List<TweakItem> _allTweaks = new();

    public TweaksPage()
    {
        InitializeComponent();
        LoadTweaks();
    }

    private void LoadTweaks()
    {
        var loc = LocalizationService.Instance;
        // Security tweaks have their own dedicated page (with status + a guided "protect now"
        // flow), so they're left out here to avoid the same switch appearing in two places.
        var all = _service.BuildCatalog()
            .Where(t => t.Category != loc["CatSecurity"])
            .ToList();

        foreach (var tweak in all)
        {
            try
            {
                tweak.IsEnabled = _service.ReadState(tweak);
            }
            catch (Exception ex)
            {
                // Missing permissions or key on this Windows edition — leave unchecked, note it.
                StatusText.Text = LocalizationService.Instance.Format("TweakReadError", tweak.Title, ex.Message);
            }
        }

        _allTweaks = all.ToList();

        var groups = all
            .GroupBy(t => t.Category)
            .Select(g => new TweakCategoryGroup { Category = g.Key, Items = g.ToList() })
            .ToList();

        CategoriesControl.ItemsSource = groups;
    }

    /// <summary>
    /// Wired to the switch's Click event, NOT Checked/Unchecked. Checked/Unchecked also fire when
    /// WPF sets the switch from the binding (while the page is being laid out) or when we revert
    /// it from code, which used to re-write registry values on tab open and could re-enter this
    /// handler endlessly after a failed write. Click is raised only by a real user action.
    /// </summary>
    private void Tweak_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not CheckBox { Tag: TweakItem tweak } cb) return;

        bool enable = cb.IsChecked == true;
        if (!TryApply(tweak, enable))
        {
            // Put the switch back. Going through the model updates the CheckBox via the binding,
            // and programmatic changes don't raise Click, so this can't loop.
            tweak.IsEnabled = !enable;
        }
    }

    /// <summary>Applies one tweak; returns false (and shows why) if it couldn't be applied.</summary>
    private bool TryApply(TweakItem tweak, bool enable, bool announce = true)
    {
        var loc = LocalizationService.Instance;
        try
        {
            // Don't touch the registry when it is already in the requested state. Every write to
            // keys like Themes\Personalize makes Windows broadcast a system-wide settings change,
            // which is pointless (and disruptive to open windows) if nothing actually changes.
            bool alreadyThere;
            try { alreadyThere = _service.ReadState(tweak) == enable; }
            catch { alreadyThere = false; }

            if (!alreadyThere)
                _service.ApplyState(tweak, enable);

            if (announce)
            {
                StatusText.Text = tweak.RequiresExplorerRestart
                    ? loc.Format("TweakAppliedRestart", tweak.Title)
                    : loc.Format("TweakApplied", tweak.Title);
            }
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

    private void SetAll(bool enable)
    {
        var loc = LocalizationService.Instance;
        int ok = 0, failed = 0;

        foreach (var tweak in _allTweaks)
        {
            if (TryApply(tweak, enable, announce: false))
            {
                tweak.IsEnabled = enable; // updates the switch through the binding; raises no Click
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

    private void SelectAll_Click(object sender, RoutedEventArgs e) => SetAll(true);

    private void DeselectAll_Click(object sender, RoutedEventArgs e) => SetAll(false);
}
