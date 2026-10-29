using System;
using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace KleopatraKindomTweaker.Services;

public record DefenderStatus(bool AntivirusEnabled, bool RealTimeProtectionEnabled, int? SignatureAgeDays);

/// <summary>
/// Read-only reporting on Windows Defender's current protection state, via the same
/// Get-MpComputerStatus cmdlet Windows Security itself is built on. This never changes
/// anything — it only tells the person what is already true, so they can see whether their
/// protection is actually active.
/// </summary>
public class SecurityStatusService
{
    public DefenderStatus GetStatus()
    {
        var (code, output, error) = RunPowerShell("""
            $s = Get-MpComputerStatus -ErrorAction Stop
            Write-Output ('AV=' + [int]$s.AntivirusEnabled)
            Write-Output ('RT=' + [int]$s.RealTimeProtectionEnabled)
            if ($s.AntivirusSignatureLastUpdated) {
                $age = [math]::Round(((Get-Date) - $s.AntivirusSignatureLastUpdated).TotalDays)
                Write-Output ('AGE=' + $age)
            }
            exit 0
            """, 20_000);

        if (code != 0)
        {
            string detail = string.IsNullOrWhiteSpace(error) ? $"exit code {code}" : error.Split('\n')[0].Trim();
            throw new InvalidOperationException(detail);
        }

        bool av = false, rt = false;
        int? age = null;
        foreach (var raw in output.Split('\n'))
        {
            var line = raw.Trim();
            if (line.StartsWith("AV=", StringComparison.Ordinal)) av = line.EndsWith("1");
            else if (line.StartsWith("RT=", StringComparison.Ordinal)) rt = line.EndsWith("1");
            else if (line.StartsWith("AGE=", StringComparison.Ordinal) &&
                     int.TryParse(line.AsSpan(4), NumberStyles.Integer, CultureInfo.InvariantCulture, out int a))
                age = a;
        }

        return new DefenderStatus(av, rt, age);
    }

    private static (int ExitCode, string Output, string Error) RunPowerShell(string script, int timeoutMs)
    {
        try
        {
            string full = "[Console]::OutputEncoding = [System.Text.Encoding]::UTF8\n" +
                          "$ProgressPreference = 'SilentlyContinue'\n" + script;
            string encoded = Convert.ToBase64String(Encoding.Unicode.GetBytes(full));

            var psi = new ProcessStartInfo
            {
                FileName = UwpAppService.FindPowerShell(),
                Arguments = "-NoProfile -NonInteractive -ExecutionPolicy Bypass -EncodedCommand " + encoded,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                StandardOutputEncoding = Encoding.UTF8,
                StandardErrorEncoding = Encoding.UTF8,
                UseShellExecute = false,
                CreateNoWindow = true,
            };
            psi.Environment["PSModulePath"] = Environment.ExpandEnvironmentVariables(
                @"%ProgramFiles%\WindowsPowerShell\Modules;%SystemRoot%\system32\WindowsPowerShell\v1.0\Modules");

            using var proc = Process.Start(psi);
            if (proc is null) return (-1, "", "Could not start PowerShell.");

            proc.StandardInput.Close();
            var outTask = proc.StandardOutput.ReadToEndAsync();
            var errTask = proc.StandardError.ReadToEndAsync();

            if (!proc.WaitForExit(timeoutMs))
            {
                try { proc.Kill(entireProcessTree: true); } catch { /* already gone */ }
                return (-1, "", "PowerShell timed out.");
            }
            proc.WaitForExit();

            return (proc.ExitCode, outTask.GetAwaiter().GetResult(), errTask.GetAwaiter().GetResult());
        }
        catch (Exception ex)
        {
            return (-1, "", ex.Message);
        }
    }
}
