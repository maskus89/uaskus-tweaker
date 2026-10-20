using System.Diagnostics;

namespace UaskusTweaks.Services;

/// <summary>Drains both pipes concurrently and bounds external command execution.</summary>
public static class ProcessRunner
{
    public static async Task<(bool Success, string Output)> RunAsync(
        ProcessStartInfo startInfo, TimeSpan? timeout = null)
    {
        startInfo.UseShellExecute = false;
        startInfo.CreateNoWindow = true;
        startInfo.RedirectStandardOutput = true;
        startInfo.RedirectStandardError = true;
        try
        {
            using var process = Process.Start(startInfo)
                ?? throw new InvalidOperationException($"Failed to start {startInfo.FileName}.");
            var stdout = process.StandardOutput.ReadToEndAsync();
            var stderr = process.StandardError.ReadToEndAsync();
            using var cancellation = new CancellationTokenSource(timeout ?? TimeSpan.FromMinutes(5));
            try
            {
                await process.WaitForExitAsync(cancellation.Token);
                await Task.WhenAll(stdout, stderr).WaitAsync(cancellation.Token);
            }
            catch (OperationCanceledException)
            {
                try { process.Kill(entireProcessTree: true); }
                catch (InvalidOperationException) { }
                return (false, $"{startInfo.FileName} timed out. Changes may have been partially applied; check the current state before retrying.");
            }

            var output = $"{await stdout}\n{await stderr}".Trim();
            return process.ExitCode == 0
                ? (true, output)
                : (false, $"{startInfo.FileName} exited with code {process.ExitCode}: {output}");
        }
        catch (Exception ex)
        {
            return (false, $"{startInfo.FileName}: {ex.Message}");
        }
    }
}
