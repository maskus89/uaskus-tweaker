using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using UaskusTweaks.Models;
using UaskusTweaks.Services;

var checks = 0;
void Check(bool condition, string message)
{
    if (!condition) throw new InvalidOperationException(message);
    checks++;
}

var categories = TweakDefinitions.GetAllCategories();
var tweaks = categories.SelectMany(c => c.Tweaks).Select(t => t.Model).ToList();
var errors = TweakCatalogValidator.Validate(tweaks);
Check(errors.Count == 0, string.Join("\n", errors));
var originalCulture = System.Globalization.CultureInfo.CurrentCulture;
try
{
    foreach (var culture in new[] { "nb-NO", "en-US", "ar-SA" })
    {
        System.Globalization.CultureInfo.CurrentCulture = new System.Globalization.CultureInfo(culture);
        var localized = TweakDefinitions.GetAllCategories().SelectMany(c => c.Tweaks).Select(t => t.Model).ToList();
        Check(TweakCatalogValidator.Validate(localized).Count == 0, $"Invalid catalog under {culture}.");
        Check(localized.SelectMany(t => t.Commands).Select(c => c.Command)
            .SequenceEqual(tweaks.SelectMany(t => t.Commands).Select(c => c.Command)), $"Locale changed commands under {culture}.");
    }
}
finally { System.Globalization.CultureInfo.CurrentCulture = originalCulture; }
Check(categories.Count == 10 && tweaks.Count == 87, "Unexpected catalog size; review additions/removals.");
Check(tweaks.All(t => !string.IsNullOrWhiteSpace(t.Category)), "Missing category.");
Check(!tweaks.Any(t => t.Id is "essential_services" or "priv_diagdata"), "Overlapping legacy tweaks returned.");
var services = tweaks.SelectMany(t => t.Commands).Where(c => c.Type == CommandType.Service).ToList();
Check(services.Select(c => c.Command.Split('|')[0]).Distinct(StringComparer.OrdinalIgnoreCase).Count() == services.Count,
    "Service controls appear in more than one tweak.");
foreach (var tweak in tweaks)
{
    Check(tweak.IsAdminRequired || !tweak.Commands.Any(c => c.Type == CommandType.Registry &&
        (c.Command.StartsWith("HKLM\\") || c.Command.StartsWith("HKU\\"))), $"{tweak.Id}: missing admin marker.");
}

var byId = tweaks.ToDictionary(t => t.Id);
Check(TweakSelectionValidator.FindConflicts(new[] { byId["perf_power_ultimate"], byId["game_power"] }).Count == 1,
    "Two power plans must conflict.");
Check(TweakSelectionValidator.FindConflicts(new[] { byId["essential_ipv4"], byId["adv_ipv6_disable"] }).Count == 1,
    "Contradictory IPv6 choices must conflict.");
var source = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "MainViewModel.source"));
var presets = Regex.Matches(source, @"ApplyPreset\(SetupMode\.\w+,\s*""[^""]+"",(?<ids>.*?)\)\)", RegexOptions.Singleline);
Check(presets.Count == 4, "Could not inspect all four production presets.");
foreach (Match preset in presets)
{
    var ids = Regex.Matches(preset.Groups["ids"].Value, @"""(?<id>[a-z0-9_]+)""")
        .Select(m => m.Groups["id"].Value).ToArray();
    Check(ids.All(byId.ContainsKey), "Preset references a removed or missing tweak.");
    Check(TweakSelectionValidator.FindConflicts(ids.Select(id => byId[id])).Count == 0, "Preset contains conflicts.");
    var ordered = ids.Select(id => byId[id]).OrderBy(t => t.ApplyOrder).ToList();
    Check(ordered.All(t => t.ExclusiveGroup != "Power plan") || ordered[0].ExclusiveGroup == "Power plan",
        "Power plan must precede changes to the current plan.");
}
Check(TweakCatalogValidator.Validate(new[] { tweaks[0], tweaks[0] }).Count >= 2, "Duplicate catalog wasn't rejected.");
foreach (var malformed in new[]
{
    new TweakCommand { Type = CommandType.Registry, Command = "HKLM\\Test|value|bad|DWord" },
    new TweakCommand { Type = CommandType.Registry, Command = "HKLM\\Test|value|1|Bogus" },
    new TweakCommand { Type = CommandType.Service, Command = "service|invalid" },
    new TweakCommand { Type = (CommandType)999, Command = "unknown" }
})
    Check(TweakCatalogValidator.ValidateCommand(malformed) is not null, "Malformed command wasn't rejected.");

ProcessStartInfo Shell(string script)
{
    var info = new ProcessStartInfo(OperatingSystem.IsWindows() ? "powershell.exe" : "/bin/sh");
    if (OperatingSystem.IsWindows())
    {
        info.ArgumentList.Add("-NoProfile");
        info.ArgumentList.Add("-NonInteractive");
        info.ArgumentList.Add("-EncodedCommand");
        info.ArgumentList.Add(Convert.ToBase64String(Encoding.Unicode.GetBytes(script)));
    }
    else
    {
        info.ArgumentList.Add("-c");
        info.ArgumentList.Add(script);
    }
    return info;
}
var success = await ProcessRunner.RunAsync(Shell(OperatingSystem.IsWindows() ? "Write-Output 'hello'" : "printf hello"));
Check(success.Success && success.Output.Contains("hello"), "Successful process lost its output.");
var failure = await ProcessRunner.RunAsync(Shell("exit 7"));
Check(!failure.Success && failure.Output.Contains("code 7:"), "Nonzero process exit wasn't reported.");
var missing = await ProcessRunner.RunAsync(new ProcessStartInfo("uaskus-nonexistent-executable-9e0896"));
Check(!missing.Success, "Missing executable was considered successful.");
var noisy = await ProcessRunner.RunAsync(Shell(OperatingSystem.IsWindows()
    ? "[Console]::Error.Write(('x' * 200000)); [Console]::Out.Write(('y' * 200000))"
    : "i=0; while [ $i -lt 20000 ]; do printf 'xxxxxxxxxx' >&2; i=$((i+1)); done; printf done"),
    TimeSpan.FromSeconds(30));
Check(noisy.Success && noisy.Output.Length > 200000, "Large stderr caused deadlock or lost output.");
var timeout = await ProcessRunner.RunAsync(Shell(OperatingSystem.IsWindows() ? "Start-Sleep -Seconds 30" : "sleep 30"),
    TimeSpan.FromMilliseconds(500));
Check(!timeout.Success && timeout.Output.Contains("timed out"), "Hung process wasn't terminated.");

if (OperatingSystem.IsWindows())
{
    var executor = new TweakExecutorService();
    Check(!(await executor.ExecuteAsync(new TweakCommand { Type = CommandType.Command, Command = "cmd.exe /c exit 7" })).Success,
        "Executor ignored a native command failure.");
    Check(!(await executor.ExecuteAsync(new TweakCommand { Type = CommandType.Service, Command = "UaskusMissingService9e0896|disabled" })).Success,
        "Missing service was reported as changed.");
    var scratch = $@"Software\UaskusTweaks\Tests\{Guid.NewGuid():N}";
    try
    {
        var registryTweak = new Tweak
        {
            Commands = new()
            {
                new() { Type = CommandType.Registry, Command = $@"HKCU\{scratch}|Negative|-1|DWord" },
                new() { Type = CommandType.Registry, Command = $@"HKCU\{scratch}|Binary|kBIDgBAAAAA=|Binary" }
            }
        };
        foreach (var command in registryTweak.Commands)
            Check((await executor.ExecuteAsync(command)).Success, "Scratch registry write failed.");
        Check(await new TweakStateDetector().CheckAsync(registryTweak) == TweakState.Enabled,
            "Written registry values weren't detected as enabled.");
    }
    finally { Microsoft.Win32.Registry.CurrentUser.DeleteSubKeyTree(scratch, throwOnMissingSubKey: false); }
    var ps = new PowerShellService();
    foreach (var script in new[] { "Write-Error 'expected failure'", "Get-Item 'Z:\\uaskus-nonexistent-9e0896'",
                 "Write-Error 'suppressed failure' -ErrorAction SilentlyContinue", "cmd.exe /c exit 7" })
        Check(!(await ps.ExecuteAsync(script)).Success, $"PowerShell failure was hidden: {script}");
    var unicode = await ps.ExecuteAsync("Write-Output 'quoted \"text\" æøå'");
    Check(unicode.Success && unicode.Output.Contains("æøå"), "PowerShell encoding/quoting broke.");
    foreach (var command in tweaks.SelectMany(t => t.Commands).Where(c => c.Type == CommandType.PowerShell))
    {
        var encoded = Convert.ToBase64String(Encoding.Unicode.GetBytes(PowerShellService.BuildScript(command.Command)));
        var syntax = await ps.ExecuteAsync(
            $"$s=[Text.Encoding]::Unicode.GetString([Convert]::FromBase64String('{encoded}')); " +
            "$t=$null; $e=$null; [System.Management.Automation.Language.Parser]::ParseInput($s,[ref]$t,[ref]$e) | Out-Null; " +
            "if ($e.Count -gt 0) { throw ($e.Message -join '; ') }");
        Check(syntax.Success, $"Catalog PowerShell syntax error: {syntax.Output}");
    }
}
else
    Console.WriteLine("SKIPPED: Windows PowerShell error/syntax tests require Windows; CI runs them there.");
Console.WriteLine($"PASS: {checks} checks; {tweaks.Count} tweaks, {tweaks.Sum(t => t.Commands.Count)} commands, {presets.Count} presets.");
