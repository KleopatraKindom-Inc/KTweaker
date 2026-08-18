using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using KleopatraKindomTweaker.Services;

namespace KleopatraKindomTweaker.Pages;

public class UwpAppRow : INotifyPropertyChanged
{
    public required UwpAppInfo App { get; init; }
    public required string DisplayName { get; init; }

    private bool _isInstalled;
    public bool IsInstalled
    {
        get => _isInstalled;
        set { _isInstalled = value; OnChanged(nameof(IsInstalled)); OnChanged(nameof(ActionLabel)); }
    }

    private string _status = "";
    public string Status
    {
        get => _status;
        set { _status = value; OnChanged(nameof(Status)); }
    }

    private bool _isBusy;
    public bool IsBusy
    {
        get => _isBusy;
        set { _isBusy = value; OnChanged(nameof(IsBusy)); OnChanged(nameof(CanAct)); }
    }

    public bool CanAct => !IsBusy;

    // Button label flips between "Remove" and "Install" depending on current state.
    public string ActionLabel => LocalizationService.Instance[IsInstalled ? "RemoveButton" : "InstallButton"];

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}

public partial class UwpAppsPage : Page
{
    private readonly UwpAppService _service = new();
    private ObservableCollection<UwpAppRow> _rows = new();

    public UwpAppsPage()
    {
        InitializeComponent();

        var loc = LocalizationService.Instance;
        _rows = new ObservableCollection<UwpAppRow>(_service.GetCatalog().Select(app => new UwpAppRow
        {
            App = app,
            DisplayName = loc[app.DisplayNameKey],
            Status = loc["UwpCheckingStatus"],
            IsBusy = true,
        }));
        AppsControl.ItemsSource = _rows;

        _ = LoadInstalledStatusAsync();
    }

    private async Task LoadInstalledStatusAsync()
    {
        var loc = LocalizationService.Instance;
        HashSet<string> installed;
        try
        {
            installed = await Task.Run(_service.GetInstalledPackageNames);
        }
        catch
        {
            installed = new HashSet<string>();
        }

        foreach (var row in _rows)
        {
            bool isInstalled = _service.IsInstalled(row.App, installed);
            row.IsInstalled = isInstalled;
            row.Status = loc[isInstalled ? "UwpInstalled" : "UwpNotInstalled"];
            row.IsBusy = false;
        }
    }

    private async void Action_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button { Tag: UwpAppRow row }) return;
        var loc = LocalizationService.Instance;

        row.IsBusy = true;

        if (row.IsInstalled)
        {
            row.Status = loc["UwpRemoving"];
            var (success, error) = await Task.Run(() => _service.Remove(row.App));

            if (success)
            {
                row.IsInstalled = false;
                row.Status = loc["UwpRemoved"];
            }
            else
            {
                row.Status = loc.Format("UwpRemoveError", error);
            }
        }
        else
        {
            row.Status = loc["UwpInstalling"];
            var (success, error) = await Task.Run(() => _service.Install(row.App));

            if (success)
            {
                row.IsInstalled = true;
                row.Status = loc["UwpInstalledNow"];
            }
            else if (error == "NOT_PROVISIONED")
            {
                // The package was fully removed from this machine (not just for this user) —
                // re-registration isn't possible offline, so send the person to the Store instead.
                _service.OpenStorePage(row.App);
                row.Status = loc["UwpOpenedStore"];
            }
            else
            {
                row.Status = loc.Format("UwpInstallError", error);
            }
        }

        row.IsBusy = false;
    }
}
