using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.Win32;

namespace KleopatraKindomTweaker.Models;

/// <summary>
/// Describes one on/off system tweak backed by a registry value.
/// OnValue / OffValue are written to KeyPath\ValueName when toggled.
/// </summary>
public class TweakItem : INotifyPropertyChanged
{
    /// <summary>Stable identifier (e.g. "TwTelemetry"), independent of the localized Title/Description
    /// below — use this for matching tweaks in code (like the Dashboard's "recommended" list) so
    /// switching language doesn't break the match.</summary>
    public required string Key { get; init; }

    /// <summary>Localized display text, resolved once by RegistryTweakService.BuildCatalog() at the
    /// current language. Re-call BuildCatalog() after a language change to get fresh text.</summary>
    public required string Title { get; init; }
    public required string Description { get; init; }
    public required string Category { get; init; }

    public required RegistryHive Hive { get; init; }
    public required string KeyPath { get; init; }
    public required string ValueName { get; init; }
    public required RegistryValueKind ValueKind { get; init; }
    public required object OnValue { get; init; }
    public required object OffValue { get; init; }

    /// <summary>Value considered "default/off" when the registry value doesn't exist yet.</summary>
    public object? MissingMeansValue { get; init; }

    /// <summary>True if this tweak requires a sign-out / Explorer restart to visibly apply.</summary>
    public bool RequiresExplorerRestart { get; init; }

    private bool _isEnabled;
    public bool IsEnabled
    {
        get => _isEnabled;
        set
        {
            if (_isEnabled == value) return;
            _isEnabled = value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
