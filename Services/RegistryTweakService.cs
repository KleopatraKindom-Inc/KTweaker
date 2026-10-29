using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Win32;
using KleopatraKindomTweaker.Models;

namespace KleopatraKindomTweaker.Services;

public class RegistryTweakService
{
    /// <summary>
    /// The built-in catalog of tweaks, grouped by category. All of these are documented,
    /// reversible Windows registry settings (no undocumented hacks, no telemetry-of-our-own).
    /// Titles/descriptions are pulled from LocalizationService at build time, so call this
    /// again after a language change if you need it re-localized.
    /// </summary>
    public ObservableCollection<TweakItem> BuildCatalog()
    {
        var loc = LocalizationService.Instance;

        var items = new ObservableCollection<TweakItem>
        {
            // --- Personalization ---
            new()
            {
                Key = "TwDarkTheme",
                Title = loc["TwDarkTheme_Title"],
                Description = loc["TwDarkTheme_Desc"],
                Category = loc["CatPersonalization"],
                Hive = RegistryHive.CurrentUser,
                KeyPath = @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize",
                ValueName = "AppsUseLightTheme",
                ValueKind = RegistryValueKind.DWord,
                OnValue = 0, OffValue = 1, MissingMeansValue = 1,
                RequiresExplorerRestart = true
            },
            new()
            {
                Key = "TwFileExt",
                Title = loc["TwFileExt_Title"],
                Description = loc["TwFileExt_Desc"],
                Category = loc["CatPersonalization"],
                Hive = RegistryHive.CurrentUser,
                KeyPath = @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced",
                ValueName = "HideFileExt",
                ValueKind = RegistryValueKind.DWord,
                OnValue = 0, OffValue = 1, MissingMeansValue = 1,
                RequiresExplorerRestart = true
            },
            new()
            {
                Key = "TwHiddenFiles",
                Title = loc["TwHiddenFiles_Title"],
                Description = loc["TwHiddenFiles_Desc"],
                Category = loc["CatPersonalization"],
                Hive = RegistryHive.CurrentUser,
                KeyPath = @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced",
                ValueName = "Hidden",
                ValueKind = RegistryValueKind.DWord,
                OnValue = 1, OffValue = 2, MissingMeansValue = 2,
                RequiresExplorerRestart = true
            },
            new()
            {
                Key = "TwClassicMenu",
                Title = loc["TwClassicMenu_Title"],
                Description = loc["TwClassicMenu_Desc"],
                Category = loc["CatPersonalization"],
                Hive = RegistryHive.CurrentUser,
                KeyPath = @"Software\Classes\CLSID\{86ca1aa0-34aa-4e8b-a509-50c905bae2a2}\InprocServer32",
                ValueName = "",
                ValueKind = RegistryValueKind.String,
                OnValue = "", OffValue = "__DELETE__", MissingMeansValue = "__DELETE__",
                RequiresExplorerRestart = true
            },

            // --- Приватность ---
            new()
            {
                Key = "TwTelemetry",
                Title = loc["TwTelemetry_Title"],
                Description = loc["TwTelemetry_Desc"],
                Category = loc["CatPrivacy"],
                Hive = RegistryHive.LocalMachine,
                KeyPath = @"SOFTWARE\Policies\Microsoft\Windows\DataCollection",
                ValueName = "AllowTelemetry",
                ValueKind = RegistryValueKind.DWord,
                OnValue = 0, OffValue = 1, MissingMeansValue = 1
            },
            new()
            {
                Key = "TwAdId",
                Title = loc["TwAdId_Title"],
                Description = loc["TwAdId_Desc"],
                Category = loc["CatPrivacy"],
                Hive = RegistryHive.LocalMachine,
                KeyPath = @"SOFTWARE\Policies\Microsoft\Windows\AdvertisingInfo",
                ValueName = "DisabledByGroupPolicy",
                ValueKind = RegistryValueKind.DWord,
                OnValue = 1, OffValue = 0, MissingMeansValue = 0
            },
            new()
            {
                Key = "TwBingSearch",
                Title = loc["TwBingSearch_Title"],
                Description = loc["TwBingSearch_Desc"],
                Category = loc["CatPrivacy"],
                Hive = RegistryHive.CurrentUser,
                KeyPath = @"Software\Microsoft\Windows\CurrentVersion\Search",
                ValueName = "BingSearchEnabled",
                ValueKind = RegistryValueKind.DWord,
                OnValue = 0, OffValue = 1, MissingMeansValue = 1
            },
            new()
            {
                Key = "TwCortana",
                Title = loc["TwCortana_Title"],
                Description = loc["TwCortana_Desc"],
                Category = loc["CatPrivacy"],
                Hive = RegistryHive.LocalMachine,
                KeyPath = @"SOFTWARE\Policies\Microsoft\Windows\Windows Search",
                ValueName = "AllowCortana",
                ValueKind = RegistryValueKind.DWord,
                OnValue = 0, OffValue = 1, MissingMeansValue = 1
            },

            // --- Производительность ---
            new()
            {
                Key = "TwAnimations",
                Title = loc["TwAnimations_Title"],
                Description = loc["TwAnimations_Desc"],
                Category = loc["CatPerformance"],
                Hive = RegistryHive.CurrentUser,
                KeyPath = @"Control Panel\Desktop\WindowMetrics",
                ValueName = "MinAnimate",
                ValueKind = RegistryValueKind.String,
                OnValue = "0", OffValue = "1", MissingMeansValue = "1",
                RequiresExplorerRestart = true
            },
            new()
            {
                Key = "TwTransparency",
                Title = loc["TwTransparency_Title"],
                Description = loc["TwTransparency_Desc"],
                Category = loc["CatPerformance"],
                Hive = RegistryHive.CurrentUser,
                KeyPath = @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize",
                ValueName = "EnableTransparency",
                ValueKind = RegistryValueKind.DWord,
                OnValue = 0, OffValue = 1, MissingMeansValue = 1
            },
            new()
            {
                Key = "TwPowerThrottle",
                Title = loc["TwPowerThrottle_Title"],
                Description = loc["TwPowerThrottle_Desc"],
                Category = loc["CatPerformance"],
                Hive = RegistryHive.LocalMachine,
                KeyPath = @"SYSTEM\CurrentControlSet\Control\Power\PowerThrottling",
                ValueName = "PowerThrottlingOff",
                ValueKind = RegistryValueKind.DWord,
                OnValue = 1, OffValue = 0, MissingMeansValue = 0
            },

            // --- Игры/Ввод ---
            new()
            {
                Key = "TwGameDvr",
                Title = loc["TwGameDvr_Title"],
                Description = loc["TwGameDvr_Desc"],
                Category = loc["CatGames"],
                Hive = RegistryHive.CurrentUser,
                KeyPath = @"System\GameConfigStore",
                ValueName = "GameDVR_Enabled",
                ValueKind = RegistryValueKind.DWord,
                OnValue = 0, OffValue = 1, MissingMeansValue = 1
            },
            new()
            {
                Key = "TwMouseAccel",
                Title = loc["TwMouseAccel_Title"],
                Description = loc["TwMouseAccel_Desc"],
                Category = loc["CatGames"],
                Hive = RegistryHive.CurrentUser,
                KeyPath = @"Control Panel\Mouse",
                ValueName = "MouseSpeed",
                ValueKind = RegistryValueKind.String,
                OnValue = "0", OffValue = "1", MissingMeansValue = "1"
            },

            // --- Windows Update ---
            new()
            {
                Key = "TwDisableWU",
                Title = loc["TwDisableWU_Title"],
                Description = loc["TwDisableWU_Desc"],
                Category = loc["CatWindowsUpdate"],
                Hive = RegistryHive.LocalMachine,
                KeyPath = @"SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate\AU",
                ValueName = "NoAutoUpdate",
                ValueKind = RegistryValueKind.DWord,
                OnValue = 1, OffValue = 0, MissingMeansValue = 0
            },
            new()
            {
                Key = "TwDisableWUReboot",
                Title = loc["TwDisableWUReboot_Title"],
                Description = loc["TwDisableWUReboot_Desc"],
                Category = loc["CatWindowsUpdate"],
                Hive = RegistryHive.LocalMachine,
                KeyPath = @"SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate\AU",
                ValueName = "NoAutoRebootWithLoggedOnUsers",
                ValueKind = RegistryValueKind.DWord,
                OnValue = 1, OffValue = 0, MissingMeansValue = 0
            },

            // --- Extra personalization ---
            new()
            {
                Key = "TwWidgets",
                Title = loc["TwWidgets_Title"],
                Description = loc["TwWidgets_Desc"],
                Category = loc["CatPersonalization"],
                Hive = RegistryHive.CurrentUser,
                KeyPath = @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced",
                ValueName = "TaskbarDa",
                ValueKind = RegistryValueKind.DWord,
                OnValue = 0, OffValue = 1, MissingMeansValue = 1,
                RequiresExplorerRestart = true
            },
            new()
            {
                Key = "TwChatIcon",
                Title = loc["TwChatIcon_Title"],
                Description = loc["TwChatIcon_Desc"],
                Category = loc["CatPersonalization"],
                Hive = RegistryHive.CurrentUser,
                KeyPath = @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced",
                ValueName = "TaskbarMn",
                ValueKind = RegistryValueKind.DWord,
                OnValue = 0, OffValue = 1, MissingMeansValue = 1,
                RequiresExplorerRestart = true
            },
            new()
            {
                Key = "TwStartRecommendations",
                Title = loc["TwStartRecommendations_Title"],
                Description = loc["TwStartRecommendations_Desc"],
                Category = loc["CatPersonalization"],
                Hive = RegistryHive.CurrentUser,
                KeyPath = @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced",
                ValueName = "Start_IrisRecommendations",
                ValueKind = RegistryValueKind.DWord,
                OnValue = 0, OffValue = 1, MissingMeansValue = 1,
                RequiresExplorerRestart = true
            },

            // --- Extra privacy ---
            new()
            {
                Key = "TwBackgroundApps",
                Title = loc["TwBackgroundApps_Title"],
                Description = loc["TwBackgroundApps_Desc"],
                Category = loc["CatPrivacy"],
                Hive = RegistryHive.CurrentUser,
                KeyPath = @"Software\Microsoft\Windows\CurrentVersion\BackgroundAccessApplications",
                ValueName = "GlobalUserDisabled",
                ValueKind = RegistryValueKind.DWord,
                OnValue = 1, OffValue = 0, MissingMeansValue = 0
            },

            // --- Extra performance ---
            new()
            {
                Key = "TwAutorunUSB",
                Title = loc["TwAutorunUSB_Title"],
                Description = loc["TwAutorunUSB_Desc"],
                Category = loc["CatPerformance"],
                Hive = RegistryHive.LocalMachine,
                KeyPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\Explorer",
                ValueName = "NoDriveTypeAutoRun",
                ValueKind = RegistryValueKind.DWord,
                OnValue = 255, OffValue = 145, MissingMeansValue = 145
            },
            new()
            {
                Key = "TwNumLock",
                Title = loc["TwNumLock_Title"],
                Description = loc["TwNumLock_Desc"],
                Category = loc["CatPerformance"],
                Hive = RegistryHive.Users,
                KeyPath = @".DEFAULT\Control Panel\Keyboard",
                ValueName = "InitialKeyboardIndicators",
                ValueKind = RegistryValueKind.String,
                OnValue = "2", OffValue = "0", MissingMeansValue = "0"
            },

            // --- More personalization ---
            new()
            {
                Key = "TwTaskbarSearch",
                Title = loc["TwTaskbarSearch_Title"],
                Description = loc["TwTaskbarSearch_Desc"],
                Category = loc["CatPersonalization"],
                Hive = RegistryHive.CurrentUser,
                KeyPath = @"Software\Microsoft\Windows\CurrentVersion\Search",
                ValueName = "SearchboxTaskbarMode",
                ValueKind = RegistryValueKind.DWord,
                OnValue = 0, OffValue = 1, MissingMeansValue = 1,
                RequiresExplorerRestart = true
            },
            new()
            {
                Key = "TwTaskbarAlignLeft",
                Title = loc["TwTaskbarAlignLeft_Title"],
                Description = loc["TwTaskbarAlignLeft_Desc"],
                Category = loc["CatPersonalization"],
                Hive = RegistryHive.CurrentUser,
                KeyPath = @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced",
                ValueName = "TaskbarAl",
                ValueKind = RegistryValueKind.DWord,
                OnValue = 0, OffValue = 1, MissingMeansValue = 1,
                RequiresExplorerRestart = true
            },
            new()
            {
                Key = "TwShowSecondsClock",
                Title = loc["TwShowSecondsClock_Title"],
                Description = loc["TwShowSecondsClock_Desc"],
                Category = loc["CatPersonalization"],
                Hive = RegistryHive.CurrentUser,
                KeyPath = @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced",
                ValueName = "ShowSecondsInSystemClock",
                ValueKind = RegistryValueKind.DWord,
                OnValue = 1, OffValue = 0, MissingMeansValue = 0,
                RequiresExplorerRestart = true
            },

            // --- More privacy ---
            new()
            {
                Key = "TwLocationTracking",
                Title = loc["TwLocationTracking_Title"],
                Description = loc["TwLocationTracking_Desc"],
                Category = loc["CatPrivacy"],
                Hive = RegistryHive.LocalMachine,
                KeyPath = @"SOFTWARE\Policies\Microsoft\Windows\LocationAndSensors",
                ValueName = "DisableLocation",
                ValueKind = RegistryValueKind.DWord,
                OnValue = 1, OffValue = 0, MissingMeansValue = 0
            },
            new()
            {
                Key = "TwActivityHistory",
                Title = loc["TwActivityHistory_Title"],
                Description = loc["TwActivityHistory_Desc"],
                Category = loc["CatPrivacy"],
                Hive = RegistryHive.LocalMachine,
                KeyPath = @"SOFTWARE\Policies\Microsoft\Windows\System",
                ValueName = "EnableActivityFeed",
                ValueKind = RegistryValueKind.DWord,
                OnValue = 0, OffValue = 1, MissingMeansValue = 1
            },
            new()
            {
                Key = "TwErrorReporting",
                Title = loc["TwErrorReporting_Title"],
                Description = loc["TwErrorReporting_Desc"],
                Category = loc["CatPrivacy"],
                Hive = RegistryHive.LocalMachine,
                KeyPath = @"SOFTWARE\Microsoft\Windows\Windows Error Reporting",
                ValueName = "Disabled",
                ValueKind = RegistryValueKind.DWord,
                OnValue = 1, OffValue = 0, MissingMeansValue = 0
            },
            new()
            {
                Key = "TwTailoredExperiences",
                Title = loc["TwTailoredExperiences_Title"],
                Description = loc["TwTailoredExperiences_Desc"],
                Category = loc["CatPrivacy"],
                Hive = RegistryHive.CurrentUser,
                KeyPath = @"Software\Policies\Microsoft\Windows\CloudContent",
                ValueName = "DisableTailoredExperiencesWithDiagnosticData",
                ValueKind = RegistryValueKind.DWord,
                OnValue = 1, OffValue = 0, MissingMeansValue = 0
            },

            // --- More performance ---
            new()
            {
                Key = "TwMenuShowDelay",
                Title = loc["TwMenuShowDelay_Title"],
                Description = loc["TwMenuShowDelay_Desc"],
                Category = loc["CatPerformance"],
                Hive = RegistryHive.CurrentUser,
                KeyPath = @"Control Panel\Desktop",
                ValueName = "MenuShowDelay",
                ValueKind = RegistryValueKind.String,
                OnValue = "0", OffValue = "400", MissingMeansValue = "400"
            },
            new()
            {
                Key = "TwVisualEffectsBest",
                Title = loc["TwVisualEffectsBest_Title"],
                Description = loc["TwVisualEffectsBest_Desc"],
                Category = loc["CatPerformance"],
                Hive = RegistryHive.CurrentUser,
                KeyPath = @"Software\Microsoft\Windows\CurrentVersion\Explorer\VisualEffects",
                ValueName = "VisualFXSetting",
                ValueKind = RegistryValueKind.DWord,
                OnValue = 2, OffValue = 0, MissingMeansValue = 0
            },
            new()
            {
                Key = "TwLastAccessUpdate",
                Title = loc["TwLastAccessUpdate_Title"],
                Description = loc["TwLastAccessUpdate_Desc"],
                Category = loc["CatPerformance"],
                Hive = RegistryHive.LocalMachine,
                KeyPath = @"SYSTEM\CurrentControlSet\Control\FileSystem",
                ValueName = "NtfsDisableLastAccessUpdate",
                ValueKind = RegistryValueKind.DWord,
                OnValue = 1, OffValue = 0, MissingMeansValue = 0
            },
            new()
            {
                Key = "TwNetworkThrottling",
                Title = loc["TwNetworkThrottling_Title"],
                Description = loc["TwNetworkThrottling_Desc"],
                Category = loc["CatPerformance"],
                Hive = RegistryHive.LocalMachine,
                KeyPath = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile",
                ValueName = "NetworkThrottlingIndex",
                ValueKind = RegistryValueKind.DWord,
                // 0xFFFFFFFF (disabled) doesn't fit in Int32, so we use -1: the same 32-bit
                // pattern is written to the registry either way (DWORDs aren't sign-aware).
                OnValue = -1, OffValue = 10, MissingMeansValue = 10
            },

            // --- More games ---
            new()
            {
                Key = "TwGameMode",
                Title = loc["TwGameMode_Title"],
                Description = loc["TwGameMode_Desc"],
                Category = loc["CatGames"],
                Hive = RegistryHive.CurrentUser,
                KeyPath = @"Software\Microsoft\GameBar",
                ValueName = "AutoGameModeEnabled",
                ValueKind = RegistryValueKind.DWord,
                OnValue = 1, OffValue = 0, MissingMeansValue = 1
            },

            // --- Round 2: more personalization ---
            new()
            {
                Key = "TwShowThisPC",
                Title = loc["TwShowThisPC_Title"],
                Description = loc["TwShowThisPC_Desc"],
                Category = loc["CatPersonalization"],
                Hive = RegistryHive.CurrentUser,
                KeyPath = @"Software\Microsoft\Windows\CurrentVersion\Explorer\HideDesktopIcons\NewStartPanel",
                ValueName = "{20D04FE0-3AEA-1069-A2D8-08002B30309D}",
                ValueKind = RegistryValueKind.DWord,
                OnValue = 0, OffValue = 1, MissingMeansValue = 1,
                RequiresExplorerRestart = true
            },
            new()
            {
                Key = "TwAccentColorTitlebar",
                Title = loc["TwAccentColorTitlebar_Title"],
                Description = loc["TwAccentColorTitlebar_Desc"],
                Category = loc["CatPersonalization"],
                Hive = RegistryHive.CurrentUser,
                KeyPath = @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize",
                ValueName = "ColorPrevalence",
                ValueKind = RegistryValueKind.DWord,
                OnValue = 1, OffValue = 0, MissingMeansValue = 0,
                RequiresExplorerRestart = true
            },
            new()
            {
                Key = "TwExplorerLaunchToThisPC",
                Title = loc["TwExplorerLaunchToThisPC_Title"],
                Description = loc["TwExplorerLaunchToThisPC_Desc"],
                Category = loc["CatPersonalization"],
                Hive = RegistryHive.CurrentUser,
                KeyPath = @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced",
                ValueName = "LaunchTo",
                ValueKind = RegistryValueKind.DWord,
                OnValue = 1, OffValue = 2, MissingMeansValue = 2,
                RequiresExplorerRestart = true
            },
            new()
            {
                Key = "TwRestoreFolderWindows",
                Title = loc["TwRestoreFolderWindows_Title"],
                Description = loc["TwRestoreFolderWindows_Desc"],
                Category = loc["CatPersonalization"],
                Hive = RegistryHive.CurrentUser,
                KeyPath = @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced",
                ValueName = "PersistBrowsers",
                ValueKind = RegistryValueKind.DWord,
                OnValue = 1, OffValue = 0, MissingMeansValue = 0
            },

            // --- Round 2: more privacy ---
            new()
            {
                Key = "TwOnlineSpeech",
                Title = loc["TwOnlineSpeech_Title"],
                Description = loc["TwOnlineSpeech_Desc"],
                Category = loc["CatPrivacy"],
                Hive = RegistryHive.CurrentUser,
                KeyPath = @"Software\Microsoft\Speech_OneCore\Settings\OnlineSpeechPrivacy",
                ValueName = "HasAccepted",
                ValueKind = RegistryValueKind.DWord,
                OnValue = 0, OffValue = 1, MissingMeansValue = 0
            },
            new()
            {
                Key = "TwClipboardHistory",
                Title = loc["TwClipboardHistory_Title"],
                Description = loc["TwClipboardHistory_Desc"],
                Category = loc["CatPrivacy"],
                Hive = RegistryHive.CurrentUser,
                KeyPath = @"Software\Microsoft\Clipboard",
                ValueName = "EnableClipboardHistory",
                ValueKind = RegistryValueKind.DWord,
                OnValue = 0, OffValue = 1, MissingMeansValue = 1
            },
            new()
            {
                Key = "TwInkTypingPersonalization",
                Title = loc["TwInkTypingPersonalization_Title"],
                Description = loc["TwInkTypingPersonalization_Desc"],
                Category = loc["CatPrivacy"],
                Hive = RegistryHive.CurrentUser,
                KeyPath = @"Software\Microsoft\InputPersonalization",
                ValueName = "RestrictImplicitTextCollection",
                ValueKind = RegistryValueKind.DWord,
                OnValue = 1, OffValue = 0, MissingMeansValue = 0
            },
            new()
            {
                Key = "TwExplorerWebSearch",
                Title = loc["TwExplorerWebSearch_Title"],
                Description = loc["TwExplorerWebSearch_Desc"],
                Category = loc["CatPrivacy"],
                Hive = RegistryHive.LocalMachine,
                KeyPath = @"SOFTWARE\Policies\Microsoft\Windows\Explorer",
                ValueName = "DisableSearchBoxSuggestions",
                ValueKind = RegistryValueKind.DWord,
                OnValue = 1, OffValue = 0, MissingMeansValue = 0
            },

            // --- Round 2: System (new category) ---
            new()
            {
                Key = "TwHibernate",
                Title = loc["TwHibernate_Title"],
                Description = loc["TwHibernate_Desc"],
                Category = loc["CatSystem"],
                Hive = RegistryHive.LocalMachine,
                KeyPath = @"SYSTEM\CurrentControlSet\Control\Power",
                ValueName = "HibernateEnabled",
                ValueKind = RegistryValueKind.DWord,
                OnValue = 0, OffValue = 1, MissingMeansValue = 1
            },
            new()
            {
                Key = "TwSuperfetch",
                Title = loc["TwSuperfetch_Title"],
                Description = loc["TwSuperfetch_Desc"],
                Category = loc["CatSystem"],
                Hive = RegistryHive.LocalMachine,
                KeyPath = @"SYSTEM\CurrentControlSet\Services\SysMain",
                ValueName = "Start",
                ValueKind = RegistryValueKind.DWord,
                OnValue = 4, OffValue = 2, MissingMeansValue = 2
            },
            new()
            {
                Key = "TwPrefetchParams",
                Title = loc["TwPrefetchParams_Title"],
                Description = loc["TwPrefetchParams_Desc"],
                Category = loc["CatSystem"],
                Hive = RegistryHive.LocalMachine,
                KeyPath = @"SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management\PrefetchParameters",
                ValueName = "EnablePrefetcher",
                ValueKind = RegistryValueKind.DWord,
                OnValue = 0, OffValue = 3, MissingMeansValue = 3
            },
            new()
            {
                Key = "TwPagingExecutive",
                Title = loc["TwPagingExecutive_Title"],
                Description = loc["TwPagingExecutive_Desc"],
                Category = loc["CatSystem"],
                Hive = RegistryHive.LocalMachine,
                KeyPath = @"SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management",
                ValueName = "DisablePagingExecutive",
                ValueKind = RegistryValueKind.DWord,
                OnValue = 1, OffValue = 0, MissingMeansValue = 0
            },
            new()
            {
                Key = "TwLargeSystemCache",
                Title = loc["TwLargeSystemCache_Title"],
                Description = loc["TwLargeSystemCache_Desc"],
                Category = loc["CatSystem"],
                Hive = RegistryHive.LocalMachine,
                KeyPath = @"SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management",
                ValueName = "LargeSystemCache",
                ValueKind = RegistryValueKind.DWord,
                OnValue = 1, OffValue = 0, MissingMeansValue = 0
            },
            new()
            {
                Key = "TwWindowsSearchIndex",
                Title = loc["TwWindowsSearchIndex_Title"],
                Description = loc["TwWindowsSearchIndex_Desc"],
                Category = loc["CatSystem"],
                Hive = RegistryHive.LocalMachine,
                KeyPath = @"SYSTEM\CurrentControlSet\Services\WSearch",
                ValueName = "Start",
                ValueKind = RegistryValueKind.DWord,
                OnValue = 4, OffValue = 2, MissingMeansValue = 2
            },
            new()
            {
                Key = "TwFastStartup",
                Title = loc["TwFastStartup_Title"],
                Description = loc["TwFastStartup_Desc"],
                Category = loc["CatSystem"],
                Hive = RegistryHive.LocalMachine,
                KeyPath = @"SYSTEM\CurrentControlSet\Control\Session Manager\Power",
                ValueName = "HiberbootEnabled",
                ValueKind = RegistryValueKind.DWord,
                OnValue = 0, OffValue = 1, MissingMeansValue = 1
            },
            new()
            {
                Key = "TwLockScreen",
                Title = loc["TwLockScreen_Title"],
                Description = loc["TwLockScreen_Desc"],
                Category = loc["CatSystem"],
                Hive = RegistryHive.LocalMachine,
                KeyPath = @"SOFTWARE\Policies\Microsoft\Windows\Personalization",
                ValueName = "NoLockScreen",
                ValueKind = RegistryValueKind.DWord,
                OnValue = 1, OffValue = 0, MissingMeansValue = 0
            },

            // --- Round 2: Network (new category) ---
            new()
            {
                Key = "TwDisableIPv6",
                Title = loc["TwDisableIPv6_Title"],
                Description = loc["TwDisableIPv6_Desc"],
                Category = loc["CatNetwork"],
                Hive = RegistryHive.LocalMachine,
                KeyPath = @"SYSTEM\CurrentControlSet\Services\Tcpip6\Parameters",
                ValueName = "DisabledComponents",
                ValueKind = RegistryValueKind.DWord,
                OnValue = 255, OffValue = 0, MissingMeansValue = 0
            },


            // --- Round 3: bulk expansion (auto-generated) ---
            new()
            {
                Key = "TwTaskbarLabels",
                Title = loc["TwTaskbarLabels_Title"],
                Description = loc["TwTaskbarLabels_Desc"],
                Category = loc["CatPersonalization"],
                Hive = RegistryHive.CurrentUser,
                KeyPath = @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced",
                ValueName = "TaskbarGlomLevel",
                ValueKind = RegistryValueKind.DWord,
                OnValue = 2, OffValue = 0, MissingMeansValue = 0,
                RequiresExplorerRestart = true
            },
            new()
            {
                Key = "TwShowRecycleBinDesktop",
                Title = loc["TwShowRecycleBinDesktop_Title"],
                Description = loc["TwShowRecycleBinDesktop_Desc"],
                Category = loc["CatPersonalization"],
                Hive = RegistryHive.CurrentUser,
                KeyPath = @"Software\Microsoft\Windows\CurrentVersion\Explorer\HideDesktopIcons\NewStartPanel",
                ValueName = "{645FF040-5081-101B-9F08-00AA002F954E}",
                ValueKind = RegistryValueKind.DWord,
                OnValue = 0, OffValue = 1, MissingMeansValue = 0,
                RequiresExplorerRestart = true
            },
            new()
            {
                Key = "TwShowControlPanelDesktop",
                Title = loc["TwShowControlPanelDesktop_Title"],
                Description = loc["TwShowControlPanelDesktop_Desc"],
                Category = loc["CatPersonalization"],
                Hive = RegistryHive.CurrentUser,
                KeyPath = @"Software\Microsoft\Windows\CurrentVersion\Explorer\HideDesktopIcons\NewStartPanel",
                ValueName = "{5399E694-6CE5-4D6C-8FCE-1D8870FDCBA0}",
                ValueKind = RegistryValueKind.DWord,
                OnValue = 0, OffValue = 1, MissingMeansValue = 1,
                RequiresExplorerRestart = true
            },
            new()
            {
                Key = "TwShowUserFolderDesktop",
                Title = loc["TwShowUserFolderDesktop_Title"],
                Description = loc["TwShowUserFolderDesktop_Desc"],
                Category = loc["CatPersonalization"],
                Hive = RegistryHive.CurrentUser,
                KeyPath = @"Software\Microsoft\Windows\CurrentVersion\Explorer\HideDesktopIcons\NewStartPanel",
                ValueName = "{59031a47-3f72-44a7-89c5-5595fe6b30ee}",
                ValueKind = RegistryValueKind.DWord,
                OnValue = 0, OffValue = 1, MissingMeansValue = 1,
                RequiresExplorerRestart = true
            },
            new()
            {
                Key = "TwShowNetworkDesktop",
                Title = loc["TwShowNetworkDesktop_Title"],
                Description = loc["TwShowNetworkDesktop_Desc"],
                Category = loc["CatPersonalization"],
                Hive = RegistryHive.CurrentUser,
                KeyPath = @"Software\Microsoft\Windows\CurrentVersion\Explorer\HideDesktopIcons\NewStartPanel",
                ValueName = "{F02C1A0D-BE21-4350-88B0-7367FC96EF3C}",
                ValueKind = RegistryValueKind.DWord,
                OnValue = 0, OffValue = 1, MissingMeansValue = 1,
                RequiresExplorerRestart = true
            },
            new()
            {
                Key = "TwDisableTaskbarPeek",
                Title = loc["TwDisableTaskbarPeek_Title"],
                Description = loc["TwDisableTaskbarPeek_Desc"],
                Category = loc["CatPersonalization"],
                Hive = RegistryHive.CurrentUser,
                KeyPath = @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced",
                ValueName = "EnableAeroPeek",
                ValueKind = RegistryValueKind.DWord,
                OnValue = 0, OffValue = 1, MissingMeansValue = 1,
                RequiresExplorerRestart = true
            },
            new()
            {
                Key = "TwFileExplorerCompactMode",
                Title = loc["TwFileExplorerCompactMode_Title"],
                Description = loc["TwFileExplorerCompactMode_Desc"],
                Category = loc["CatPersonalization"],
                Hive = RegistryHive.CurrentUser,
                KeyPath = @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced",
                ValueName = "UseCompactMode",
                ValueKind = RegistryValueKind.DWord,
                OnValue = 1, OffValue = 0, MissingMeansValue = 0,
                RequiresExplorerRestart = true
            },
            new()
            {
                Key = "TwShowLibrariesExplorer",
                Title = loc["TwShowLibrariesExplorer_Title"],
                Description = loc["TwShowLibrariesExplorer_Desc"],
                Category = loc["CatPersonalization"],
                Hive = RegistryHive.CurrentUser,
                KeyPath = @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced",
                ValueName = "NavPaneShowAllFolders",
                ValueKind = RegistryValueKind.DWord,
                OnValue = 1, OffValue = 0, MissingMeansValue = 0,
                RequiresExplorerRestart = true
            },
            new()
            {
                Key = "TwShowNavPaneExpand",
                Title = loc["TwShowNavPaneExpand_Title"],
                Description = loc["TwShowNavPaneExpand_Desc"],
                Category = loc["CatPersonalization"],
                Hive = RegistryHive.CurrentUser,
                KeyPath = @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced",
                ValueName = "NavPaneExpandToCurrentFolder",
                ValueKind = RegistryValueKind.DWord,
                OnValue = 1, OffValue = 0, MissingMeansValue = 0,
                RequiresExplorerRestart = true
            },
            new()
            {
                Key = "TwDisableSharingWizard",
                Title = loc["TwDisableSharingWizard_Title"],
                Description = loc["TwDisableSharingWizard_Desc"],
                Category = loc["CatPersonalization"],
                Hive = RegistryHive.CurrentUser,
                KeyPath = @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced",
                ValueName = "SharingWizardOn",
                ValueKind = RegistryValueKind.DWord,
                OnValue = 0, OffValue = 1, MissingMeansValue = 1
            },
            new()
            {
                Key = "TwDisableBalloonTips",
                Title = loc["TwDisableBalloonTips_Title"],
                Description = loc["TwDisableBalloonTips_Desc"],
                Category = loc["CatPersonalization"],
                Hive = RegistryHive.CurrentUser,
                KeyPath = @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced",
                ValueName = "EnableBalloonTips",
                ValueKind = RegistryValueKind.DWord,
                OnValue = 0, OffValue = 1, MissingMeansValue = 1
            },
            new()
            {
                Key = "TwDisableTaskbarBadging",
                Title = loc["TwDisableTaskbarBadging_Title"],
                Description = loc["TwDisableTaskbarBadging_Desc"],
                Category = loc["CatPersonalization"],
                Hive = RegistryHive.CurrentUser,
                KeyPath = @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced",
                ValueName = "TaskbarBadges",
                ValueKind = RegistryValueKind.DWord,
                OnValue = 0, OffValue = 1, MissingMeansValue = 1,
                RequiresExplorerRestart = true
            },
            new()
            {
                Key = "TwDisableThumbsDb",
                Title = loc["TwDisableThumbsDb_Title"],
                Description = loc["TwDisableThumbsDb_Desc"],
                Category = loc["CatPersonalization"],
                Hive = RegistryHive.CurrentUser,
                KeyPath = @"Software\Microsoft\Windows\CurrentVersion\Explorer",
                ValueName = "DisableThumbsDBOnNetworkFolders",
                ValueKind = RegistryValueKind.DWord,
                OnValue = 1, OffValue = 0, MissingMeansValue = 0
            },
            new()
            {
                Key = "TwDisableThumbnailCache",
                Title = loc["TwDisableThumbnailCache_Title"],
                Description = loc["TwDisableThumbnailCache_Desc"],
                Category = loc["CatPersonalization"],
                Hive = RegistryHive.CurrentUser,
                KeyPath = @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced",
                ValueName = "DisableThumbnailCache",
                ValueKind = RegistryValueKind.DWord,
                OnValue = 1, OffValue = 0, MissingMeansValue = 0,
                RequiresExplorerRestart = true
            },
            new()
            {
                Key = "TwDisableWindowsInkWorkspace",
                Title = loc["TwDisableWindowsInkWorkspace_Title"],
                Description = loc["TwDisableWindowsInkWorkspace_Desc"],
                Category = loc["CatPersonalization"],
                Hive = RegistryHive.CurrentUser,
                KeyPath = @"Software\Policies\Microsoft\WindowsInkWorkspace",
                ValueName = "AllowWindowsInkWorkspace",
                ValueKind = RegistryValueKind.DWord,
                OnValue = 0, OffValue = 1, MissingMeansValue = 1
            },
            new()
            {
                Key = "TwDisableNotificationCenter",
                Title = loc["TwDisableNotificationCenter_Title"],
                Description = loc["TwDisableNotificationCenter_Desc"],
                Category = loc["CatPersonalization"],
                Hive = RegistryHive.CurrentUser,
                KeyPath = @"Software\Microsoft\Windows\CurrentVersion\PushNotifications",
                ValueName = "ToastEnabled",
                ValueKind = RegistryValueKind.DWord,
                OnValue = 0, OffValue = 1, MissingMeansValue = 1
            },
            new()
            {
                Key = "TwDisableStartupSound",
                Title = loc["TwDisableStartupSound_Title"],
                Description = loc["TwDisableStartupSound_Desc"],
                Category = loc["CatPersonalization"],
                Hive = RegistryHive.CurrentUser,
                KeyPath = @"Software\Microsoft\Windows\CurrentVersion\Authentication\LogonUI\BootAnimation",
                ValueName = "DisableStartupSound",
                ValueKind = RegistryValueKind.DWord,
                OnValue = 1, OffValue = 0, MissingMeansValue = 0
            },
            new()
            {
                Key = "TwDisableAppSuggestions",
                Title = loc["TwDisableAppSuggestions_Title"],
                Description = loc["TwDisableAppSuggestions_Desc"],
                Category = loc["CatAdsSuggestions"],
                Hive = RegistryHive.CurrentUser,
                KeyPath = @"Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager",
                ValueName = "SystemPaneSuggestionsEnabled",
                ValueKind = RegistryValueKind.DWord,
                OnValue = 0, OffValue = 1, MissingMeansValue = 1,
                RequiresExplorerRestart = true
            },
            new()
            {
                Key = "TwDisableLockScreenAds",
                Title = loc["TwDisableLockScreenAds_Title"],
                Description = loc["TwDisableLockScreenAds_Desc"],
                Category = loc["CatAdsSuggestions"],
                Hive = RegistryHive.CurrentUser,
                KeyPath = @"Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager",
                ValueName = "RotatingLockScreenOverlayEnabled",
                ValueKind = RegistryValueKind.DWord,
                OnValue = 0, OffValue = 1, MissingMeansValue = 1
            },
            new()
            {
                Key = "TwDisableStartSuggestions",
                Title = loc["TwDisableStartSuggestions_Title"],
                Description = loc["TwDisableStartSuggestions_Desc"],
                Category = loc["CatAdsSuggestions"],
                Hive = RegistryHive.CurrentUser,
                KeyPath = @"Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager",
                ValueName = "SubscribedContent-338388Enabled",
                ValueKind = RegistryValueKind.DWord,
                OnValue = 0, OffValue = 1, MissingMeansValue = 1,
                RequiresExplorerRestart = true
            },
            new()
            {
                Key = "TwDisableWelcomeExperience",
                Title = loc["TwDisableWelcomeExperience_Title"],
                Description = loc["TwDisableWelcomeExperience_Desc"],
                Category = loc["CatAdsSuggestions"],
                Hive = RegistryHive.CurrentUser,
                KeyPath = @"Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager",
                ValueName = "SubscribedContent-310093Enabled",
                ValueKind = RegistryValueKind.DWord,
                OnValue = 0, OffValue = 1, MissingMeansValue = 1
            },
            new()
            {
                Key = "TwDisableTipsTricks",
                Title = loc["TwDisableTipsTricks_Title"],
                Description = loc["TwDisableTipsTricks_Desc"],
                Category = loc["CatAdsSuggestions"],
                Hive = RegistryHive.CurrentUser,
                KeyPath = @"Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager",
                ValueName = "SubscribedContent-338389Enabled",
                ValueKind = RegistryValueKind.DWord,
                OnValue = 0, OffValue = 1, MissingMeansValue = 1
            },
            new()
            {
                Key = "TwDisableContentDelivery",
                Title = loc["TwDisableContentDelivery_Title"],
                Description = loc["TwDisableContentDelivery_Desc"],
                Category = loc["CatAdsSuggestions"],
                Hive = RegistryHive.CurrentUser,
                KeyPath = @"Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager",
                ValueName = "ContentDeliveryAllowed",
                ValueKind = RegistryValueKind.DWord,
                OnValue = 0, OffValue = 1, MissingMeansValue = 1
            },
            new()
            {
                Key = "TwDisableOemApps",
                Title = loc["TwDisableOemApps_Title"],
                Description = loc["TwDisableOemApps_Desc"],
                Category = loc["CatAdsSuggestions"],
                Hive = RegistryHive.LocalMachine,
                KeyPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\ContentDeliveryManager",
                ValueName = "OemPreInstalledAppsEnabled",
                ValueKind = RegistryValueKind.DWord,
                OnValue = 0, OffValue = 1, MissingMeansValue = 1
            },
            new()
            {
                Key = "TwDisablePreinstalledApps",
                Title = loc["TwDisablePreinstalledApps_Title"],
                Description = loc["TwDisablePreinstalledApps_Desc"],
                Category = loc["CatAdsSuggestions"],
                Hive = RegistryHive.LocalMachine,
                KeyPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\ContentDeliveryManager",
                ValueName = "PreInstalledAppsEnabled",
                ValueKind = RegistryValueKind.DWord,
                OnValue = 0, OffValue = 1, MissingMeansValue = 1
            },
            new()
            {
                Key = "TwDisableSilentAppInstalls",
                Title = loc["TwDisableSilentAppInstalls_Title"],
                Description = loc["TwDisableSilentAppInstalls_Desc"],
                Category = loc["CatAdsSuggestions"],
                Hive = RegistryHive.CurrentUser,
                KeyPath = @"Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager",
                ValueName = "SilentInstalledAppsEnabled",
                ValueKind = RegistryValueKind.DWord,
                OnValue = 0, OffValue = 1, MissingMeansValue = 1
            },
            new()
            {
                Key = "TwDisableSubscribedContent",
                Title = loc["TwDisableSubscribedContent_Title"],
                Description = loc["TwDisableSubscribedContent_Desc"],
                Category = loc["CatAdsSuggestions"],
                Hive = RegistryHive.CurrentUser,
                KeyPath = @"Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager",
                ValueName = "SubscribedContentEnabled",
                ValueKind = RegistryValueKind.DWord,
                OnValue = 0, OffValue = 1, MissingMeansValue = 1
            },
            new()
            {
                Key = "TwDisableSpotlight",
                Title = loc["TwDisableSpotlight_Title"],
                Description = loc["TwDisableSpotlight_Desc"],
                Category = loc["CatAdsSuggestions"],
                Hive = RegistryHive.CurrentUser,
                KeyPath = @"Software\Policies\Microsoft\Windows\CloudContent",
                ValueName = "DisableSpotlightCollectionOnDesktop",
                ValueKind = RegistryValueKind.DWord,
                OnValue = 1, OffValue = 0, MissingMeansValue = 0
            },
            new()
            {
                Key = "TwDisableSpotlightFeatures",
                Title = loc["TwDisableSpotlightFeatures_Title"],
                Description = loc["TwDisableSpotlightFeatures_Desc"],
                Category = loc["CatAdsSuggestions"],
                Hive = RegistryHive.CurrentUser,
                KeyPath = @"Software\Policies\Microsoft\Windows\CloudContent",
                ValueName = "DisableWindowsSpotlightFeatures",
                ValueKind = RegistryValueKind.DWord,
                OnValue = 1, OffValue = 0, MissingMeansValue = 0
            },
            new()
            {
                Key = "TwDisableConsumerFeatures",
                Title = loc["TwDisableConsumerFeatures_Title"],
                Description = loc["TwDisableConsumerFeatures_Desc"],
                Category = loc["CatAdsSuggestions"],
                Hive = RegistryHive.LocalMachine,
                KeyPath = @"SOFTWARE\Policies\Microsoft\Windows\CloudContent",
                ValueName = "DisableWindowsConsumerFeatures",
                ValueKind = RegistryValueKind.DWord,
                OnValue = 1, OffValue = 0, MissingMeansValue = 0
            },
            new()
            {
                Key = "TwDisableFindMyDevice",
                Title = loc["TwDisableFindMyDevice_Title"],
                Description = loc["TwDisableFindMyDevice_Desc"],
                Category = loc["CatPrivacy"],
                Hive = RegistryHive.LocalMachine,
                KeyPath = @"SOFTWARE\Policies\Microsoft\Windows\FindMyDevice",
                ValueName = "AllowFindMyDevice",
                ValueKind = RegistryValueKind.DWord,
                OnValue = 0, OffValue = 1, MissingMeansValue = 1
            },
            new()
            {
                Key = "TwDisableDiagTrackService",
                Title = loc["TwDisableDiagTrackService_Title"],
                Description = loc["TwDisableDiagTrackService_Desc"],
                Category = loc["CatPrivacy"],
                Hive = RegistryHive.LocalMachine,
                KeyPath = @"SYSTEM\CurrentControlSet\Services\DiagTrack",
                ValueName = "Start",
                ValueKind = RegistryValueKind.DWord,
                OnValue = 4, OffValue = 2, MissingMeansValue = 2
            },
            new()
            {
                Key = "TwDisableDmwappush",
                Title = loc["TwDisableDmwappush_Title"],
                Description = loc["TwDisableDmwappush_Desc"],
                Category = loc["CatPrivacy"],
                Hive = RegistryHive.LocalMachine,
                KeyPath = @"SYSTEM\CurrentControlSet\Services\dmwappushservice",
                ValueName = "Start",
                ValueKind = RegistryValueKind.DWord,
                OnValue = 4, OffValue = 2, MissingMeansValue = 2
            },
            new()
            {
                Key = "TwDisableRemoteAssistance",
                Title = loc["TwDisableRemoteAssistance_Title"],
                Description = loc["TwDisableRemoteAssistance_Desc"],
                Category = loc["CatPrivacy"],
                Hive = RegistryHive.LocalMachine,
                KeyPath = @"SYSTEM\CurrentControlSet\Control\Remote Assistance",
                ValueName = "fAllowToGetHelp",
                ValueKind = RegistryValueKind.DWord,
                OnValue = 0, OffValue = 1, MissingMeansValue = 1
            },
            new()
            {
                Key = "TwDisableRemoteDesktop",
                Title = loc["TwDisableRemoteDesktop_Title"],
                Description = loc["TwDisableRemoteDesktop_Desc"],
                Category = loc["CatPrivacy"],
                Hive = RegistryHive.LocalMachine,
                KeyPath = @"SYSTEM\CurrentControlSet\Control\Terminal Server",
                ValueName = "fDenyTSConnections",
                ValueKind = RegistryValueKind.DWord,
                OnValue = 1, OffValue = 0, MissingMeansValue = 0
            },
            new()
            {
                Key = "TwDisableAutoplay",
                Title = loc["TwDisableAutoplay_Title"],
                Description = loc["TwDisableAutoplay_Desc"],
                Category = loc["CatPrivacy"],
                Hive = RegistryHive.CurrentUser,
                KeyPath = @"Software\Microsoft\Windows\CurrentVersion\Policies\Explorer",
                ValueName = "NoAutoplayfornonVolume",
                ValueKind = RegistryValueKind.DWord,
                OnValue = 1, OffValue = 0, MissingMeansValue = 0
            },
            new()
            {
                Key = "TwDisableRemoteRegistry",
                Title = loc["TwDisableRemoteRegistry_Title"],
                Description = loc["TwDisableRemoteRegistry_Desc"],
                Category = loc["CatPrivacy"],
                Hive = RegistryHive.LocalMachine,
                KeyPath = @"SYSTEM\CurrentControlSet\Services\RemoteRegistry",
                ValueName = "Start",
                ValueKind = RegistryValueKind.DWord,
                OnValue = 4, OffValue = 3, MissingMeansValue = 3
            },
            new()
            {
                Key = "TwDisableSharedExperiences",
                Title = loc["TwDisableSharedExperiences_Title"],
                Description = loc["TwDisableSharedExperiences_Desc"],
                Category = loc["CatPrivacy"],
                Hive = RegistryHive.CurrentUser,
                KeyPath = @"Software\Microsoft\Windows\CurrentVersion\CDP",
                ValueName = "RomeSdkChannelUserAuthzPolicy",
                ValueKind = RegistryValueKind.DWord,
                OnValue = 0, OffValue = 1, MissingMeansValue = 1
            },
            new()
            {
                Key = "TwDisableCloudClipboardSync",
                Title = loc["TwDisableCloudClipboardSync_Title"],
                Description = loc["TwDisableCloudClipboardSync_Desc"],
                Category = loc["CatPrivacy"],
                Hive = RegistryHive.CurrentUser,
                KeyPath = @"Software\Microsoft\Clipboard",
                ValueName = "CloudClipboardAutomaticUpload",
                ValueKind = RegistryValueKind.DWord,
                OnValue = 0, OffValue = 1, MissingMeansValue = 1
            },
            new()
            {
                Key = "TwStartupDelay",
                Title = loc["TwStartupDelay_Title"],
                Description = loc["TwStartupDelay_Desc"],
                Category = loc["CatSystem"],
                Hive = RegistryHive.CurrentUser,
                KeyPath = @"Software\Microsoft\Windows\CurrentVersion\Explorer\Serialize",
                ValueName = "StartupDelayInMSec",
                ValueKind = RegistryValueKind.DWord,
                OnValue = 0, OffValue = 1, MissingMeansValue = 1
            },
            new()
            {
                Key = "TwLowDiskSpaceWarning",
                Title = loc["TwLowDiskSpaceWarning_Title"],
                Description = loc["TwLowDiskSpaceWarning_Desc"],
                Category = loc["CatSystem"],
                Hive = RegistryHive.CurrentUser,
                KeyPath = @"Software\Microsoft\Windows\CurrentVersion\Policies\Explorer",
                ValueName = "NoLowDiskSpaceChecks",
                ValueKind = RegistryValueKind.DWord,
                OnValue = 1, OffValue = 0, MissingMeansValue = 0
            },
            new()
            {
                Key = "TwDisableAutoRestartSignIn",
                Title = loc["TwDisableAutoRestartSignIn_Title"],
                Description = loc["TwDisableAutoRestartSignIn_Desc"],
                Category = loc["CatSystem"],
                Hive = RegistryHive.LocalMachine,
                KeyPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System",
                ValueName = "DisableAutomaticRestartSignOn",
                ValueKind = RegistryValueKind.DWord,
                OnValue = 1, OffValue = 0, MissingMeansValue = 0
            },
            new()
            {
                Key = "TwDisableEdgeStartupBoost",
                Title = loc["TwDisableEdgeStartupBoost_Title"],
                Description = loc["TwDisableEdgeStartupBoost_Desc"],
                Category = loc["CatSystem"],
                Hive = RegistryHive.LocalMachine,
                KeyPath = @"SOFTWARE\Policies\Microsoft\Edge",
                ValueName = "StartupBoostEnabled",
                ValueKind = RegistryValueKind.DWord,
                OnValue = 0, OffValue = 1, MissingMeansValue = 1
            },
            new()
            {
                Key = "TwDisableEdgeBackgroundMode",
                Title = loc["TwDisableEdgeBackgroundMode_Title"],
                Description = loc["TwDisableEdgeBackgroundMode_Desc"],
                Category = loc["CatSystem"],
                Hive = RegistryHive.LocalMachine,
                KeyPath = @"SOFTWARE\Policies\Microsoft\Edge",
                ValueName = "BackgroundModeEnabled",
                ValueKind = RegistryValueKind.DWord,
                OnValue = 0, OffValue = 1, MissingMeansValue = 1
            },
            new()
            {
                Key = "TwDisableCopilot",
                Title = loc["TwDisableCopilot_Title"],
                Description = loc["TwDisableCopilot_Desc"],
                Category = loc["CatSystem"],
                Hive = RegistryHive.LocalMachine,
                KeyPath = @"SOFTWARE\Policies\Microsoft\Windows\WindowsCopilot",
                ValueName = "TurnOffWindowsCopilot",
                ValueKind = RegistryValueKind.DWord,
                OnValue = 1, OffValue = 0, MissingMeansValue = 0,
                RequiresExplorerRestart = true
            },
            new()
            {
                Key = "TwDisableRecall",
                Title = loc["TwDisableRecall_Title"],
                Description = loc["TwDisableRecall_Desc"],
                Category = loc["CatSystem"],
                Hive = RegistryHive.LocalMachine,
                KeyPath = @"SOFTWARE\Policies\Microsoft\Windows\WindowsAI",
                ValueName = "DisableAIDataAnalysis",
                ValueKind = RegistryValueKind.DWord,
                OnValue = 1, OffValue = 0, MissingMeansValue = 0
            },
            new()
            {
                Key = "TwDisableDeliveryOptimization",
                Title = loc["TwDisableDeliveryOptimization_Title"],
                Description = loc["TwDisableDeliveryOptimization_Desc"],
                Category = loc["CatSystem"],
                Hive = RegistryHive.LocalMachine,
                KeyPath = @"SOFTWARE\Policies\Microsoft\Windows\DeliveryOptimization",
                ValueName = "DODownloadMode",
                ValueKind = RegistryValueKind.DWord,
                OnValue = 0, OffValue = 1, MissingMeansValue = 1
            },
            new()
            {
                Key = "TwDisableFastUserSwitching",
                Title = loc["TwDisableFastUserSwitching_Title"],
                Description = loc["TwDisableFastUserSwitching_Desc"],
                Category = loc["CatSystem"],
                Hive = RegistryHive.LocalMachine,
                KeyPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System",
                ValueName = "HideFastUserSwitching",
                ValueKind = RegistryValueKind.DWord,
                OnValue = 1, OffValue = 0, MissingMeansValue = 0
            },
            new()
            {
                Key = "TwDisableInsiderService",
                Title = loc["TwDisableInsiderService_Title"],
                Description = loc["TwDisableInsiderService_Desc"],
                Category = loc["CatSystem"],
                Hive = RegistryHive.LocalMachine,
                KeyPath = @"SYSTEM\CurrentControlSet\Services\wisvc",
                ValueName = "Start",
                ValueKind = RegistryValueKind.DWord,
                OnValue = 4, OffValue = 3, MissingMeansValue = 3
            },
            new()
            {
                Key = "TwDisableLLMNR",
                Title = loc["TwDisableLLMNR_Title"],
                Description = loc["TwDisableLLMNR_Desc"],
                Category = loc["CatNetwork"],
                Hive = RegistryHive.LocalMachine,
                KeyPath = @"SOFTWARE\Policies\Microsoft\Windows NT\DNSClient",
                ValueName = "EnableMulticast",
                ValueKind = RegistryValueKind.DWord,
                OnValue = 0, OffValue = 1, MissingMeansValue = 1
            },


            // --- Round 4: v2.0 expansion (auto-generated) ---
            new()
            {
                Key = "TwDisableStickyKeysHotkey",
                Title = loc["TwDisableStickyKeysHotkey_Title"],
                Description = loc["TwDisableStickyKeysHotkey_Desc"],
                Category = loc["CatSystem"],
                Hive = RegistryHive.CurrentUser,
                KeyPath = @"Control Panel\Accessibility\StickyKeys",
                ValueName = "Flags",
                ValueKind = RegistryValueKind.String,
                OnValue = "506", OffValue = "58", MissingMeansValue = "506"
            },
            new()
            {
                Key = "TwDisableToggleKeysHotkey",
                Title = loc["TwDisableToggleKeysHotkey_Title"],
                Description = loc["TwDisableToggleKeysHotkey_Desc"],
                Category = loc["CatSystem"],
                Hive = RegistryHive.CurrentUser,
                KeyPath = @"Control Panel\Accessibility\ToggleKeys",
                ValueName = "Flags",
                ValueKind = RegistryValueKind.String,
                OnValue = "62", OffValue = "58", MissingMeansValue = "62"
            },
            new()
            {
                Key = "TwDisableAeroShake",
                Title = loc["TwDisableAeroShake_Title"],
                Description = loc["TwDisableAeroShake_Desc"],
                Category = loc["CatPersonalization"],
                Hive = RegistryHive.CurrentUser,
                KeyPath = @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced",
                ValueName = "DisallowShaking",
                ValueKind = RegistryValueKind.DWord,
                OnValue = 1, OffValue = 0, MissingMeansValue = 0,
                RequiresExplorerRestart = true
            },
            new()
            {
                Key = "TwDisableStartupTiles",
                Title = loc["TwDisableStartupTiles_Title"],
                Description = loc["TwDisableStartupTiles_Desc"],
                Category = loc["CatPersonalization"],
                Hive = RegistryHive.CurrentUser,
                KeyPath = @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced",
                ValueName = "NoStartupSoundEffects",
                ValueKind = RegistryValueKind.DWord,
                OnValue = 1, OffValue = 0, MissingMeansValue = 0
            },
            new()
            {
                Key = "TwDisableAppCompatEngine",
                Title = loc["TwDisableAppCompatEngine_Title"],
                Description = loc["TwDisableAppCompatEngine_Desc"],
                Category = loc["CatPrivacy"],
                Hive = RegistryHive.LocalMachine,
                KeyPath = @"SOFTWARE\Policies\Microsoft\Windows\AppCompat",
                ValueName = "DisableInventory",
                ValueKind = RegistryValueKind.DWord,
                OnValue = 1, OffValue = 0, MissingMeansValue = 0
            },
            new()
            {
                Key = "TwDisableHandwritingErrorReports",
                Title = loc["TwDisableHandwritingErrorReports_Title"],
                Description = loc["TwDisableHandwritingErrorReports_Desc"],
                Category = loc["CatPrivacy"],
                Hive = RegistryHive.CurrentUser,
                KeyPath = @"Software\Microsoft\Windows\CurrentVersion\TabletPC",
                ValueName = "PreventHandwritingErrorReports",
                ValueKind = RegistryValueKind.DWord,
                OnValue = 1, OffValue = 0, MissingMeansValue = 0
            },
            new()
            {
                Key = "TwDisableFirstLogonAnimation",
                Title = loc["TwDisableFirstLogonAnimation_Title"],
                Description = loc["TwDisableFirstLogonAnimation_Desc"],
                Category = loc["CatSystem"],
                Hive = RegistryHive.LocalMachine,
                KeyPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System",
                ValueName = "EnableFirstLogonAnimation",
                ValueKind = RegistryValueKind.DWord,
                OnValue = 0, OffValue = 1, MissingMeansValue = 1
            },
            new()
            {
                Key = "TwDisableSmartScreenExplorer",
                Title = loc["TwDisableSmartScreenExplorer_Title"],
                Description = loc["TwDisableSmartScreenExplorer_Desc"],
                Category = loc["CatSystem"],
                Hive = RegistryHive.CurrentUser,
                KeyPath = @"Software\Microsoft\Windows\CurrentVersion\Policies\Attachments",
                ValueName = "SaveZoneInformation",
                ValueKind = RegistryValueKind.DWord,
                OnValue = 1, OffValue = 2, MissingMeansValue = 2
            },
            new()
            {
                Key = "TwEnableClearTypeText",
                Title = loc["TwEnableClearTypeText_Title"],
                Description = loc["TwEnableClearTypeText_Desc"],
                Category = loc["CatPersonalization"],
                Hive = RegistryHive.CurrentUser,
                KeyPath = @"Control Panel\Desktop",
                ValueName = "FontSmoothing",
                ValueKind = RegistryValueKind.String,
                OnValue = "2", OffValue = "0", MissingMeansValue = "2",
                RequiresExplorerRestart = true
            },
            new()
            {
                Key = "TwDisableOfferRemoteAssistance",
                Title = loc["TwDisableOfferRemoteAssistance_Title"],
                Description = loc["TwDisableOfferRemoteAssistance_Desc"],
                Category = loc["CatPrivacy"],
                Hive = RegistryHive.LocalMachine,
                KeyPath = @"SYSTEM\CurrentControlSet\Control\Remote Assistance",
                ValueName = "fAllowUnsolicited",
                ValueKind = RegistryValueKind.DWord,
                OnValue = 0, OffValue = 1, MissingMeansValue = 1
            },


            // --- Security tab: ransomware / screen-locker prevention (auto-generated) ---
            new()
            {
                Key = "TwRansomwareProtection",
                Title = loc["TwRansomwareProtection_Title"],
                Description = loc["TwRansomwareProtection_Desc"],
                Category = loc["CatSecurity"],
                Hive = RegistryHive.LocalMachine,
                KeyPath = @"SOFTWARE\Microsoft\Windows Defender\Windows Defender Exploit Guard\Controlled Folder Access",
                ValueName = "EnableControlledFolderAccess",
                ValueKind = RegistryValueKind.DWord,
                OnValue = 1, OffValue = 0, MissingMeansValue = 0
            },
            new()
            {
                Key = "TwAsrRansomware",
                Title = loc["TwAsrRansomware_Title"],
                Description = loc["TwAsrRansomware_Desc"],
                Category = loc["CatSecurity"],
                Hive = RegistryHive.LocalMachine,
                KeyPath = @"SOFTWARE\Microsoft\Windows Defender\Windows Defender Exploit Guard\ASR\Rules",
                ValueName = "c1db55ab-c21a-4637-bb3f-a12568109d35",
                ValueKind = RegistryValueKind.DWord,
                OnValue = 1, OffValue = 0, MissingMeansValue = 0
            },
            new()
            {
                Key = "TwAsrEmailExec",
                Title = loc["TwAsrEmailExec_Title"],
                Description = loc["TwAsrEmailExec_Desc"],
                Category = loc["CatSecurity"],
                Hive = RegistryHive.LocalMachine,
                KeyPath = @"SOFTWARE\Microsoft\Windows Defender\Windows Defender Exploit Guard\ASR\Rules",
                ValueName = "be9ba2d9-53ea-4cdc-84e5-9b1eeee46550",
                ValueKind = RegistryValueKind.DWord,
                OnValue = 1, OffValue = 0, MissingMeansValue = 0
            },
            new()
            {
                Key = "TwAsrObfuscatedScripts",
                Title = loc["TwAsrObfuscatedScripts_Title"],
                Description = loc["TwAsrObfuscatedScripts_Desc"],
                Category = loc["CatSecurity"],
                Hive = RegistryHive.LocalMachine,
                KeyPath = @"SOFTWARE\Microsoft\Windows Defender\Windows Defender Exploit Guard\ASR\Rules",
                ValueName = "5beb7efe-fd9a-4556-801d-275e5ffc04cc",
                ValueKind = RegistryValueKind.DWord,
                OnValue = 1, OffValue = 0, MissingMeansValue = 0
            },
            new()
            {
                Key = "TwSmartScreenAppsFiles",
                Title = loc["TwSmartScreenAppsFiles_Title"],
                Description = loc["TwSmartScreenAppsFiles_Desc"],
                Category = loc["CatSecurity"],
                Hive = RegistryHive.LocalMachine,
                KeyPath = @"SOFTWARE\Policies\Microsoft\Windows\System",
                ValueName = "EnableSmartScreen",
                ValueKind = RegistryValueKind.DWord,
                OnValue = 1, OffValue = 0, MissingMeansValue = 0
            },
            new()
            {
                Key = "TwFirewallPrivateProfile",
                Title = loc["TwFirewallPrivateProfile_Title"],
                Description = loc["TwFirewallPrivateProfile_Desc"],
                Category = loc["CatSecurity"],
                Hive = RegistryHive.LocalMachine,
                KeyPath = @"SYSTEM\CurrentControlSet\Services\SharedAccess\Parameters\FirewallPolicy\StandardProfile",
                ValueName = "EnableFirewall",
                ValueKind = RegistryValueKind.DWord,
                OnValue = 1, OffValue = 0, MissingMeansValue = 1
            },
            new()
            {
                Key = "TwFirewallPublicProfile",
                Title = loc["TwFirewallPublicProfile_Title"],
                Description = loc["TwFirewallPublicProfile_Desc"],
                Category = loc["CatSecurity"],
                Hive = RegistryHive.LocalMachine,
                KeyPath = @"SYSTEM\CurrentControlSet\Services\SharedAccess\Parameters\FirewallPolicy\PublicProfile",
                ValueName = "EnableFirewall",
                ValueKind = RegistryValueKind.DWord,
                OnValue = 1, OffValue = 0, MissingMeansValue = 1
            },
            new()
            {
                Key = "TwUacSecureDefault",
                Title = loc["TwUacSecureDefault_Title"],
                Description = loc["TwUacSecureDefault_Desc"],
                Category = loc["CatSecurity"],
                Hive = RegistryHive.LocalMachine,
                KeyPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System",
                ValueName = "ConsentPromptBehaviorAdmin",
                ValueKind = RegistryValueKind.DWord,
                OnValue = 5, OffValue = 0, MissingMeansValue = 5
            },
        };
        return items;
    }

    /// <summary>Reads the current on/off state for a tweak from the registry.</summary>
    public bool ReadState(TweakItem tweak)
    {
        using var baseKey = RegistryKey.OpenBaseKey(tweak.Hive, RegistryView.Registry64);
        using var key = baseKey.OpenSubKey(tweak.KeyPath, writable: false);
        var current = key?.GetValue(tweak.ValueName.Length == 0 ? null : tweak.ValueName);

        if (current is null)
            return Equals(tweak.MissingMeansValue, tweak.OnValue);

        return ValuesEqual(current, tweak.OnValue);
    }

    /// <summary>Applies the on/off state for a tweak, creating the key path if needed.</summary>
    public void ApplyState(TweakItem tweak, bool enable)
    {
        using var baseKey = RegistryKey.OpenBaseKey(tweak.Hive, RegistryView.Registry64);

        var target = enable ? tweak.OnValue : tweak.OffValue;

        if (target is string s && s == "__DELETE__")
        {
            baseKey.DeleteSubKeyTree(tweak.KeyPath, throwOnMissingSubKey: false);
            return;
        }

        using var key = baseKey.CreateSubKey(tweak.KeyPath, writable: true)
            ?? throw new InvalidOperationException($"Could not open/create key: {tweak.KeyPath}");

        key.SetValue(tweak.ValueName.Length == 0 ? null : tweak.ValueName, target!, tweak.ValueKind);
    }

    private static bool ValuesEqual(object regValue, object expected)
    {
        // Registry DWORDs come back as int; compare loosely across numeric/string types.
        return string.Equals(Convert.ToString(regValue), Convert.ToString(expected), StringComparison.OrdinalIgnoreCase);
    }
}
