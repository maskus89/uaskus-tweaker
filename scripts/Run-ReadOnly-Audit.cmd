@echo off
powershell.exe -NoProfile -ExecutionPolicy Bypass -File "%~dp0Test-Tweaks.ps1" -ExecutablePath "%~dp0UaskusTweaks.exe" -ReportPath "%~dp0tweak-audit.json"
set "auditExitCode=%ERRORLEVEL%"
echo.
echo Audit exit code: %auditExitCode%
pause
exit /b %auditExitCode%
