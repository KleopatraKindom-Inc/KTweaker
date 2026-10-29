using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace KleopatraKindomTweaker.Services;

public record CleanupTarget(string Key, string Path);

/// <param name="Skipped">Files that couldn't be deleted (in use / no permission).</param>
/// <param name="Errors">First few error messages only — the full count is in <paramref name="Skipped"/>.</param>
public record CleanupResult(string Key, long BytesFreed, int FilesDeleted, int Skipped, List<string> Errors);

public class CleanupService
{
    private const int MaxErrorMessagesKept = 50;

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

    public long ScanSizeBytes(CleanupTarget target)
    {
        if (!Directory.Exists(target.Path)) return 0;
        long total = 0;
        foreach (var file in SafeEnumerateFiles(target.Path))
        {
            try { total += new FileInfo(file).Length; } catch { /* locked/inaccessible, skip */ }
        }
        return total;
    }

    public CleanupResult Clean(CleanupTarget target)
    {
        long freed = 0;
        int deleted = 0;
        int skipped = 0;
        var errors = new List<string>();

        if (!Directory.Exists(target.Path))
            return new CleanupResult(target.Key, 0, 0, 0, errors);

        foreach (var file in SafeEnumerateFiles(target.Path))
        {
            try
            {
                var info = new FileInfo(file);
                long size = info.Length;

                // Read-only files can't be deleted until the flag is cleared.
                if ((info.Attributes & FileAttributes.ReadOnly) != 0)
                    info.Attributes &= ~FileAttributes.ReadOnly;

                info.Delete();
                freed += size;
                deleted++;
            }
            catch (Exception ex)
            {
                // File in use / permission denied — expected for some cache files, skip and continue.
                skipped++;
                if (errors.Count < MaxErrorMessagesKept)
                    errors.Add($"{Path.GetFileName(file)}: {ex.Message}");
            }
        }

        return new CleanupResult(target.Key, freed, deleted, skipped, errors);
    }

    /// <summary>
    /// Enumerates files under <paramref name="root"/> without ever throwing. The old
    /// Directory.EnumerateFiles(..., SearchOption.AllDirectories) overload throws
    /// UnauthorizedAccessException from MoveNext() the moment it meets a folder it can't
    /// open (common in Windows\Temp, Prefetch, SoftwareDistribution), which aborted the whole
    /// scan/clean. EnumerationOptions.IgnoreInaccessible skips those folders instead, and
    /// skipping reparse points (junctions/symlinks) avoids loops and leaving the target folder.
    /// </summary>
    private static IEnumerable<string> SafeEnumerateFiles(string root)
    {
        var options = new EnumerationOptions
        {
            RecurseSubdirectories = true,
            IgnoreInaccessible = true,
            AttributesToSkip = FileAttributes.ReparsePoint,
            ReturnSpecialDirectories = false,
        };

        IEnumerator<string> enumerator;
        try
        {
            enumerator = Directory.EnumerateFiles(root, "*", options).GetEnumerator();
        }
        catch
        {
            yield break;
        }

        using (enumerator)
        {
            while (true)
            {
                string current;
                try
                {
                    if (!enumerator.MoveNext()) yield break;
                    current = enumerator.Current;
                }
                catch
                {
                    yield break; // belt and braces: never let enumeration take the caller down
                }
                yield return current;
            }
        }
    }
}
