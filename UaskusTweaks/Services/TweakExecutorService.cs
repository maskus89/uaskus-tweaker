using System.Diagnostics;
using System.Text;
using Microsoft.Win32;
using UaskusTweaks.Models;

namespace UaskusTweaks.Services;

public class TweakExecutorService
{
    private readonly RegistryService _registry = new();
    private readonly PowerShellService _ps = new();

    public async Task<(bool Success, string Message)> ExecuteAsync(TweakCommand command)
    {
        try
        {
            return command.Type switch
            {
                CommandType.Registry => ExecuteRegistry(command),
                CommandType.PowerShell => await _ps.ExecuteAsync(command.Command),
                CommandType.Service => await ExecuteServiceAsync(command),
                CommandType.BcdEdit => await ExecuteProcessAsync("bcdedit", command.Command),
                CommandType.NetSh => await ExecuteProcessAsync("netsh", command.Command),
                CommandType.Command => await ExecuteShellCommandAsync(command.Command),
                _ => (false, $"Unknown command type: {command.Type}")
            };
        }
        catch (Exception ex)
        {
            return (false, $"Exception in {command.Type}: {ex.Message}");
        }
    }

    private (bool, string) ExecuteRegistry(TweakCommand command)
    {
        // Format: "path|name|value|kind"
        var parts = command.Command.Split('|');
        if (parts.Length < 4)
            return (false, $"Invalid registry command format: {command.Command}");

        var path = parts[0];
        var name = parts[1];
        var rawValue = parts[2];
        var kindStr = parts[3];

        if (!Enum.TryParse<RegistryValueKind>(kindStr, out var kind))
            kind = RegistryValueKind.DWord;

        object value = kind switch
        {
            RegistryValueKind.DWord => int.TryParse(rawValue, out var iv) ? iv : Convert.ToInt32(rawValue, 16),
            RegistryValueKind.QWord => long.TryParse(rawValue, out var lv) ? lv : Convert.ToInt64(rawValue, 16),
            RegistryValueKind.Binary => Convert.FromBase64String(rawValue),
            RegistryValueKind.String => rawValue,
            RegistryValueKind.ExpandString => rawValue,
            RegistryValueKind.MultiString => rawValue.Split('\n'),
            _ => rawValue
        };

        _registry.SetValue(path, name, value, kind);
        return (true, $"Set {path}\\{name} = {rawValue}");
    }

    private static async Task<(bool, string)> ExecuteServiceAsync(TweakCommand command)
    {
        // Format: "serviceName|startType"   startType: demand | disabled | auto | boot | system
        var parts = command.Command.Split('|');
        if (parts.Length < 2)
            return (false, $"Invalid service command: {command.Command}");

        var svcName = parts[0];
        var startType = parts[1]; // demand, disabled, auto

        // Map to sc.exe start type names
        var scType = startType.ToLowerInvariant() switch
        {
            "demand" => "demand",
            "disabled" => "disabled",
            "auto" => "auto",
            "boot" => "boot",
            "system" => "system",
            _ => startType
        };

        var sb = new StringBuilder();
        bool ok = true;

        if (scType == "disabled")
        {
            // Stop first, then disable
            var stopResult = await ExecuteProcessAsync("sc.exe", $"stop {svcName}");
            // Ignore stop failure (service may already be stopped)
            var configResult = await ExecuteProcessAsync("sc.exe", $"config {svcName} start= disabled");
            ok = configResult.Item1;
            sb.AppendLine(configResult.Item2);
        }
        else
        {
            var result = await ExecuteProcessAsync("sc.exe", $"config {svcName} start= {scType}");
            ok = result.Item1;
            sb.AppendLine(result.Item2);
        }

        return (ok, sb.ToString().Trim());
    }

    private static async Task<(bool, string)> ExecuteProcessAsync(string exe, string args)
    {
        try
        {
            var psi = new ProcessStartInfo(exe, args)
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                StandardOutputEncoding = Encoding.UTF8
            };
            using var proc = Process.Start(psi)!;
            var stdout = await proc.StandardOutput.ReadToEndAsync();
            var stderr = await proc.StandardError.ReadToEndAsync();
            await proc.WaitForExitAsync();
            var combined = $"{stdout}{stderr}".Trim();
            return (proc.ExitCode == 0, combined);
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
    }

    private static async Task<(bool, string)> ExecuteShellCommandAsync(string command)
    {
        try
        {
            // Split into exe and args
            var spaceIdx = command.IndexOf(' ');
            string exe, args;
            if (spaceIdx >= 0)
            {
                exe = command[..spaceIdx];
                args = command[(spaceIdx + 1)..];
            }
            else
            {
                exe = command;
                args = string.Empty;
            }
            return await ExecuteProcessAsync(exe, args);
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
    }
}
