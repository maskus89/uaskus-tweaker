using System.Diagnostics;
using System.Text;

namespace UaskusTweaks.Services;

public class PowerShellService
{
    public async Task<(bool Success, string Output)> ExecuteAsync(string command)
    {
        try
        {
            // Use -EncodedCommand (Base64 Unicode) to avoid all shell-escaping issues
            var bytes = Encoding.Unicode.GetBytes(command);
            var encoded = Convert.ToBase64String(bytes);

            var psi = new ProcessStartInfo("powershell.exe")
            {
                Arguments = $"-NoProfile -NonInteractive -ExecutionPolicy Bypass -EncodedCommand {encoded}",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                StandardOutputEncoding = Encoding.UTF8,
                StandardErrorEncoding = Encoding.UTF8
            };

            using var process = Process.Start(psi)
                ?? throw new InvalidOperationException("Failed to start PowerShell process.");

            var stdout = await process.StandardOutput.ReadToEndAsync();
            var stderr = await process.StandardError.ReadToEndAsync();
            await process.WaitForExitAsync();

            var output = string.IsNullOrWhiteSpace(stderr)
                ? stdout.Trim()
                : $"{stdout.Trim()}\nERR: {stderr.Trim()}";

            return (process.ExitCode == 0, output);
        }
        catch (Exception ex)
        {
            return (false, $"Exception: {ex.Message}");
        }
    }
}
