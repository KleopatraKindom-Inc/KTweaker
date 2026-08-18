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
    private bool _isLoading = true;
    private List<TweakItem> _allTweaks = new();

    public TweaksPage()
    {
        InitializeComponent();
        LoadTweaks();
    }

    private void LoadTweaks()
    {
        var all = _service.BuildCatalog();

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
        _isLoading = false;
    }

    private void Tweak_Toggled(object sender, RoutedEventArgs e)
    {
        if (_isLoading) return;
        if (sender is not CheckBox { Tag: TweakItem tweak } cb) return;

        ApplyOne(tweak, cb.IsChecked == true, cb);
    }

    private void ApplyOne(TweakItem tweak, bool enable, CheckBox? cb = null)
    {
        var loc = LocalizationService.Instance;
        try
        {
            _service.ApplyState(tweak, enable);
            StatusText.Text = tweak.RequiresExplorerRestart
                ? loc.Format("TweakAppliedRestart", tweak.Title)
                : loc.Format("TweakApplied", tweak.Title);
        }
        catch (UnauthorizedAccessException)
        {
            StatusText.Text = loc["TweakNoPermission"];
            if (cb is not null) cb.IsChecked = !enable; // revert visual state
        }
        catch (Exception ex)
        {
            StatusText.Text = loc.Format("TweakError", ex.Message);
            if (cb is not null) cb.IsChecked = !enable;
        }
    }

    private void SelectAll_Click(object sender, RoutedEventArgs e)
    {
        foreach (var tweak in _allTweaks)
            tweak.IsEnabled = true; // bound TwoWay to each CheckBox -> fires Tweak_Toggled per item
    }

    private void DeselectAll_Click(object sender, RoutedEventArgs e)
    {
        foreach (var tweak in _allTweaks)
            tweak.IsEnabled = false;
    }
}
