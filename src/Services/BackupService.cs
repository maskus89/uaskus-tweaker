using System.Diagnostics;
using System.IO;

namespace UaskusTweaks.Services;

public class BackupService
{
    private static readonly string BackupDir =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "UaskusTweaksBackups");

    public async Task<string> BackupRegistryAsync()
    {
        Directory.CreateDirectory(BackupDir);
        var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        var backupPath = Path.Combine(BackupDir, $"reg_backup_{timestamp}.reg");

        // Export HKLM\SOFTWARE key as a broad backup
        var (_, _) = await RunRegExportAsync("HKLM\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion", backupPath);
        return backupPath;
    }

    public async Task<bool> RestoreRegistryAsync(string backupPath)
    {
        if (!File.Exists(backupPath)) return false;
        var psi = new ProcessStartInfo("reg", $"import \"{backupPath}\"")
        {
            UseShellExecute = true,
            Verb = "runas",
            CreateNoWindow = true
        };
        using var proc = Process.Start(psi);
        if (proc == null) return false;
        await proc.WaitForExitAsync();
        return proc.ExitCode == 0;
    }

    private static async Task<(bool, string)> RunRegExportAsync(string keyPath, string filePath)
    {
        try
        {
            var psi = new ProcessStartInfo("reg", $"export \"{keyPath}\" \"{filePath}\" /y")
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            using var proc = Process.Start(psi)!;
            var err = await proc.StandardError.ReadToEndAsync();
            await proc.WaitForExitAsync();
            return (proc.ExitCode == 0, err);
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
    }
}
