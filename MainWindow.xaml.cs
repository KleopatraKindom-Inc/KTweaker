using System.Windows;
using System.Windows.Controls;
using KleopatraKindomTweaker.Pages;

namespace KleopatraKindomTweaker;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        // Set after InitializeComponent (not via IsChecked="True" in XAML) so ContentFrame
        // is guaranteed to exist before Nav_Checked fires and tries to use it.
        NavDashboard.IsChecked = true;
    }

    private void Nav_Checked(object sender, RoutedEventArgs e)
    {
        if (sender != NavDashboard && sender != NavTweaks && sender != NavProcesses &&
            sender != NavCleanup && sender != NavUwp && sender != NavSystemInfo && sender != NavSettings)
            return;

        if (ContentFrame is null) return; // guard against firing before the frame exists

        Page page = sender switch
        {
            _ when sender == NavDashboard => new DashboardPage(),
            _ when sender == NavTweaks => new TweaksPage(),
            _ when sender == NavProcesses => new ProcessManagerPage(),
            _ when sender == NavCleanup => new CleanupPage(),
            _ when sender == NavUwp => new UwpAppsPage(),
            _ when sender == NavSystemInfo => new SystemInfoPage(),
            _ when sender == NavSettings => new SettingsPage(),
            _ => new DashboardPage()
        };

        ContentFrame.Navigate(page);
    }

    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) => App.SaveSettings();
}
