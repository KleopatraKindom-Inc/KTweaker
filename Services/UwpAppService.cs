using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace KleopatraKindomTweaker.Services;

public record UwpAppInfo(string Key, string PackagePattern, string DisplayNameKey);

public class UwpAppService
{
    /// <summary>
    /// Common pre-installed UWP apps people usually want gone. PackagePattern is matched as
    /// a case-insensitive substring against package names (wildcarded in PowerShell).
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

    // ------------------------------------------------------------------
    // PowerShell plumbing
    // ------------------------------------------------------------------

    /// <summary>
    /// Full path to Windows PowerShell 5.1 (the Appx cmdlets live there). Using the absolute
    /// path avoids picking up some other "powershell" that happens to be earlier on PATH.
    /// </summary>
    internal static string FindPowerShell()
    {
        string path = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.System),
            "WindowsPowerShell", "v1.0", "powershell.exe");
        return File.Exists(path) ? path : "powershell.exe";
    }

    /// <summary>
    /// Runs a script through -EncodedCommand (Base64 UTF-16), which sidesteps every
    /// command-line quoting problem. Returns (-1, "", message) if PowerShell can't be
    /// started or doesn't finish within the timeout.
    /// </summary>
    internal static (int ExitCode, string Output, string Error) RunPowerShell(string script, int timeoutMs)
    {
        try
        {
            // UTF-8 output so non-English Windows error messages don't turn into mojibake.
            string full = "[Console]::OutputEncoding = [System.Text.Encoding]::UTF8\n" +
                          "$ProgressPreference = 'SilentlyContinue'\n" + script;
            string encoded = Convert.ToBase64String(Encoding.Unicode.GetBytes(full));

            var psi = new ProcessStartInfo
            {
                FileName = FindPowerShell(),
                Arguments = "-NoProfile -NonInteractive -ExecutionPolicy Bypass -EncodedCommand " + encoded,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                StandardOutputEncoding = Encoding.UTF8,
                StandardErrorEncoding = Encoding.UTF8,
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            // If this app was started from a PowerShell 7 console, the inherited PSModulePath
            // points at PS7 modules and Windows PowerShell can fail to load the Appx module.
            psi.Environment["PSModulePath"] = Environment.ExpandEnvironmentVariables(
                @"%ProgramFiles%\WindowsPowerShell\Modules;%SystemRoot%\system32\WindowsPowerShell\v1.0\Modules");

            using var proc = Process.Start(psi);
            if (proc is null) return (-1, "", "Could not start PowerShell.");

            proc.StandardInput.Close(); // nothing to feed it; avoids any wait on stdin

            var outTask = proc.StandardOutput.ReadToEndAsync();
            var errTask = proc.StandardError.ReadToEndAsync();

            if (!proc.WaitForExit(timeoutMs))
            {
                try { proc.Kill(entireProcessTree: true); } catch { /* already gone */ }
                return (-1, "", "PowerShell timed out.");
            }
            proc.WaitForExit(); // let the async readers drain

            return (proc.ExitCode, outTask.GetAwaiter().GetResult(), errTask.GetAwaiter().GetResult());
        }
        catch (Exception ex)
        {
            return (-1, "", ex.Message);
        }
    }

    private static string Quote(string pattern) => pattern.Replace("'", "''");

    internal static string BuildListScript() => """
        Get-AppxPackage |
            Where-Object { -not $_.NonRemovable -and -not $_.IsFramework } |
            ForEach-Object { $_.Name }
        exit 0
        """;

    internal static string BuildRemoveScript(string pattern) => $$"""
        $ErrorActionPreference = 'Continue'
        $pattern = '*{{Quote(pattern)}}*'
        $pkgs = @(Get-AppxPackage -AllUsers -Name $pattern | Where-Object { -not $_.NonRemovable -and -not $_.IsFramework })
        if ($pkgs.Count -eq 0) { Write-Output 'RESULT:NOTFOUND'; exit 0 }
        $errors = @()
        foreach ($p in $pkgs) {
            try {
                Remove-AppxPackage -Package $p.PackageFullName -AllUsers -ErrorAction Stop
            } catch {
                # Some builds refuse the all-users removal; fall back to the current user only.
                try {
                    Remove-AppxPackage -Package $p.PackageFullName -ErrorAction Stop
                } catch {
                    $errors += ($p.Name + ': ' + $_.Exception.Message)
                }
            }
        }
        $left = @(Get-AppxPackage -Name $pattern | Where-Object { -not $_.NonRemovable -and -not $_.IsFramework }).Count
        Write-Output ('RESULT:LEFT=' + $left)
        if ($errors.Count -gt 0) { Write-Output ('ERRORS:' + (($errors -join ' | ') -replace '\s+', ' ')) }
        exit 0
        """;

    internal static string BuildInstallScript(string pattern) => $$"""
        $ErrorActionPreference = 'Continue'
        $pattern = '*{{Quote(pattern)}}*'
        $pkgs = @(Get-AppxPackage -AllUsers -Name $pattern | Where-Object { -not $_.IsFramework })
        if ($pkgs.Count -eq 0) { Write-Output 'RESULT:NOTFOUND'; exit 0 }
        $errors = @()
        foreach ($p in $pkgs) {
            $ok = $false
            if ($p.InstallLocation) {
                $manifest = Join-Path $p.InstallLocation 'AppXManifest.xml'
                if (Test-Path $manifest) {
                    try {
                        Add-AppxPackage -DisableDevelopmentMode -Register $manifest -ErrorAction Stop
                        $ok = $true
                    } catch {
                        $errors += ($p.Name + ': ' + $_.Exception.Message)
                    }
                }
            }
            if (-not $ok -and $p.PackageFamilyName) {
                try {
                    Add-AppxPackage -RegisterByFamilyName -MainPackage $p.PackageFamilyName -ErrorAction Stop
                } catch {
                    $errors += ($p.Name + ': ' + $_.Exception.Message)
                }
            }
        }
        $now = @(Get-AppxPackage -Name $pattern | Where-Object { -not $_.IsFramework }).Count
        Write-Output ('RESULT:NOW=' + $now)
        if ($errors.Count -gt 0) { Write-Output ('ERRORS:' + (($errors -join ' | ') -replace '\s+', ' ')) }
        exit 0
        """;

    // ------------------------------------------------------------------
    // Output parsing helpers
    // ------------------------------------------------------------------

    private static int? ReadMarker(string output, string marker)
    {
        foreach (var raw in output.Split('\n'))
        {
            var line = raw.Trim();
            if (line.StartsWith(marker, StringComparison.Ordinal) &&
                int.TryParse(line.AsSpan(marker.Length), out int value))
                return value;
        }
        return null;
    }

    private static string ReadErrors(string output)
    {
        foreach (var raw in output.Split('\n'))
        {
            var line = raw.Trim();
            if (line.StartsWith("ERRORS:", StringComparison.Ordinal))
                return line.Substring("ERRORS:".Length).Trim();
        }
        return "";
    }

    private static string Shorten(string text, int max = 180)
    {
        text = (text ?? "").Trim().Replace("\r", " ").Replace("\n", " ");
        return text.Length <= max ? text : text.Substring(0, max) + "…";
    }

    private static string DescribeFailure(string output, string error, int exitCode)
    {
        string details = ReadErrors(output);
        if (details.Length == 0) details = error;
        if (string.IsNullOrWhiteSpace(details)) details = $"PowerShell exit code {exitCode}";
        return Shorten(details);
    }

    // ------------------------------------------------------------------
    // Public API used by UwpAppsPage
    // ------------------------------------------------------------------

    /// <summary>
    /// Names of the removable, non-framework packages installed for the current user.
    /// Throws InvalidOperationException (with a readable message) if the list can't be read,
    /// so the page can say so instead of silently showing every app as "not installed".
    /// </summary>
    public HashSet<string> GetInstalledPackageNames()
    {
        var (code, output, error) = RunPowerShell(BuildListScript(), 60_000);
        if (code != 0)
            throw new InvalidOperationException(DescribeFailure(output, error, code));

        var result = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var line in output.Split('\n'))
        {
            var name = line.Trim();
            if (name.Length > 0) result.Add(name);
        }
        return result;
    }

    public bool IsInstalled(UwpAppInfo app, HashSet<string> installedNames) =>
        installedNames.Any(n => n.Contains(app.PackagePattern, StringComparison.OrdinalIgnoreCase));

    /// <summary>
    /// Removes the app. Success is judged by what is actually left installed afterwards,
    /// not by PowerShell's exit code (which is non-zero if any single package refuses).
    /// </summary>
    public (bool Success, string Error) Remove(UwpAppInfo app)
    {
        var (code, output, error) = RunPowerShell(BuildRemoveScript(app.PackagePattern), 180_000);
        if (code == -1) return (false, Shorten(error));

        if (output.Contains("RESULT:NOTFOUND", StringComparison.Ordinal))
            return (true, ""); // nothing to remove — already gone

        if (ReadMarker(output, "RESULT:LEFT=") == 0)
            return (true, "");

        return (false, DescribeFailure(output, error, code));
    }

    /// <summary>
    /// Re-installs a removed app by re-registering the package files that Windows still has
    /// (staged / provisioned), no internet needed. Returns (false, "NOT_PROVISIONED") when no
    /// such package exists on this PC, so the page can send the person to the Store instead.
    /// </summary>
    public (bool Success, string Error) Install(UwpAppInfo app)
    {
        var (code, output, error) = RunPowerShell(BuildInstallScript(app.PackagePattern), 180_000);
        if (code == -1) return (false, Shorten(error));

        if (output.Contains("RESULT:NOTFOUND", StringComparison.Ordinal))
            return (false, "NOT_PROVISIONED");

        if (ReadMarker(output, "RESULT:NOW=") > 0)
            return (true, "");

        return (false, DescribeFailure(output, error, code));
    }

    /// <summary>Opens the app's Microsoft Store page as a fallback when re-registration isn't possible.</summary>
    public void OpenStorePage(UwpAppInfo app)
    {
        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = $"ms-windows-store://search/?query={Uri.EscapeDataString(app.PackagePattern)}",
                UseShellExecute = true,
            });
        }
        catch { /* best-effort — if the Store isn't available, there's nothing else to try */ }
    }
}
