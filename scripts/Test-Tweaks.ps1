[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)][string]$ExecutablePath,
    [string]$ReportPath = (Join-Path $PWD ("tweak-report-{0}.json" -f (Get-Date -Format 'yyyyMMdd-HHmmss'))),
    [string]$TweakId,
    [switch]$Execute,
    [switch]$DisposableTestMachine
)

$ErrorActionPreference = 'Stop'
if ($env:OS -ne 'Windows_NT') { throw 'This runner requires Windows.' }
if ($Execute -and -not $DisposableTestMachine) {
    throw 'Execution changes Windows settings and removes apps. Restore a disposable VM checkpoint first, then pass -DisposableTestMachine.'
}
$exe = (Resolve-Path -LiteralPath $ExecutablePath).Path
$report = [IO.Path]::GetFullPath($ReportPath)
$arguments = @('--audit-tweaks', '--report', ('"{0}"' -f $report))
if ($Execute) { $arguments[0] = '--test-tweaks'; $arguments += '--disposable-test-machine' }
if ($TweakId) {
    if ($TweakId -notmatch '^[a-z0-9_]+$') { throw 'Invalid tweak ID.' }
    $arguments += @('--tweak-id', $TweakId)
}
$startedAt = [DateTimeOffset]::Now
$process = Start-Process -FilePath $exe -ArgumentList $arguments -PassThru -Wait
if (-not (Test-Path -LiteralPath $report)) { throw "No report was written. Exit code: $($process.ExitCode). Check %TEMP%\uaskus_debug.log." }
$result = Get-Content -LiteralPath $report -Raw | ConvertFrom-Json
if ([DateTimeOffset]::Parse($result.StartedAt) -lt $startedAt) {
    throw 'The executable did not produce a fresh report. Ensure you are using the new build with diagnostic support.'
}
Write-Host ("Mode: {0}; completed: {1}; tweaks: {2}" -f $result.Mode, $result.Completed, @($result.Tweaks).Count)
foreach ($tweak in $result.Tweaks) {
    foreach ($command in $tweak.Commands) {
        Write-Host ("[{0}] {1}: {2}" -f $command.Status, $tweak.Id, $command.Message)
    }
}
foreach ($errorMessage in $result.CatalogErrors) { Write-Warning $errorMessage }
Write-Host "Full report: $report"
Write-Host $result.Limitations
exit $process.ExitCode
