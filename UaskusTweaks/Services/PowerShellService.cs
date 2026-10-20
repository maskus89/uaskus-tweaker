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
            var bytes = Encoding.Unicode.GetBytes(BuildScript(command));
            var encoded = Convert.ToBase64String(bytes);

            var psi = new ProcessStartInfo("powershell.exe")
            {
                Arguments = $"-NoProfile -NonInteractive -ExecutionPolicy Bypass -OutputFormat Text -EncodedCommand {encoded}",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                StandardOutputEncoding = Encoding.UTF8,
                StandardErrorEncoding = Encoding.UTF8
            };

            return await ProcessRunner.RunAsync(psi);
        }
        catch (Exception ex)
        {
            return (false, $"Exception: {ex.Message}");
        }
    }

    // Windows PowerShell otherwise permits non-terminating cmdlet errors to exit 0.
    // Scripts containing several native commands must also check each exit code.
    public static string BuildScript(string command) =>
        "[Console]::OutputEncoding = New-Object System.Text.UTF8Encoding; " +
        "$ErrorActionPreference = 'Stop'; $ProgressPreference = 'SilentlyContinue'; " +
        "$global:LASTEXITCODE = 0; $Error.Clear(); try {\n" + command +
        "\nif ($LASTEXITCODE -ne 0) { throw \"Native command exited with code $LASTEXITCODE\" }; " +
        "if ($Error.Count -gt 0) { throw $Error[0] }; exit 0\n" +
        "} catch { [Console]::Error.WriteLine($_.ToString()); exit 1 }";
}
