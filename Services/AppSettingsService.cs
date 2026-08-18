using System;
using System.Collections.Generic;
using System.IO;

namespace KleopatraKindomTweaker.Services;

public record AppSettings(App.Theme Theme, AppLanguage Language);

/// <summary>
/// Persists a handful of user preferences (theme, language) to a small key=value text file
/// in %AppData%\Roaming\KleopatraKindom\TweakerConfig.conf. Kept as plain text (not JSON) so
/// it's trivially readable/editable by hand, matching the ".conf" naming.
/// </summary>
public static class AppSettingsService
{
    private static readonly string FolderPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "KleopatraKindom");

    private static readonly string FilePath = Path.Combine(FolderPath, "TweakerConfig.conf");

    public static AppSettings Load()
    {
        var defaults = new AppSettings(App.Theme.Dark, AppLanguage.Ru);

        if (!File.Exists(FilePath))
            return defaults;

        try
        {
            var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var rawLine in File.ReadAllLines(FilePath))
            {
                var line = rawLine.Trim();
                if (line.Length == 0 || line.StartsWith('#')) continue;

                var idx = line.IndexOf('=');
                if (idx <= 0) continue;

                values[line[..idx].Trim()] = line[(idx + 1)..].Trim();
            }

            var theme = values.TryGetValue("Theme", out var themeStr) &&
                        Enum.TryParse<App.Theme>(themeStr, ignoreCase: true, out var parsedTheme)
                ? parsedTheme : defaults.Theme;

            var language = values.TryGetValue("Language", out var langStr) &&
                           Enum.TryParse<AppLanguage>(langStr, ignoreCase: true, out var parsedLang)
                ? parsedLang : defaults.Language;

            return new AppSettings(theme, language);
        }
        catch
        {
            // Corrupt/unreadable config — fall back to defaults rather than crash startup.
            return defaults;
        }
    }

    public static void Save(AppSettings settings)
    {
        try
        {
            Directory.CreateDirectory(FolderPath);
            var content =
                "# KleopatraKindomTweaker settings — safe to edit by hand.\n" +
                $"Theme={settings.Theme}\n" +
                $"Language={settings.Language}\n";
            File.WriteAllText(FilePath, content);
        }
        catch
        {
            // Best-effort: if AppData isn't writable for some reason, just skip persisting
            // rather than crashing on window close.
        }
    }
}
