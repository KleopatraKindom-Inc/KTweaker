using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace KleopatraKindomTweaker.Services;

public record ProcessRow(int Pid, string Name, double MemoryMb, string Priority, string? WindowTitle);

public class ProcessService
{
    public List<ProcessRow> GetProcesses()
    {
        var rows = new List<ProcessRow>();
        foreach (var p in Process.GetProcesses())
        {
            try
            {
                rows.Add(new ProcessRow(
                    p.Id,
                    p.ProcessName,
                    Math.Round(p.WorkingSet64 / 1024.0 / 1024.0, 1),
                    SafePriority(p),
                    string.IsNullOrWhiteSpace(p.MainWindowTitle) ? null : p.MainWindowTitle));
            }
            catch
            {
                // Some system processes deny access to their properties; skip them.
            }
        }
        return rows.OrderByDescending(r => r.MemoryMb).ToList();
    }

    private static string SafePriority(Process p)
    {
        try { return p.PriorityClass.ToString(); }
        catch { return "N/A"; }
    }

    public void Kill(int pid)
    {
        using var p = Process.GetProcessById(pid);
        p.Kill(entireProcessTree: false);
    }

    public void SetPriority(int pid, ProcessPriorityClass priority)
    {
        using var p = Process.GetProcessById(pid);
        p.PriorityClass = priority;
    }
}
