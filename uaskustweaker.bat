@echo off
title Uaskus Tweaker v1.0
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
echo   UASKUS TWEAKER v1.0
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
    wmic.exe /Namespace:\\root\default Path SystemRestore Call CreateRestorePoint "Uaskus Tweaker - Before Changes", 100, 7 >nul 2>&1
    echo   Restore point created!
    timeout /t 2 >nul
)

:MENU
cls
echo.
echo   ___  ___  ________  ________  ___  __    ___  ___  ________      
echo  ^|\  \^|\  \^|\   __  \^|\   ____\^|\  \^|\  \ ^|\  \^|\  \^|\   ____\     
echo  ^\ \  \\\  \ \  \^|\  \ \  \___^|^\ \  \/  /^|^\ \  \\\  \ \  \___^|     
echo   ^\ \  \\\  \ \   __  \ \_____  \ \   ___  \ \  \\\  \ \_____  \    
echo    ^\ \  \\\  \ \  \ \  \^|____^|\  \ \  \\ \  \ \  \\\  \^|____^|\  \   
echo     ^\ \_______\ \__\ \__\____\_\  \ \__\\ \__\ \_______\____\_\  \  
echo      \^|_______^|\^|__^|\^|__^|\_________\^|__^| \^|__^|\^|_______^|\_________\ 
echo                      \^|_________^|                    \^|_________^| 
echo.
echo   _________  ___       ___  _______   ________  ___  __    _______   ________     
echo  ^|\___   ___\\  \     ^|\  \^|\  ___ \ ^|\   __  \^|\  \^|\  \ ^|\  ___ \ ^|\   __  \    
echo  \^|___ \  \_\ \  \    \ \  \ \   __/^|^\ \  \^|\  \ \  \/  /^|^\ \   __/^|^\ \  \^|\  \   
echo       ^\ \  \ ^\ \  \  __\ \  \ \  \_^|/__\ \   __  \ \   ___  \ \  \_^|/__\ \   _  _\  
echo        ^\ \  \ ^\ \  \^|\__\_\  \ \  \_^|\ \ \  \ \  \ \  \\ \  \ \  \_^|\ \ \  \\  \^| 
echo         ^\ \__\ ^\ \____________\ \_______\ \__\ \__\ \__\\ \__\ \_______\ \__\\ _\  
echo          \^|__^|  \^|____________^|\^|_______^|\^|__^|\^|__^|\^|__^| \^|__^|\^|_______^|\^|__^|\^|__^| 
echo.
echo                              Created by: maskus
echo                              Version: 1.0
echo   ================================================================
echo.
echo.
echo       [ 1 ] Essential Tweaks (Recommended) [ 2 ] Performance Tweaks
echo.
echo       [ 3 ] Privacy Tweaks                 [ 4 ] Network Tweaks
echo.
echo       [ 5 ] Debloat Windows                [ 6 ] Gaming Tweaks
echo.
echo       [ 0 ] Exit
echo.
echo.
set /p choice="  Select an option: "

if "%choice%"=="1" goto ESSENTIAL
if "%choice%"=="2" goto PERFORMANCE
if "%choice%"=="3" goto PRIVACY
if "%choice%"=="4" goto NETWORK
if "%choice%"=="5" goto DEBLOAT
if "%choice%"=="6" goto GAMING
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
echo   [-] Telemetry disabled

:: Disable Activity History
reg add "HKLM\SOFTWARE\Policies\Microsoft\Windows\System" /v EnableActivityFeed /t REG_DWORD /d 0 /f >nul 2>&1
reg add "HKLM\SOFTWARE\Policies\Microsoft\Windows\System" /v PublishUserActivities /t REG_DWORD /d 0 /f >nul 2>&1
echo   [-] Activity history disabled

:: Disable Consumer Features (Sponsored apps)
reg add "HKCU\Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager" /v SubscribedContent-338389Enabled /t REG_DWORD /d 0 /f >nul 2>&1
reg add "HKCU\Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager" /v SubscribedContent-338388Enabled /t REG_DWORD /d 0 /f >nul 2>&1
echo   [-] Consumer features disabled

:: Disable Game DVR
reg add "HKCU\System\GameConfigStore" /v GameDVR_Enabled /t REG_DWORD /d 0 /f >nul 2>&1
reg add "HKLM\SOFTWARE\Policies\Microsoft\Windows\GameDVR" /v AllowGameDVR /t REG_DWORD /d 0 /f >nul 2>&1
echo   [-] Game DVR disabled

:: Disable Hibernation
powercfg -h off >nul 2>&1
echo   [-] Hibernation disabled

:: Disable Location Tracking
reg add "HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\CapabilityAccessManager\ConsentStore\location" /v Value /t REG_SZ /d Deny /f >nul 2>&1
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
echo   [-] Unnecessary services set to manual

:: Debloat Edge
reg add "HKLM\SOFTWARE\Policies\Microsoft\Edge" /v StartupBoostEnabled /t REG_DWORD /d 0 /f >nul 2>&1
reg add "HKLM\SOFTWARE\Policies\Microsoft\Edge" /v BackgroundModeEnabled /t REG_DWORD /d 0 /f >nul 2>&1
echo   [-] Edge debloated (startup boost & background mode disabled)

echo.
echo   [✓] Essential tweaks complete! (Chris Titus Recommended)
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
echo   [-] Animations disabled

:: High Performance Power Plan
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
echo   [-] Prefetch disabled

:: Disable Paging Executive
reg add "HKLM\SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management" /v DisablePagingExecutive /t REG_DWORD /d 1 /f >nul 2>&1
echo   [-] Paging executive disabled

:: Disable Windows Search Indexing
sc stop "WSearch" >nul 2>&1
sc config "WSearch" start= disabled >nul 2>&1
echo   [-] Windows Search indexing disabled

:: Disable Print Spooler (if you don't print)
sc config "Spooler" start= demand >nul 2>&1
echo   [-] Print Spooler set to manual

echo.
echo   [✓] Performance tweaks complete!
timeout /t 3 >nul
goto MENU

:RAM
cls
echo.
echo   [+] Applying RAM Optimizations...
echo.

:: Disable SuperFetch/SysMain
sc stop "SysMain" >nul 2>&1
sc config "SysMain" start= disabled >nul 2>&1
echo   [-] SuperFetch disabled

:: Disable Prefetch
reg add "HKLM\SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management\PrefetchParameters" /v EnablePrefetcher /t REG_DWORD /d 0 /f >nul 2>&1
echo   [-] Prefetch disabled

:: Clear Page File on shutdown
reg add "HKLM\SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management" /v ClearPageFileAtShutdown /t REG_DWORD /d 1 /f >nul 2>&1
echo   [-] Page file clear on shutdown enabled

:: Disable Paging Executive
reg add "HKLM\SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management" /v DisablePagingExecutive /t REG_DWORD /d 1 /f >nul 2>&1
echo   [-] Paging executive disabled

echo.
echo   [✓] RAM optimizations complete!
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
echo   [-] Cortana disabled

:: Disable Windows Feedback
reg add "HKCU\Software\Microsoft\Siuf\Rules" /v NumberOfSIUFInPeriod /t REG_DWORD /d 0 /f >nul 2>&1
echo   [-] Windows feedback disabled

:: Disable Background Apps
reg add "HKCU\Software\Microsoft\Windows\CurrentVersion\BackgroundAccessApplications" /v GlobalUserDisabled /t REG_DWORD /d 1 /f >nul 2>&1
echo   [-] Background apps disabled (may affect Steam/Epic - restart them after)

:: Disable Windows Feedback  
reg add "HKCU\Software\Microsoft\Siuf\Rules" /v NumberOfSIUFInPeriod /t REG_DWORD /d 0 /f >nul 2>&1
echo   [-] Windows feedback disabled

echo.
echo   [✓] Privacy tweaks complete!
echo   [!] You may need to re-login to Steam, Epic, Discord, etc.
timeout /t 4 >nul
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
echo   [-] TCP auto-tuning optimized

:: Enable TCP Window Scaling
netsh int tcp set global timestamps=enabled >nul 2>&1
echo   [-] TCP timestamps enabled

:: Prefer IPv4 over IPv6
reg add "HKLM\SYSTEM\CurrentControlSet\Services\Tcpip6\Parameters" /v DisabledComponents /t REG_DWORD /d 32 /f >nul 2>&1
echo   [-] IPv4 preferred over IPv6

:: Disable QoS Packet Scheduler (optional - for max bandwidth)
reg add "HKLM\SOFTWARE\Policies\Microsoft\Windows\Psched" /v NonBestEffortLimit /t REG_DWORD /d 0 /f >nul 2>&1
echo   [-] QoS bandwidth reservation disabled

echo.
echo   [!] OPTIONAL: Run Winsock/IP reset? (Will log you out of everything)
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

powershell -Command "Get-AppxPackage *xbox* | Remove-AppxPackage" >nul 2>&1
echo   [-] Removed Xbox apps

powershell -Command "Get-AppxPackage *zunemusic* | Remove-AppxPackage" >nul 2>&1
echo   [-] Removed Groove Music

powershell -Command "Get-AppxPackage *zunevideo* | Remove-AppxPackage" >nul 2>&1
echo   [-] Removed Movies & TV

echo.
echo   [✓] Windows debloat complete!
timeout /t 3 >nul
goto MENU

:GAMING
cls
echo.
echo   [+] Applying Gaming Tweaks...
echo.

:: Enable Game Mode
reg add "HKCU\Software\Microsoft\GameBar" /v AllowAutoGameMode /t REG_DWORD /d 1 /f >nul 2>&1
echo   [-] Game mode enabled

:: Disable Game DVR
reg add "HKCU\System\GameConfigStore" /v GameDVR_Enabled /t REG_DWORD /d 0 /f >nul 2>&1
reg add "HKLM\SOFTWARE\Policies\Microsoft\Windows\GameDVR" /v AllowGameDVR /t REG_DWORD /d 0 /f >nul 2>&1
echo   [-] Game DVR disabled

:: Disable Fullscreen Optimizations
reg add "HKCU\System\GameConfigStore" /v GameDVR_DXGIHonorFSEWindowsCompatible /t REG_DWORD /d 1 /f >nul 2>&1
echo   [-] Fullscreen optimizations disabled

:: High Performance Power Plan
powercfg -setactive 8c5e7fda-e8bf-4a96-9a85-a6e23a8c635c >nul 2>&1
echo   [-] High performance power plan activated

:: Disable Windows Update during gaming hours
reg add "HKLM\SOFTWARE\Microsoft\WindowsUpdate\UX\Settings" /v ActiveHoursStart /t REG_DWORD /d 8 /f >nul 2>&1
reg add "HKLM\SOFTWARE\Microsoft\WindowsUpdate\UX\Settings" /v ActiveHoursEnd /t REG_DWORD /d 23 /f >nul 2>&1
echo   [-] Active hours set (8 AM - 11 PM)

:: Disable Nagle's Algorithm for gaming
reg add "HKLM\SYSTEM\CurrentControlSet\Services\Tcpip\Parameters\Interfaces" /v TcpAckFrequency /t REG_DWORD /d 1 /f >nul 2>&1
reg add "HKLM\SYSTEM\CurrentControlSet\Services\Tcpip\Parameters\Interfaces" /v TCPNoDelay /t REG_DWORD /d 1 /f >nul 2>&1
echo   [-] Nagle's algorithm disabled (lower latency)

:: GPU Scheduling (Windows 10 2004+)
reg add "HKLM\SYSTEM\CurrentControlSet\Control\GraphicsDrivers" /v HwSchMode /t REG_DWORD /d 2 /f >nul 2>&1
echo   [-] Hardware-accelerated GPU scheduling enabled

echo.
echo   [✓] Gaming tweaks complete!
timeout /t 3 >nul
goto MENU