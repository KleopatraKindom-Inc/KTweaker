using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace KleopatraKindomTweaker.Services;

public record UwpAppInfo(string Key, string PackagePattern, string DisplayNameKey);

public class UwpAppService
{
    /// <summary>
    /// Common pre-installed UWP apps people usually want gone. PackagePattern is matched as
    /// a case-insensitive substring against installed package names (wildcarded in PowerShell).
    /// </summary>
    public List<UwpAppInfo> GetCatalog() => new()
    {
        new("Xbox", "Xbox", "UwpXbox"),
        new("Skype", "SkypeApp", "UwpSkype"),
        new("GetHelp", "GetHelp", "UwpGetHelp"),
        new("3DViewer", "3DViewer", "Uwp3DViewer"),
        new("FeedbackHub", "WindowsFeedbackHub", "UwpFeedbackHub"),
        new("MixedReality", "MixedReality.Portal", "UwpMixedReality"),
        new("OfficeHub", "MicrosoftOfficeHub", "UwpOfficeHub"),
        new("OneNote", "Office.OneNote", "UwpOneNote"),
        new("People", "People", "UwpPeople"),
        new("Maps", "WindowsMaps", "UwpMaps"),
        new("Weather", "BingWeather", "UwpWeather"),
        new("Solitaire", "SolitaireCollection", "UwpSolitaire"),
        new("ZuneMusic", "ZuneMusic", "UwpZuneMusic"),
        new("ZuneVideo", "ZuneVideo", "UwpZuneVideo"),
        new("YourPhone", "YourPhone", "UwpYourPhone"),
        new("Teams", "Teams", "UwpTeams"),
        new("Clipchamp", "Clipchamp", "UwpClipchamp"),
        new("Copilot", "Copilot", "UwpCopilot"),
        new("ToDo", "Todos", "UwpToDo"),
    };

    /// <summary>
    /// One batched PowerShell call returning every installed package name, so we don't spawn
    /// 19 separate processes just to show install status.
    /// </summary>
    public HashSet<string> GetInstalledPackageNames()
    {
        var result = new HashSet<string>(System.StringComparer.OrdinalIgnoreCase);
        try
        {
            var psi = new ProcessStartInfo
            {
                FileName = "powershell.exe",
                Arguments = "-NoProfile -NonInteractive -Command \"Get-AppxPackage | Select-Object -ExpandProperty Name\"",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };
            using var proc = Process.Start(psi);
            if (proc is null) return result;
            string output = proc.StandardOutput.ReadToEnd();
            proc.WaitForExit(15000);

            foreach (var line in output.Split('\n'))
            {
                var name = line.Trim();
                if (name.Length > 0) result.Add(name);
            }
        }
        catch
        {
            // PowerShell unavailable / blocked — status just won't be pre-filled, Remove still works.
        }
        return result;
    }

    public bool IsInstalled(UwpAppInfo app, HashSet<string> installedNames) =>
        installedNames.Any(n => n.Contains(app.PackagePattern, System.StringComparison.OrdinalIgnoreCase));

    /// <summary>Removes an app for the current user (and all users where permitted) via PowerShell.</summary>
    public (bool Success, string Error) Remove(UwpAppInfo app)
    {
        try
        {
            var psi = new ProcessStartInfo
            {
                FileName = "powershell.exe",
                Arguments = "-NoProfile -NonInteractive -Command " +
                    $"\"Get-AppxPackage -AllUsers *{app.PackagePattern}* | Remove-AppxPackage -AllUsers\"",
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };
            using var proc = Process.Start(psi);
            if (proc is null) return (false, "Could not start PowerShell.");

            string error = proc.StandardError.ReadToEnd();
            proc.WaitForExit(30000);

            return proc.ExitCode == 0 || string.IsNullOrWhiteSpace(error)
                ? (true, "")
                : (false, error.Split('\n').FirstOrDefault()?.Trim() ?? error);
        }
        catch (System.Exception ex)
        {
            return (false, ex.Message);
        }
    }

    /// <summary>
    /// Re-installs a removed app. Remove() above only removes the package for existing user
    /// profiles (it never runs Remove-AppxProvisionedPackage), so the app's provisioned
    /// package definition stays on disk and can be re-registered for the current user without
    /// needing internet access or the Microsoft Store. Falls back to opening the Store page
    /// if no provisioned package is found (e.g. the app was never provisioned on this machine).
    /// </summary>
    public (bool Success, string Error) Install(UwpAppInfo app)
    {
        try
        {
            var psi = new ProcessStartInfo
            {
                FileName = "powershell.exe",
                Arguments = "-NoProfile -NonInteractive -Command " +
                    $"\"$p = Get-AppxProvisionedPackage -Online | Where-Object {{ $_.PackageName -like '*{app.PackagePattern}*' }} | Select-Object -First 1; " +
                    "if ($p) { Add-AppxPackage -DisableDevelopmentMode -Register \\\"$($p.InstallLocation)\\AppXManifest.xml\\\" } " +
                    "else { Write-Error 'NOT_PROVISIONED' }\"",
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };
            using var proc = Process.Start(psi);
            if (proc is null) return (false, "Could not start PowerShell.");

            string error = proc.StandardError.ReadToEnd();
            proc.WaitForExit(30000);

            if (proc.ExitCode == 0 && string.IsNullOrWhiteSpace(error))
                return (true, "");

            if (error.Contains("NOT_PROVISIONED"))
                return (false, "NOT_PROVISIONED");

            return (false, error.Split('\n').FirstOrDefault()?.Trim() ?? error);
        }
        catch (System.Exception ex)
        {
            return (false, ex.Message);
        }
    }

    /// <summary>Opens the app's Microsoft Store page as a fallback when re-registration isn't possible.</summary>
    public void OpenStorePage(UwpAppInfo app)
    {
        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = $"ms-windows-store://search/?query={app.PackagePattern}",
                UseShellExecute = true,
            });
        }
        catch { /* best-effort — if the Store isn't available, there's nothing else to try */ }
    }
}
