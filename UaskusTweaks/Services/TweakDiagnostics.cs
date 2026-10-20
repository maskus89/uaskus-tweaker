using System.IO;
using System.Security.Principal;
using System.Text;
using System.Text.Json;
using Microsoft.Win32;
using UaskusTweaks.Models;

namespace UaskusTweaks.Services;

/// <summary>Read-only audit and explicitly opt-in execution on a disposable Windows machine.</summary>
public static class TweakDiagnostics
{
    public static async Task<int> RunAsync(string[] args)
    {
        var apply = args.Contains("--test-tweaks", StringComparer.OrdinalIgnoreCase);
        var reportPath = Path.GetFullPath(GetOption(args, "--report") ??
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                $"UaskusTweaks-{(apply ? "execution" : "audit")}-{DateTime.Now:yyyyMMdd-HHmmss}.json"));
        var report = new DiagnosticReport
        {
            Mode = apply ? "Execution on disposable test machine" : "Read-only audit",
            WindowsVersion = Environment.OSVersion.ToString()
        };
        try
        {
            if (apply && !args.Contains("--disposable-test-machine", StringComparer.OrdinalIgnoreCase))
                throw new ArgumentException("Execution requires --disposable-test-machine. Use a VM checkpoint; this mode changes Windows settings and can remove apps.");
            if (apply && !new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator))
                throw new InvalidOperationException("Execution tests require Administrator privileges.");

            var catalog = TweakDefinitions.GetAllCategories().SelectMany(c => c.Tweaks).Select(t => t.Model).ToList();
            report.CatalogErrors.AddRange(TweakCatalogValidator.Validate(catalog));
            if (report.CatalogErrors.Count > 0) { Save(reportPath, report); return 1; }
            var id = GetOption(args, "--tweak-id");
            var selected = catalog.Where(t => id is null || string.Equals(t.Id, id, StringComparison.OrdinalIgnoreCase))
                .OrderBy(t => t.ApplyOrder).ToList();
            if (selected.Count == 0) throw new ArgumentException($"Unknown tweak ID: {id}");
            // All-tweak execution intentionally includes alternatives. Report those overlaps.
            report.SelectionConflicts.AddRange(TweakSelectionValidator.FindConflicts(selected));
            var executor = new TweakExecutorService();
            var detector = new TweakStateDetector();
            var ps = new PowerShellService();
            Save(reportPath, report);
            foreach (var tweak in selected)
            {
                var result = new DiagnosticTweak { Id = tweak.Id, Name = tweak.Name, Category = tweak.Category };
                report.Tweaks.Add(result);
                result.StateBefore = (await detector.CheckAsync(tweak)).ToString();
                var failed = false;
                foreach (var command in tweak.Commands)
                {
                    var item = new DiagnosticCommand { Type = command.Type.ToString(), Command = command.Command };
                    result.Commands.Add(item);
                    if (apply && failed)
                    {
                        item.Status = "Skipped";
                        item.Message = "An earlier command in this tweak failed.";
                    }
                    else if (apply)
                    {
                        // Save the in-flight command so a crash or reboot leaves a useful journal.
                        item.Status = "Running";
                        Save(reportPath, report);
                        var execution = await executor.ExecuteAsync(command);
                        item.Status = execution.Success ? "Succeeded" : "Failed";
                        item.Message = execution.Message;
                        failed = !execution.Success;
                    }
                    else
                    {
                        try
                        {
                            var audit = await AuditCommandAsync(command, ps);
                            item.Status = audit.Success ? "Needs execution test" : "Unavailable or invalid";
                            item.Message = audit.Message;
                        }
                        catch (Exception ex)
                        {
                            item.Status = "Unavailable or invalid";
                            item.Message = ex.Message;
                        }
                    }
                    Save(reportPath, report);
                }
                result.StateAfter = (await detector.CheckAsync(tweak)).ToString();
                Save(reportPath, report);
            }
            report.Completed = true;
            Save(reportPath, report);
            return report.Tweaks.SelectMany(t => t.Commands)
                .Any(c => c.Status is "Failed" or "Unavailable or invalid") ? 1 : 0;
        }
        catch (Exception ex)
        {
            report.CatalogErrors.Add(ex.Message);
            Save(reportPath, report);
            return 1;
        }
    }

    private static Task<(bool Success, string Message)> AuditCommandAsync(TweakCommand command, PowerShellService ps)
    {
        if (command.Type == CommandType.Registry)
            return Task.FromResult((true, "Format checked only. Registry writability and whether Windows honors the value require an execution test."));
        if (command.Type == CommandType.Service)
        {
            var name = command.Command.Split('|')[0];
            using var key = Registry.LocalMachine.OpenSubKey($@"SYSTEM\CurrentControlSet\Services\{name}");
            return Task.FromResult((key is not null, key is null
                ? $"Service '{name}' is not installed."
                : "Service exists. Stop/configure permissions and runtime behavior are not tested."));
        }

        if (command.Type == CommandType.PowerShell)
        {
            var encoded = Convert.ToBase64String(Encoding.Unicode.GetBytes(command.Command));
            // Parse the script as data. Never invoke the parsed commands during an audit.
            return ps.ExecuteAsync(
                $"$source = [Text.Encoding]::Unicode.GetString([Convert]::FromBase64String('{encoded}')); " +
                "$tokens = $null; $parseErrors = $null; " +
                "$ast = [System.Management.Automation.Language.Parser]::ParseInput($source, [ref]$tokens, [ref]$parseErrors); " +
                "if ($parseErrors.Count -gt 0) { throw ($parseErrors.Message -join '; ') }; " +
                "$nodes = $ast.FindAll({param($n) $n -is [System.Management.Automation.Language.CommandAst]}, $true); " +
                "foreach ($node in $nodes) { $name = $node.GetCommandName(); " +
                "if ($name) { Get-Command -Name $name -ErrorAction Stop | Out-Null } }; " +
                "Write-Output 'Syntax and named commands checked. Parameters, permissions, hardware support, and effects still require an execution test.'");
        }
        var exe = command.Type switch
        {
            CommandType.BcdEdit => "bcdedit.exe",
            CommandType.NetSh => "netsh.exe",
            _ => command.Command.Split(' ', 2)[0]
        };
        var quoted = exe.Replace("'", "''");
        return ps.ExecuteAsync($"Get-Command -Name '{quoted}' -CommandType Application -ErrorAction Stop | Out-Null; " +
                               "Write-Output 'Executable found. Arguments and effects have not been tested.'");
    }

    private static string? GetOption(string[] args, string option)
    {
        var index = Array.FindIndex(args, a => string.Equals(a, option, StringComparison.OrdinalIgnoreCase));
        if (index < 0) return null;
        if (index + 1 >= args.Length || args[index + 1].StartsWith("--"))
            throw new ArgumentException($"Missing value for {option}.");
        return args[index + 1];
    }

    private static void Save(string path, DiagnosticReport report)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        var temporary = path + ".tmp";
        File.WriteAllText(temporary, JsonSerializer.Serialize(report, new JsonSerializerOptions { WriteIndented = true }));
        File.Move(temporary, path, overwrite: true);
    }

    private sealed class DiagnosticReport
    {
        public DateTimeOffset StartedAt { get; set; } = DateTimeOffset.Now;
        public string Mode { get; set; } = "";
        public string WindowsVersion { get; set; } = "";
        public bool Completed { get; set; }
        public string Limitations { get; set; } = "Command success does not prove a performance benefit or that Windows honors a registry value. All-tweak execution is sequential, not isolated; restore a VM checkpoint between individual --tweak-id tests for independent results. Some effects require reboot.";
        public List<string> CatalogErrors { get; } = new();
        public List<string> SelectionConflicts { get; } = new();
        public List<DiagnosticTweak> Tweaks { get; } = new();
    }

    private sealed class DiagnosticTweak
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public string Category { get; set; } = "";
        public string StateBefore { get; set; } = "";
        public string StateAfter { get; set; } = "";
        public List<DiagnosticCommand> Commands { get; } = new();
    }

    private sealed class DiagnosticCommand
    {
        public string Type { get; set; } = "";
        public string Command { get; set; } = "";
        public string Status { get; set; } = "";
        public string Message { get; set; } = "";
    }
}
