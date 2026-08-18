using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using KleopatraKindomTweaker.Services;

namespace KleopatraKindomTweaker.Pages;

public partial class ProcessManagerPage : Page
{
    private readonly ProcessService _service = new();

    public ProcessManagerPage()
    {
        InitializeComponent();
        Refresh();
    }

    private void Refresh() => ProcessGrid.ItemsSource = _service.GetProcesses();

    private void Refresh_Click(object sender, RoutedEventArgs e) => Refresh();

    private void Kill_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button { Tag: ProcessRow row }) return;
        var loc = LocalizationService.Instance;

        var confirm = MessageBox.Show(loc.Format("KillConfirmMsg", row.Name, row.Pid),
            loc["KillConfirmTitle"], MessageBoxButton.YesNo, MessageBoxImage.Warning);
        if (confirm != MessageBoxResult.Yes) return;

        try
        {
            _service.Kill(row.Pid);
        }
        catch (System.Exception ex)
        {
            MessageBox.Show(loc.Format("KillErrorMsg", ex.Message), loc["ErrorTitle"],
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
        Refresh();
    }

    private void SetHighPriority_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button { Tag: ProcessRow row }) return;
        var loc = LocalizationService.Instance;

        try
        {
            _service.SetPriority(row.Pid, ProcessPriorityClass.High);
        }
        catch (System.Exception ex)
        {
            MessageBox.Show(loc.Format("PriorityErrorMsg", ex.Message), loc["ErrorTitle"],
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
        Refresh();
    }
}
