# Tweak audit and Windows test runner

Audit date: 2026-10-20. The catalog contains **87 tweaks, 125 commands, and 10 categories**.

## What was checked

The entire catalog was inspected and checked for duplicate IDs, identical command sets,
registry/service command formats, missing administrator markers, missing categories,
and invalid preset references. All four presets are checked for conflicting selections.
The portable checks also exercise actual external-process success, failure, missing
executables, large stderr output, and timeouts.

This work was performed on macOS. The Windows application was cross-compiled, but its
Windows tweaks were **not executed**. Successful builds and catalog checks do not
establish Windows compatibility, measurable performance improvements, or that Windows
honors every registry value.

## Changes and findings

| Area | Finding and change |
| --- | --- |
| Repeated service controls | Removed `essential_services`. It set seven services to Manual, while their dedicated Privacy, Gaming, and Performance entries disabled them. These were overlapping alternatives, not identical commands. |
| Diagnostic-data entry | Removed `priv_diagdata`, which wrote `AllowDiagnosticData`. The documented diagnostic-data policy uses `AllowTelemetry`, already covered by `essential_telemetry`. The Privacy preset now selects ten tweaks. The tooltip explains edition limitations. |
| Ultimate Performance | The old commands duplicated a plan to a newly generated GUID, then activated the source GUID. The replacement creates/reuses a known destination GUID and checks every native exit code. |
| Power-plan order | Plan activation now runs before settings that modify the current plan. Selecting High Performance and Ultimate Performance together is blocked. Failed activation stops the normal Apply operation. |
| Core parking | Replaced writes to power-setting metadata (`ValueMax`) with a current-plan minimum-unparked-core setting of 100% on AC power. |
| CPU idle | Replaced the `disablecpuidle` BCD command with the processor idle power setting on AC power. |
| TCP tuning | Removed old chimney, DCA, and NetDMA switches from the auto-tuning tweak. |
| Scheduled tasks | Removed error redirection. Missing tasks are reported individually; errors disabling installed tasks fail the command. |
| Debloat | Removed `SilentlyContinue` from package removals. Missing packages are reported as already removed; protected packages and removal failures are reported. |
| HPET reset | Checks the current boot entry before deleting an override, so an absent override is not treated as a deletion failure. |
| DNS | No active adapters now produces an explicit failure. |
| Page file | Handles the Windows drive and matching page-file entry instead of assigning properties to an arbitrary collection. Corrected the size-range description, risk level, and restart requirement. |
| Error handling | PowerShell errors and final native exit codes fail visibly. Scripts with multiple native calls check each exit code. Native stdout/stderr are drained concurrently; stalled commands time out after five minutes. |
| Services | Missing services and stop/configure errors are reported. An already-stopped service remains valid. |
| Locale | Registry numeric serialization and parsing now use invariant culture. Tests caught a Norwegian minus-sign parsing failure. |
| Apply reporting | Reports command output, stops a tweak after its first failure, and counts skipped tweaks if plan activation fails. |
| Metadata | Corrected administrator markers and restart markers for affected settings. |

The remaining IPv4 preference versus IPv6-disable choices intentionally remain separate:
they request different behavior, and selecting both is blocked. Visual-effects, animation,
and transparency settings overlap in purpose but use distinct settings, so they remain.

## Windows-specific questions still requiring execution

- Service, scheduled-task, and Appx availability varies with Windows version and installed apps.
- Power-plan availability depends on hardware and power-management support.
- A successful registry write does not prove Windows uses that setting. In particular,
  vendor GPU telemetry keys, legacy shell settings, and policy keys need verification on
  supported Windows builds and drivers.
- GPU scheduling needs a compatible GPU/driver and reboot. Policy behavior varies by edition.
- Boot settings may be restricted by device security configuration.
- Changes to page files, networking, boot settings, and app removal require a disposable
  test system for comprehensive execution tests.
- Normal application Undo only restores its supported registry/service snapshots;
  it is not a substitute for a VM checkpoint. Diagnostic execution deliberately does not
  overwrite the application's last-apply backup and does not automatically undo changes.

## Read-only audit

Use the **new build from this work**, not an older release. Open Windows PowerShell
as Administrator beside the executable and [runner](../scripts/Test-Tweaks.ps1):

```powershell
powershell.exe -NoProfile -ExecutionPolicy Bypass -File .\Test-Tweaks.ps1 `
  -ExecutablePath .\UaskusTweaks.exe -ReportPath .\tweak-audit.json
```

In the repository, the runner is at `scripts\Test-Tweaks.ps1`.
This reads current registry/service states, validates all command formats, parses
PowerShell syntax, and resolves named PowerShell commands/native executables. It does
not run the tweak commands. Its positive status is **Needs execution test**, not “passed.”
Missing dependencies and syntax errors appear as **Unavailable or invalid**.

## Execute on a disposable Windows VM

Create a VM checkpoint first. To test one tweak independently, restore that checkpoint
before each test and use the ID from the audit report:

```powershell
powershell.exe -NoProfile -ExecutionPolicy Bypass -File .\Test-Tweaks.ps1 `
  -ExecutablePath .\UaskusTweaks.exe -ReportPath .\ultimate-performance-test.json `
  -Execute -DisposableTestMachine -TweakId perf_power_ultimate
```

To exercise the whole catalog sequentially:

```powershell
powershell.exe -NoProfile -ExecutionPolicy Bypass -File .\Test-Tweaks.ps1 `
  -ExecutablePath .\UaskusTweaks.exe -ReportPath .\tweak-execution.json `
  -Execute -DisposableTestMachine
```

This **actually changes Windows, removes apps, and can interrupt networking**.
The full run includes mutually exclusive alternatives, recorded under `SelectionConflicts`.
It is useful for collecting failures, but later tweaks inherit earlier changes.
Use individual tests from restored checkpoints for independent results.

Reports record each command, its result/output, category, tweak ID, and detected state
before/after the tweak. They are saved after every command. A crash may leave a command
marked **Running** and `Completed: false`. A failed command skips the rest of that tweak.
No automatic reboot occurs; reboot the VM when required and verify the resulting behavior.

Exit code 0 means the run completed without detected errors in that mode. Exit code 1
means errors or unavailable dependencies were found. In audit mode, exit code 0 does
not mean execution will succeed. Preserve the JSON report when reporting failures.

## Developer checks

```powershell
dotnet run --project tests/UaskusTweaks.Checks -c Release
dotnet build UaskusTweaks/UaskusTweaks.csproj -c Release
```

The portable test project compiles the production catalog and command services using
small data-only adapters for the WPF catalog view models. It does not test WPF rendering.
On Windows, additional checks exercise PowerShell error handling, parse every catalog
PowerShell script, and test registry write/state-detection in a temporary test-only HKCU key.
The key is removed afterward. No production tweak is applied by these regression checks.

CI runs those checks plus the existing WPF startup/preset smoke test. It uploads a
read-only availability audit; missing components on a CI host are reported without
blocking the release. It never runs the destructive full-catalog execution mode.

## References used in the audit

- [Microsoft powercfg command options](https://learn.microsoft.com/en-us/windows-hardware/design/device-experiences/powercfg-command-line-options)
- [Microsoft minimum unparked cores setting](https://learn.microsoft.com/en-us/windows-hardware/customize/power-settings/options-for-core-parking-cpmincores)
- [Microsoft processor idle configuration](https://github.com/MicrosoftDocs/windows-iot-public/blob/main/windows-iot/iot-enterprise/Soft-Real-Time/Soft-Real-Time-Device.md)
- [Microsoft diagnostic-data policy mapping](https://learn.microsoft.com/en-us/windows/client-management/mdm/policy-csp-system#allowtelemetry)
- [Microsoft network offload deprecation guidance](https://learn.microsoft.com/en-us/windows-server/networking/technologies/network-subsystem/net-sub-performance-tuning-nics)
