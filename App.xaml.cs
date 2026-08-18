using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using KleopatraKindomTweaker.Services;

namespace KleopatraKindomTweaker;

public partial class App : Application
{
    public enum Theme { Dark, Light }

    public static Theme CurrentTheme { get; private set; } = Theme.Dark;

    protected override void OnStartup(StartupEventArgs e)
    {
        // Without these hooks, an exception thrown before the first window shows
        // (e.g. during resource loading or a service constructor) kills the process
        // silently — no window, no error, nothing in Explorer. These hooks make sure
        // we always see *something*.
        DispatcherUnhandledException += (_, args) =>
        {
            LogAndShow(args.Exception, "UI thread");
            args.Handled = true; // keep the app alive so the user can read the message
        };

        AppDomain.CurrentDomain.UnhandledException += (_, args) =>
        {
            if (args.ExceptionObject is Exception ex)
                LogAndShow(ex, "background thread (fatal)");
        };

        // Restore theme + language saved from the last session, before the main window
        // is created (base.OnStartup triggers it via App.xaml's StartupUri).
        var saved = AppSettingsService.Load();
        SwitchTheme(saved.Theme);
        LocalizationService.Instance.Language = saved.Language;

        base.OnStartup(e);
    }

    private static void LogAndShow(Exception ex, string context)
    {
        string logPath = Path.Combine(AppContext.BaseDirectory, "crash.log");
        try
        {
            File.AppendAllText(logPath,
                $"[{DateTime.Now:u}] Unhandled exception on {context}:\n{ex}\n\n");
        }
        catch { /* if we can't even write the log, just show the dialog below */ }

        MessageBox.Show(
            $"Произошла ошибка при запуске ({context}):\n\n{ex.Message}\n\n" +
            $"Подробности сохранены в:\n{logPath}",
            "Kleopatra Kindom Tweaker — ошибка",
            MessageBoxButton.OK, MessageBoxImage.Error);
    }

    public static void SwitchTheme(Theme theme)
    {
        var dicts = Current.Resources.MergedDictionaries;
        var themeDict = dicts.FirstOrDefault(d => d.Source is not null &&
            (d.Source.OriginalString.Contains("Dark.xaml") || d.Source.OriginalString.Contains("Light.xaml")));

        if (themeDict is not null)
            dicts.Remove(themeDict);

        var newSource = theme == Theme.Dark ? "Themes/Dark.xaml" : "Themes/Light.xaml";
        dicts.Insert(0, new ResourceDictionary { Source = new Uri(newSource, UriKind.Relative) });
        CurrentTheme = theme;
    }

    /// <summary>Called from MainWindow.Closing so preferences survive to the next launch.</summary>
    public static void SaveSettings() =>
        AppSettingsService.Save(new AppSettings(CurrentTheme, LocalizationService.Instance.Language));
}
