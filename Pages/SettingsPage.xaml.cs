using System.Windows;
using System.Windows.Controls;
using KleopatraKindomTweaker.Services;

namespace KleopatraKindomTweaker.Pages;

public partial class SettingsPage : Page
{
    private bool _isLoading = true;

    public SettingsPage()
    {
        InitializeComponent();

        // Reflect whatever language is currently active (e.g. after navigating back
        // to this page) instead of always defaulting the radio buttons to Russian.
        switch (LocalizationService.Instance.Language)
        {
            case AppLanguage.En: LangEnOption.IsChecked = true; break;
            case AppLanguage.Uk: LangUkOption.IsChecked = true; break;
            default: LangRuOption.IsChecked = true; break;
        }
        _isLoading = false;
    }

    private void Dark_Click(object sender, RoutedEventArgs e) => App.SwitchTheme(App.Theme.Dark);
    private void Light_Click(object sender, RoutedEventArgs e) => App.SwitchTheme(App.Theme.Light);

    private void LangRu_Checked(object sender, RoutedEventArgs e)
    {
        if (_isLoading) return;
        LocalizationService.Instance.Language = AppLanguage.Ru;
    }

    private void LangEn_Checked(object sender, RoutedEventArgs e)
    {
        if (_isLoading) return;
        LocalizationService.Instance.Language = AppLanguage.En;
    }

    private void LangUk_Checked(object sender, RoutedEventArgs e)
    {
        if (_isLoading) return;
        LocalizationService.Instance.Language = AppLanguage.Uk;
    }
}
