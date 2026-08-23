using System.Collections.Generic;
using System.ComponentModel;

namespace KleopatraKindomTweaker.Services;

public enum AppLanguage { Ru, En, Uk }

/// <summary>
/// Singleton translation source. XAML binds to it like:
/// Text="{Binding Source={x:Static svc:LocalizationService.Instance}, Path=[NavDashboard]}"
/// Changing Language raises a change on the indexer, which refreshes every such binding at once.
/// </summary>
public class LocalizationService : INotifyPropertyChanged
{
    public static readonly LocalizationService Instance = new();

    private AppLanguage _language = AppLanguage.Ru;
    public AppLanguage Language
    {
        get => _language;
        set
        {
            if (_language == value) return;
            _language = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Item[]"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Language)));
        }
    }

    public string this[string key] =>
        _map.TryGetValue(key, out var t)
            ? _language switch { AppLanguage.En => t.En, AppLanguage.Uk => t.Uk, _ => t.Ru }
            : key;

    /// <summary>Convenience: translate a key and apply string.Format with the given args.</summary>
    public string Format(string key, params object[] args) => string.Format(this[key], args);

    public event PropertyChangedEventHandler? PropertyChanged;

    private static readonly Dictionary<string, (string Ru, string En, string Uk)> _map = new()
    {
        // Sidebar
        ["NavDashboard"] = ("🏠  Панель", "🏠  Dashboard", "🏠  Панель"),
        ["NavTweaks"] = ("🛠  Системные твики", "🛠  System Tweaks", "🛠  Системні твіки"),
        ["NavProcesses"] = ("📊  Менеджер процессов", "📊  Process Manager", "📊  Менеджер процесів"),
        ["NavCleanup"] = ("🧹  Очистка системы", "🧹  System Cleanup", "🧹  Очищення системи"),
        ["NavSystemInfo"] = ("💻  Инфо о системе", "💻  System Info", "💻  Інфо про систему"),
        ["NavSettings"] = ("⚙  Настройки", "⚙  Settings", "⚙  Налаштування"),

        // Dashboard
        ["DashboardTitle"] = ("Панель", "Dashboard", "Панель"),
        ["QuickOptimizeTitle"] = ("Быстрая настройка", "Quick Optimize", "Швидке налаштування"),
        ["QuickOptimizeDesc"] = (
            "Одним нажатием применяет набор рекомендованных твиков: отключает телеметрию, рекламный ID, Cortana, веб-поиск в Пуск и анимации окон.",
            "Applies a set of recommended tweaks in one click: disables telemetry, advertising ID, Cortana, Start menu web search and window animations.",
            "Одним натисканням застосовує набір рекомендованих твіків: вимикає телеметрію, рекламний ID, Cortana, веб-пошук у Пуску та анімації вікон."),
        ["QuickOptimizeButton"] = ("Применить рекомендуемые твики", "Apply recommended tweaks", "Застосувати рекомендовані твіки"),
        ["SystemCardTitle"] = ("Система", "System", "Система"),

        // Tweaks
        ["TweaksTitle"] = ("Системные твики", "System Tweaks", "Системні твіки"),
        ["SelectAllButton"] = ("Выбрать все", "Select all", "Вибрати всі"),
        ["DeselectAllButton"] = ("Снять все", "Deselect all", "Зняти всі"),

        // Process manager
        ["ProcessesTitle"] = ("Менеджер процессов", "Process Manager", "Менеджер процесів"),
        ["RefreshButton"] = ("Обновить", "Refresh", "Оновити"),

        // Cleanup
        ["CleanupTitle"] = ("Очистка системы", "System Cleanup", "Очищення системи"),
        ["ScanButton"] = ("Сканировать", "Scan", "Сканувати"),
        ["CleanSelectedButton"] = ("Очистить выбранное", "Clean selected", "Очистити вибране"),

        // System info
        ["SystemInfoTitle"] = ("Информация о системе", "System Information", "Інформація про систему"),
        ["DisksTitle"] = ("Диски", "Disks", "Диски"),

        // Settings
        ["SettingsTitle"] = ("Настройки", "Settings", "Налаштування"),
        ["ThemeSectionTitle"] = ("Тема оформления", "Theme", "Тема оформлення"),
        ["DarkThemeButton"] = ("🌙 Тёмная", "🌙 Dark", "🌙 Темна"),
        ["LightThemeButton"] = ("☀ Светлая", "☀ Light", "☀ Світла"),
        ["LanguageSectionTitle"] = ("Язык", "Language", "Мова"),
        ["LangRu"] = ("Русский", "Russian", "Російська"),
        ["LangEn"] = ("Английский", "English", "Англійська"),
        ["LangUk"] = ("Украинский", "Ukrainian", "Українська"),
        ["Status_AdminRequired"] = ("Недостаточно прав. Запусти приложение от имени администратора.",
                                     "Not enough permissions. Run the app as administrator.",
                                     "Недостатньо прав. Запусти застосунок від імені адміністратора."),
        ["AboutSectionTitle"] = ("О программе", "About", "Про програму"),
        ["AboutText"] = ("KleopatraKindomTweaker v2.0 by KleoptraKindom, Inc.",
                          "KleopatraKindomTweaker v2.0 by KleoptraKindom, Inc.",
                          "KleopatraKindomTweaker v2.0 by KleoptraKindom, Inc."),

        // Dashboard summary (code-behind formatted strings)
        ["SummaryOs"] = ("ОС: {0}", "OS: {0}", "ОС: {0}"),
        ["SummaryCpu"] = ("Процессор: {0} ({1} ядер / {2} потоков)", "CPU: {0} ({1} cores / {2} threads)", "Процесор: {0} ({1} ядер / {2} потоків)"),
        ["SummaryRam"] = ("Память: {0} ГБ свободно из {1} ГБ", "Memory: {0} GB free of {1} GB", "Пам'ять: {0} ГБ вільно з {1} ГБ"),
        ["SummaryGpu"] = ("Видеокарта: {0}", "GPU: {0}", "Відеокарта: {0}"),
        ["SummaryError"] = ("Не удалось получить данные о системе: {0}", "Failed to get system data: {0}", "Не вдалося отримати дані про систему: {0}"),
        ["QuickStatusOk"] = ("Готово: применено {0} твиков.", "Done: applied {0} tweaks.", "Готово: застосовано {0} твіків."),
        ["QuickStatusPartial"] = ("Применено {0}, не удалось {1} (нужны права администратора).", "Applied {0}, failed {1} (administrator rights needed).", "Застосовано {0}, не вдалося {1} (потрібні права адміністратора)."),

        // Tweaks page status
        ["TweakAppliedRestart"] = ("«{0}» применено. Может понадобиться перезапуск Проводника.", "\"{0}\" applied. Explorer restart may be needed.", "«{0}» застосовано. Може знадобитися перезапуск Провідника."),
        ["TweakApplied"] = ("«{0}» применено.", "\"{0}\" applied.", "«{0}» застосовано."),
        ["TweakNoPermission"] = ("Недостаточно прав. Запусти приложение от имени администратора.", "Not enough permissions. Run the app as administrator.", "Недостатньо прав. Запусти застосунок від імені адміністратора."),
        ["TweakError"] = ("Ошибка: {0}", "Error: {0}", "Помилка: {0}"),
        ["TweakReadError"] = ("Не удалось прочитать «{0}»: {1}", "Failed to read \"{0}\": {1}", "Не вдалося прочитати «{0}»: {1}"),

        // Tweak categories
        ["CatPersonalization"] = ("Персонализация", "Personalization", "Персоналізація"),
        ["CatPrivacy"] = ("Приватность", "Privacy", "Приватність"),
        ["CatPerformance"] = ("Производительность", "Performance", "Продуктивність"),
        ["CatGames"] = ("Игры", "Games", "Ігри"),

        // Individual tweaks: title / description
        ["TwDarkTheme_Title"] = ("Тёмная тема (Проводник и приложения)", "Dark theme (Explorer and apps)", "Темна тема (Провідник і застосунки)"),
        ["TwDarkTheme_Desc"] = ("Переключает системную тему оформления на тёмную.", "Switches the system color theme to dark.", "Перемикає системну тему оформлення на темну."),
        ["TwFileExt_Title"] = ("Показывать расширения файлов", "Show file extensions", "Показувати розширення файлів"),
        ["TwFileExt_Desc"] = ("Отображает расширения (.txt, .exe и т.д.) в Проводнике.", "Shows extensions (.txt, .exe, etc.) in Explorer.", "Відображає розширення (.txt, .exe тощо) у Провіднику."),
        ["TwHiddenFiles_Title"] = ("Показывать скрытые файлы", "Show hidden files", "Показувати приховані файли"),
        ["TwHiddenFiles_Desc"] = ("Отображает скрытые файлы и папки в Проводнике.", "Shows hidden files and folders in Explorer.", "Відображає приховані файли та папки у Провіднику."),
        ["TwClassicMenu_Title"] = ("Классическое контекстное меню (Win11)", "Classic context menu (Win11)", "Класичне контекстне меню (Win11)"),
        ["TwClassicMenu_Desc"] = ("Возвращает полное контекстное меню правой кнопки мыши из Windows 10.", "Restores the full right-click context menu from Windows 10.", "Повертає повне контекстне меню правої кнопки миші з Windows 10."),
        ["TwTelemetry_Title"] = ("Отключить сбор телеметрии", "Disable telemetry collection", "Вимкнути збір телеметрії"),
        ["TwTelemetry_Desc"] = ("Ограничивает уровень сбора диагностических данных Windows до минимума.", "Limits Windows diagnostic data collection to the minimum.", "Обмежує рівень збору діагностичних даних Windows до мінімуму."),
        ["TwAdId_Title"] = ("Отключить рекламный ID", "Disable advertising ID", "Вимкнути рекламний ID"),
        ["TwAdId_Desc"] = ("Запрещает приложениям использовать персональный рекламный идентификатор.", "Prevents apps from using the personalized advertising ID.", "Забороняє застосункам використовувати персональний рекламний ідентифікатор."),
        ["TwBingSearch_Title"] = ("Отключить веб-поиск в меню Пуск", "Disable Start menu web search", "Вимкнути веб-пошук у меню Пуск"),
        ["TwBingSearch_Desc"] = ("Поиск в меню Пуск ищет только по локальным файлам и приложениям.", "Start menu search only searches local files and apps.", "Пошук у меню Пуск шукає лише локальні файли й застосунки."),
        ["TwCortana_Title"] = ("Отключить Cortana", "Disable Cortana", "Вимкнути Cortana"),
        ["TwCortana_Desc"] = ("Полностью отключает голосового помощника Cortana.", "Completely disables the Cortana voice assistant.", "Повністю вимикає голосового помічника Cortana."),
        ["TwAnimations_Title"] = ("Отключить анимации окон", "Disable window animations", "Вимкнути анімації вікон"),
        ["TwAnimations_Desc"] = ("Убирает анимации сворачивания/разворачивания окон для отзывчивости UI.", "Removes minimize/restore window animations for a snappier UI.", "Прибирає анімації згортання/розгортання вікон для швидшого UI."),
        ["TwTransparency_Title"] = ("Отключить прозрачность интерфейса", "Disable UI transparency", "Вимкнути прозорість інтерфейсу"),
        ["TwTransparency_Desc"] = ("Отключает эффекты прозрачности панели задач и меню Пуск.", "Disables transparency effects on the taskbar and Start menu.", "Вимикає ефекти прозорості панелі завдань і меню Пуск."),
        ["TwPowerThrottle_Title"] = ("Высокая производительность плана питания", "High-performance power plan", "Висока продуктивність плану живлення"),
        ["TwPowerThrottle_Desc"] = ("Отключает энергосберегающий фоновый режим приложений (Power Throttling).", "Disables background app power throttling.", "Вимикає енергозберігаючий фоновий режим застосунків (Power Throttling)."),
        ["TwGameDvr_Title"] = ("Отключить Game DVR / Game Bar", "Disable Game DVR / Game Bar", "Вимкнути Game DVR / Game Bar"),
        ["TwGameDvr_Desc"] = ("Снижает нагрузку и задержки при записи геймплея в фоне.", "Reduces load and input lag from background gameplay recording.", "Знижує навантаження й затримки від фонового запису геймплею."),
        ["TwMouseAccel_Title"] = ("Отключить ускорение указателя мыши", "Disable mouse pointer acceleration", "Вимкнути прискорення вказівника миші"),
        ["TwMouseAccel_Desc"] = ("Делает движение курсора линейным (1:1), полезно для игр/дизайна.", "Makes cursor movement linear (1:1), useful for gaming/design.", "Робить рух курсора лінійним (1:1), корисно для ігор/дизайну."),

        // --- New tweaks, added to match MakuTweaker's feature set ---
        ["CatWindowsUpdate"] = ("Windows Update", "Windows Update", "Windows Update"),

        ["TwDisableWU_Title"] = ("Отключить автоматические обновления", "Disable automatic updates", "Вимкнути автоматичні оновлення"),
        ["TwDisableWU_Desc"] = ("Windows перестаёт скачивать и устанавливать обновления автоматически.", "Windows stops downloading and installing updates automatically.", "Windows перестає автоматично завантажувати й встановлювати оновлення."),
        ["TwDisableWUReboot_Title"] = ("Отключить авто-перезагрузку после обновлений", "Disable auto-restart after updates", "Вимкнути авто-перезавантаження після оновлень"),
        ["TwDisableWUReboot_Desc"] = ("Не даёт Windows перезагружать компьютер самостоятельно, пока вы за ним работаете.", "Stops Windows from restarting the PC on its own while you're using it.", "Не дає Windows перезавантажувати комп'ютер самостійно, поки ви за ним працюєте."),

        ["TwWidgets_Title"] = ("Отключить виджеты на панели задач", "Disable taskbar widgets", "Вимкнути віджети на панелі завдань"),
        ["TwWidgets_Desc"] = ("Убирает иконку виджетов (погода/новости) с панели задач.", "Removes the widgets icon (weather/news) from the taskbar.", "Прибирає значок віджетів (погода/новини) з панелі завдань."),
        ["TwChatIcon_Title"] = ("Отключить значок Chat/Teams на панели задач", "Disable taskbar Chat/Teams icon", "Вимкнути значок Chat/Teams на панелі завдань"),
        ["TwChatIcon_Desc"] = ("Убирает встроенную иконку чата Teams с панели задач.", "Removes the built-in Teams chat icon from the taskbar.", "Прибирає вбудований значок чату Teams з панелі завдань."),
        ["TwStartRecommendations_Title"] = ("Отключить рекомендации в меню Пуск", "Disable Start menu recommendations", "Вимкнути рекомендації в меню Пуск"),
        ["TwStartRecommendations_Desc"] = ("Убирает блок «Рекомендуемое» (недавние файлы/приложения) из меню Пуск.", "Removes the \"Recommended\" section (recent files/apps) from the Start menu.", "Прибирає блок «Рекомендоване» (нещодавні файли/застосунки) з меню Пуск."),

        ["TwBackgroundApps_Title"] = ("Отключить фоновые приложения", "Disable background apps", "Вимкнути фонові застосунки"),
        ["TwBackgroundApps_Desc"] = ("Запрещает UWP-приложениям работать в фоне и расходовать батарею/ресурсы.", "Prevents UWP apps from running in the background and draining battery/resources.", "Забороняє UWP-застосункам працювати у фоні та витрачати батарею/ресурси."),

        ["TwAutorunUSB_Title"] = ("Отключить автозапуск съёмных накопителей", "Disable removable drive autorun", "Вимкнути автозапуск знімних накопичувачів"),
        ["TwAutorunUSB_Desc"] = ("Флешки и внешние диски больше не будут запускать autorun.inf при подключении.", "USB drives and external disks will no longer trigger autorun.inf on connect.", "Флешки та зовнішні диски більше не запускатимуть autorun.inf при підключенні."),
        ["TwNumLock_Title"] = ("Включать NumLock при загрузке", "Enable NumLock at startup", "Вмикати NumLock при завантаженні"),
        ["TwNumLock_Desc"] = ("NumLock будет включён сразу после входа в систему, а не только на экране блокировки.", "NumLock will be on right after sign-in, not just on the lock screen.", "NumLock буде увімкнено одразу після входу в систему, а не лише на екрані блокування."),

        // --- Even more tweaks ---
        ["TwTaskbarSearch_Title"] = ("Скрыть поле поиска на панели задач", "Hide taskbar search box", "Приховати поле пошуку на панелі завдань"),
        ["TwTaskbarSearch_Desc"] = ("Убирает поле/значок поиска с панели задач, оставляя поиск доступным через меню Пуск.", "Removes the search box/icon from the taskbar; search is still available from the Start menu.", "Прибирає поле/значок пошуку з панелі завдань, пошук лишається доступним через меню Пуск."),
        ["TwTaskbarAlignLeft_Title"] = ("Панель задач слева (как в Windows 10)", "Left-align taskbar (like Windows 10)", "Панель завдань зліва (як у Windows 10)"),
        ["TwTaskbarAlignLeft_Desc"] = ("Прижимает значки панели задач к левому краю вместо центра (Win11).", "Aligns taskbar icons to the left edge instead of the center (Win11 default).", "Притискає значки панелі завдань до лівого краю замість центру (за замовчуванням у Win11)."),
        ["TwShowSecondsClock_Title"] = ("Показывать секунды в системных часах", "Show seconds in the system clock", "Показувати секунди в системному годиннику"),
        ["TwShowSecondsClock_Desc"] = ("Добавляет секунды к времени на панели задач.", "Adds seconds to the taskbar clock display.", "Додає секунди до часу на панелі завдань."),

        ["TwLocationTracking_Title"] = ("Отключить отслеживание местоположения", "Disable location tracking", "Вимкнути відстеження місцезнаходження"),
        ["TwLocationTracking_Desc"] = ("Запрещает Windows и приложениям определять географическое положение устройства.", "Prevents Windows and apps from determining the device's geographic location.", "Забороняє Windows і застосункам визначати географічне розташування пристрою."),
        ["TwActivityHistory_Title"] = ("Отключить журнал действий (Timeline)", "Disable activity history (Timeline)", "Вимкнути журнал дій (Timeline)"),
        ["TwActivityHistory_Desc"] = ("Останавливает сбор и отправку истории активности в Microsoft.", "Stops collecting and sending activity history to Microsoft.", "Зупиняє збір та надсилання історії активності до Microsoft."),
        ["TwErrorReporting_Title"] = ("Отключить отчёты об ошибках Windows", "Disable Windows Error Reporting", "Вимкнути звіти про помилки Windows"),
        ["TwErrorReporting_Desc"] = ("Windows перестаёт автоматически отправлять отчёты о сбоях в Microsoft.", "Windows stops automatically sending crash reports to Microsoft.", "Windows перестає автоматично надсилати звіти про збої до Microsoft."),
        ["TwTailoredExperiences_Title"] = ("Отключить персонализированные рекомендации", "Disable tailored experiences", "Вимкнути персоналізовані рекомендації"),
        ["TwTailoredExperiences_Desc"] = ("Запрещает Windows использовать диагностические данные для персонализации советов и рекламы в системе.", "Prevents Windows from using diagnostic data to personalize tips and in-OS suggestions.", "Забороняє Windows використовувати діагностичні дані для персоналізації порад і реклами в системі."),

        ["TwMenuShowDelay_Title"] = ("Ускорить открытие меню", "Speed up menu opening", "Пришвидшити відкриття меню"),
        ["TwMenuShowDelay_Desc"] = ("Убирает задержку перед открытием подменю (например, в меню Пуск и Проводнике).", "Removes the delay before submenus open (e.g. in the Start menu and Explorer).", "Прибирає затримку перед відкриттям підменю (наприклад, у меню Пуск та Провіднику)."),
        ["TwVisualEffectsBest_Title"] = ("Визуальные эффекты: максимальная производительность", "Visual effects: best performance", "Візуальні ефекти: максимальна продуктивність"),
        ["TwVisualEffectsBest_Desc"] = ("Отключает большинство визуальных эффектов Windows ради быстродействия на слабом железе.", "Disables most Windows visual effects for better performance on weaker hardware.", "Вимикає більшість візуальних ефектів Windows заради швидкодії на слабкому залізі."),
        ["TwLastAccessUpdate_Title"] = ("Отключить обновление времени доступа к файлам (NTFS)", "Disable NTFS last-access timestamp updates", "Вимкнути оновлення часу доступу до файлів (NTFS)"),
        ["TwLastAccessUpdate_Desc"] = ("Снижает нагрузку на диск: Windows не обновляет метку последнего доступа при каждом открытии файла.", "Reduces disk load: Windows won't update the last-access timestamp every time a file is opened.", "Знижує навантаження на диск: Windows не оновлює мітку останнього доступу при кожному відкритті файлу."),
        ["TwNetworkThrottling_Title"] = ("Отключить ограничение пропускной способности сети", "Disable network throttling", "Вимкнути обмеження пропускної здатності мережі"),
        ["TwNetworkThrottling_Desc"] = ("Снимает системное ограничение на сетевой трафик мультимедиа-приложений, введённое для QoS.", "Removes the system-level QoS throttle on network traffic for multimedia applications.", "Знімає системне обмеження на мережевий трафік мультимедіа-застосунків, запроваджене для QoS."),

        ["TwGameMode_Title"] = ("Принудительно включить игровой режим", "Force-enable Game Mode", "Примусово увімкнути ігровий режим"),
        ["TwGameMode_Desc"] = ("Гарантирует, что Windows отдаёт приоритет ресурсам игре в полноэкранном режиме.", "Ensures Windows prioritizes system resources for the game running in fullscreen.", "Гарантує, що Windows віддає пріоритет ресурсам грі в повноекранному режимі."),

        // --- Even more tweaks, round 2 ---
        ["CatSystem"] = ("Система", "System", "Система"),
        ["CatNetwork"] = ("Сеть", "Network", "Мережа"),

        ["TwShowThisPC_Title"] = ("Показать значок «Этот компьютер» на рабочем столе", "Show \"This PC\" icon on desktop", "Показати значок «Цей комп'ютер» на робочому столі"),
        ["TwShowThisPC_Desc"] = ("Добавляет иконку «Этот компьютер» на рабочий стол.", "Adds the \"This PC\" icon to the desktop.", "Додає значок «Цей комп'ютер» на робочий стіл."),
        ["TwAccentColorTitlebar_Title"] = ("Цвет темы на заголовках окон", "Accent color on window title bars", "Колір теми на заголовках вікон"),
        ["TwAccentColorTitlebar_Desc"] = ("Красит заголовки окон и панель задач в выбранный акцентный цвет.", "Colors window title bars and the taskbar with the selected accent color.", "Забарвлює заголовки вікон і панель завдань у вибраний акцентний колір."),
        ["TwExplorerLaunchToThisPC_Title"] = ("Открывать Проводник с «Этот компьютер»", "Open Explorer to \"This PC\"", "Відкривати Провідник з «Цей комп'ютер»"),
        ["TwExplorerLaunchToThisPC_Desc"] = ("Проводник при запуске показывает «Этот компьютер» вместо «Быстрый доступ».", "Explorer opens to \"This PC\" instead of \"Quick access\" by default.", "Провідник під час запуску показує «Цей комп'ютер» замість «Швидкий доступ»."),
        ["TwRestoreFolderWindows_Title"] = ("Восстанавливать окна папок при входе в систему", "Restore folder windows at sign-in", "Відновлювати вікна папок при вході в систему"),
        ["TwRestoreFolderWindows_Desc"] = ("Проводник переоткрывает папки, которые были открыты при завершении сеанса.", "Explorer reopens the folders that were open when you last signed out.", "Провідник повторно відкриває папки, які були відкриті під час завершення сеансу."),

        ["TwOnlineSpeech_Title"] = ("Отключить распознавание речи в облаке", "Disable online speech recognition", "Вимкнути розпізнавання мовлення в хмарі"),
        ["TwOnlineSpeech_Desc"] = ("Запрещает Windows отправлять голосовые данные на серверы Microsoft для распознавания.", "Prevents Windows from sending voice data to Microsoft's servers for recognition.", "Забороняє Windows надсилати голосові дані на сервери Microsoft для розпізнавання."),
        ["TwClipboardHistory_Title"] = ("Отключить историю буфера обмена (Win+V)", "Disable clipboard history (Win+V)", "Вимкнути історію буфера обміну (Win+V)"),
        ["TwClipboardHistory_Desc"] = ("Отключает журнал буфера обмена, доступный по Win+V.", "Disables the clipboard history log accessible via Win+V.", "Вимикає журнал буфера обміну, доступний через Win+V."),
        ["TwInkTypingPersonalization_Title"] = ("Отключить персонализацию ввода текста", "Disable typing personalization", "Вимкнути персоналізацію введення тексту"),
        ["TwInkTypingPersonalization_Desc"] = ("Запрещает Windows собирать образцы ввода для улучшения предсказания текста.", "Prevents Windows from collecting typing samples to improve text prediction.", "Забороняє Windows збирати зразки введення для покращення передбачення тексту."),
        ["TwExplorerWebSearch_Title"] = ("Отключить веб-подсказки в поиске Проводника", "Disable web suggestions in Explorer search", "Вимкнути веб-підказки в пошуку Провідника"),
        ["TwExplorerWebSearch_Desc"] = ("Поиск в Проводнике перестаёт показывать результаты из интернета.", "Explorer search stops showing suggestions pulled from the web.", "Пошук у Провіднику перестає показувати результати з інтернету."),

        ["TwHibernate_Title"] = ("Отключить гибернацию", "Disable hibernation", "Вимкнути гібернацію"),
        ["TwHibernate_Desc"] = ("Убирает режим гибернации и освобождает место на диске, занятое файлом hiberfil.sys.", "Removes hibernation mode and frees the disk space used by hiberfil.sys.", "Прибирає режим гібернації та звільняє місце на диску, зайняте файлом hiberfil.sys."),
        ["TwSuperfetch_Title"] = ("Отключить службу SysMain (Superfetch)", "Disable SysMain (Superfetch) service", "Вимкнути службу SysMain (Superfetch)"),
        ["TwSuperfetch_Desc"] = ("Останавливает предзагрузку часто используемых приложений в память; может ускорить работу на SSD.", "Stops pre-loading frequently used apps into memory; can help on SSD-based systems.", "Зупиняє попереднє завантаження часто використовуваних застосунків у пам'ять; може пришвидшити роботу на SSD."),
        ["TwPrefetchParams_Title"] = ("Отключить Prefetch", "Disable Prefetch", "Вимкнути Prefetch"),
        ["TwPrefetchParams_Desc"] = ("Отключает кэширование запуска приложений; рекомендуется в основном на SSD.", "Disables application launch caching; mainly recommended on SSDs.", "Вимикає кешування запуску застосунків; рекомендовано переважно на SSD."),
        ["TwPagingExecutive_Title"] = ("Держать ядро системы в оперативной памяти", "Keep system kernel in RAM", "Тримати ядро системи в оперативній пам'яті"),
        ["TwPagingExecutive_Desc"] = ("Запрещает выгрузку ядра и драйверов в файл подкачки; полезно при большом объёме ОЗУ.", "Prevents the kernel and drivers from being paged to disk; useful with plenty of RAM.", "Забороняє вивантаження ядра й драйверів у файл підкачки; корисно за великого обсягу ОЗП."),
        ["TwLargeSystemCache_Title"] = ("Увеличенный системный кэш файлов", "Larger system file cache", "Збільшений системний кеш файлів"),
        ["TwLargeSystemCache_Desc"] = ("Отдаёт больше памяти под кэш файловой системы вместо приложений; полезно для файловых серверов.", "Gives more memory to the file system cache instead of applications; useful for file-server-like workloads.", "Віддає більше пам'яті під кеш файлової системи замість застосунків; корисно для файлових серверів."),
        ["TwWindowsSearchIndex_Title"] = ("Отключить индексирование поиска Windows", "Disable Windows Search indexing", "Вимкнути індексування пошуку Windows"),
        ["TwWindowsSearchIndex_Desc"] = ("Останавливает службу индексирования; поиск станет медленнее, но снизится нагрузка на диск.", "Stops the indexing service; search becomes slower but disk load drops.", "Зупиняє службу індексування; пошук стане повільнішим, але навантаження на диск знизиться."),
        ["TwFastStartup_Title"] = ("Отключить быстрый запуск", "Disable fast startup", "Вимкнути швидкий запуск"),
        ["TwFastStartup_Desc"] = ("Отключает гибридный режим загрузки Windows; полезно при проблемах с драйверами после включения.", "Disables Windows' hybrid boot mode; helps with driver issues that appear after power-on.", "Вимикає гібридний режим завантаження Windows; корисно за проблем із драйверами після ввімкнення."),
        ["TwLockScreen_Title"] = ("Отключить экран блокировки", "Disable the lock screen", "Вимкнути екран блокування"),
        ["TwLockScreen_Desc"] = ("Windows сразу показывает экран входа, минуя картинку блокировки.", "Windows goes straight to the sign-in screen, skipping the lock screen image.", "Windows одразу показує екран входу, минаючи зображення блокування."),

        ["TwDisableIPv6_Title"] = ("Отключить IPv6", "Disable IPv6", "Вимкнути IPv6"),
        ["TwDisableIPv6_Desc"] = ("Отключает протокол IPv6 на всех сетевых адаптерах; иногда ускоряет соединение в старых сетях.", "Disables the IPv6 protocol on all network adapters; can speed up connections on older networks.", "Вимикає протокол IPv6 на всіх мережевих адаптерах; іноді пришвидшує з'єднання у старих мережах."),

        // --- Round 3: bulk expansion translations (auto-generated) ---
        ["CatAdsSuggestions"] = ("Реклама и предложения", "Ads & Suggestions", "Реклама і пропозиції"),
        ["TwTaskbarLabels_Title"] = ("Никогда не группировать значки на панели задач", "Never combine taskbar icons", "Ніколи не групувати значки на панелі завдань"),
        ["TwTaskbarLabels_Desc"] = ("Каждое окно приложения получает отдельную кнопку на панели задач.", "Every app window gets its own separate taskbar button.", "Кожне вікно застосунку отримує окрему кнопку на панелі завдань."),
        ["TwShowRecycleBinDesktop_Title"] = ("Показать «Корзину» на рабочем столе", "Show \"Recycle Bin\" on desktop", "Показати «Кошик» на робочому столі"),
        ["TwShowRecycleBinDesktop_Desc"] = ("Добавляет иконку корзины на рабочий стол.", "Adds the Recycle Bin icon to the desktop.", "Додає значок кошика на робочий стіл."),
        ["TwShowControlPanelDesktop_Title"] = ("Показать «Панель управления» на рабочем столе", "Show \"Control Panel\" on desktop", "Показати «Панель керування» на робочому столі"),
        ["TwShowControlPanelDesktop_Desc"] = ("Добавляет иконку панели управления на рабочий стол.", "Adds the Control Panel icon to the desktop.", "Додає значок панелі керування на робочий стіл."),
        ["TwShowUserFolderDesktop_Title"] = ("Показать папку пользователя на рабочем столе", "Show user folder on desktop", "Показати папку користувача на робочому столі"),
        ["TwShowUserFolderDesktop_Desc"] = ("Добавляет иконку личной папки пользователя на рабочий стол.", "Adds the user's personal folder icon to the desktop.", "Додає значок особистої папки користувача на робочий стіл."),
        ["TwShowNetworkDesktop_Title"] = ("Показать «Сеть» на рабочем столе", "Show \"Network\" on desktop", "Показати «Мережа» на робочому столі"),
        ["TwShowNetworkDesktop_Desc"] = ("Добавляет иконку сетевого окружения на рабочий стол.", "Adds the network neighborhood icon to the desktop.", "Додає значок мережевого оточення на робочий стіл."),
        ["TwDisableTaskbarPeek_Title"] = ("Отключить просмотр рабочего стола (Aero Peek)", "Disable desktop peek (Aero Peek)", "Вимкнути перегляд робочого столу (Aero Peek)"),
        ["TwDisableTaskbarPeek_Desc"] = ("Отключает предпросмотр рабочего стола при наведении на край панели задач.", "Disables the desktop preview shown when hovering the taskbar's edge.", "Вимикає попередній перегляд робочого столу під час наведення на край панелі завдань."),
        ["TwFileExplorerCompactMode_Title"] = ("Компактный режим Проводника", "Compact mode for Explorer", "Компактний режим Провідника"),
        ["TwFileExplorerCompactMode_Desc"] = ("Уменьшает отступы между элементами списка файлов для более плотного отображения.", "Reduces spacing between file list items for a denser view.", "Зменшує відступи між елементами списку файлів для щільнішого відображення."),
        ["TwShowLibrariesExplorer_Title"] = ("Показывать все папки в дереве навигации", "Show all folders in the navigation pane", "Показувати всі папки в дереві навігації"),
        ["TwShowLibrariesExplorer_Desc"] = ("Отображает библиотеки и все папки в левой панели Проводника.", "Displays libraries and all folders in Explorer's left-hand pane.", "Відображає бібліотеки та всі папки в лівій панелі Провідника."),
        ["TwShowNavPaneExpand_Title"] = ("Разворачивать дерево до текущей папки", "Auto-expand to current folder", "Розгортати дерево до поточної папки"),
        ["TwShowNavPaneExpand_Desc"] = ("Дерево папок в Проводнике автоматически разворачивается до открытой папки.", "Explorer's folder tree automatically expands to the currently open folder.", "Дерево папок у Провіднику автоматично розгортається до відкритої папки."),
        ["TwDisableSharingWizard_Title"] = ("Отключить мастер общего доступа", "Disable the Sharing Wizard", "Вимкнути майстер спільного доступу"),
        ["TwDisableSharingWizard_Desc"] = ("Использует классическое диалоговое окно разрешений вместо мастера общего доступа.", "Uses the classic permissions dialog instead of the Sharing Wizard.", "Використовує класичне діалогове вікно дозволів замість майстра спільного доступу."),
        ["TwDisableBalloonTips_Title"] = ("Отключить всплывающие подсказки", "Disable balloon tips", "Вимкнути спливаючі підказки"),
        ["TwDisableBalloonTips_Desc"] = ("Отключает всплывающие текстовые подсказки в области уведомлений.", "Disables the pop-up balloon tips in the notification area.", "Вимикає спливаючі текстові підказки в області сповіщень."),
        ["TwDisableTaskbarBadging_Title"] = ("Отключить значки уведомлений на панели задач", "Disable taskbar notification badges", "Вимкнути значки сповіщень на панелі завдань"),
        ["TwDisableTaskbarBadging_Desc"] = ("Убирает числовые/точечные бейджи уведомлений на значках панели задач.", "Removes the numeric/dot notification badges on taskbar icons.", "Прибирає числові/крапкові бейджі сповіщень на значках панелі завдань."),
        ["TwDisableThumbsDb_Title"] = ("Не создавать Thumbs.db на сетевых папках", "Don't create Thumbs.db on network folders", "Не створювати Thumbs.db у мережевих папках"),
        ["TwDisableThumbsDb_Desc"] = ("Запрещает Проводнику создавать скрытые файлы кэша миниатюр в сетевых папках.", "Prevents Explorer from creating hidden thumbnail cache files on network folders.", "Забороняє Провіднику створювати приховані файли кешу мініатюр у мережевих папках."),
        ["TwDisableThumbnailCache_Title"] = ("Отключить кэш миниатюр", "Disable thumbnail cache", "Вимкнути кеш мініатюр"),
        ["TwDisableThumbnailCache_Desc"] = ("Проводник всегда показывает обычные значки вместо миниатюр, экономя место на диске.", "Explorer always shows generic icons instead of thumbnails, saving disk space.", "Провідник завжди показує звичайні значки замість мініатюр, заощаджуючи місце на диску."),
        ["TwDisableWindowsInkWorkspace_Title"] = ("Отключить рабочую область Windows Ink", "Disable Windows Ink Workspace", "Вимкнути робочу область Windows Ink"),
        ["TwDisableWindowsInkWorkspace_Desc"] = ("Убирает значок и функцию рабочей области пера/рукописного ввода.", "Removes the pen/inking workspace icon and feature.", "Прибирає значок і функцію робочої області пера/рукописного введення."),
        ["TwDisableNotificationCenter_Title"] = ("Отключить все всплывающие уведомления", "Disable all toast notifications", "Вимкнути всі спливаючі сповіщення"),
        ["TwDisableNotificationCenter_Desc"] = ("Полностью отключает всплывающие уведомления приложений и системы.", "Completely disables toast notifications from apps and the system.", "Повністю вимикає спливаючі сповіщення застосунків і системи."),
        ["TwDisableStartupSound_Title"] = ("Отключить звук при запуске Windows", "Disable Windows startup sound", "Вимкнути звук під час запуску Windows"),
        ["TwDisableStartupSound_Desc"] = ("Отключает звуковой сигнал, который проигрывается при входе в систему.", "Disables the chime played when signing into Windows.", "Вимикає звуковий сигнал, який відтворюється під час входу в систему."),
        ["TwDisableAppSuggestions_Title"] = ("Отключить рекомендации приложений", "Disable app suggestions", "Вимкнути рекомендації застосунків"),
        ["TwDisableAppSuggestions_Desc"] = ("Останавливает показ рекомендованных приложений в системном интерфейсе.", "Stops showing recommended app suggestions across the system UI.", "Зупиняє показ рекомендованих застосунків у системному інтерфейсі."),
        ["TwDisableLockScreenAds_Title"] = ("Отключить рекламу на экране блокировки", "Disable lock screen ads", "Вимкнути рекламу на екрані блокування"),
        ["TwDisableLockScreenAds_Desc"] = ("Отключает показ рекламных изображений и советов на экране блокировки.", "Disables promotional images and tips shown on the lock screen.", "Вимикає показ рекламних зображень і порад на екрані блокування."),
        ["TwDisableStartSuggestions_Title"] = ("Отключить рекомендуемые приложения в Пуск", "Disable suggested apps in Start", "Вимкнути рекомендовані застосунки в меню Пуск"),
        ["TwDisableStartSuggestions_Desc"] = ("Убирает случайные рекомендации приложений из меню Пуск.", "Removes occasional app suggestions from the Start menu.", "Прибирає випадкові рекомендації застосунків із меню Пуск."),
        ["TwDisableWelcomeExperience_Title"] = ("Отключить приветственный экран после обновлений", "Disable the post-update welcome experience", "Вимкнути вітальний екран після оновлень"),
        ["TwDisableWelcomeExperience_Desc"] = ("Отключает полноэкранные советы, которые Windows показывает после крупных обновлений.", "Disables the full-screen tips Windows shows after major updates.", "Вимикає повноекранні поради, які Windows показує після великих оновлень."),
        ["TwDisableTipsTricks_Title"] = ("Отключить советы и подсказки Windows", "Disable Windows tips & tricks", "Вимкнути поради та підказки Windows"),
        ["TwDisableTipsTricks_Desc"] = ("Останавливает показ советов по использованию Windows в уведомлениях и Параметрах.", "Stops Windows usage tips from appearing in notifications and Settings.", "Зупиняє показ порад щодо використання Windows у сповіщеннях і Параметрах."),
        ["TwDisableContentDelivery_Title"] = ("Отключить доставку рекламного контента", "Disable content delivery", "Вимкнути доставку рекламного контенту"),
        ["TwDisableContentDelivery_Desc"] = ("Главный переключатель, останавливающий загрузку промо-контента от Microsoft.", "Master switch that stops Microsoft's promotional content from being downloaded.", "Головний перемикач, що зупиняє завантаження промо-контенту від Microsoft."),
        ["TwDisableOemApps_Title"] = ("Отключить предустановленные OEM-приложения", "Disable OEM pre-installed apps", "Вимкнути попередньо встановлені OEM-застосунки"),
        ["TwDisableOemApps_Desc"] = ("Запрещает автоматическую установку приложений, добавленных производителем ПК.", "Prevents automatic installation of apps added by the PC manufacturer.", "Забороняє автоматичне встановлення застосунків, доданих виробником ПК."),
        ["TwDisablePreinstalledApps_Title"] = ("Отключить автоустановку рекламируемых приложений", "Disable auto-install of promoted apps", "Вимкнути автовстановлення рекламованих застосунків"),
        ["TwDisablePreinstalledApps_Desc"] = ("Запрещает Windows автоматически ставить сторонние приложения из Store без вашего запроса.", "Prevents Windows from automatically installing third-party Store apps without you asking.", "Забороняє Windows автоматично встановлювати сторонні застосунки зі Store без вашого запиту."),
        ["TwDisableSilentAppInstalls_Title"] = ("Отключить тихую установку приложений", "Disable silent app installs", "Вимкнути тиху інсталяцію застосунків"),
        ["TwDisableSilentAppInstalls_Desc"] = ("Запрещает Windows устанавливать приложения в фоне без уведомления пользователя.", "Prevents Windows from installing apps in the background without notifying the user.", "Забороняє Windows встановлювати застосунки у фоні без сповіщення користувача."),
        ["TwDisableSubscribedContent_Title"] = ("Отключить весь рекомендуемый контент", "Disable all subscribed content", "Вимкнути весь рекомендований контент"),
        ["TwDisableSubscribedContent_Desc"] = ("Общий переключатель для всех видов рекомендаций и советов Microsoft в системе.", "Overall switch for all types of Microsoft recommendations and tips across the OS.", "Загальний перемикач для всіх видів рекомендацій і порад Microsoft у системі."),
        ["TwDisableSpotlight_Title"] = ("Отключить Windows Spotlight на рабочем столе", "Disable Windows Spotlight on desktop", "Вимкнути Windows Spotlight на робочому столі"),
        ["TwDisableSpotlight_Desc"] = ("Отключает автоматическую смену фоновых изображений Spotlight на рабочем столе.", "Disables automatic Spotlight background image rotation on the desktop.", "Вимикає автоматичну зміну фонових зображень Spotlight на робочому столі."),
        ["TwDisableSpotlightFeatures_Title"] = ("Отключить функции Windows Spotlight", "Disable Windows Spotlight features", "Вимкнути функції Windows Spotlight"),
        ["TwDisableSpotlightFeatures_Desc"] = ("Отключает персонализированные советы и предложения Spotlight по всей системе.", "Disables Spotlight's personalized tips and suggestions across the system.", "Вимикає персоналізовані поради та пропозиції Spotlight по всій системі."),
        ["TwDisableConsumerFeatures_Title"] = ("Отключить автоустановку сторонних приложений", "Disable Windows consumer features", "Вимкнути автовстановлення сторонніх застосунків"),
        ["TwDisableConsumerFeatures_Desc"] = ("Останавливает автоматическую установку сторонних приложений вроде игр и подписок при первом входе.", "Stops third-party apps like games and subscriptions from auto-installing on first sign-in.", "Зупиняє автоматичне встановлення сторонніх застосунків на кшталт ігор і підписок під час першого входу."),
        ["TwDisableFindMyDevice_Title"] = ("Отключить «Найти устройство»", "Disable \"Find My Device\"", "Вимкнути «Знайти пристрій»"),
        ["TwDisableFindMyDevice_Desc"] = ("Запрещает Windows отслеживать местоположение устройства для функции поиска.", "Prevents Windows from tracking the device's location for the Find My Device feature.", "Забороняє Windows відстежувати місцезнаходження пристрою для функції пошуку."),
        ["TwDisableDiagTrackService_Title"] = ("Отключить службу телеметрии (DiagTrack)", "Disable the DiagTrack telemetry service", "Вимкнути службу телеметрії (DiagTrack)"),
        ["TwDisableDiagTrackService_Desc"] = ("Останавливает и отключает службу сбора диагностических данных Connected User Experiences.", "Stops and disables the Connected User Experiences and Telemetry service.", "Зупиняє та вимикає службу збору діагностичних даних Connected User Experiences."),
        ["TwDisableDmwappush_Title"] = ("Отключить службу маршрутизации push-сообщений", "Disable WAP Push Message Routing service", "Вимкнути службу маршрутизації push-повідомлень"),
        ["TwDisableDmwappush_Desc"] = ("Отключает вспомогательную службу, которая может инициировать отправку диагностических данных.", "Disables a helper service that can trigger diagnostic data submissions.", "Вимикає допоміжну службу, яка може ініціювати надсилання діагностичних даних."),
        ["TwDisableRemoteAssistance_Title"] = ("Отключить удалённый помощник", "Disable Remote Assistance", "Вимкнути віддалену допомогу"),
        ["TwDisableRemoteAssistance_Desc"] = ("Запрещает другим людям подключаться к этому ПК через удалённого помощника.", "Prevents other people from connecting to this PC via Remote Assistance.", "Забороняє іншим людям підключатися до цього ПК через віддалену допомогу."),
        ["TwDisableRemoteDesktop_Title"] = ("Отключить удалённый рабочий стол", "Disable Remote Desktop", "Вимкнути віддалений робочий стіл"),
        ["TwDisableRemoteDesktop_Desc"] = ("Запрещает входящие подключения к этому ПК по протоколу RDP.", "Blocks incoming RDP connections to this PC.", "Забороняє вхідні підключення до цього ПК за протоколом RDP."),
        ["TwDisableAutoplay_Title"] = ("Отключить автовоспроизведение для всех устройств", "Disable AutoPlay for all devices", "Вимкнути автовідтворення для всіх пристроїв"),
        ["TwDisableAutoplay_Desc"] = ("Отключает всплывающее окно автовоспроизведения при подключении дисков и накопителей.", "Disables the AutoPlay prompt that appears when discs or drives are connected.", "Вимикає спливаюче вікно автовідтворення під час підключення дисків і накопичувачів."),
        ["TwDisableRemoteRegistry_Title"] = ("Отключить удалённый доступ к реестру", "Disable Remote Registry access", "Вимкнути віддалений доступ до реєстру"),
        ["TwDisableRemoteRegistry_Desc"] = ("Отключает службу, позволяющую редактировать реестр этого ПК по сети.", "Disables the service that lets this PC's registry be edited over the network.", "Вимикає службу, яка дозволяє редагувати реєстр цього ПК через мережу."),
        ["TwDisableSharedExperiences_Title"] = ("Отключить общий доступ между устройствами", "Disable shared experiences across devices", "Вимкнути спільний доступ між пристроями"),
        ["TwDisableSharedExperiences_Desc"] = ("Отключает функцию продолжения действий и обмена контентом между вашими устройствами.", "Disables the feature for continuing tasks and sharing content across your devices.", "Вимикає функцію продовження дій та обміну контентом між вашими пристроями."),
        ["TwDisableCloudClipboardSync_Title"] = ("Отключить синхронизацию буфера обмена в облаке", "Disable cloud clipboard sync", "Вимкнути синхронізацію буфера обміну в хмарі"),
        ["TwDisableCloudClipboardSync_Desc"] = ("Останавливает отправку содержимого буфера обмена на другие ваши устройства через Microsoft.", "Stops clipboard contents from being synced to your other devices via Microsoft.", "Зупиняє надсилання вмісту буфера обміну на інші ваші пристрої через Microsoft."),
        ["TwStartupDelay_Title"] = ("Убрать задержку запуска приложений автозагрузки", "Remove startup app launch delay", "Прибрати затримку запуску застосунків автозавантаження"),
        ["TwStartupDelay_Desc"] = ("Windows запускает приложения автозагрузки сразу, без искусственной паузы.", "Windows launches startup apps immediately, without the built-in artificial delay.", "Windows запускає застосунки автозавантаження одразу, без штучної паузи."),
        ["TwLowDiskSpaceWarning_Title"] = ("Отключить предупреждение о нехватке места на диске", "Disable low disk space warnings", "Вимкнути попередження про брак місця на диску"),
        ["TwLowDiskSpaceWarning_Desc"] = ("Убирает всплывающие уведомления о заканчивающемся месте на диске.", "Removes pop-up notifications about running low on disk space.", "Прибирає спливаючі сповіщення про закінчення місця на диску."),
        ["TwDisableAutoRestartSignIn_Title"] = ("Не входить в систему автоматически после обновления", "Don't automatically sign in after an update", "Не входити в систему автоматично після оновлення"),
        ["TwDisableAutoRestartSignIn_Desc"] = ("После перезагрузки на обновлении Windows покажет экран входа вместо автоматического входа.", "After an update-triggered restart, Windows shows the sign-in screen instead of logging in automatically.", "Після перезавантаження через оновлення Windows покаже екран входу замість автоматичного входу."),
        ["TwDisableEdgeStartupBoost_Title"] = ("Отключить фоновый запуск Microsoft Edge", "Disable Edge startup boost", "Вимкнути фоновий запуск Microsoft Edge"),
        ["TwDisableEdgeStartupBoost_Desc"] = ("Запрещает Edge запускаться в фоне при старте Windows для ускорения открытия.", "Prevents Edge from pre-launching in the background at Windows startup.", "Забороняє Edge запускатися у фоні під час старту Windows для пришвидшення відкриття."),
        ["TwDisableEdgeBackgroundMode_Title"] = ("Отключить фоновый режим Microsoft Edge", "Disable Edge background mode", "Вимкнути фоновий режим Microsoft Edge"),
        ["TwDisableEdgeBackgroundMode_Desc"] = ("Запрещает Edge продолжать работать в фоне после закрытия последнего окна.", "Prevents Edge from continuing to run in the background after the last window closes.", "Забороняє Edge продовжувати працювати у фоні після закриття останнього вікна."),
        ["TwDisableCopilot_Title"] = ("Отключить Windows Copilot", "Disable Windows Copilot", "Вимкнути Windows Copilot"),
        ["TwDisableCopilot_Desc"] = ("Убирает значок и функцию Copilot из панели задач и системы.", "Removes the Copilot icon and feature from the taskbar and system.", "Прибирає значок і функцію Copilot із панелі завдань і системи."),
        ["TwDisableRecall_Title"] = ("Отключить анализ данных ИИ (Recall)", "Disable AI data analysis (Recall)", "Вимкнути аналіз даних ШІ (Recall)"),
        ["TwDisableRecall_Desc"] = ("Запрещает системе анализировать активность пользователя средствами ИИ для функции Recall.", "Prevents the system from analyzing user activity with AI for the Recall feature.", "Забороняє системі аналізувати активність користувача засобами ШІ для функції Recall."),
        ["TwDisableDeliveryOptimization_Title"] = ("Отключить раздачу обновлений другим ПК", "Disable Delivery Optimization P2P sharing", "Вимкнути роздачу оновлень іншим ПК"),
        ["TwDisableDeliveryOptimization_Desc"] = ("Запрещает Windows отправлять части загруженных обновлений на другие компьютеры по сети/интернету.", "Prevents Windows from uploading downloaded update chunks to other PCs over the network/internet.", "Забороняє Windows надсилати частини завантажених оновлень на інші комп'ютери мережею/інтернетом."),
        ["TwDisableFastUserSwitching_Title"] = ("Отключить быстрое переключение пользователей", "Disable fast user switching", "Вимкнути швидке перемикання користувачів"),
        ["TwDisableFastUserSwitching_Desc"] = ("Убирает возможность переключаться между учётными записями без выхода из системы.", "Removes the ability to switch between accounts without signing out.", "Прибирає можливість перемикатися між обліковими записами без виходу з системи."),
        ["TwDisableInsiderService_Title"] = ("Отключить службу программы Windows Insider", "Disable the Windows Insider service", "Вимкнути службу програми Windows Insider"),
        ["TwDisableInsiderService_Desc"] = ("Отключает фоновую службу предварительных сборок Windows Insider.", "Disables the background service for the Windows Insider preview program.", "Вимикає фонову службу попередніх збірок Windows Insider."),
        ["TwDisableLLMNR_Title"] = ("Отключить LLMNR", "Disable LLMNR", "Вимкнути LLMNR"),
        ["TwDisableLLMNR_Desc"] = ("Отключает протокол разрешения имён в локальной сети (Link-Local Multicast Name Resolution).", "Disables Link-Local Multicast Name Resolution on the local network.", "Вимикає протокол розв'язання імен у локальній мережі (Link-Local Multicast Name Resolution)."),

        // --- Round 4: v2.0 expansion translations (auto-generated) ---
        ["TwDisableStickyKeysHotkey_Title"] = ("Отключить залипание клавиш по 5xShift", "Disable Sticky Keys hotkey (5x Shift)", "Вимкнути залипання клавіш по 5xShift"),
        ["TwDisableStickyKeysHotkey_Desc"] = ("Запрещает включение залипания клавиш пятикратным нажатием Shift.", "Prevents Sticky Keys from being toggled by pressing Shift five times.", "Забороняє вмикання залипання клавіш п'ятикратним натисканням Shift."),
        ["TwDisableToggleKeysHotkey_Title"] = ("Отключить озвучку клавиш по удержанию NumLock", "Disable Toggle Keys hotkey (hold NumLock)", "Вимкнути озвучення клавіш утриманням NumLock"),
        ["TwDisableToggleKeysHotkey_Desc"] = ("Запрещает включение звукового сопровождения клавиш удержанием NumLock 5 секунд.", "Prevents Toggle Keys from being enabled by holding NumLock for 5 seconds.", "Забороняє вмикання звукового супроводу клавіш утриманням NumLock 5 секунд."),
        ["TwDisableAeroShake_Title"] = ("Отключить сворачивание окон встряхиванием", "Disable Aero Shake (minimize by shaking)", "Вимкнути згортання вікон струшуванням"),
        ["TwDisableAeroShake_Desc"] = ("Запрещает сворачивание остальных окон при встряхивании заголовка активного окна.", "Prevents other windows from minimizing when you shake an active window's title bar.", "Забороняє згортання решти вікон під час струшування заголовка активного вікна."),
        ["TwDisableStartupTiles_Title"] = ("Отключить всплывающие анимации плиток в Пуск", "Disable Start menu tile animations", "Вимкнути анімації плиток у меню Пуск"),
        ["TwDisableStartupTiles_Desc"] = ("Отключает анимацию появления живых плиток в меню Пуск при запуске.", "Disables the animation shown when live tiles appear in the Start menu at launch.", "Вимикає анімацію появи живих плиток у меню Пуск під час запуску."),
        ["TwDisableAppCompatEngine_Title"] = ("Отключить сбор данных совместимости приложений", "Disable Application Compatibility data collection", "Вимкнути збір даних сумісності застосунків"),
        ["TwDisableAppCompatEngine_Desc"] = ("Останавливает фоновый сбор диагностических данных о совместимости запускаемых программ.", "Stops the background collection of app compatibility diagnostic data.", "Зупиняє фоновий збір діагностичних даних про сумісність запущених програм."),
        ["TwDisableHandwritingErrorReports_Title"] = ("Отключить отчёты об ошибках рукописного ввода", "Disable handwriting error reporting", "Вимкнути звіти про помилки рукописного введення"),
        ["TwDisableHandwritingErrorReports_Desc"] = ("Запрещает отправку данных о распознавании рукописного ввода в Microsoft.", "Prevents handwriting recognition data from being sent to Microsoft.", "Забороняє надсилання даних розпізнавання рукописного введення до Microsoft."),
        ["TwDisableFirstLogonAnimation_Title"] = ("Отключить приветственную анимацию при первом входе", "Disable first sign-in animation", "Вимкнути вітальну анімацію під час першого входу"),
        ["TwDisableFirstLogonAnimation_Desc"] = ("Ускоряет первый вход в систему, пропуская анимацию приветствия.", "Speeds up the first sign-in by skipping the welcome animation.", "Пришвидшує перший вхід у систему, пропускаючи вітальну анімацію."),
        ["TwDisableSmartScreenExplorer_Title"] = ("Отключить проверку файлов SmartScreen в Проводнике", "Disable SmartScreen check for downloaded files", "Вимкнути перевірку файлів SmartScreen у Провіднику"),
        ["TwDisableSmartScreenExplorer_Desc"] = ("Проводник перестаёт проверять запускаемые файлы через облачный SmartScreen.", "Explorer stops checking launched files against the cloud SmartScreen service.", "Провідник перестає перевіряти файли, що запускаються, через хмарний SmartScreen."),
        ["TwEnableClearTypeText_Title"] = ("Улучшить сглаживание шрифтов (ClearType)", "Improve font smoothing (ClearType)", "Покращити згладжування шрифтів (ClearType)"),
        ["TwEnableClearTypeText_Desc"] = ("Включает технологию сглаживания шрифтов ClearType для более чёткого текста на ЖК-экранах.", "Enables ClearType font smoothing for sharper text on LCD screens.", "Вмикає технологію згладжування шрифтів ClearType для чіткішого тексту на РК-екранах."),
        ["TwDisableOfferRemoteAssistance_Title"] = ("Запретить удалённую помощь без запроса", "Block unsolicited Remote Assistance offers", "Заборонити віддалену допомогу без запиту"),
        ["TwDisableOfferRemoteAssistance_Desc"] = ("Запрещает другим людям предлагать удалённую помощь без явного запроса пользователя.", "Prevents other people from offering unsolicited Remote Assistance sessions.", "Забороняє іншим людям пропонувати віддалену допомогу без явного запиту користувача."),

        // UWP app install/removal page
        ["NavUwp"] = ("📦  UWP-приложения", "📦  UWP Apps", "📦  UWP-застосунки"),
        ["UwpTitle"] = ("Удаление и установка UWP-приложений", "Install and remove UWP apps", "Видалення та встановлення UWP-застосунків"),
        ["UwpDesc"] = ("Удаляет или переустанавливает стандартные UWP-приложения Windows для текущего пользователя.",
                        "Removes or reinstalls built-in Windows UWP apps for the current user.",
                        "Видаляє або перевстановлює стандартні UWP-застосунки Windows для поточного користувача."),
        ["RemoveButton"] = ("Удалить", "Remove", "Видалити"),
        ["InstallButton"] = ("Установить", "Install", "Встановити"),
        ["UwpInstalled"] = ("установлено", "installed", "встановлено"),
        ["UwpNotInstalled"] = ("не установлено", "not installed", "не встановлено"),
        ["UwpRemoving"] = ("Удаление…", "Removing…", "Видалення…"),
        ["UwpRemoved"] = ("Удалено", "Removed", "Видалено"),
        ["UwpRemoveError"] = ("Ошибка: {0}", "Error: {0}", "Помилка: {0}"),
        ["UwpInstalling"] = ("Установка…", "Installing…", "Встановлення…"),
        ["UwpInstalledNow"] = ("Установлено", "Installed", "Встановлено"),
        ["UwpInstallError"] = ("Ошибка: {0}", "Error: {0}", "Помилка: {0}"),
        ["UwpOpenedStore"] = ("Открыт Microsoft Store — приложение было полностью удалено, переустановите оттуда.",
                               "Opened Microsoft Store — the app was fully removed from this PC, reinstall it from there.",
                               "Відкрито Microsoft Store — застосунок був повністю видалений із цього ПК, перевстановіть його звідти."),
        ["UwpCheckingStatus"] = ("Проверка установленных приложений…", "Checking installed apps…", "Перевірка встановлених застосунків…"),


        ["UwpXbox"] = ("Xbox", "Xbox", "Xbox"),
        ["UwpSkype"] = ("Skype", "Skype", "Skype"),
        ["UwpGetHelp"] = ("Получить справку", "Get Help", "Отримати довідку"),
        ["Uwp3DViewer"] = ("Средство просмотра 3D", "3D Viewer", "Засіб перегляду 3D"),
        ["UwpFeedbackHub"] = ("Центр отзывов", "Feedback Hub", "Центр відгуків"),
        ["UwpMixedReality"] = ("Mixed Reality Portal", "Mixed Reality Portal", "Mixed Reality Portal"),
        ["UwpOfficeHub"] = ("Приложение Office", "Office app", "Застосунок Office"),
        ["UwpOneNote"] = ("OneNote", "OneNote", "OneNote"),
        ["UwpPeople"] = ("Люди", "People", "Люди"),
        ["UwpMaps"] = ("Карты", "Maps", "Карти"),
        ["UwpWeather"] = ("Погода", "Weather", "Погода"),
        ["UwpSolitaire"] = ("Microsoft Solitaire Collection", "Microsoft Solitaire Collection", "Microsoft Solitaire Collection"),
        ["UwpZuneMusic"] = ("Groove Музыка", "Groove Music", "Groove Музика"),
        ["UwpZuneVideo"] = ("Кино и ТВ", "Movies & TV", "Кіно й ТБ"),
        ["UwpYourPhone"] = ("Связь с телефоном", "Phone Link", "Зв'язок із телефоном"),
        ["UwpTeams"] = ("Microsoft Teams", "Microsoft Teams", "Microsoft Teams"),
        ["UwpClipchamp"] = ("Clipchamp", "Clipchamp", "Clipchamp"),
        ["UwpCopilot"] = ("Copilot", "Copilot", "Copilot"),
        ["UwpToDo"] = ("Microsoft To Do", "Microsoft To Do", "Microsoft To Do"),

        // Process manager
        ["ColPid"] = ("PID", "PID", "PID"),
        ["ColName"] = ("Имя процесса", "Process name", "Ім'я процесу"),
        ["ColWindowTitle"] = ("Заголовок окна", "Window title", "Заголовок вікна"),
        ["ColMemory"] = ("Память, МБ", "Memory, MB", "Пам'ять, МБ"),
        ["ColPriority"] = ("Приоритет", "Priority", "Пріоритет"),
        ["ColActions"] = ("Действия", "Actions", "Дії"),
        ["BtnKill"] = ("Завершить", "End task", "Завершити"),
        ["BtnHighPriority"] = ("Высокий приоритет", "High priority", "Високий пріоритет"),
        ["KillConfirmTitle"] = ("Подтверждение", "Confirm", "Підтвердження"),
        ["KillConfirmMsg"] = ("Завершить процесс «{0}» (PID {1})?", "End process \"{0}\" (PID {1})?", "Завершити процес «{0}» (PID {1})?"),
        ["ErrorTitle"] = ("Ошибка", "Error", "Помилка"),
        ["KillErrorMsg"] = ("Не удалось завершить процесс: {0}", "Failed to end process: {0}", "Не вдалося завершити процес: {0}"),
        ["PriorityErrorMsg"] = ("Не удалось изменить приоритет: {0}", "Failed to change priority: {0}", "Не вдалося змінити пріоритет: {0}"),

        // Cleanup targets + messages
        ["Clean_UserTemp"] = ("Временные файлы пользователя", "User temp files", "Тимчасові файли користувача"),
        ["Clean_WinTemp"] = ("Временные файлы Windows", "Windows temp files", "Тимчасові файли Windows"),
        ["Clean_ThumbCache"] = ("Кэш миниатюр Explorer", "Explorer thumbnail cache", "Кеш мініатюр Provідника"),
        ["Clean_WUDownload"] = ("Журналы Windows Update", "Windows Update logs", "Журнали Windows Update"),
        ["Clean_Prefetch"] = ("Кэш Prefetch", "Prefetch cache", "Кеш Prefetch"),
        ["CleanupNotScanned"] = ("не сканировано", "not scanned", "не сканувалося"),
        ["CleanupScanError"] = ("ошибка сканирования", "scan error", "помилка сканування"),
        ["CleanupNothingSelected"] = ("Ничего не выбрано.", "Nothing selected.", "Нічого не вибрано."),
        ["CleanupConfirmTitle"] = ("Подтверждение очистки", "Confirm cleanup", "Підтвердження очищення"),
        ["CleanupConfirmMsg"] = ("Удалить содержимое {0} выбранных папок с временными файлами? Действие необратимо.", "Delete contents of {0} selected temp folders? This cannot be undone.", "Видалити вміст {0} вибраних папок з тимчасовими файлами? Дію неможливо скасувати."),
        ["CleanupResultOk"] = ("Готово: удалено {0} файлов, освобождено {1}.", "Done: {0} files deleted, {1} freed.", "Готово: видалено {0} файлів, звільнено {1}."),
        ["CleanupResultPartial"] = ("Удалено {0} файлов, освобождено {1}. {2} файлов пропущено (были заняты другими процессами).", "{0} files deleted, {1} freed. {2} files skipped (locked by other processes).", "Видалено {0} файлів, звільнено {1}. {2} файлів пропущено (були зайняті іншими процесами)."),
        ["UnitMb"] = ("МБ", "MB", "МБ"),
        ["UnitGb"] = ("ГБ", "GB", "ГБ"),

        // System info labels
        ["InfoOs"] = ("Операционная система", "Operating system", "Операційна система"),
        ["InfoCpu"] = ("Процессор", "Processor", "Процесор"),
        ["InfoRam"] = ("Оперативная память", "Memory", "Оперативна пам'ять"),
        ["InfoGpu"] = ("Видеокарта", "Graphics card", "Відеокарта"),
        ["InfoBoard"] = ("Материнская плата", "Motherboard", "Материнська плата"),
        ["InfoOsValue"] = ("{0} (build {1})", "{0} (build {1})", "{0} (build {1})"),
        ["InfoCpuValue"] = ("{0} — {1} ядер / {2} потоков", "{0} — {1} cores / {2} threads", "{0} — {1} ядер / {2} потоків"),
        ["InfoRamValue"] = ("{0} ГБ свободно из {1} ГБ", "{0} GB free of {1} GB", "{0} ГБ вільно з {1} ГБ"),
        ["InfoDiskValue"] = ("{0} ГБ свободно из {1} ГБ ({2}% занято)", "{0} GB free of {1} GB ({2}% used)", "{0} ГБ вільно з {1} ГБ ({2}% зайнято)"),
        ["InfoLoadError"] = ("Ошибка", "Error", "Помилка"),

        // Tweak categories
        ["Cat_Personalization"] = ("Персонализация", "Personalization", "Персоналізація"),
        ["Cat_Privacy"] = ("Приватность", "Privacy", "Приватність"),
        ["Cat_Performance"] = ("Производительность", "Performance", "Продуктивність"),
        ["Cat_Gaming"] = ("Игры", "Gaming", "Ігри"),

        // Individual tweaks
        ["Tweak_DarkTheme_Title"] = ("Тёмная тема (Проводник и приложения)", "Dark theme (Explorer and apps)", "Темна тема (Провідник і застосунки)"),
        ["Tweak_DarkTheme_Desc"] = ("Переключает системную тему оформления на тёмную.", "Switches the system-wide theme to dark.", "Перемикає системну тему оформлення на темну."),
        ["Tweak_FileExt_Title"] = ("Показывать расширения файлов", "Show file extensions", "Показувати розширення файлів"),
        ["Tweak_FileExt_Desc"] = ("Отображает расширения (.txt, .exe и т.д.) в Проводнике.", "Shows extensions (.txt, .exe etc.) in File Explorer.", "Відображає розширення (.txt, .exe тощо) у Провіднику."),
        ["Tweak_HiddenFiles_Title"] = ("Показывать скрытые файлы", "Show hidden files", "Показувати приховані файли"),
        ["Tweak_HiddenFiles_Desc"] = ("Отображает скрытые файлы и папки в Проводнике.", "Shows hidden files and folders in File Explorer.", "Відображає приховані файли та папки у Провіднику."),
        ["Tweak_ClassicMenu_Title"] = ("Классическое контекстное меню (Win11)", "Classic context menu (Win11)", "Класичне контекстне меню (Win11)"),
        ["Tweak_ClassicMenu_Desc"] = ("Возвращает полное контекстное меню правой кнопки мыши из Windows 10.", "Restores the full right-click context menu from Windows 10.", "Повертає повне контекстне меню правої кнопки миші з Windows 10."),

        ["Tweak_Telemetry_Title"] = ("Отключить сбор телеметрии", "Disable telemetry collection", "Вимкнути збір телеметрії"),
        ["Tweak_Telemetry_Desc"] = ("Ограничивает уровень сбора диагностических данных Windows до минимума.", "Limits Windows diagnostic data collection to the minimum.", "Обмежує рівень збору діагностичних даних Windows до мінімуму."),
        ["Tweak_AdId_Title"] = ("Отключить рекламный ID", "Disable advertising ID", "Вимкнути рекламний ID"),
        ["Tweak_AdId_Desc"] = ("Запрещает приложениям использовать персональный рекламный идентификатор.", "Prevents apps from using your personal advertising identifier.", "Забороняє застосункам використовувати персональний рекламний ідентифікатор."),
        ["Tweak_BingSearch_Title"] = ("Отключить веб-поиск в меню Пуск", "Disable web search in Start menu", "Вимкнути веб-пошук у меню Пуск"),
        ["Tweak_BingSearch_Desc"] = ("Поиск в меню Пуск ищет только по локальным файлам и приложениям.", "Start menu search will only search local files and apps.", "Пошук у меню Пуск шукатиме лише локальні файли й застосунки."),
        ["Tweak_Cortana_Title"] = ("Отключить Cortana", "Disable Cortana", "Вимкнути Cortana"),
        ["Tweak_Cortana_Desc"] = ("Полностью отключает голосового помощника Cortana.", "Fully disables the Cortana voice assistant.", "Повністю вимикає голосового помічника Cortana."),

        ["Tweak_WinAnim_Title"] = ("Отключить анимации окон", "Disable window animations", "Вимкнути анімації вікон"),
        ["Tweak_WinAnim_Desc"] = ("Убирает анимации сворачивания/разворачивания окон для отзывчивости UI.", "Removes minimize/restore animations for a snappier UI.", "Прибирає анімації згортання/розгортання вікон для швидшого UI."),
        ["Tweak_Transparency_Title"] = ("Отключить прозрачность интерфейса", "Disable UI transparency", "Вимкнути прозорість інтерфейсу"),
        ["Tweak_Transparency_Desc"] = ("Отключает эффекты прозрачности панели задач и меню Пуск.", "Disables transparency effects on the taskbar and Start menu.", "Вимикає ефекти прозорості панелі завдань і меню Пуск."),
        ["Tweak_PowerThrottling_Title"] = ("Высокая производительность плана питания", "High-performance power plan", "Висока продуктивність плану живлення"),
        ["Tweak_PowerThrottling_Desc"] = ("Отключает энергосберегающий фоновый режим приложений (Power Throttling).", "Disables background app power throttling.", "Вимикає енергозберігаючий фоновий режим застосунків (Power Throttling)."),

        ["Tweak_GameDvr_Title"] = ("Отключить Game DVR / Game Bar", "Disable Game DVR / Game Bar", "Вимкнути Game DVR / Game Bar"),
        ["Tweak_GameDvr_Desc"] = ("Снижает нагрузку и задержки при записи геймплея в фоне.", "Reduces overhead and latency from background game recording.", "Зменшує навантаження та затримки від фонового запису геймплею."),
        ["Tweak_MouseAccel_Title"] = ("Отключить ускорение указателя мыши", "Disable mouse pointer acceleration", "Вимкнути прискорення вказівника миші"),
        ["Tweak_MouseAccel_Desc"] = ("Делает движение курсора линейным (1:1), полезно для игр/дизайна.", "Makes cursor movement linear (1:1), useful for gaming/design.", "Робить рух курсора лінійним (1:1), корисно для ігор/дизайну."),

        // Cleanup status/dialog messages
        ["Cleanup_NotScanned"] = ("не сканировано", "not scanned", "не сканувалося"),
        ["Cleanup_NothingSelected"] = ("Ничего не выбрано.", "Nothing selected.", "Нічого не вибрано."),
        ["Cleanup_ConfirmTitle"] = ("Подтверждение очистки", "Confirm cleanup", "Підтвердження очищення"),
        ["Cleanup_ConfirmBody"] = ("Удалить содержимое {0} выбранных папок с временными файлами? Действие необратимо.",
                                    "Delete the contents of {0} selected temp folders? This cannot be undone.",
                                    "Видалити вміст {0} вибраних папок із тимчасовими файлами? Дію не можна скасувати."),

        // Process manager
        ["Col_Pid"] = ("PID", "PID", "PID"),
        ["Col_ProcessName"] = ("Имя процесса", "Process name", "Ім'я процесу"),
        ["Col_WindowTitle"] = ("Заголовок окна", "Window title", "Заголовок вікна"),
        ["Col_MemoryMb"] = ("Память, МБ", "Memory, MB", "Пам'ять, МБ"),
        ["Col_Priority"] = ("Приоритет", "Priority", "Пріоритет"),
        ["Col_Actions"] = ("Действия", "Actions", "Дії"),
        ["Action_Kill"] = ("Завершить", "End task", "Завершити"),
        ["Action_HighPriority"] = ("Высокий приоритет", "High priority", "Високий пріоритет"),

        // System info labels
        ["Info_Os"] = ("Операционная система", "Operating system", "Операційна система"),
        ["Info_Cpu"] = ("Процессор", "Processor", "Процесор"),
        ["Info_Ram"] = ("Оперативная память", "Memory", "Оперативна пам'ять"),
        ["Info_Gpu"] = ("Видеокарта", "Graphics card", "Відеокарта"),
        ["Info_Board"] = ("Материнская плата", "Motherboard", "Материнська плата"),
    };
}
