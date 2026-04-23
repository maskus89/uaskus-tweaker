using Microsoft.Win32;
using UaskusTweaks.Models;
using UaskusTweaks.ViewModels;

namespace UaskusTweaks.Services;

// Helper record for registry commands
file record RegCmd(string Path, string Name, object Value, RegistryValueKind Kind = RegistryValueKind.DWord)
{
    public TweakCommand ToTweakCommand()
        => new()
        {
            Type = CommandType.Registry,
            Command = $"{Path}|{Name}|{Value}|{Kind}",
            Description = $"Set {Path}\\{Name} = {Value}"
        };
}

public static class TweakDefinitions
{
    public static List<TweakCategoryViewModel> GetAllCategories()
    {
        return new List<TweakCategoryViewModel>
        {
            BuildEssential(),
            BuildAdvanced(),
            BuildUltimatePerformance(),
            BuildPrivacy(),
            BuildGaming(),
            BuildNetwork(),
            BuildDebloat(),
            BuildVisual(),
            BuildCustomize(),
            BuildExtreme()
        };
    }

    // ─── ESSENTIAL ────────────────────────────────────────────────────────────

    private static TweakCategoryViewModel BuildEssential() => Cat("Essential Tweaks",
        "Safe, recommended tweaks for every Windows installation.", "⚙",
        new[]
        {
            Tweak("essential_telemetry", "Disable Telemetry",
                "Stops Microsoft from collecting usage data and diagnostics.",
                "Disabling telemetry reduces background CPU/network usage and improves privacy. Sets AllowTelemetry to 0 in both HKLM policy locations.",
                RiskLevel.Low, true, false,
                Reg("HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows\\DataCollection", "AllowTelemetry", 0),
                Reg("HKLM\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Policies\\DataCollection", "AllowTelemetry", 0)),

            Tweak("essential_activity", "Disable Activity History",
                "Prevents Windows from tracking your activities and syncing them.",
                "Disables activity feed and user activity uploads to Microsoft.",
                RiskLevel.Low, true, false,
                Reg("HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows\\System", "EnableActivityFeed", 0),
                Reg("HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows\\System", "PublishUserActivities", 0),
                Reg("HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows\\System", "UploadUserActivities", 0)),

            Tweak("essential_consumer", "Disable Consumer Features / Ads",
                "Removes suggested apps, ads, and tips from Windows.",
                "Disables content delivery manager features that push ads and suggestions to the Start menu and lock screen.",
                RiskLevel.Low, false, false,
                Reg("HKCU\\Software\\Microsoft\\Windows\\CurrentVersion\\ContentDeliveryManager", "ContentDeliveryAllowed", 0),
                Reg("HKCU\\Software\\Microsoft\\Windows\\CurrentVersion\\ContentDeliveryManager", "OemPreInstalledAppsEnabled", 0),
                Reg("HKCU\\Software\\Microsoft\\Windows\\CurrentVersion\\ContentDeliveryManager", "PreInstalledAppsEnabled", 0),
                Reg("HKCU\\Software\\Microsoft\\Windows\\CurrentVersion\\ContentDeliveryManager", "PreInstalledAppsEverEnabled", 0),
                Reg("HKCU\\Software\\Microsoft\\Windows\\CurrentVersion\\ContentDeliveryManager", "SilentInstalledAppsEnabled", 0),
                Reg("HKCU\\Software\\Microsoft\\Windows\\CurrentVersion\\ContentDeliveryManager", "SubscribedContentEnabled", 0),
                Reg("HKCU\\Software\\Microsoft\\Windows\\CurrentVersion\\ContentDeliveryManager", "SystemPaneSuggestionsEnabled", 0)),

            Tweak("essential_gamedvr", "Disable Game DVR",
                "Disables Xbox Game DVR background recording.",
                "Game DVR consumes GPU and CPU resources in the background even when you are not recording. Disabling it frees resources for gaming.",
                RiskLevel.Low, false, false,
                Reg("HKCU\\System\\GameConfigStore", "GameDVR_Enabled", 0),
                Reg("HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows\\GameDVR", "AllowGameDVR", 0)),

            Tweak("essential_hibernate", "Disable Hibernation",
                "Removes the hiberfil.sys file and disables hibernate.",
                "Frees disk space used by hibernation file (usually equal to installed RAM). Run 'powercfg -h on' to re-enable.",
                RiskLevel.Low, false, false,
                Cmd("powercfg -h off")),

            Tweak("essential_location", "Disable Location Tracking",
                "Prevents apps from accessing your device location.",
                "Sets location consent to Deny in the registry, stopping background location access.",
                RiskLevel.Low, true, false,
                Reg("HKLM\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\CapabilityAccessManager\\ConsentStore\\location", "Value", "Deny", RegistryValueKind.String)),

            Tweak("essential_pstelemetry", "Disable PowerShell Telemetry",
                "Opts out of PowerShell usage telemetry.",
                "Sets the POWERSHELL_TELEMETRY_OPTOUT environment variable system-wide.",
                RiskLevel.Low, false, false,
                Cmd("setx POWERSHELL_TELEMETRY_OPTOUT 1 /M")),

            Tweak("essential_storagesense", "Disable Storage Sense",
                "Prevents Windows from automatically deleting files.",
                "Storage Sense can unexpectedly delete files from Downloads and Temp. Disabling gives you control.",
                RiskLevel.Low, false, false,
                Reg("HKCU\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\StorageSense\\Parameters\\StoragePolicy", "01", 0)),

            Tweak("essential_endtask", "Enable End Task (Right-Click)",
                "Adds 'End Task' to the taskbar right-click menu.",
                "Enables the TaskbarEndTask developer setting so you can quickly kill unresponsive apps from the taskbar.",
                RiskLevel.Low, false, false,
                Reg("HKCU\\Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced\\TaskbarDeveloperSettings", "TaskbarEndTask", 1)),

            Tweak("essential_ipv4", "Prefer IPv4 over IPv6",
                "Configures Windows to prefer IPv4 connections.",
                "Sets DisabledComponents to 32 which makes Windows prefer IPv4 but keeps IPv6 functional.",
                RiskLevel.Low, true, false,
                Reg("HKLM\\SYSTEM\\CurrentControlSet\\Services\\Tcpip6\\Parameters", "DisabledComponents", 32)),

            Tweak("essential_services", "Set Bloat Services to Manual",
                "Reduces startup overhead by setting unnecessary services to manual start.",
                "Services: DiagTrack, dmwappushservice, SysMain, WSearch, XblAuthManager, XblGameSave, XboxNetApiSvc set to demand start.",
                RiskLevel.Low, true, false,
                Svc("DiagTrack", "demand"),
                Svc("dmwappushservice", "demand"),
                Svc("SysMain", "demand"),
                Svc("WSearch", "demand"),
                Svc("XblAuthManager", "demand"),
                Svc("XblGameSave", "demand"),
                Svc("XboxNetApiSvc", "demand")),

            Tweak("essential_edge", "Debloat Microsoft Edge",
                "Disables Edge startup boost and background running.",
                "Stops Edge from loading at Windows startup and running in the background, freeing RAM.",
                RiskLevel.Low, true, false,
                Reg("HKLM\\SOFTWARE\\Policies\\Microsoft\\Edge", "StartupBoostEnabled", 0),
                Reg("HKLM\\SOFTWARE\\Policies\\Microsoft\\Edge", "BackgroundModeEnabled", 0))
        });

    // ─── ADVANCED ─────────────────────────────────────────────────────────────

    private static TweakCategoryViewModel BuildAdvanced() => Cat("Advanced Tweaks",
        "More impactful tweaks. Review each one before applying.", "🔧",
        new[]
        {
            Tweak("adv_bgapps", "Disable Background Apps",
                "Prevents UWP apps from running in the background.",
                "Disables global background access for all UWP applications, reducing CPU and battery usage.",
                RiskLevel.Medium, false, false,
                Reg("HKCU\\Software\\Microsoft\\Windows\\CurrentVersion\\BackgroundAccessApplications", "GlobalUserDisabled", 1)),

            Tweak("adv_fso", "Disable Fullscreen Optimizations",
                "Disables fullscreen optimizations for better gaming performance.",
                "Windows Fullscreen Optimizations can cause input lag in games. Disabling may improve frame times.",
                RiskLevel.Medium, false, false,
                Reg("HKCU\\System\\GameConfigStore", "GameDVR_DXGIHonorFSEWindowsCompatible", 1)),

            Tweak("adv_ipv6_disable", "Completely Disable IPv6",
                "Fully disables IPv6 on all adapters.",
                "Sets DisabledComponents to 255 to disable all IPv6 components. Only do this if you don't use IPv6.",
                RiskLevel.Medium, true, false,
                Reg("HKLM\\SYSTEM\\CurrentControlSet\\Services\\Tcpip6\\Parameters", "DisabledComponents", 255)),

            Tweak("adv_notifications", "Disable Notification Tray",
                "Removes the Action Center and disables toast notifications.",
                "Clears distractions from the notification tray and stops apps from showing toast notifications.",
                RiskLevel.Medium, false, false,
                Reg("HKCU\\Software\\Policies\\Microsoft\\Windows\\Explorer", "DisableNotificationCenter", 1),
                Reg("HKCU\\Software\\Microsoft\\Windows\\CurrentVersion\\PushNotifications", "ToastEnabled", 0)),

            Tweak("adv_teredo", "Disable Teredo",
                "Disables Teredo IPv6 tunneling.",
                "Teredo is an IPv6 transition technology rarely needed. Disabling it can slightly improve network performance.",
                RiskLevel.Low, true, false,
                NetSh("interface teredo set state disabled")),

            Tweak("adv_explorer_home", "Remove Home/Gallery from Explorer",
                "Hides the Home and Gallery shortcuts from the Explorer sidebar.",
                "Removes clutter from the File Explorer sidebar by unpinning Home and Gallery namespace entries.",
                RiskLevel.Low, false, false,
                Reg("HKCU\\Software\\Classes\\CLSID\\{e88865ea-0e1c-4e20-9aa6-edcd0212c87c}", "System.IsPinnedToNameSpaceTree", 0),
                Reg("HKCU\\Software\\Classes\\CLSID\\{f874310e-b6b7-47dc-bc84-b9e6b38f5903}", "System.IsPinnedToNameSpaceTree", 0)),

            Tweak("adv_classic_menu", "Enable Classic Right-Click Menu",
                "Restores the Windows 10-style right-click context menu.",
                "Removes the Windows 11 simplified context menu and restores the full classic menu directly.",
                RiskLevel.Low, false, false,
                Reg("HKCU\\Software\\Classes\\CLSID\\{86ca1aa0-34aa-4e8b-a509-50c905bae2a2}\\InprocServer32", "", "", RegistryValueKind.String)),

            Tweak("adv_adobe_hosts", "Block Adobe Activation Servers",
                "Adds Adobe license server domains to the hosts file to block them.",
                "Blocks lmlicenses.wip4.adobe.com and lm.licenses.adobe.com via the hosts file.",
                RiskLevel.Medium, true, false,
                PS("Add-Content -Path $env:SystemRoot\\System32\\drivers\\etc\\hosts -Value \"`n127.0.0.1 lmlicenses.wip4.adobe.com`n127.0.0.1 lm.licenses.adobe.com\" -Force"))
        });

    // ─── ULTIMATE PERFORMANCE ─────────────────────────────────────────────────

    private static TweakCategoryViewModel BuildUltimatePerformance() => Cat("Ultimate Performance",
        "Maximize CPU, memory, and GPU performance.", "⚡",
        new[]
        {
            Tweak("perf_power_ultimate", "Enable Ultimate Performance Power Plan",
                "Activates the hidden Ultimate Performance power plan.",
                "Duplicates and activates the Ultimate Performance scheme which disables all power saving features.",
                RiskLevel.Low, true, false,
                Cmd("powercfg -duplicatescheme e9a42b02-d5df-448d-aa00-03f14749eb61"),
                Cmd("powercfg -setactive e9a42b02-d5df-448d-aa00-03f14749eb61")),

            Tweak("perf_core_parking", "Disable CPU Core Parking",
                "Keeps all CPU cores active at all times.",
                "Sets ValueMax to 0 for core parking, preventing Windows from parking cores during light load.",
                RiskLevel.Low, true, false,
                Reg("HKLM\\SYSTEM\\CurrentControlSet\\Control\\Power\\PowerSettings\\54533251-82be-4824-96c1-47b60b740d00\\0cc5b647-c1df-4637-891a-dec35c318583", "ValueMax", 0)),

            Tweak("perf_cpu_throttle", "Disable CPU Throttling",
                "Prevents Windows from throttling CPU performance.",
                "Sets PowerThrottlingOff=1 which disables the energy efficiency algorithm introduced in Windows 10 1709.",
                RiskLevel.Medium, true, false,
                Reg("HKLM\\SYSTEM\\CurrentControlSet\\Control\\Power\\PowerThrottling", "PowerThrottlingOff", 1)),

            Tweak("perf_cpu_100", "Set Processor to 100% Minimum",
                "Forces the CPU to always run at maximum speed.",
                "Sets minimum processor state to 100% in the active power scheme.",
                RiskLevel.Low, true, false,
                Cmd("powercfg -setacvalueindex scheme_current sub_processor PROCTHROTTLEMIN 100"),
                Cmd("powercfg -setactive scheme_current")),

            Tweak("perf_fast_startup", "Disable Fast Startup",
                "Disables hybrid boot/fast startup for cleaner shutdowns.",
                "Fast Startup can cause issues with updates and drivers. A full shutdown ensures a clean state.",
                RiskLevel.Low, true, false,
                Reg("HKLM\\SYSTEM\\CurrentControlSet\\Control\\Session Manager\\Power", "HiberbootEnabled", 0)),

            Tweak("perf_gpu_sched", "Enable Hardware GPU Scheduling",
                "Enables WDDM 2.7 hardware-accelerated GPU scheduling.",
                "Reduces latency between CPU and GPU by letting the GPU manage its own memory. Requires a supported GPU and driver.",
                RiskLevel.Low, true, false,
                Reg("HKLM\\SYSTEM\\CurrentControlSet\\Control\\GraphicsDrivers", "HwSchMode", 2)),

            Tweak("perf_visual_effects", "Disable All Visual Effects",
                "Turns off all Windows animations and visual effects.",
                "Removes Aero Glass, animations, and other visual effects for maximum responsiveness on lower-end systems.",
                RiskLevel.Low, false, false,
                Reg("HKCU\\Control Panel\\Desktop", "UserPreferencesMask", new byte[] { 0x90, 0x12, 0x03, 0x80, 0x10, 0x00, 0x00, 0x00 }, RegistryValueKind.Binary),
                Reg("HKCU\\Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\VisualEffects", "VisualFXSetting", 2)),

            Tweak("perf_animations", "Disable Animations",
                "Removes window minimize/maximize and menu animations.",
                "Sets MinAnimate to 0 and MenuShowDelay to 0 for instant window and menu response.",
                RiskLevel.Low, false, false,
                Reg("HKCU\\Control Panel\\Desktop\\WindowMetrics", "MinAnimate", "0", RegistryValueKind.String),
                Reg("HKCU\\Control Panel\\Desktop", "MenuShowDelay", "0", RegistryValueKind.String)),

            Tweak("perf_transparency", "Disable Transparency",
                "Turns off transparent window effects.",
                "Transparency effects use GPU resources. Disabling them improves performance on weak GPUs.",
                RiskLevel.Low, false, false,
                Reg("HKCU\\Software\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize", "EnableTransparency", 0)),

            Tweak("perf_superfetch", "Disable SuperFetch/Prefetch",
                "Stops the SysMain service and disables prefetch.",
                "On SSDs, SuperFetch/Prefetch is unnecessary and can cause high disk activity. Recommended for SSD users.",
                RiskLevel.Medium, true, false,
                Svc("SysMain", "disabled"),
                Reg("HKLM\\SYSTEM\\CurrentControlSet\\Control\\Session Manager\\Memory Management\\PrefetchParameters", "EnablePrefetcher", 0),
                Reg("HKLM\\SYSTEM\\CurrentControlSet\\Control\\Session Manager\\Memory Management\\PrefetchParameters", "EnableSuperfetch", 0)),

            Tweak("perf_paging_exec", "Disable Paging Executive",
                "Keeps kernel drivers in RAM, not on disk.",
                "DisablePagingExecutive=1 prevents the kernel and drivers from being paged to disk, improving responsiveness but requiring adequate RAM.",
                RiskLevel.Medium, true, false,
                Reg("HKLM\\SYSTEM\\CurrentControlSet\\Control\\Session Manager\\Memory Management", "DisablePagingExecutive", 1)),

            Tweak("perf_responsiveness", "Optimize System Responsiveness",
                "Removes network throttling and sets max multimedia responsiveness.",
                "SystemResponsiveness=0 dedicates multimedia tasks to full CPU, NetworkThrottlingIndex=0xffffffff removes network packet throttling.",
                RiskLevel.Low, true, false,
                Reg("HKLM\\SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Multimedia\\SystemProfile", "SystemResponsiveness", 0),
                Reg("HKLM\\SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Multimedia\\SystemProfile", "NetworkThrottlingIndex", unchecked((int)0xffffffff))),

            Tweak("perf_cpu_priority", "Optimize CPU Priority Separation",
                "Optimizes foreground/background thread scheduling.",
                "Win32PrioritySeparation=38 gives maximum boost to foreground threads, improving interactive application responsiveness.",
                RiskLevel.Low, true, false,
                Reg("HKLM\\SYSTEM\\CurrentControlSet\\Control\\PriorityControl", "Win32PrioritySeparation", 38)),

            Tweak("perf_winsearch", "Disable Windows Search Service",
                "Stops the Windows Search indexing service.",
                "WSearch can cause sudden high disk usage. Disabling it frees I/O. You can still search files, but indexing is off.",
                RiskLevel.Medium, true, false,
                Svc("WSearch", "disabled")),

            Tweak("perf_tasks", "Disable Unnecessary Scheduled Tasks",
                "Disables Microsoft telemetry and compatibility scheduled tasks.",
                "Disables tasks: Microsoft Compatibility Appraiser, ProgramDataUpdater, Autochk\\Proxy, CEIP tasks, DiskDiagnostic.",
                RiskLevel.Low, true, false,
                PS("schtasks /Change /TN \"Microsoft\\Windows\\Application Experience\\Microsoft Compatibility Appraiser\" /DISABLE 2>$null; " +
                   "schtasks /Change /TN \"Microsoft\\Windows\\Application Experience\\ProgramDataUpdater\" /DISABLE 2>$null; " +
                   "schtasks /Change /TN \"Microsoft\\Windows\\Autochk\\Proxy\" /DISABLE 2>$null; " +
                   "schtasks /Change /TN \"Microsoft\\Windows\\Customer Experience Improvement Program\\Consolidator\" /DISABLE 2>$null; " +
                   "schtasks /Change /TN \"Microsoft\\Windows\\Customer Experience Improvement Program\\UsbCeip\" /DISABLE 2>$null; " +
                   "schtasks /Change /TN \"Microsoft\\Windows\\DiskDiagnostic\\Microsoft-Windows-DiskDiagnosticDataCollector\" /DISABLE 2>$null"))
        });

    // ─── PRIVACY ──────────────────────────────────────────────────────────────

    private static TweakCategoryViewModel BuildPrivacy() => Cat("Privacy Tweaks",
        "Protect your data and stop Microsoft tracking.", "🔒",
        new[]
        {
            Tweak("priv_telemetry_svc", "Disable Telemetry Services",
                "Disables DiagTrack and dmwappushservice completely.",
                "These services collect and upload diagnostic data to Microsoft. Disabling them stops all data uploads.",
                RiskLevel.Low, true, false,
                Svc("DiagTrack", "disabled"),
                Svc("dmwappushservice", "disabled")),

            Tweak("priv_activity", "Disable Activity History",
                "Prevents tracking of your app and file usage.",
                "Disables the activity feed and prevents user activities from being published or uploaded.",
                RiskLevel.Low, true, false,
                Reg("HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows\\System", "EnableActivityFeed", 0),
                Reg("HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows\\System", "PublishUserActivities", 0),
                Reg("HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows\\System", "UploadUserActivities", 0)),

            Tweak("priv_adid", "Disable Advertising ID",
                "Prevents apps from using your Advertising ID.",
                "Each Windows installation has a unique Advertising ID used to serve targeted ads. Disabling it stops this tracking.",
                RiskLevel.Low, false, false,
                Reg("HKCU\\Software\\Microsoft\\Windows\\CurrentVersion\\AdvertisingInfo", "Enabled", 0)),

            Tweak("priv_cortana", "Disable Cortana",
                "Prevents Cortana from running and collecting data.",
                "Sets AllowCortana=0 via policy, fully disabling Cortana assistant features.",
                RiskLevel.Low, true, false,
                Reg("HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows\\Windows Search", "AllowCortana", 0)),

            Tweak("priv_feedback", "Disable Windows Feedback",
                "Stops Windows from asking for feedback.",
                "Sets NumberOfSIUFInPeriod=0 which prevents the Windows Feedback experience from ever appearing.",
                RiskLevel.Low, false, false,
                Reg("HKCU\\Software\\Microsoft\\Siuf\\Rules", "NumberOfSIUFInPeriod", 0)),

            Tweak("priv_timeline", "Disable Timeline",
                "Disables the Windows Timeline (Task View history) feature.",
                "Disables the activity feed used by Timeline, preventing history collection.",
                RiskLevel.Low, true, false,
                Reg("HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows\\System", "EnableActivityFeed", 0)),

            Tweak("priv_websearch", "Disable Web Search Suggestions",
                "Stops Bing from appearing in Start menu search.",
                "Disables BingSearchEnabled and DisableSearchBoxSuggestions to keep searches local.",
                RiskLevel.Low, false, false,
                Reg("HKCU\\Software\\Microsoft\\Windows\\CurrentVersion\\Search", "BingSearchEnabled", 0),
                Reg("HKCU\\Software\\Policies\\Microsoft\\Windows\\Explorer", "DisableSearchBoxSuggestions", 1)),

            Tweak("priv_tailored", "Disable Tailored Experiences",
                "Stops personalized tips, ads, and recommendations.",
                "Disables TailoredExperiencesWithDiagnosticDataEnabled which uses your data to customize Windows tips.",
                RiskLevel.Low, false, false,
                Reg("HKCU\\Software\\Microsoft\\Windows\\CurrentVersion\\Privacy", "TailoredExperiencesWithDiagnosticDataEnabled", 0)),

            Tweak("priv_diagdata", "Disable Diagnostic Data",
                "Prevents diagnostic data from being sent to Microsoft.",
                "Sets AllowDiagnosticData=0 via policy, stopping all diagnostic telemetry uploads.",
                RiskLevel.Low, true, false,
                Reg("HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows\\DataCollection", "AllowDiagnosticData", 0)),

            Tweak("priv_errorreporting", "Disable Error Reporting",
                "Prevents crash dumps from being sent to Microsoft.",
                "Disables Windows Error Reporting service and sets the Disabled policy key.",
                RiskLevel.Low, true, false,
                Svc("WerSvc", "demand"),
                Reg("HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows\\Windows Error Reporting", "Disabled", 1))
        });

    // ─── GAMING ───────────────────────────────────────────────────────────────

    private static TweakCategoryViewModel BuildGaming() => Cat("Gaming Tweaks",
        "Optimize Windows specifically for gaming performance.", "🎮",
        new[]
        {
            Tweak("game_mode", "Enable Game Mode",
                "Activates Windows Game Mode for automatic resource allocation.",
                "Game Mode prioritizes CPU and GPU resources for the active game window.",
                RiskLevel.Low, false, false,
                Reg("HKCU\\Software\\Microsoft\\GameBar", "AllowAutoGameMode", 1),
                Reg("HKCU\\Software\\Microsoft\\GameBar", "AutoGameModeEnabled", 1)),

            Tweak("game_dvr_bar", "Disable Game DVR / GameBar",
                "Removes the Xbox overlay and background recording.",
                "GameBar and DVR consume GPU memory and CPU cycles. Disabling them is standard for competitive gaming.",
                RiskLevel.Low, false, false,
                Reg("HKCU\\System\\GameConfigStore", "GameDVR_Enabled", 0),
                Reg("HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows\\GameDVR", "AllowGameDVR", 0),
                Reg("HKCU\\Software\\Microsoft\\GameBar", "UseNexusForGameBarEnabled", 0)),

            Tweak("game_fso", "Disable Fullscreen Optimizations",
                "Forces true exclusive fullscreen for games.",
                "Disables DXGI fullscreen optimizations which can introduce latency and stuttering in some games.",
                RiskLevel.Medium, false, false,
                Reg("HKCU\\System\\GameConfigStore", "GameDVR_DXGIHonorFSEWindowsCompatible", 1)),

            Tweak("game_power", "Activate High Performance Power Plan",
                "Switches to the High Performance power plan.",
                "The High Performance plan prevents CPU frequency scaling, ensuring consistent frame times.",
                RiskLevel.Low, true, false,
                Cmd("powercfg -setactive 8c5e7fda-e8bf-4a96-9a85-a6e23a8c635c")),

            Tweak("game_active_hours", "Set Windows Update Active Hours",
                "Prevents Windows Update restarts from 8AM to 11PM.",
                "Sets active hours so Windows will not force a restart during typical gaming/working hours.",
                RiskLevel.Low, true, false,
                Reg("HKLM\\SOFTWARE\\Microsoft\\WindowsUpdate\\UX\\Settings", "ActiveHoursStart", 8),
                Reg("HKLM\\SOFTWARE\\Microsoft\\WindowsUpdate\\UX\\Settings", "ActiveHoursEnd", 23)),

            Tweak("game_nagle", "Disable Nagle's Algorithm",
                "Reduces network latency by disabling packet coalescing.",
                "TcpAckFrequency=1 and TCPNoDelay=1 ensure packets are sent immediately without buffering, reducing ping.",
                RiskLevel.Medium, true, false,
                Reg("HKLM\\SYSTEM\\CurrentControlSet\\Services\\Tcpip\\Parameters", "TcpAckFrequency", 1),
                Reg("HKLM\\SYSTEM\\CurrentControlSet\\Services\\Tcpip\\Parameters", "TCPNoDelay", 1)),

            Tweak("game_gpu_sched", "Enable Hardware GPU Scheduling",
                "Enables WDDM 2.7 hardware-accelerated GPU scheduling.",
                "Reduces GPU-induced input lag by offloading scheduling to the GPU itself.",
                RiskLevel.Low, true, false,
                Reg("HKLM\\SYSTEM\\CurrentControlSet\\Control\\GraphicsDrivers", "HwSchMode", 2)),

            Tweak("game_cpu_prio", "Optimize CPU Priority for Games",
                "Maximizes foreground thread priority boost.",
                "Win32PrioritySeparation=38 gives the active game maximum CPU time slices.",
                RiskLevel.Low, true, false,
                Reg("HKLM\\SYSTEM\\CurrentControlSet\\Control\\PriorityControl", "Win32PrioritySeparation", 38)),

            Tweak("game_xbox_svc", "Disable Xbox Services",
                "Fully disables all Xbox background services.",
                "Disables XblAuthManager, XblGameSave, XboxNetApiSvc, and XboxGipSvc which are unneeded without Xbox features.",
                RiskLevel.Low, true, false,
                Svc("XblAuthManager", "disabled"),
                Svc("XblGameSave", "disabled"),
                Svc("XboxNetApiSvc", "disabled"),
                Svc("XboxGipSvc", "disabled"))
        });

    // ─── NETWORK ──────────────────────────────────────────────────────────────

    private static TweakCategoryViewModel BuildNetwork() => Cat("Network Tweaks",
        "Optimize networking stack for speed and low latency.", "🌐",
        new[]
        {
            Tweak("net_flushdns", "Flush DNS Cache",
                "Clears the DNS resolver cache.",
                "Removes stale DNS entries that can cause slow lookups or failed connections.",
                RiskLevel.Low, true, false,
                Cmd("ipconfig /flushdns")),

            Tweak("net_throttle", "Disable Network Throttling",
                "Removes Windows multimedia network throttling.",
                "NetworkThrottlingIndex=0xffffffff disables the mechanism that reduces network throughput during media playback.",
                RiskLevel.Low, true, false,
                Reg("HKLM\\SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Multimedia\\SystemProfile", "NetworkThrottlingIndex", unchecked((int)0xffffffff))),

            Tweak("net_tcp_optimize", "Optimize TCP Settings",
                "Enables auto-tuning, chimney, and other TCP optimizations.",
                "Applies netsh commands to enable TCP autotuning, chimney offload, DCA, and NetDMA for better throughput.",
                RiskLevel.Low, true, false,
                NetSh("int tcp set global autotuninglevel=normal"),
                NetSh("int tcp set global chimney=enabled"),
                NetSh("int tcp set global dca=enabled"),
                NetSh("int tcp set global netdma=enabled")),

            Tweak("net_tcp_timestamps", "Enable TCP Timestamps and RSS",
                "Enables TCP timestamps and receive-side scaling.",
                "Timestamps improve retransmission accuracy; RSS distributes network processing across CPU cores.",
                RiskLevel.Low, true, false,
                NetSh("int tcp set global timestamps=enabled"),
                NetSh("int tcp set global rss=enabled")),

            Tweak("net_ipv4_prefer", "Prefer IPv4 over IPv6",
                "Configures Windows to prefer IPv4 connections.",
                "Sets DisabledComponents=32 which makes IPv4 preferred while keeping IPv6 functional.",
                RiskLevel.Low, true, false,
                Reg("HKLM\\SYSTEM\\CurrentControlSet\\Services\\Tcpip6\\Parameters", "DisabledComponents", 32)),

            Tweak("net_qos", "Disable QoS Bandwidth Reservation",
                "Removes the 20% bandwidth reservation for QoS.",
                "By default Windows reserves bandwidth for QoS. Setting NonBestEffortLimit=0 makes all bandwidth available.",
                RiskLevel.Low, true, false,
                Reg("HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows\\Psched", "NonBestEffortLimit", 0)),

            Tweak("net_dns_cloudflare", "Set DNS to Cloudflare (1.1.1.1)",
                "Switches the primary DNS to Cloudflare for faster lookups.",
                "Configures the active network interface to use 1.1.1.1 (primary) and 1.0.0.1 (secondary).",
                RiskLevel.Low, true, false,
                PS("$adapters = Get-NetAdapter | Where-Object {$_.Status -eq 'Up'}; " +
                   "foreach ($a in $adapters) { " +
                   "Set-DnsClientServerAddress -InterfaceIndex $a.InterfaceIndex -ServerAddresses ('1.1.1.1','1.0.0.1') " +
                   "}")),

            Tweak("net_winsock_reset", "Reset Winsock Catalog",
                "Resets the Winsock catalog and IP stack. Requires restart.",
                "Fixes corrupted network stacks. This is a heavy operation that resets all socket settings. Restart required.",
                RiskLevel.High, true, true,
                NetSh("winsock reset"),
                NetSh("int ip reset"))
        });

    // ─── DEBLOAT ──────────────────────────────────────────────────────────────

    private static TweakCategoryViewModel BuildDebloat() => Cat("Debloat Windows",
        "Remove pre-installed apps and bloatware.", "🗑",
        new[]
        {
            AppxTweak("debloat_3dbuilder",    "Remove 3D Builder",            "*3dbuilder*"),
            AppxTweak("debloat_bingnews",     "Remove Bing News",             "*bingnews*"),
            AppxTweak("debloat_bingweather",  "Remove Bing Weather",          "*bingweather*"),
            AppxTweak("debloat_gethelp",      "Remove Get Help",              "*gethelp*"),
            AppxTweak("debloat_getstarted",   "Remove Get Started",           "*getstarted*"),
            AppxTweak("debloat_officehub",    "Remove Office Hub",            "*officehub*"),
            AppxTweak("debloat_solitaire",    "Remove Solitaire Collection",  "*solitaire*"),
            AppxTweak("debloat_people",       "Remove People",                "*people*"),
            AppxTweak("debloat_skype",        "Remove Skype",                 "*skypeapp*"),
            AppxTweak("debloat_maps",         "Remove Maps",                  "*windowsmaps*"),
            AppxTweak("debloat_mail",         "Remove Mail & Calendar",       "*windowscommunicationsapps*"),
            AppxTweak("debloat_groove",       "Remove Groove Music / Movies", "*zune*"),
            AppxTweak("debloat_xbox",         "Remove Xbox Apps",             "*xbox*",           RiskLevel.Medium),
            AppxTweak("debloat_mixedreality", "Remove Mixed Reality Portal",  "*mixedreality*"),
            AppxTweak("debloat_feedback",     "Remove Feedback Hub",          "*feedback*"),
            AppxTweak("debloat_yourphone",    "Remove Your Phone / Link to Windows", "*yourphone*")
        });

    // ─── VISUAL ───────────────────────────────────────────────────────────────

    private static TweakCategoryViewModel BuildVisual() => Cat("Visual Tweaks",
        "Customize the Windows interface and File Explorer.", "🎨",
        new[]
        {
            Tweak("vis_fileext", "Show File Extensions",
                "Makes Windows show file extensions (.exe, .txt, etc.).",
                "HideFileExt=0 ensures you always see the full file name including extension.",
                RiskLevel.Low, false, false,
                Reg("HKCU\\Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced", "HideFileExt", 0)),

            Tweak("vis_hidden", "Show Hidden Files",
                "Makes hidden files and folders visible in Explorer.",
                "Hidden=1 shows hidden files, useful for troubleshooting and advanced file management.",
                RiskLevel.Low, false, false,
                Reg("HKCU\\Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced", "Hidden", 1)),

            Tweak("vis_snap", "Disable Snap Assist",
                "Stops the Snap Assist popup when dragging windows.",
                "WindowArrangementActive=0 disables the snap layout popup that appears when dragging to screen edges.",
                RiskLevel.Low, false, false,
                Reg("HKCU\\Control Panel\\Desktop", "WindowArrangementActive", "0", RegistryValueKind.String)),

            Tweak("vis_shake", "Disable Aero Shake",
                "Prevents shaking a window from minimizing all others.",
                "DisallowShaking=1 stops the Aero Shake gesture which can accidentally minimize all windows.",
                RiskLevel.Low, false, false,
                Reg("HKCU\\Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced", "DisallowShaking", 1)),

            Tweak("vis_this_pc", "Show This PC on Desktop",
                "Adds the This PC icon to the desktop.",
                "Unpins the This PC entry in the new start panel to show it on the desktop.",
                RiskLevel.Low, false, false,
                Reg("HKCU\\Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\HideDesktopIcons\\NewStartPanel", "{20D04FE0-3AEA-1069-A2D8-08002B30309D}", 0)),

            Tweak("vis_taskbar_left", "Align Taskbar to Left",
                "Moves the taskbar Start button and icons to the left.",
                "TaskbarAl=0 restores the classic left-aligned taskbar layout from Windows 10.",
                RiskLevel.Low, false, false,
                Reg("HKCU\\Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced", "TaskbarAl", 0)),

            Tweak("vis_searchbox", "Hide Search Box on Taskbar",
                "Hides the search box/icon from the taskbar.",
                "SearchboxTaskbarMode=0 completely removes the search icon from the taskbar.",
                RiskLevel.Low, false, false,
                Reg("HKCU\\Software\\Microsoft\\Windows\\CurrentVersion\\Search", "SearchboxTaskbarMode", 0)),

            Tweak("vis_taskview", "Hide Task View Button",
                "Removes the Task View button from the taskbar.",
                "ShowTaskViewButton=0 hides the multi-desktop Task View button.",
                RiskLevel.Low, false, false,
                Reg("HKCU\\Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced", "ShowTaskViewButton", 0)),

            Tweak("vis_widgets", "Disable Widgets",
                "Removes the Widgets (News & Interests) button.",
                "TaskbarDa=0 hides the Widgets button from the taskbar.",
                RiskLevel.Low, false, false,
                Reg("HKCU\\Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced", "TaskbarDa", 0)),

            Tweak("vis_chat", "Hide Chat Icon",
                "Removes the Teams/Chat icon from the taskbar.",
                "TaskbarMn=0 hides the built-in Teams Chat shortcut from the taskbar.",
                RiskLevel.Low, false, false,
                Reg("HKCU\\Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced", "TaskbarMn", 0)),

            Tweak("vis_classic_menu", "Enable Classic Context Menu",
                "Restores the Windows 10 right-click menu everywhere.",
                "Creates the InprocServer32 key with an empty default value to bypass the new Win11 menu.",
                RiskLevel.Low, false, false,
                Reg("HKCU\\Software\\Classes\\CLSID\\{86ca1aa0-34aa-4e8b-a509-50c905bae2a2}\\InprocServer32", "", "", RegistryValueKind.String))
        });

    // ─── CUSTOMIZE PREFERENCES ────────────────────────────────────────────────

    private static TweakCategoryViewModel BuildCustomize() => Cat("Customize Preferences",
        "Personalize Windows to your workflow.", "🖥",
        new[]
        {
            Tweak("cust_darkmode", "Enable Dark Mode",
                "Switches Windows and apps to dark theme.",
                "Sets AppsUseLightTheme=0 and SystemUsesLightTheme=0 for full dark mode.",
                RiskLevel.Low, false, false,
                Reg("HKCU\\Software\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize", "AppsUseLightTheme", 0),
                Reg("HKCU\\Software\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize", "SystemUsesLightTheme", 0)),

            Tweak("cust_numlock", "Enable NumLock on Startup",
                "Turns NumLock on automatically at login.",
                "Sets InitialKeyboardIndicators=2 so NumLock is active on the login screen.",
                RiskLevel.Low, false, false,
                Reg("HKU\\.DEFAULT\\Control Panel\\Keyboard", "InitialKeyboardIndicators", "2", RegistryValueKind.String)),

            Tweak("cust_mouse_accel", "Disable Mouse Acceleration",
                "Removes the mouse pointer precision (acceleration) curve.",
                "Sets MouseSpeed=0, MouseThreshold1=0, MouseThreshold2=0 for a 1:1 linear mouse response.",
                RiskLevel.Low, false, false,
                Reg("HKCU\\Control Panel\\Mouse", "MouseSpeed", "0", RegistryValueKind.String),
                Reg("HKCU\\Control Panel\\Mouse", "MouseThreshold1", "0", RegistryValueKind.String),
                Reg("HKCU\\Control Panel\\Mouse", "MouseThreshold2", "0", RegistryValueKind.String)),

            Tweak("cust_bing_start", "Disable Bing in Start Menu",
                "Stops Bing web results from appearing in Start menu search.",
                "BingSearchEnabled=0 keeps Start menu searches local.",
                RiskLevel.Low, false, false,
                Reg("HKCU\\Software\\Microsoft\\Windows\\CurrentVersion\\Search", "BingSearchEnabled", 0)),

            Tweak("cust_websearch", "Disable Web Search Suggestions",
                "Hides web suggestions in the search box.",
                "Disables BingSearchEnabled and DisableSearchBoxSuggestions.",
                RiskLevel.Low, false, false,
                Reg("HKCU\\Software\\Microsoft\\Windows\\CurrentVersion\\Search", "BingSearchEnabled", 0),
                Reg("HKCU\\Software\\Policies\\Microsoft\\Windows\\Explorer", "DisableSearchBoxSuggestions", 1)),

            Tweak("cust_tailored", "Disable Tailored Experiences",
                "Removes personalized tips and ads based on your data.",
                "Disables TailoredExperiencesWithDiagnosticDataEnabled.",
                RiskLevel.Low, false, false,
                Reg("HKCU\\Software\\Microsoft\\Windows\\CurrentVersion\\Privacy", "TailoredExperiencesWithDiagnosticDataEnabled", 0)),

            Tweak("cust_taskbar_left", "Align Taskbar to Left",
                "Moves the taskbar icons to the left side.",
                "TaskbarAl=0 restores the classic left-aligned taskbar.",
                RiskLevel.Low, false, false,
                Reg("HKCU\\Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced", "TaskbarAl", 0)),

            Tweak("cust_fileext", "Show File Extensions",
                "Shows file extensions in Explorer.",
                "HideFileExt=0 reveals full file names including extensions.",
                RiskLevel.Low, false, false,
                Reg("HKCU\\Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced", "HideFileExt", 0)),

            Tweak("cust_hidden", "Show Hidden Files",
                "Reveals hidden files and folders in Explorer.",
                "Hidden=1 makes hidden items visible.",
                RiskLevel.Low, false, false,
                Reg("HKCU\\Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced", "Hidden", 1)),

            Tweak("cust_snap", "Disable Snap Assist",
                "Removes the snap layout popup when dragging windows.",
                "WindowArrangementActive=0 disables snap layout suggestions.",
                RiskLevel.Low, false, false,
                Reg("HKCU\\Control Panel\\Desktop", "WindowArrangementActive", "0", RegistryValueKind.String)),

            Tweak("cust_bsod", "Show Detailed BSOD Info",
                "Displays full stop code and parameters on blue screens.",
                "DisplayParameters=1 shows the detailed crash parameters on the BSOD screen for easier diagnostics.",
                RiskLevel.Low, true, false,
                Reg("HKLM\\SYSTEM\\CurrentControlSet\\Control\\CrashControl", "DisplayParameters", 1))
        });

    // ─── EXTREME ──────────────────────────────────────────────────────────────

    private static TweakCategoryViewModel BuildExtreme() => Cat("EXTREME Performance",
        "⚠ Advanced low-level tweaks. High risk. For enthusiasts only.", "💀",
        new[]
        {
            Tweak("ext_hpet", "Disable HPET",
                "Removes the High Precision Event Timer from the boot config.",
                "Disabling HPET can reduce timer overhead on some systems. Requires restart. May cause instability.",
                RiskLevel.Medium, true, true,
                Bcd("/deletevalue useplatformclock")),

            Tweak("ext_dynamictick", "Disable Dynamic Tick",
                "Forces a fixed timer interrupt rate.",
                "disabledynamictick=yes forces a constant timer interrupt, which can reduce latency on some CPUs.",
                RiskLevel.Medium, true, true,
                Bcd("/set disabledynamictick yes")),

            Tweak("ext_timer_res", "Set Timer Resolution to 0.5ms",
                "Forces the system timer to the highest resolution.",
                "useplatformtick=yes and GlobalTimerResolutionRequests=1 request 0.5ms timer resolution for lower input latency.",
                RiskLevel.Low, true, false,
                Bcd("/set useplatformtick yes"),
                Reg("HKLM\\SYSTEM\\CurrentControlSet\\Control\\Session Manager\\kernel", "GlobalTimerResolutionRequests", 1)),

            Tweak("ext_memcompression", "Disable Memory Compression",
                "Stops Windows from compressing memory pages. Recommended for 16GB+ RAM.",
                "Disable-MMAgent -MemoryCompression removes CPU overhead of compressing RAM. Only beneficial with plenty of RAM.",
                RiskLevel.Medium, true, false,
                PS("Disable-MMAgent -MemoryCompression")),

            Tweak("ext_mmcss", "Optimize MMCSS for Games",
                "Sets GPU and CPU priority for the Games MMCSS profile.",
                "GPU Priority=8, Priority=6, Scheduling Category=High in the Games MMCSS profile for maximum scheduling priority.",
                RiskLevel.Low, true, false,
                Reg("HKLM\\SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Multimedia\\SystemProfile\\Tasks\\Games", "GPU Priority", 8),
                Reg("HKLM\\SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Multimedia\\SystemProfile\\Tasks\\Games", "Priority", 6),
                Reg("HKLM\\SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Multimedia\\SystemProfile\\Tasks\\Games", "Scheduling Category", "High", RegistryValueKind.String)),

            Tweak("ext_gpu_telemetry", "Disable GPU Driver Telemetry",
                "Disables NVIDIA/AMD telemetry in the driver.",
                "Sets NvCplEnableHWAPI=0 in NVIDIA registry path to disable driver telemetry callbacks.",
                RiskLevel.Low, true, false,
                Reg("HKLM\\SYSTEM\\CurrentControlSet\\Services\\nvlddmkm\\Global\\NvTweak", "NvCplEnableHWAPI", 0),
                Reg("HKLM\\SYSTEM\\CurrentControlSet\\Services\\amdkmpfd\\Parameters", "TelemetryEnabled", 0)),

            Tweak("ext_cpu_sched", "CPU Scheduler Optimization",
                "Maximizes CPU time for the foreground process.",
                "Win32PrioritySeparation=38 is the same as perf_cpu_priority but placed here for extreme tuning bundles.",
                RiskLevel.Low, true, false,
                Reg("HKLM\\SYSTEM\\CurrentControlSet\\Control\\PriorityControl", "Win32PrioritySeparation", 38)),

            Tweak("ext_spectre", "Disable Spectre/Meltdown Mitigations",
                "⚠ CRITICAL: Removes CPU vulnerability mitigations for performance.",
                "Disabling Spectre/Meltdown protections improves performance but exposes the CPU to side-channel attacks. DO NOT USE on shared or internet-facing systems.",
                RiskLevel.Critical, true, true,
                Bcd("/set {current} kva shadowintegrity disabled"),
                Bcd("/set {current} nx optout"),
                Bcd("/set {current} kva shadows no")),

            Tweak("ext_pagefile", "Optimize Page File",
                "Sets a fixed-size page file for predictable performance.",
                "A fixed page file prevents resize overhead. 4096-8192 MB is recommended. PowerShell sets the fixed size on C:.",
                RiskLevel.Low, true, false,
                PS("$cs = Get-WmiObject -Class Win32_ComputerSystem; " +
                   "$cs.AutomaticManagedPagefile = $False; $cs.Put() | Out-Null; " +
                   "$pf = Get-WmiObject -Class Win32_PageFileSetting; " +
                   "if ($pf) { $pf.InitialSize = 4096; $pf.MaximumSize = 8192; $pf.Put() | Out-Null } " +
                   "else { Set-WmiInstance -Class Win32_PageFileSetting -Arguments @{Name='C:\\pagefile.sys';InitialSize=4096;MaximumSize=8192} | Out-Null }")),

            Tweak("ext_cstates", "Disable C-States (CPU Idle States)",
                "Prevents the CPU from entering deep sleep states. Requires restart.",
                "disablecpuidle=1 keeps the CPU in C0 at all times, eliminating wake latency at the cost of higher power consumption.",
                RiskLevel.High, true, true,
                Bcd("/set disablecpuidle 1"))
        });

    // ─── Builder helpers ──────────────────────────────────────────────────────

    private static TweakCategoryViewModel Cat(string name, string description, string icon,
        IEnumerable<TweakViewModel> tweaks)
    {
        var cat = new TweakCategoryViewModel { Name = name, Description = description, Icon = icon };
        foreach (var t in tweaks) cat.Tweaks.Add(t);
        cat.NotifyCounts();
        return cat;
    }

    private static TweakViewModel Tweak(string id, string name, string description, string tooltip,
        RiskLevel risk, bool adminRequired = false, bool requiresRestart = false,
        params TweakCommand[] commands)
    {
        var model = new Models.Tweak
        {
            Id = id,
            Name = name,
            Description = description,
            Tooltip = tooltip,
            RiskLevel = risk,
            IsAdminRequired = adminRequired,
            RequiresRestart = requiresRestart
        };
        model.Commands.AddRange(commands);
        return new TweakViewModel(model);
    }

    private static TweakViewModel AppxTweak(string id, string name, string package,
        RiskLevel risk = RiskLevel.Low)
    {
        return Tweak(id, name,
            $"Removes the {name.Replace("Remove ", "")} built-in app.",
            $"Uses Get-AppxPackage {package} | Remove-AppxPackage -ErrorAction SilentlyContinue to remove packages safely, skipping any that fail.",
            risk, true, false,
            PS($"Get-AppxPackage {package} -ErrorAction SilentlyContinue | Remove-AppxPackage -ErrorAction SilentlyContinue"));
    }

    private static TweakCommand Reg(string path, string name, object value,
        RegistryValueKind kind = RegistryValueKind.DWord)
    {
        string valStr;
        if (value is byte[] bytes)
            valStr = Convert.ToBase64String(bytes);
        else
            valStr = value?.ToString() ?? string.Empty;

        return new TweakCommand
        {
            Type = CommandType.Registry,
            Command = $"{path}|{name}|{valStr}|{kind}",
            Description = $"Set {path}\\{name} = {value}"
        };
    }

    private static TweakCommand Cmd(string command) =>
        new() { Type = CommandType.Command, Command = command, Description = command };

    private static TweakCommand PS(string command) =>
        new() { Type = CommandType.PowerShell, Command = command, Description = command };

    private static TweakCommand Svc(string service, string startType) =>
        new() { Type = CommandType.Service, Command = $"{service}|{startType}", Description = $"Set {service} to {startType}" };

    private static TweakCommand NetSh(string args) =>
        new() { Type = CommandType.NetSh, Command = args, Description = $"netsh {args}" };

    private static TweakCommand Bcd(string args) =>
        new() { Type = CommandType.BcdEdit, Command = args, Description = $"bcdedit {args}" };
}
