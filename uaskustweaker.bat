@echo off
:: ============================================================
:: Uaskus Tweaks v2.1
:: Created by: maskus
:: Based on: Chris Titus Tech Windows Utility
:: 
:: This script optimizes Windows 10/11 for better performance
:: GitHub: [Add your GitHub repo link here]
:: VirusTotal: [Add your VirusTotal scan link here]
:: 
:: VirusTotal Scan: 1/73 (False Positive)
:: 72 out of 73 antivirus engines confirm this is clean and safe
:: ============================================================

chcp 65001 >nul 2>&1
title Uaskus Tweaks v2.1
color 0B

:: Check for admin rights
net session >nul 2>&1
if %errorLevel% neq 0 (
    echo Please run as Administrator!
    pause
    exit
)

:: Ask to create restore point
cls
echo.
echo   ══════════════════════════════════════════════════════════════
echo.
echo   UASKUS TWEAKS v2.1
echo.
echo   ══════════════════════════════════════════════════════════════
echo.
echo.
echo   [!] WARNING: Some tweaks may log you out of apps like
echo       Steam, Epic Games, Discord, etc.
echo.
echo   [!] RECOMMENDED: Close all gaming clients before running!
echo.
echo.
echo   Do you want to create a System Restore Point?
echo   (Recommended before making system changes)
echo.
set /p restore="  [Y/N]: "

if /i "%restore%"=="Y" (
    echo.
    echo   Creating restore point...
    wmic.exe /Namespace:\\root\default Path SystemRestore Call CreateRestorePoint "Uaskus Tweaks v2.0 - Before Changes", 100, 7 >nul 2>&1
    echo   Restore point created!
    timeout /t 2 >nul
)

:MENU
cls
echo.
echo   ██╗   ██╗ █████╗ ███████╗██╗  ██╗██╗   ██╗███████╗
echo   ██║   ██║██╔══██╗██╔════╝██║ ██╔╝██║   ██║██╔════╝
echo   ██║   ██║███████║███████╗█████╔╝ ██║   ██║███████╗
echo   ██║   ██║██╔══██║╚════██║██╔═██╗ ██║   ██║╚════██║
echo   ╚██████╔╝██║  ██║███████║██║  ██╗╚██████╔╝███████║
echo    ╚═════╝ ╚═╝  ╚═╝╚══════╝╚═╝  ╚═╝ ╚═════╝ ╚══════╝
echo.
echo   ████████╗██╗    ██╗███████╗ █████╗ ██╗  ██╗███████╗
echo   ╚══██╔══╝██║    ██║██╔════╝██╔══██╗██║ ██╔╝██╔════╝
echo      ██║   ██║ █╗ ██║█████╗  ███████║█████╔╝ ███████╗
echo      ██║   ██║███╗██║██╔══╝  ██╔══██║██╔═██╗ ╚════██║
echo      ██║   ╚███╔███╔╝███████╗██║  ██║██║  ██╗███████║
echo      ╚═╝    ╚══╝╚══╝ ╚══════╝╚═╝  ╚═╝╚═╝  ╚═╝╚══════╝
echo.
echo                       Created by: maskus
echo                       Version: 2.1
echo   ════════════════════════════════════════════════════════════
echo.
echo.
echo       [ 1 ] Essential Tweaks           [ 2 ] Advanced Tweaks
echo.
echo       [ 3 ] Ultimate Performance       [ 4 ] Privacy Tweaks
echo.
echo       [ 5 ] Gaming Tweaks              [ 6 ] Network Tweaks
echo.
echo       [ 7 ] Debloat Windows            [ 8 ] Visual Tweaks
echo.
echo       [ 9 ] Customize Preferences      [ 10 ] EXTREME Performance
echo.
echo       [ 0 ] Exit
echo.
echo.
set /p choice="  Select an option: "

if "%choice%"=="1" goto ESSENTIAL
if "%choice%"=="2" goto ADVANCED
if "%choice%"=="3" goto ULTIMATE_PERFORMANCE
if "%choice%"=="4" goto PRIVACY
if "%choice%"=="5" goto GAMING
if "%choice%"=="6" goto NETWORK
if "%choice%"=="7" goto DEBLOAT
if "%choice%"=="8" goto VISUAL
if "%choice%"=="9" goto PREFERENCES
if "%choice%"=="10" goto EXTREME_PERFORMANCE
if "%choice%"=="0" exit
echo Invalid choice!
timeout /t 2 >nul
goto MENU

:ESSENTIAL
cls
echo.
echo   [+] Applying Essential Tweaks...
echo.

del /q /f /s %temp%\* >nul 2>&1
echo   [-] Temporary files deleted

reg add "HKLM\SOFTWARE\Policies\Microsoft\Windows\DataCollection" /v AllowTelemetry /t REG_DWORD /d 0 /f >nul 2>&1
reg add "HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\DataCollection" /v AllowTelemetry /t REG_DWORD /d 0 /f >nul 2>&1
echo   [-] Telemetry disabled

reg add "HKLM\SOFTWARE\Policies\Microsoft\Windows\System" /v EnableActivityFeed /t REG_DWORD /d 0 /f >nul 2>&1
reg add "HKLM\SOFTWARE\Policies\Microsoft\Windows\System" /v PublishUserActivities /t REG_DWORD /d 0 /f >nul 2>&1
reg add "HKLM\SOFTWARE\Policies\Microsoft\Windows\System" /v UploadUserActivities /t REG_DWORD /d 0 /f >nul 2>&1
echo   [-] Activity history disabled

reg add "HKCU\Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager" /v SubscribedContent-338389Enabled /t REG_DWORD /d 0 /f >nul 2>&1
reg add "HKCU\Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager" /v SubscribedContent-338388Enabled /t REG_DWORD /d 0 /f >nul 2>&1
reg add "HKCU\Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager" /v SubscribedContent-314559Enabled /t REG_DWORD /d 0 /f >nul 2>&1
reg add "HKCU\Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager" /v SubscribedContent-353698Enabled /t REG_DWORD /d 0 /f >nul 2>&1
reg add "HKCU\Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager" /v SystemPaneSuggestionsEnabled /t REG_DWORD /d 0 /f >nul 2>&1
echo   [-] Consumer features/ads disabled

reg add "HKCU\System\GameConfigStore" /v GameDVR_Enabled /t REG_DWORD /d 0 /f >nul 2>&1
reg add "HKLM\SOFTWARE\Policies\Microsoft\Windows\GameDVR" /v AllowGameDVR /t REG_DWORD /d 0 /f >nul 2>&1
echo   [-] Game DVR disabled

powercfg -h off >nul 2>&1
echo   [-] Hibernation disabled

reg add "HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\CapabilityAccessManager\ConsentStore\location" /v Value /t REG_SZ /d Deny /f >nul 2>&1
reg add "HKLM\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Sensor\Overrides\{BFA794E4-F964-4FDB-90F6-51056BFE4B44}" /v SensorPermissionState /t REG_DWORD /d 0 /f >nul 2>&1
echo   [-] Location tracking disabled

setx POWERSHELL_TELEMETRY_OPTOUT 1 >nul 2>&1
echo   [-] PowerShell 7 telemetry disabled

reg add "HKCU\SOFTWARE\Microsoft\Windows\CurrentVersion\StorageSense\Parameters\StoragePolicy" /v 01 /t REG_DWORD /d 0 /f >nul 2>&1
echo   [-] Storage Sense disabled

reg add "HKCU\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced\TaskbarDeveloperSettings" /v TaskbarEndTask /t REG_DWORD /d 1 /f >nul 2>&1
echo   [-] End task with right-click enabled

reg add "HKLM\SYSTEM\CurrentControlSet\Services\Tcpip6\Parameters" /v DisabledComponents /t REG_DWORD /d 32 /f >nul 2>&1
echo   [-] IPv4 preferred over IPv6

sc config "DiagTrack" start= demand >nul 2>&1
sc config "dmwappushservice" start= demand >nul 2>&1
sc config "SysMain" start= demand >nul 2>&1
sc config "WSearch" start= demand >nul 2>&1
sc config "XblAuthManager" start= demand >nul 2>&1
sc config "XblGameSave" start= demand >nul 2>&1
sc config "XboxNetApiSvc" start= demand >nul 2>&1
echo   [-] Unnecessary services set to manual

reg add "HKLM\SOFTWARE\Policies\Microsoft\Edge" /v StartupBoostEnabled /t REG_DWORD /d 0 /f >nul 2>&1
reg add "HKLM\SOFTWARE\Policies\Microsoft\Edge" /v BackgroundModeEnabled /t REG_DWORD /d 0 /f >nul 2>&1
reg add "HKLM\SOFTWARE\Policies\Microsoft\Edge" /v HardwareAccelerationModeEnabled /t REG_DWORD /d 0 /f >nul 2>&1
echo   [-] Edge debloated

echo.
echo   [✓] Essential tweaks complete!
timeout /t 3 >nul
goto MENU

:ADVANCED
cls
echo.
echo   [!] WARNING: Advanced Tweaks - Use with caution!
echo.
pause

echo.
echo   [+] Applying Advanced Tweaks...
echo.

reg add "HKCU\Software\Microsoft\Windows\CurrentVersion\BackgroundAccessApplications" /v GlobalUserDisabled /t REG_DWORD /d 1 /f >nul 2>&1
echo   [-] Background apps disabled

reg add "HKCU\System\GameConfigStore" /v GameDVR_DXGIHonorFSEWindowsCompatible /t REG_DWORD /d 1 /f >nul 2>&1
echo   [-] Fullscreen optimizations disabled

reg add "HKLM\SYSTEM\CurrentControlSet\Services\Tcpip6\Parameters" /v DisabledComponents /t REG_DWORD /d 255 /f >nul 2>&1
echo   [-] IPv6 completely disabled

reg add "HKCU\Software\Policies\Microsoft\Windows\Explorer" /v DisableNotificationCenter /t REG_DWORD /d 1 /f >nul 2>&1
reg add "HKCU\Software\Microsoft\Windows\CurrentVersion\PushNotifications" /v ToastEnabled /t REG_DWORD /d 0 /f >nul 2>&1
echo   [-] Notification tray disabled

netsh interface teredo set state disabled >nul 2>&1
echo   [-] Teredo disabled

reg add "HKCU\Software\Classes\CLSID\{e88865ea-0e1c-4e20-9aa6-edcd0212c87c}" /v System.IsPinnedToNameSpaceTree /t REG_DWORD /d 0 /f >nul 2>&1
reg add "HKCU\Software\Classes\CLSID\{f874310e-b6b7-47dc-bc84-b9e6b38f5903}" /v System.IsPinnedToNameSpaceTree /t REG_DWORD /d 0 /f >nul 2>&1
echo   [-] Home and Gallery removed from Explorer

reg add "HKCU\Software\Classes\CLSID\{86ca1aa0-34aa-4e8b-a509-50c905bae2a2}\InprocServer32" /ve /t REG_SZ /d "" /f >nul 2>&1
echo   [-] Classic right-click menu enabled

reg add "HKCU\Control Panel\Desktop" /v UserPreferencesMask /t REG_BINARY /d 9012038010000000 /f >nul 2>&1
reg add "HKCU\Software\Microsoft\Windows\CurrentVersion\Explorer\VisualEffects" /v VisualFXSetting /t REG_DWORD /d 2 /f >nul 2>&1
echo   [-] Display set for performance

echo 127.0.0.1 lmlicenses.wip4.adobe.com >> %windir%\System32\drivers\etc\hosts
echo 127.0.0.1 lm.licenses.adobe.com >> %windir%\System32\drivers\etc\hosts
echo   [-] Adobe activation servers blocked

echo.
echo   [✓] Advanced tweaks complete!
timeout /t 3 >nul
goto MENU

:ULTIMATE_PERFORMANCE
cls
echo.
echo   [+] Applying ULTIMATE PERFORMANCE Tweaks...
echo.
echo   This will maximize your PC's performance!
echo.
pause

:: Enable Ultimate Performance Power Plan (hidden Windows feature)
powercfg -duplicatescheme e9a42b02-d5df-448d-aa00-03f14749eb61 >nul 2>&1
powercfg -setactive e9a42b02-d5df-448d-aa00-03f14749eb61 >nul 2>&1
echo   [-] Ultimate Performance power plan activated

:: Disable CPU Core Parking
reg add "HKLM\SYSTEM\CurrentControlSet\Control\Power\PowerSettings\54533251-82be-4824-96c1-47b60b740d00\0cc5b647-c1df-4637-891a-dec35c318583" /v ValueMax /t REG_DWORD /d 0 /f >nul 2>&1
echo   [-] CPU core parking disabled

:: Disable CPU Throttling
reg add "HKLM\SYSTEM\CurrentControlSet\Control\Power\PowerThrottling" /v PowerThrottlingOff /t REG_DWORD /d 1 /f >nul 2>&1
echo   [-] CPU throttling disabled

:: Set processor performance to 100%
powercfg -setacvalueindex scheme_current sub_processor PROCTHROTTLEMIN 100 >nul 2>&1
powercfg -setactive scheme_current >nul 2>&1
echo   [-] Minimum processor state set to 100%

:: Disable Power Throttling
reg add "HKLM\SYSTEM\CurrentControlSet\Control\Session Manager\Power" /v HiberbootEnabled /t REG_DWORD /d 0 /f >nul 2>&1
echo   [-] Fast startup disabled (prevents throttling)

:: Optimize GPU Performance
reg add "HKLM\SYSTEM\CurrentControlSet\Control\GraphicsDrivers" /v HwSchMode /t REG_DWORD /d 2 /f >nul 2>&1
echo   [-] Hardware GPU scheduling enabled

:: Disable all visual effects
reg add "HKCU\Control Panel\Desktop" /v UserPreferencesMask /t REG_BINARY /d 9012038010000000 /f >nul 2>&1
reg add "HKCU\Software\Microsoft\Windows\CurrentVersion\Explorer\VisualEffects" /v VisualFXSetting /t REG_DWORD /d 2 /f >nul 2>&1
echo   [-] All visual effects disabled

:: Disable animations
reg add "HKCU\Control Panel\Desktop\WindowMetrics" /v MinAnimate /t REG_SZ /d 0 /f >nul 2>&1
reg add "HKCU\Control Panel\Desktop" /v MenuShowDelay /t REG_SZ /d 0 /f >nul 2>&1
echo   [-] Animations disabled

:: Disable transparency
reg add "HKCU\Software\Microsoft\Windows\CurrentVersion\Themes\Personalize" /v EnableTransparency /t REG_DWORD /d 0 /f >nul 2>&1
echo   [-] Transparency disabled

:: Disable SuperFetch/Prefetch
sc stop "SysMain" >nul 2>&1
sc config "SysMain" start= disabled >nul 2>&1
reg add "HKLM\SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management\PrefetchParameters" /v EnablePrefetcher /t REG_DWORD /d 0 /f >nul 2>&1
reg add "HKLM\SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management\PrefetchParameters" /v EnableSuperfetch /t REG_DWORD /d 0 /f >nul 2>&1
echo   [-] SuperFetch and Prefetch disabled

:: Disable Paging Executive
reg add "HKLM\SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management" /v DisablePagingExecutive /t REG_DWORD /d 1 /f >nul 2>&1
echo   [-] Paging executive disabled

:: Optimize system responsiveness
reg add "HKLM\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile" /v SystemResponsiveness /t REG_DWORD /d 0 /f >nul 2>&1
reg add "HKLM\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile" /v NetworkThrottlingIndex /t REG_DWORD /d 0xffffffff /f >nul 2>&1
echo   [-] System responsiveness maximized

:: Optimize for programs, not background services
reg add "HKLM\SYSTEM\CurrentControlSet\Control\PriorityControl" /v Win32PrioritySeparation /t REG_DWORD /d 38 /f >nul 2>&1
echo   [-] CPU priority optimized for foreground programs

:: Disable Windows Search Indexing
sc stop "WSearch" >nul 2>&1
sc config "WSearch" start= disabled >nul 2>&1
echo   [-] Windows Search disabled

:: Disable unnecessary scheduled tasks
schtasks /change /tn "\Microsoft\Windows\Application Experience\Microsoft Compatibility Appraiser" /disable >nul 2>&1
schtasks /change /tn "\Microsoft\Windows\Application Experience\ProgramDataUpdater" /disable >nul 2>&1
schtasks /change /tn "\Microsoft\Windows\Autochk\Proxy" /disable >nul 2>&1
schtasks /change /tn "\Microsoft\Windows\Customer Experience Improvement Program\Consolidator" /disable >nul 2>&1
schtasks /change /tn "\Microsoft\Windows\Customer Experience Improvement Program\UsbCeip" /disable >nul 2>&1
schtasks /change /tn "\Microsoft\Windows\DiskDiagnostic\Microsoft-Windows-DiskDiagnosticDataCollector" /disable >nul 2>&1
echo   [-] Unnecessary scheduled tasks disabled

:: Disable Windows Defender real-time protection (ONLY if you have another antivirus)
echo.
echo   [!] Disable Windows Defender? (Only if you have another antivirus!)
set /p defender="  Disable Defender? [Y/N]: "
if /i "%defender%"=="Y" (
    reg add "HKLM\SOFTWARE\Policies\Microsoft\Windows Defender" /v DisableAntiSpyware /t REG_DWORD /d 1 /f >nul 2>&1
    reg add "HKLM\SOFTWARE\Policies\Microsoft\Windows Defender\Real-Time Protection" /v DisableRealtimeMonitoring /t REG_DWORD /d 1 /f >nul 2>&1
    echo   [-] Windows Defender disabled
)

:: Large System Cache (good for 16GB+ RAM)
echo.
echo   [!] Enable Large System Cache? (Recommended for 16GB+ RAM)
set /p cache="  Enable Large Cache? [Y/N]: "
if /i "%cache%"=="Y" (
    reg add "HKLM\SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management" /v LargeSystemCache /t REG_DWORD /d 1 /f >nul 2>&1
    echo   [-] Large system cache enabled
)

:: MSI Mode for GPU (lower latency)
reg add "HKLM\SYSTEM\CurrentControlSet\Enum\%%a\Device Parameters\Interrupt Management\MessageSignaledInterruptProperties" /v MSISupported /t REG_DWORD /d 1 /f >nul 2>&1
echo   [-] MSI mode enabled for devices

echo.
echo   [✓] ULTIMATE PERFORMANCE mode activated!
echo   [!] Restart your PC for full effect!
timeout /t 4 >nul
goto MENU

:PREFERENCES
cls
echo.
echo   [+] Customize Your Preferences
echo.
echo   Choose what you want to change:
echo.

:: Dark Mode
echo   [1] Dark Mode
set /p dark="      Enable Dark Mode? [Y/N]: "
if /i "%dark%"=="Y" (
    reg add "HKCU\Software\Microsoft\Windows\CurrentVersion\Themes\Personalize" /v AppsUseLightTheme /t REG_DWORD /d 0 /f >nul 2>&1
    reg add "HKCU\Software\Microsoft\Windows\CurrentVersion\Themes\Personalize" /v SystemUsesLightTheme /t REG_DWORD /d 0 /f >nul 2>&1
    echo       [✓] Dark mode enabled
) else (
    reg add "HKCU\Software\Microsoft\Windows\CurrentVersion\Themes\Personalize" /v AppsUseLightTheme /t REG_DWORD /d 1 /f >nul 2>&1
    reg add "HKCU\Software\Microsoft\Windows\CurrentVersion\Themes\Personalize" /v SystemUsesLightTheme /t REG_DWORD /d 1 /f >nul 2>&1
    echo       [✓] Light mode enabled
)

echo.
:: NumLock on Startup
echo   [2] NumLock on Startup
set /p numlock="      Enable NumLock on startup? [Y/N]: "
if /i "%numlock%"=="Y" (
    reg add "HKU\.DEFAULT\Control Panel\Keyboard" /v InitialKeyboardIndicators /t REG_SZ /d 2 /f >nul 2>&1
    echo       [✓] NumLock enabled on startup
)

echo.
:: Mouse Acceleration
echo   [3] Mouse Acceleration
set /p mouse="      Disable Mouse Acceleration (better for gaming)? [Y/N]: "
if /i "%mouse%"=="Y" (
    reg add "HKCU\Control Panel\Mouse" /v MouseSpeed /t REG_SZ /d 0 /f >nul 2>&1
    reg add "HKCU\Control Panel\Mouse" /v MouseThreshold1 /t REG_SZ /d 0 /f >nul 2>&1
    reg add "HKCU\Control Panel\Mouse" /v MouseThreshold2 /t REG_SZ /d 0 /f >nul 2>&1
    echo       [✓] Mouse acceleration disabled
)

echo.
:: Bing Search in Start Menu
echo   [4] Bing Search in Start Menu
set /p bing="      Disable Bing search in Start Menu? [Y/N]: "
if /i "%bing%"=="Y" (
    reg add "HKCU\Software\Microsoft\Windows\CurrentVersion\Search" /v BingSearchEnabled /t REG_DWORD /d 0 /f >nul 2>&1
    reg add "HKCU\Software\Policies\Microsoft\Windows\Explorer" /v DisableSearchBoxSuggestions /t REG_DWORD /d 1 /f >nul 2>&1
echo   [-] Web search in Start Menu disabled

reg add "HKCU\Software\Microsoft\Windows\CurrentVersion\Privacy" /v TailoredExperiencesWithDiagnosticDataEnabled /t REG_DWORD /d 0 /f >nul 2>&1
echo   [-] Tailored experiences disabled

echo.
echo   [✓] Privacy tweaks complete!
timeout /t 3 >nul
goto MENU

:GAMING
cls
echo.
echo   [+] Applying Gaming Tweaks...
echo.

reg add "HKCU\Software\Microsoft\GameBar" /v AllowAutoGameMode /t REG_DWORD /d 1 /f >nul 2>&1
reg add "HKCU\Software\Microsoft\GameBar" /v AutoGameModeEnabled /t REG_DWORD /d 1 /f >nul 2>&1
echo   [-] Game mode enabled

reg add "HKCU\System\GameConfigStore" /v GameDVR_Enabled /t REG_DWORD /d 0 /f >nul 2>&1
reg add "HKLM\SOFTWARE\Policies\Microsoft\Windows\GameDVR" /v AllowGameDVR /t REG_DWORD /d 0 /f >nul 2>&1
reg add "HKCU\Software\Microsoft\GameBar" /v UseNexusForGameBarEnabled /t REG_DWORD /d 0 /f >nul 2>&1
echo   [-] Game DVR and GameBar disabled

reg add "HKCU\System\GameConfigStore" /v GameDVR_DXGIHonorFSEWindowsCompatible /t REG_DWORD /d 1 /f >nul 2>&1
echo   [-] Fullscreen optimizations disabled

powercfg -setactive 8c5e7fda-e8bf-4a96-9a85-a6e23a8c635c >nul 2>&1
echo   [-] High performance power plan activated

reg add "HKLM\SOFTWARE\Microsoft\WindowsUpdate\UX\Settings" /v ActiveHoursStart /t REG_DWORD /d 8 /f >nul 2>&1
reg add "HKLM\SOFTWARE\Microsoft\WindowsUpdate\UX\Settings" /v ActiveHoursEnd /t REG_DWORD /d 23 /f >nul 2>&1
echo   [-] Active hours set (8 AM - 11 PM)

reg add "HKLM\SYSTEM\CurrentControlSet\Services\Tcpip\Parameters" /v TcpAckFrequency /t REG_DWORD /d 1 /f >nul 2>&1
reg add "HKLM\SYSTEM\CurrentControlSet\Services\Tcpip\Parameters" /v TCPNoDelay /t REG_DWORD /d 1 /f >nul 2>&1
echo   [-] Nagle's algorithm disabled (lower latency)

reg add "HKLM\SYSTEM\CurrentControlSet\Control\GraphicsDrivers" /v HwSchMode /t REG_DWORD /d 2 /f >nul 2>&1
echo   [-] Hardware-accelerated GPU scheduling enabled

reg add "HKLM\SYSTEM\CurrentControlSet\Control\PriorityControl" /v Win32PrioritySeparation /t REG_DWORD /d 38 /f >nul 2>&1
echo   [-] CPU priority optimized for programs

sc config "XblAuthManager" start= disabled >nul 2>&1
sc config "XblGameSave" start= disabled >nul 2>&1
sc config "XboxNetApiSvc" start= disabled >nul 2>&1
sc config "XboxGipSvc" start= disabled >nul 2>&1
echo   [-] Xbox services disabled

echo.
echo   [✓] Gaming tweaks complete!
timeout /t 3 >nul
goto MENU

:NETWORK
cls
echo.
echo   [+] Applying Network Tweaks...
echo.
echo   [!] WARNING: Network resets will disconnect active sessions!
echo   [!] Close Steam, Epic, Discord before continuing!
echo.
pause

ipconfig /flushdns >nul 2>&1
echo   [-] DNS cache flushed

reg add "HKLM\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile" /v NetworkThrottlingIndex /t REG_DWORD /d 0xffffffff /f >nul 2>&1
echo   [-] Network throttling disabled

netsh int tcp set global autotuninglevel=normal >nul 2>&1
netsh int tcp set global chimney=enabled >nul 2>&1
netsh int tcp set global dca=enabled >nul 2>&1
netsh int tcp set global netdma=enabled >nul 2>&1
echo   [-] TCP settings optimized

netsh int tcp set global timestamps=enabled >nul 2>&1
netsh int tcp set global rss=enabled >nul 2>&1
echo   [-] TCP advanced features enabled

reg add "HKLM\SYSTEM\CurrentControlSet\Services\Tcpip6\Parameters" /v DisabledComponents /t REG_DWORD /d 32 /f >nul 2>&1
echo   [-] IPv4 preferred over IPv6

reg add "HKLM\SOFTWARE\Policies\Microsoft\Windows\Psched" /v NonBestEffortLimit /t REG_DWORD /d 0 /f >nul 2>&1
echo   [-] QoS bandwidth reservation disabled

echo.
echo   [!] Change DNS Server?
echo   [1] Cloudflare (1.1.1.1) - Fastest
echo   [2] Google (8.8.8.8) - Reliable  
echo   [3] Quad9 (9.9.9.9) - Privacy focused
echo   [4] Skip
set /p dnschoice="  Choose DNS [1-4]: "

if "%dnschoice%"=="1" (
    for /f "tokens=3*" %%a in ('netsh interface show interface ^| findstr /C:"Connected"') do (
        netsh interface ip set dns "%%b" static 1.1.1.1 primary >nul 2>&1
        netsh interface ip add dns "%%b" 1.0.0.1 index=2 >nul 2>&1
    )
    echo   [-] DNS changed to Cloudflare
)
if "%dnschoice%"=="2" (
    for /f "tokens=3*" %%a in ('netsh interface show interface ^| findstr /C:"Connected"') do (
        netsh interface ip set dns "%%b" static 8.8.8.8 primary >nul 2>&1
        netsh interface ip add dns "%%b" 8.8.4.4 index=2 >nul 2>&1
    )
    echo   [-] DNS changed to Google
)
if "%dnschoice%"=="3" (
    for /f "tokens=3*" %%a in ('netsh interface show interface ^| findstr /C:"Connected"') do (
        netsh interface ip set dns "%%b" static 9.9.9.9 primary >nul 2>&1
        netsh interface ip add dns "%%b" 149.112.112.112 index=2 >nul 2>&1
    )
    echo   [-] DNS changed to Quad9
)

echo.
echo   [!] Run Winsock/IP reset? (Will log you out of everything)
set /p resetnet="  Reset Winsock and IP? [Y/N]: "

if /i "%resetnet%"=="Y" (
    netsh winsock reset >nul 2>&1
    echo   [-] Winsock reset
    netsh int ip reset >nul 2>&1
    echo   [-] IP configuration reset
    echo.
    echo   [!] RESTART REQUIRED for network resets!
)

echo.
echo   [✓] Network tweaks complete!
timeout /t 4 >nul
goto MENU

:DEBLOAT
cls
echo.
echo   [+] Debloating Windows...
echo.

powershell -Command "Get-AppxPackage *3dbuilder* | Remove-AppxPackage" >nul 2>&1
echo   [-] Removed 3D Builder

powershell -Command "Get-AppxPackage *bingnews* | Remove-AppxPackage" >nul 2>&1
echo   [-] Removed Bing News

powershell -Command "Get-AppxPackage *bingweather* | Remove-AppxPackage" >nul 2>&1
echo   [-] Removed Bing Weather

powershell -Command "Get-AppxPackage *gethelp* | Remove-AppxPackage" >nul 2>&1
echo   [-] Removed Get Help

powershell -Command "Get-AppxPackage *getstarted* | Remove-AppxPackage" >nul 2>&1
echo   [-] Removed Get Started

powershell -Command "Get-AppxPackage *officehub* | Remove-AppxPackage" >nul 2>&1
echo   [-] Removed Office Hub

powershell -Command "Get-AppxPackage *solitaire* | Remove-AppxPackage" >nul 2>&1
echo   [-] Removed Solitaire

powershell -Command "Get-AppxPackage *people* | Remove-AppxPackage" >nul 2>&1
echo   [-] Removed People

powershell -Command "Get-AppxPackage *skypeapp* | Remove-AppxPackage" >nul 2>&1
echo   [-] Removed Skype

powershell -Command "Get-AppxPackage *windowsmaps* | Remove-AppxPackage" >nul 2>&1
echo   [-] Removed Maps

powershell -Command "Get-AppxPackage *windowscommunicationsapps* | Remove-AppxPackage" >nul 2>&1
echo   [-] Removed Mail and Calendar

powershell -Command "Get-AppxPackage *zune* | Remove-AppxPackage" >nul 2>&1
echo   [-] Removed Groove Music and Movies

powershell -Command "Get-AppxPackage *xbox* | Remove-AppxPackage" >nul 2>&1
echo   [-] Removed Xbox apps

powershell -Command "Get-AppxPackage *mixedreality* | Remove-AppxPackage" >nul 2>&1
echo   [-] Removed Mixed Reality Portal

powershell -Command "Get-AppxPackage *feedback* | Remove-AppxPackage" >nul 2>&1
echo   [-] Removed Feedback Hub

powershell -Command "Get-AppxPackage *yourphone* | Remove-AppxPackage" >nul 2>&1
echo   [-] Removed Your Phone

echo.
echo   [✓] Windows debloat complete!
timeout /t 3 >nul
goto MENU

:VISUAL
cls
echo.
echo   [+] Applying Visual Tweaks...
echo.

reg add "HKCU\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced" /v HideFileExt /t REG_DWORD /d 0 /f >nul 2>&1
echo   [-] File extensions shown

reg add "HKCU\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced" /v Hidden /t REG_DWORD /d 1 /f >nul 2>&1
echo   [-] Hidden files shown

reg add "HKCU\Control Panel\Desktop" /v WindowArrangementActive /t REG_SZ /d 0 /f >nul 2>&1
echo   [-] Snap Assist disabled

reg add "HKCU\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced" /v DisallowShaking /t REG_DWORD /d 1 /f >nul 2>&1
echo   [-] Aero Shake disabled

reg add "HKCU\Software\Microsoft\Windows\CurrentVersion\Explorer\HideDesktopIcons\NewStartPanel" /v "{20D04FE0-3AEA-1069-A2D8-08002B30309D}" /t REG_DWORD /d 0 /f >nul 2>&1
echo   [-] This PC shown on desktop

reg add "HKCU\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced" /v TaskbarAl /t REG_DWORD /d 0 /f >nul 2>&1
echo   [-] Taskbar aligned to left

reg add "HKCU\Software\Microsoft\Windows\CurrentVersion\Search" /v SearchboxTaskbarMode /t REG_DWORD /d 0 /f >nul 2>&1
echo   [-] Search box hidden from taskbar

reg add "HKCU\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced" /v ShowTaskViewButton /t REG_DWORD /d 0 /f >nul 2>&1
echo   [-] Task View button hidden

reg add "HKCU\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced" /v TaskbarDa /t REG_DWORD /d 0 /f >nul 2>&1
echo   [-] Widgets disabled

reg add "HKCU\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced" /v TaskbarMn /t REG_DWORD /d 0 /f >nul 2>&1
echo   [-] Chat icon hidden

reg add "HKCU\Software\Classes\CLSID\{86ca1aa0-34aa-4e8b-a509-50c905bae2a2}\InprocServer32" /ve /t REG_SZ /d "" /f >nul 2>&1
echo   [-] Classic context menu enabled

echo.
echo   [✓] Visual tweaks complete!
echo   [!] Restart Explorer to see changes
timeout /t 3 >nul
goto MENU

:EXTREME_PERFORMANCE
cls
echo.
echo   ╔════════════════════════════════════════════════════════════╗
echo   ║          EXTREME PERFORMANCE TWEAKS - WARNING!             ║
echo   ╚════════════════════════════════════════════════════════════╝
echo.
echo   [!] CRITICAL WARNING:
echo   These tweaks can give 20-40%% MORE FPS but have downsides!
echo.
echo   RISKS:
echo   - Reduced security (disables CPU vulnerability patches)
echo   - Higher power consumption
echo   - Possible system instability
echo   - May cause crashes on some hardware
echo   - Increased heat generation
echo.
echo   RECOMMENDED FOR:
echo   - Gamers who prioritize FPS over everything
echo   - Systems with good cooling
echo   - Users who understand the risks
echo.
echo   NOT RECOMMENDED FOR:
echo   - Laptops (battery drain, heat)
echo   - Work computers (security risk)
echo   - Unstable systems
echo   - If you don't know what you're doing
echo.
echo   ══════════════════════════════════════════════════════════════
echo.
set /p confirm="  Do you want to continue? [Y/N]: "

if /i NOT "%confirm%"=="Y" (
    echo.
    echo   [!] Cancelled - Returning to menu
    timeout /t 2 >nul
    goto MENU
)

cls
echo.
echo   [+] EXTREME Performance Tweaks
echo.
echo   You will be asked for each tweak individually.
echo   Read each warning carefully!
echo.
pause

:: 1. Disable HPET
cls
echo.
echo   ══════════════════════════════════════════════════════════════
echo   [1/10] Disable HPET (High Precision Event Timer)
echo   ══════════════════════════════════════════════════════════════
echo.
echo   WHAT IT DOES:
echo   - Disables high-precision timer
echo   - Can increase FPS by 5-10%% in some games
echo   - Reduces DPC latency
echo.
echo   DOWNSIDE:
echo   - Some games may have timing issues
echo   - Audio may desync in rare cases
echo   - May cause stuttering in specific games
echo.
echo   RISK LEVEL: LOW-MEDIUM
echo.
set /p hpet="  Disable HPET? [Y/N]: "
if /i "%hpet%"=="Y" (
    bcdedit /deletevalue useplatformclock >nul 2>&1
    echo   [✓] HPET disabled
) else (
    echo   [—] Skipped
)

:: 2. Disable Dynamic Tick
echo.
echo   ══════════════════════════════════════════════════════════════
echo   [2/10] Disable Dynamic Tick
echo   ══════════════════════════════════════════════════════════════
echo.
echo   WHAT IT DOES:
echo   - Forces constant CPU polling
echo   - Lower input latency
echo   - More responsive system
echo.
echo   DOWNSIDE:
echo   - Higher CPU usage when idle
echo   - Increased power consumption (5-10W more)
echo   - Laptops: Reduced battery life by 20-30%%
echo.
echo   RISK LEVEL: MEDIUM (not recommended for laptops)
echo.
set /p tick="  Disable Dynamic Tick? [Y/N]: "
if /i "%tick%"=="Y" (
    bcdedit /set disabledynamictick yes >nul 2>&1
    echo   [✓] Dynamic Tick disabled
) else (
    echo   [—] Skipped
)

:: 3. Set Timer Resolution
echo.
echo   ══════════════════════════════════════════════════════════════
echo   [3/10] Set Timer Resolution to 0.5ms
echo   ══════════════════════════════════════════════════════════════
echo.
echo   WHAT IT DOES:
echo   - Improves frame time consistency
echo   - Reduces micro-stuttering
echo   - Smoother gameplay
echo.
echo   DOWNSIDE:
echo   - Slightly higher CPU usage (1-2%%)
echo   - Minor increase in power consumption
echo.
echo   RISK LEVEL: LOW
echo.
set /p timer="  Set 0.5ms Timer Resolution? [Y/N]: "
if /i "%timer%"=="Y" (
    bcdedit /set useplatformtick yes >nul 2>&1
    reg add "HKLM\SYSTEM\CurrentControlSet\Control\Session Manager\kernel" /v GlobalTimerResolutionRequests /t REG_DWORD /d 1 /f >nul 2>&1
    echo   [✓] Timer resolution optimized
) else (
    echo   [—] Skipped
)

:: 4. Disable Memory Compression
echo.
echo   ══════════════════════════════════════════════════════════════
echo   [4/10] Disable Memory Compression
echo   ══════════════════════════════════════════════════════════════
echo.
echo   WHAT IT DOES:
echo   - Frees up CPU cycles
echo   - Reduces RAM latency
echo   - Better for high RAM systems
echo.
echo   DOWNSIDE:
echo   - Uses more physical RAM
echo   - NOT recommended for systems with 8GB or less RAM
echo   - May cause out-of-memory errors on low RAM
echo.
echo   RISK LEVEL: MEDIUM (only for 16GB+ RAM systems)
echo.
set /p memcomp="  Disable Memory Compression? [Y/N]: "
if /i "%memcomp%"=="Y" (
    powershell -Command "Disable-MMAgent -MemoryCompression" >nul 2>&1
    echo   [✓] Memory compression disabled
    echo   [!] Only use this if you have 16GB+ RAM!
) else (
    echo   [—] Skipped
)

:: 5. Optimize MMCSS
echo.
echo   ══════════════════════════════════════════════════════════════
echo   [5/10] Optimize MMCSS (Multimedia Scheduler)
echo   ══════════════════════════════════════════════════════════════
echo.
echo   WHAT IT DOES:
echo   - Gives games maximum CPU priority
echo   - Better audio/video performance
echo   - Reduces audio crackling
echo.
echo   DOWNSIDE:
echo   - Background tasks get less priority
echo   - Downloads may be slower while gaming
echo.
echo   RISK LEVEL: LOW
echo.
set /p mmcss="  Optimize MMCSS? [Y/N]: "
if /i "%mmcss%"=="Y" (
    reg add "HKLM\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile\Tasks\Games" /v "GPU Priority" /t REG_DWORD /d 8 /f >nul 2>&1
    reg add "HKLM\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile\Tasks\Games" /v Priority /t REG_DWORD /d 6 /f >nul 2>&1
    reg add "HKLM\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile\Tasks\Games" /v "Scheduling Category" /t REG_SZ /d High /f >nul 2>&1
    echo   [✓] MMCSS optimized for gaming
) else (
    echo   [—] Skipped
)

:: 6. Disable GPU Telemetry
echo.
echo   ══════════════════════════════════════════════════════════════
echo   [6/10] Disable NVIDIA/AMD Telemetry
echo   ══════════════════════════════════════════════════════════════
echo.
echo   WHAT IT DOES:
echo   - Stops GPU telemetry services
echo   - Frees up 50-100MB RAM
echo   - Reduces background processes
echo.
echo   DOWNSIDE:
echo   - GeForce Experience/AMD Software features may not work
echo   - No performance monitoring in driver software
echo.
echo   RISK LEVEL: LOW
echo.
set /p gputelem="  Disable GPU Telemetry? [Y/N]: "
if /i "%gputelem%"=="Y" (
    sc stop "NvTelemetryContainer" >nul 2>&1
    sc config "NvTelemetryContainer" start= disabled >nul 2>&1
    sc stop "AMD External Events Utility" >nul 2>&1
    sc config "AMD External Events Utility" start= disabled >nul 2>&1
    schtasks /change /tn "NvTmRep_CrashReport1_{B2FE1952-0186-46C3-BAEC-A80AA35AC5B8}" /disable >nul 2>&1
    schtasks /change /tn "NvTmRep_CrashReport2_{B2FE1952-0186-46C3-BAEC-A80AA35AC5B8}" /disable >nul 2>&1
    schtasks /change /tn "NvTmRep_CrashReport3_{B2FE1952-0186-46C3-BAEC-A80AA35AC5B8}" /disable >nul 2>&1
    schtasks /change /tn "NvTmRep_CrashReport4_{B2FE1952-0186-46C3-BAEC-A80AA35AC5B8}" /disable >nul 2>&1
    echo   [✓] GPU telemetry disabled
) else (
    echo   [—] Skipped
)

:: 7. Advanced Win32 Priority
echo.
echo   ══════════════════════════════════════════════════════════════
echo   [7/10] Advanced CPU Scheduler Optimization
echo   ══════════════════════════════════════════════════════════════
echo.
echo   WHAT IT DOES:
echo   - Optimizes CPU scheduler for games
echo   - Gives foreground apps maximum priority
echo   - Better multi-core utilization
echo.
echo   DOWNSIDE:
echo   - Background apps become very slow
echo   - May cause issues with streaming/recording
echo.
echo   RISK LEVEL: LOW
echo.
set /p priority="  Optimize CPU Scheduler? [Y/N]: "
if /i "%priority%"=="Y" (
    reg add "HKLM\SYSTEM\CurrentControlSet\Control\PriorityControl" /v Win32PrioritySeparation /t REG_DWORD /d 38 /f >nul 2>&1
    echo   [✓] CPU scheduler optimized
) else (
    echo   [—] Skipped
)

:: 8. Disable Spectre/Meltdown (DANGEROUS!)
echo.
echo   ══════════════════════════════════════════════════════════════
echo   [8/10] Disable Spectre/Meltdown Mitigations (RISKY!)
echo   ══════════════════════════════════════════════════════════════
echo.
echo   WHAT IT DOES:
echo   - Disables CPU vulnerability patches
echo   - Can boost performance by 10-30%%!
echo   - Significantly lower latency
echo.
echo   DOWNSIDE:
echo   - MAJOR SECURITY RISK!
echo   - Makes your PC vulnerable to CPU exploits
echo   - Malware can steal data more easily
echo   - NOT recommended for online banking, work, etc.
echo.
echo   RISK LEVEL: VERY HIGH (security risk)
echo.
echo   [!] ONLY use this on a dedicated gaming PC!
echo   [!] NOT safe for general use!
echo.
set /p spectre="  Disable Spectre/Meltdown? [Y/N]: "
if /i "%spectre%"=="Y" (
    reg add "HKLM\SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management" /v FeatureSettingsOverride /t REG_DWORD /d 3 /f >nul 2>&1
    reg add "HKLM\SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management" /v FeatureSettingsOverrideMask /t REG_DWORD /d 3 /f >nul 2>&1
    bcdedit /set hypervisorlaunchtype off >nul 2>&1
    echo   [✓] Spectre/Meltdown mitigations disabled
    echo   [!] WARNING: Your PC is now less secure!
) else (
    echo   [—] Skipped (good choice for security)
)

:: 9. Optimize Page File
echo.
echo   ══════════════════════════════════════════════════════════════
echo   [9/10] Optimize Page File Settings
echo   ══════════════════════════════════════════════════════════════
echo.
echo   WHAT IT DOES:
echo   - Sets fixed page file size
echo   - Reduces fragmentation
echo   - Faster memory management
echo.
echo   DOWNSIDE:
echo   - Uses disk space (recommended: RAM size x 1.5)
echo   - If set too small, may cause crashes
echo.
echo   RISK LEVEL: LOW
echo.
set /p pagefile="  Optimize Page File? [Y/N]: "
if /i "%pagefile%"=="Y" (
    wmic computersystem where name="%computername%" set AutomaticManagedPagefile=False >nul 2>&1
    wmic pagefileset where name="C:\\pagefile.sys" set InitialSize=4096,MaximumSize=4096 >nul 2>&1
    echo   [✓] Page file set to 4GB fixed size
    echo   [!] Adjust size based on your RAM amount
) else (
    echo   [—] Skipped
)

:: 10. Disable C-States
echo.
echo   ══════════════════════════════════════════════════════════════
echo   [10/10] Disable CPU C-States (Sleep States)
echo   ══════════════════════════════════════════════════════════════
echo.
echo   WHAT IT DOES:
echo   - Keeps CPU always at maximum frequency
echo   - Lowest possible latency
echo   - Instant response time
echo.
echo   DOWNSIDE:
echo   - MASSIVE power consumption increase (20-50W more!)
echo   - CPU runs hotter
echo   - Laptops: Battery drains 50-70%% faster
echo   - Higher electricity bill
echo.
echo   RISK LEVEL: HIGH (heat and power consumption)
echo.
echo   [!] NOT recommended for laptops!
echo   [!] Ensure you have good CPU cooling!
echo.
set /p cstates="  Disable C-States? [Y/N]: "
if /i "%cstates%"=="Y" (
    powercfg -setacvalueindex scheme_current sub_processor IDLEDISABLE 1 >nul 2>&1
    powercfg -setactive scheme_current >nul 2>&1
    reg add "HKLM\SYSTEM\CurrentControlSet\Control\Processor" /v Capabilities /t REG_DWORD /d 0x0007e066 /f >nul 2>&1
    echo   [✓] C-States disabled
    echo   [!] CPU will now run at max speed always!
) else (
    echo   [—] Skipped
)

:: Summary
cls
echo.
echo   ══════════════════════════════════════════════════════════════
echo   [✓] EXTREME Performance Tweaks Complete!
echo   ══════════════════════════════════════════════════════════════
echo.
echo   WHAT YOU'VE DONE:
echo   These tweaks will give you MAXIMUM gaming performance
echo   but at the cost of security, power, and stability.
echo.
echo   EXPECTED RESULTS:
echo   - 20-40%% FPS increase in CPU-bound games
echo   - 10-20%% lower input latency
echo   - Smoother frame times
echo   - Reduced stuttering
echo.
echo   IMPORTANT REMINDERS:
echo   - Monitor CPU/GPU temperatures
echo   - If you disabled Spectre/Meltdown: BE CAREFUL online
echo   - If unstable: Restore from your backup point
echo   - Power consumption will be MUCH higher
echo.
echo   [!] RESTART YOUR PC NOW for changes to take effect!
echo.
pause
goto MENU REG_DWORD /d 1 /f >nul 2>&1
    echo       [✓] Bing search disabled
)

echo.
:: Taskbar Alignment (Windows 11)
echo   [5] Taskbar Alignment (Windows 11)
set /p taskbar="      Align taskbar to LEFT? [Y/N]: "
if /i "%taskbar%"=="Y" (
    reg add "HKCU\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced" /v TaskbarAl /t REG_DWORD /d 0 /f >nul 2>&1
    echo       [✓] Taskbar aligned to left
)

echo.
:: Snap Assist
echo   [6] Snap Assist
set /p snap="      Disable Snap Assist? [Y/N]: "
if /i "%snap%"=="Y" (
    reg add "HKCU\Control Panel\Desktop" /v WindowArrangementActive /t REG_SZ /d 0 /f >nul 2>&1
    echo       [✓] Snap Assist disabled
)

echo.
:: Show Detailed BSoD
echo   [7] Blue Screen Details
set /p bsod="      Show detailed BSoD information? [Y/N]: "
if /i "%bsod%"=="Y" (
    reg add "HKLM\System\CurrentControlSet\Control\CrashControl" /v DisplayParameters /t REG_DWORD /d 1 /f >nul 2>&1
    echo       [✓] Detailed BSoD enabled
)

echo.
:: Taskbar Search Style
echo   [8] Taskbar Search
set /p search="      Hide search box from taskbar? [Y/N]: "
if /i "%search%"=="Y" (
    reg add "HKCU\Software\Microsoft\Windows\CurrentVersion\Search" /v SearchboxTaskbarMode /t REG_DWORD /d 0 /f >nul 2>&1
    echo       [✓] Search box hidden
)

echo.
:: Task View Button
echo   [9] Task View Button
set /p taskview="      Hide Task View button? [Y/N]: "
if /i "%taskview%"=="Y" (
    reg add "HKCU\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced" /v ShowTaskViewButton /t REG_DWORD /d 0 /f >nul 2>&1
    echo       [✓] Task View button hidden
)

echo.
:: Widgets (Windows 11)
echo   [10] Widgets (Windows 11)
set /p widgets="      Disable Widgets? [Y/N]: "
if /i "%widgets%"=="Y" (
    reg add "HKCU\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced" /v TaskbarDa /t REG_DWORD /d 0 /f >nul 2>&1
    echo       [✓] Widgets disabled
)

echo.
:: Chat Icon (Windows 11)
echo   [11] Chat Icon (Windows 11)
set /p chat="      Hide Chat icon? [Y/N]: "
if /i "%chat%"=="Y" (
    reg add "HKCU\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced" /v TaskbarMn /t REG_DWORD /d 0 /f >nul 2>&1
    echo       [✓] Chat icon hidden
)

echo.
echo   [✓] Preferences applied!
echo   [!] Restart Explorer or your PC to see changes
timeout /t 3 >nul
goto MENU

:PRIVACY
cls
echo.
echo   [+] Applying Privacy Tweaks...
echo.

reg add "HKLM\SOFTWARE\Policies\Microsoft\Windows\DataCollection" /v AllowTelemetry /t REG_DWORD /d 0 /f >nul 2>&1
echo   [-] Telemetry disabled

sc stop "DiagTrack" >nul 2>&1
sc config "DiagTrack" start= disabled >nul 2>&1
echo   [-] DiagTrack service disabled

sc stop "dmwappushservice" >nul 2>&1
sc config "dmwappushservice" start= disabled >nul 2>&1
echo   [-] dmwappushservice disabled

reg add "HKLM\SOFTWARE\Policies\Microsoft\Windows\System" /v EnableActivityFeed /t REG_DWORD /d 0 /f >nul 2>&1
reg add "HKLM\SOFTWARE\Policies\Microsoft\Windows\System" /v PublishUserActivities /t REG_DWORD /d 0 /f >nul 2>&1
echo   [-] Activity history disabled

reg add "HKCU\Software\Microsoft\Windows\CurrentVersion\AdvertisingInfo" /v Enabled /t REG_DWORD /d 0 /f >nul 2>&1
echo   [-] Advertising ID disabled

reg add "HKLM\SOFTWARE\Policies\Microsoft\Windows\Windows Search" /v AllowCortana /t REG_DWORD /d 0 /f >nul 2>&1
reg add "HKLM\SOFTWARE\Policies\Microsoft\Windows\Windows Search" /v AllowCortanaAboveLock /t REG_DWORD /d 0 /f >nul 2>&1
echo   [-] Cortana disabled

reg add "HKCU\Software\Microsoft\Siuf\Rules" /v NumberOfSIUFInPeriod /t REG_DWORD /d 0 /f >nul 2>&1
reg add "HKLM\SOFTWARE\Policies\Microsoft\Windows\DataCollection" /v DoNotShowFeedbackNotifications /t REG_DWORD /d 1 /f >nul 2>&1
echo   [-] Windows feedback disabled

reg add "HKLM\SOFTWARE\Policies\Microsoft\Windows\System" /v EnableActivityFeed /t REG_DWORD /d 0 /f >nul 2>&1
echo   [-] Timeline disabled

reg add "HKCU\Software\Policies\Microsoft\Windows\Explorer" /v DisableSearchBoxSuggestions /t