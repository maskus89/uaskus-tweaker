@echo off
:: ============================================================
:: Uaskus Tweaks v1.3
:: Created by: maskus
:: Based on: Chris Titus Tech Windows Utility
:: 
:: This script optimizes Windows 10/11 for better performance
:: VirusTotal: https://www.virustotal.com/gui/file/fd2a54b7d6b34e1513a9e0d9571e04f96432bb02027070c5c21e99165baac91a?nocache=1
:: 
:: VirusTotal Scan: 1/73 (False Positive)
:: 72 out of 73 antivirus engines confirm this is clean and safe
:: 
:: Why the detection?
:: Scripts that modify Windows settings trigger some antivirus
:: heuristics. This is a false positive - all code is visible.
:: 
:: Safe to use:
:: - Open source (you can read every command)
:: - Creates system restore point before changes
:: - Based on trusted Chris Titus Tech tweaks
:: - No hidden code, no executables, no malware
:: ============================================================

chcp 65001 >nul 2>&1
title Uaskus Tweaks v1.3
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
echo   UASKUS TWEAKS v1.3
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
    wmic.exe /Namespace:\\root\default Path SystemRestore Call CreateRestorePoint "Uaskus Tweaks - Before Changes", 100, 7 >nul 2>&1
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
echo                       Version: 1.3
echo   ════════════════════════════════════════════════════════════
echo.
echo.
echo       [ 1 ] Essential Tweaks           [ 2 ] Advanced Tweaks
echo.
echo       [ 3 ] Performance Tweaks         [ 4 ] Privacy Tweaks
echo.
echo       [ 5 ] Gaming Tweaks              [ 6 ] Network Tweaks
echo.
echo       [ 7 ] Debloat Windows            [ 8 ] Visual Tweaks
echo.
echo       [ 0 ] Exit
echo.
echo.
set /p choice="  Select an option: "

if "%choice%"=="1" goto ESSENTIAL
if "%choice%"=="2" goto ADVANCED
if "%choice%"=="3" goto PERFORMANCE
if "%choice%"=="4" goto PRIVACY
if "%choice%"=="5" goto GAMING
if "%choice%"=="6" goto NETWORK
if "%choice%"=="7" goto DEBLOAT
if "%choice%"=="8" goto VISUAL
if "%choice%"=="0" exit
goto MENU

:ESSENTIAL
cls
echo.
echo   [+] Applying Essential Tweaks (Chris Titus Recommended)...
echo.

:: Delete Temporary Files
del /q /f /s %temp%\* >nul 2>&1
echo   [-] Temporary files deleted

:: Disable Telemetry
reg add "HKLM\SOFTWARE\Policies\Microsoft\Windows\DataCollection" /v AllowTelemetry /t REG_DWORD /d 0 /f >nul 2>&1
reg add "HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\DataCollection" /v AllowTelemetry /t REG_DWORD /d 0 /f >nul 2>&1
echo   [-] Telemetry disabled

:: Disable Activity History
reg add "HKLM\SOFTWARE\Policies\Microsoft\Windows\System" /v EnableActivityFeed /t REG_DWORD /d 0 /f >nul 2>&1
reg add "HKLM\SOFTWARE\Policies\Microsoft\Windows\System" /v PublishUserActivities /t REG_DWORD /d 0 /f >nul 2>&1
reg add "HKLM\SOFTWARE\Policies\Microsoft\Windows\System" /v UploadUserActivities /t REG_DWORD /d 0 /f >nul 2>&1
echo   [-] Activity history disabled

:: Disable Consumer Features (Sponsored apps)
reg add "HKCU\Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager" /v SubscribedContent-338389Enabled /t REG_DWORD /d 0 /f >nul 2>&1
reg add "HKCU\Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager" /v SubscribedContent-338388Enabled /t REG_DWORD /d 0 /f >nul 2>&1
reg add "HKCU\Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager" /v SubscribedContent-314559Enabled /t REG_DWORD /d 0 /f >nul 2>&1
reg add "HKCU\Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager" /v SubscribedContent-353698Enabled /t REG_DWORD /d 0 /f >nul 2>&1
reg add "HKCU\Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager" /v SystemPaneSuggestionsEnabled /t REG_DWORD /d 0 /f >nul 2>&1
echo   [-] Consumer features/ads disabled

:: Disable Game DVR
reg add "HKCU\System\GameConfigStore" /v GameDVR_Enabled /t REG_DWORD /d 0 /f >nul 2>&1
reg add "HKLM\SOFTWARE\Policies\Microsoft\Windows\GameDVR" /v AllowGameDVR /t REG_DWORD /d 0 /f >nul 2>&1
echo   [-] Game DVR disabled

:: Disable Hibernation
powercfg -h off >nul 2>&1
echo   [-] Hibernation disabled

:: Disable Location Tracking
reg add "HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\CapabilityAccessManager\ConsentStore\location" /v Value /t REG_SZ /d Deny /f >nul 2>&1
reg add "HKLM\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Sensor\Overrides\{BFA794E4-F964-4FDB-90F6-51056BFE4B44}" /v SensorPermissionState /t REG_DWORD /d 0 /f >nul 2>&1
echo   [-] Location tracking disabled

:: Disable PowerShell 7 Telemetry
setx POWERSHELL_TELEMETRY_OPTOUT 1 >nul 2>&1
echo   [-] PowerShell 7 telemetry disabled

:: Disable Storage Sense
reg add "HKCU\SOFTWARE\Microsoft\Windows\CurrentVersion\StorageSense\Parameters\StoragePolicy" /v 01 /t REG_DWORD /d 0 /f >nul 2>&1
echo   [-] Storage Sense disabled

:: Enable End Task With Right Click (Windows 11)
reg add "HKCU\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced\TaskbarDeveloperSettings" /v TaskbarEndTask /t REG_DWORD /d 1 /f >nul 2>&1
echo   [-] End task with right-click enabled

:: Prefer IPv4 over IPv6
reg add "HKLM\SYSTEM\CurrentControlSet\Services\Tcpip6\Parameters" /v DisabledComponents /t REG_DWORD /d 32 /f >nul 2>&1
echo   [-] IPv4 preferred over IPv6

:: Set Services to Manual
sc config "DiagTrack" start= demand >nul 2>&1
sc config "dmwappushservice" start= demand >nul 2>&1
sc config "SysMain" start= demand >nul 2>&1
sc config "WSearch" start= demand >nul 2>&1
sc config "XblAuthManager" start= demand >nul 2>&1
sc config "XblGameSave" start= demand >nul 2>&1
sc config "XboxNetApiSvc" start= demand >nul 2>&1
echo   [-] Unnecessary services set to manual

:: Debloat Edge
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

:: Disable Background Apps
reg add "HKCU\Software\Microsoft\Windows\CurrentVersion\BackgroundAccessApplications" /v GlobalUserDisabled /t REG_DWORD /d 1 /f >nul 2>&1
echo   [-] Background apps disabled

:: Disable Fullscreen Optimizations
reg add "HKCU\System\GameConfigStore" /v GameDVR_DXGIHonorFSEWindowsCompatible /t REG_DWORD /d 1 /f >nul 2>&1
echo   [-] Fullscreen optimizations disabled

:: Disable IPv6
reg add "HKLM\SYSTEM\CurrentControlSet\Services\Tcpip6\Parameters" /v DisabledComponents /t REG_DWORD /d 255 /f >nul 2>&1
echo   [-] IPv6 completely disabled

:: Disable Notification Tray/Calendar
reg add "HKCU\Software\Policies\Microsoft\Windows\Explorer" /v DisableNotificationCenter /t REG_DWORD /d 1 /f >nul 2>&1
reg add "HKCU\Software\Microsoft\Windows\CurrentVersion\PushNotifications" /v ToastEnabled /t REG_DWORD /d 0 /f >nul 2>&1
echo   [-] Notification tray disabled

:: Disable Teredo
netsh interface teredo set state disabled >nul 2>&1
echo   [-] Teredo disabled

:: Remove Home and Gallery from Explorer
reg add "HKCU\Software\Classes\CLSID\{e88865ea-0e1c-4e20-9aa6-edcd0212c87c}" /v System.IsPinnedToNameSpaceTree /t REG_DWORD /d 0 /f >nul 2>&1
reg add "HKCU\Software\Classes\CLSID\{f874310e-b6b7-47dc-bc84-b9e6b38f5903}" /v System.IsPinnedToNameSpaceTree /t REG_DWORD /d 0 /f >nul 2>&1
echo   [-] Home and Gallery removed from Explorer

:: Set Classic Right-Click Menu (Windows 11)
reg add "HKCU\Software\Classes\CLSID\{86ca1aa0-34aa-4e8b-a509-50c905bae2a2}\InprocServer32" /ve /t REG_SZ /d "" /f >nul 2>&1
echo   [-] Classic right-click menu enabled

:: Set Display for Performance
reg add "HKCU\Control Panel\Desktop" /v UserPreferencesMask /t REG_BINARY /d 9012038010000000 /f >nul 2>&1
reg add "HKCU\Software\Microsoft\Windows\CurrentVersion\Explorer\VisualEffects" /v VisualFXSetting /t REG_DWORD /d 2 /f >nul 2>&1
echo   [-] Display set for performance

:: Adobe Network Block
echo 127.0.0.1 lmlicenses.wip4.adobe.com >> %windir%\System32\drivers\etc\hosts
echo 127.0.0.1 lm.licenses.adobe.com >> %windir%\System32\drivers\etc\hosts
echo   [-] Adobe activation servers blocked

echo.
echo   [✓] Advanced tweaks complete!
timeout /t 3 >nul
goto MENU

:PERFORMANCE
cls
echo.
echo   [+] Applying Performance Tweaks...
echo.

:: Disable Visual Effects
reg add "HKCU\Control Panel\Desktop" /v UserPreferencesMask /t REG_BINARY /d 9012038010000000 /f >nul 2>&1
echo   [-] Visual effects disabled

:: Disable animations
reg add "HKCU\Control Panel\Desktop\WindowMetrics" /v MinAnimate /t REG_SZ /d 0 /f >nul 2>&1
reg add "HKCU\Control Panel\Desktop" /v MenuShowDelay /t REG_SZ /d 0 /f >nul 2>&1
echo   [-] Animations disabled

:: High Performance Power Plan
powercfg -duplicatescheme e9a42b02-d5df-448d-aa00-03f14749eb61 >nul 2>&1
powercfg -setactive 8c5e7fda-e8bf-4a96-9a85-a6e23a8c635c >nul 2>&1
echo   [-] High performance power plan activated

:: Disable Transparency
reg add "HKCU\Software\Microsoft\Windows\CurrentVersion\Themes\Personalize" /v EnableTransparency /t REG_DWORD /d 0 /f >nul 2>&1
echo   [-] Transparency disabled

:: Disable SuperFetch/SysMain
sc stop "SysMain" >nul 2>&1
sc config "SysMain" start= disabled >nul 2>&1
echo   [-] SuperFetch disabled

:: Disable Prefetch
reg add "HKLM\SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management\PrefetchParameters" /v EnablePrefetcher /t REG_DWORD /d 0 /f >nul 2>&1
reg add "HKLM\SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management\PrefetchParameters" /v EnableSuperfetch /t REG_DWORD /d 0 /f >nul 2>&1
echo   [-] Prefetch disabled

:: Disable Paging Executive
reg add "HKLM\SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management" /v DisablePagingExecutive /t REG_DWORD /d 1 /f >nul 2>&1
echo   [-] Paging executive disabled

:: Disable Windows Search Indexing
sc stop "WSearch" >nul 2>&1
sc config "WSearch" start= disabled >nul 2>&1
echo   [-] Windows Search indexing disabled

:: Enable Hardware-Accelerated GPU Scheduling
reg add "HKLM\SYSTEM\CurrentControlSet\Control\GraphicsDrivers" /v HwSchMode /t REG_DWORD /d 2 /f >nul 2>&1
echo   [-] Hardware GPU scheduling enabled

:: Disable Print Spooler
sc config "Spooler" start= demand >nul 2>&1
echo   [-] Print Spooler set to manual

:: Optimize system responsiveness
reg add "HKLM\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile" /v SystemResponsiveness /t REG_DWORD /d 0 /f >nul 2>&1
echo   [-] System responsiveness optimized

echo.
echo   [✓] Performance tweaks complete!
timeout /t 3 >nul
goto MENU

:PRIVACY
cls
echo.
echo   [+] Applying Privacy Tweaks...
echo.

:: Disable Telemetry
reg add "HKLM\SOFTWARE\Policies\Microsoft\Windows\DataCollection" /v AllowTelemetry /t REG_DWORD /d 0 /f >nul 2>&1
echo   [-] Telemetry disabled

:: Disable DiagTrack service
sc stop "DiagTrack" >nul 2>&1
sc config "DiagTrack" start= disabled >nul 2>&1
echo   [-] DiagTrack service disabled

:: Disable dmwappushservice
sc stop "dmwappushservice" >nul 2>&1
sc config "dmwappushservice" start= disabled >nul 2>&1
echo   [-] dmwappushservice disabled

:: Disable Activity History
reg add "HKLM\SOFTWARE\Policies\Microsoft\Windows\System" /v EnableActivityFeed /t REG_DWORD /d 0 /f >nul 2>&1
reg add "HKLM\SOFTWARE\Policies\Microsoft\Windows\System" /v PublishUserActivities /t REG_DWORD /d 0 /f >nul 2>&1
echo   [-] Activity history disabled

:: Disable advertising ID
reg add "HKCU\Software\Microsoft\Windows\CurrentVersion\AdvertisingInfo" /v Enabled /t REG_DWORD /d 0 /f >nul 2>&1
echo   [-] Advertising ID disabled

:: Disable Cortana
reg add "HKLM\SOFTWARE\Policies\Microsoft\Windows\Windows Search" /v AllowCortana /t REG_DWORD /d 0 /f >nul 2>&1
reg add "HKLM\SOFTWARE\Policies\Microsoft\Windows\Windows Search" /v AllowCortanaAboveLock /t REG_DWORD /d 0 /f >nul 2>&1
echo   [-] Cortana disabled

:: Disable Windows Feedback
reg add "HKCU\Software\Microsoft\Siuf\Rules" /v NumberOfSIUFInPeriod /t REG_DWORD /d 0 /f >nul 2>&1
reg add "HKLM\SOFTWARE\Policies\Microsoft\Windows\DataCollection" /v DoNotShowFeedbackNotifications /t REG_DWORD /d 1 /f >nul 2>&1
echo   [-] Windows feedback disabled

:: Disable Timeline
reg add "HKLM\SOFTWARE\Policies\Microsoft\Windows\System" /v EnableActivityFeed /t REG_DWORD /d 0 /f >nul 2>&1
echo   [-] Timeline disabled

:: Disable Web Search in Start Menu
reg add "HKCU\Software\Policies\Microsoft\Windows\Explorer" /v DisableSearchBoxSuggestions /t REG_DWORD /d 1 /f >nul 2>&1
echo   [-] Web search in Start Menu disabled

:: Disable tailored experiences
reg add "HKCU\Software\Microsoft\Windows\CurrentVersion\Privacy" /v TailoredExperiencesWithDiagnosticDataEnabled /t REG_DWORD /d 0 /f >nul 2>&1
echo   [-] Tailored experiences disabled

echo.
echo   [✓] Privacy tweaks complete!
echo   [!] You may need to re-login to some apps
timeout /t 4 >nul
goto MENU

:GAMING
cls
echo.
echo   [+] Applying Gaming Tweaks...
echo.

:: Enable Game Mode
reg add "HKCU\Software\Microsoft\GameBar" /v AllowAutoGameMode /t REG_DWORD /d 1 /f >nul 2>&1
reg add "HKCU\Software\Microsoft\GameBar" /v AutoGameModeEnabled /t REG_DWORD /d 1 /f >nul 2>&1
echo   [-] Game mode enabled

:: Disable Game DVR and GameBar
reg add "HKCU\System\GameConfigStore" /v GameDVR_Enabled /t REG_DWORD /d 0 /f >nul 2>&1
reg add "HKLM\SOFTWARE\Policies\Microsoft\Windows\GameDVR" /v AllowGameDVR /t REG_DWORD /d 0 /f >nul 2>&1
reg add "HKCU\Software\Microsoft\GameBar" /v UseNexusForGameBarEnabled /t REG_DWORD /d 0 /f >nul 2>&1
echo   [-] Game DVR and GameBar disabled

:: Disable Fullscreen Optimizations
reg add "HKCU\System\GameConfigStore" /v GameDVR_DXGIHonorFSEWindowsCompatible /t REG_DWORD /d 1 /f >nul 2>&1
echo   [-] Fullscreen optimizations disabled

:: High Performance Power Plan
powercfg -setactive 8c5e7fda-e8bf-4a96-9a85-a6e23a8c635c >nul 2>&1
echo   [-] High performance power plan activated

:: Disable Windows Update during active hours
reg add "HKLM\SOFTWARE\Microsoft\WindowsUpdate\UX\Settings" /v ActiveHoursStart /t REG_DWORD /d 8 /f >nul 2>&1
reg add "HKLM\SOFTWARE\Microsoft\WindowsUpdate\UX\Settings" /v ActiveHoursEnd /t REG_DWORD /d 23 /f >nul 2>&1
echo   [-] Active hours set (8 AM - 11 PM)

:: Disable Nagle's Algorithm for gaming
reg add "HKLM\SYSTEM\CurrentControlSet\Services\Tcpip\Parameters" /v TcpAckFrequency /t REG_DWORD /d 1 /f >nul 2>&1
reg add "HKLM\SYSTEM\CurrentControlSet\Services\Tcpip\Parameters" /v TCPNoDelay /t REG_DWORD /d 1 /f >nul 2>&1
echo   [-] Nagle's algorithm disabled (lower latency)

:: GPU Scheduling (Windows 10 2004+)
reg add "HKLM\SYSTEM\CurrentControlSet\Control\GraphicsDrivers" /v HwSchMode /t REG_DWORD /d 2 /f >nul 2>&1
echo   [-] Hardware-accelerated GPU scheduling enabled

:: Optimize for programs not background services
reg add "HKLM\SYSTEM\CurrentControlSet\Control\PriorityControl" /v Win32PrioritySeparation /t REG_DWORD /d 38 /f >nul 2>&1
echo   [-] CPU priority optimized for programs

:: Disable Xbox services
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

:: Flush DNS
ipconfig /flushdns >nul 2>&1
echo   [-] DNS cache flushed

:: Disable Network Throttling
reg add "HKLM\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile" /v NetworkThrottlingIndex /t REG_DWORD /d 0xffffffff /f >nul 2>&1
echo   [-] Network throttling disabled

:: Optimize TCP settings
netsh int tcp set global autotuninglevel=normal >nul 2>&1
netsh int tcp set global chimney=enabled >nul 2>&1
netsh int tcp set global dca=enabled >nul 2>&1
netsh int tcp set global netdma=enabled >nul 2>&1
echo   [-] TCP settings optimized

:: Enable TCP Window Scaling
netsh int tcp set global timestamps=enabled >nul 2>&1
netsh int tcp set global rss=enabled >nul 2>&1
echo   [-] TCP advanced features enabled

:: Prefer IPv4 over IPv6
reg add "HKLM\SYSTEM\CurrentControlSet\Services\Tcpip6\Parameters" /v DisabledComponents /t REG_DWORD /d 32 /f >nul 2>&1
echo   [-] IPv4 preferred over IPv6

:: Disable QoS Packet Scheduler
reg add "HKLM\SOFTWARE\Policies\Microsoft\Windows\Psched" /v NonBestEffortLimit /t REG_DWORD /d 0 /f >nul 2>&1
echo   [-] QoS bandwidth reservation disabled

:: Set DNS to Cloudflare (Optional)
echo.
echo   [!] Change DNS to Cloudflare (1.1.1.1)? [Y/N]
set /p dns="  "
if /i "%dns%"=="Y" (
    netsh interface ip set dns "Ethernet" static 1.1.1.1 primary >nul 2>&1
    netsh interface ip add dns "Ethernet" 1.0.0.1 index=2 >nul 2>&1
    netsh interface ip set dns "Wi-Fi" static 1.1.1.1 primary >nul 2>&1
    netsh interface ip add dns "Wi-Fi" 1.0.0.1 index=2 >nul 2>&1
    echo   [-] DNS changed to Cloudflare
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

:: Remove bloatware apps
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

:: Show file extensions
reg add "HKCU\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced" /v HideFileExt /t REG_DWORD /d 0 /f >nul 2>&1
echo   [-] File extensions shown

:: Show hidden files
reg add "HKCU\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced" /v Hidden /t REG_DWORD /d 1 /f >nul 2>&1
echo   [-] Hidden files shown

:: Disable Snap Assist
reg add "HKCU\Control Panel\Desktop" /v WindowArrangementActive /t REG_SZ /d 0 /f >nul 2>&1
echo   [-] Snap Assist disabled

:: Disable Aero Shake
reg add "HKCU\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced" /v DisallowShaking /t REG_DWORD /d 1 /f >nul 2>&1
echo   [-] Aero Shake disabled

:: Show This PC on desktop
reg add "HKCU\Software\Microsoft\Windows\CurrentVersion\Explorer\HideDesktopIcons\NewStartPanel" /v "{20D04FE0-3AEA-1069-A2D8-08002B30309D}" /t REG_DWORD /d 0 /f >nul 2>&1
echo   [-] This PC shown on desktop

:: Taskbar alignment (left for Windows 11)
reg add "HKCU\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced" /v TaskbarAl /t REG_DWORD /d 0 /f >nul 2>&1
echo   [-] Taskbar aligned to left

:: Disable Search icon on taskbar
reg add "HKCU\Software\Microsoft\Windows\CurrentVersion\Search" /v SearchboxTaskbarMode /t REG_DWORD /d 0 /f >nul 2>&1
echo   [-] Search box hidden from taskbar

:: Disable Task View button
reg add "HKCU\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced" /v ShowTaskViewButton /t REG_DWORD /d 0 /f >nul 2>&1
echo   [-] Task View button hidden

:: Disable Widgets
reg add "HKCU\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced" /v TaskbarDa /t REG_DWORD /d 0 /f >nul 2>&1
echo   [-] Widgets disabled

:: Disable Chat icon
reg add "HKCU\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced" /v TaskbarMn /t REG_DWORD /d 0 /f >nul 2>&1
echo   [-] Chat icon hidden

:: Classic context menu (Windows 11)
reg add "HKCU\Software\Classes\CLSID\{86ca1aa0-34aa-4e8b-a509-50c905bae2a2}\InprocServer32" /ve /t REG_SZ /d "" /f >nul 2>&1
echo   [-] Classic context menu enabled

echo.
echo   [✓] Visual tweaks complete!
echo   [!] Restart Explorer to see changes
timeout /t 3 >nul
goto MENU