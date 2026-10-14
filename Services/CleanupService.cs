using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace KleopatraKindomTweaker.Services;

public record CleanupTarget(string Key, string Path);
public record CleanupResult(string Key, long BytesFreed, int FilesDeleted, List<string> Errors);

public class CleanupService
{
    /// <summary>
    /// Known-safe junk locations. We never touch user documents/desktop — only
    /// caches that Windows/apps regenerate automatically.
    /// </summary>
    public List<CleanupTarget> GetTargets()
    {
        string local = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        string windir = Environment.GetFolderPath(Environment.SpecialFolder.Windows);

        return new List<CleanupTarget>
        {
            new("Clean_UserTemp", Path.GetTempPath()),
            new("Clean_WinTemp", Path.Combine(windir, "Temp")),
            new("Clean_ThumbCache", Path.Combine(local, @"Microsoft\Windows\Explorer")),
            new("Clean_WUDownload", Path.Combine(windir, "SoftwareDistribution", "Download")),
            new("Clean_Prefetch", Path.Combine(windir, "Prefetch")),
        };
    }

    public long ScanSizeBytes(CleanupTarget target, Action<int>? progress = null)
    {
        if (!Directory.Exists(target.Path)) return 0;
        long total = 0;
        int count = 0;
        foreach (var file in SafeEnumerateFiles(target.Path))
        {
            count++;
            if (count % 25 == 0) progress?.Invoke(count);
            try { total += new FileInfo(file).Length; } catch { }
        }
        progress?.Invoke(count);
        return total;
    }

    public CleanupResult Clean(CleanupTarget target, Action<int>? progress = null)
    {
        long freed = 0;
        int deleted = 0;
        var errors = new List<string>();

        if (!Directory.Exists(target.Path))
            return new CleanupResult(target.Key, 0, 0, errors);

        int count = 0;
        foreach (var file in SafeEnumerateFiles(target.Path))
        {
            count++;
            if (count % 25 == 0) progress?.Invoke(count);
            try
            {
                var info = new FileInfo(file);
                long size = info.Length;
                info.Delete();
                freed += size;
                deleted++;
            }
            catch (Exception ex)
            {
                // File in use / permission denied — expected for some cache files, skip and continue.
                errors.Add($"{Path.GetFileName(file)}: {ex.Message}");
            }
        }

        progress?.Invoke(count);
        return new CleanupResult(target.Key, freed, deleted, errors);
    }

    private static IEnumerable<string> SafeEnumerateFiles(string root)
    {
        var pending = new Stack<string>();
        pending.Push(root);
        while (pending.Count > 0)
        {
            string current = pending.Pop();
            string[] files;
            try { files = Directory.GetFiles(current); } catch { continue; }
            foreach (var file in files) yield return file;
            string[] directories;
            try { directories = Directory.GetDirectories(current); } catch { continue; }
            foreach (var directory in directories)
            {
                try
                {
                    var attributes = File.GetAttributes(directory);
                    if ((attributes & FileAttributes.ReparsePoint) == 0) pending.Push(directory);
                }
                catch { }
            }
        }
    }
}
