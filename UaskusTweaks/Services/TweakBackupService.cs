using System.IO;
using System.Text.Json;
using Microsoft.Win32;
using UaskusTweaks.Models;

namespace UaskusTweaks.Services;

/// <summary>Persists the pre-change registry state for the most recent apply operation.</summary>
public sealed class TweakBackupService
{
    private static readonly string BackupPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
        "UaskusTweaksBackups", "last-apply.json");
    private readonly RegistryService _registry = new();

    public Task<TweakBackup> CaptureAsync(IEnumerable<Tweak> tweaks) => Task.Run(() =>
    {
        var backup = new TweakBackup { TweakNames = tweaks.Select(t => t.Name).ToList() };
        var captured = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var command in tweaks.SelectMany(t => t.Commands))
        {
            if (command.Type == CommandType.Registry)
            {
                var parts = command.Command.Split('|');
                if (parts.Length < 4) { backup.UnsupportedCommandCount++; continue; }
                CaptureValue(backup, captured, parts[0], parts[1]);
            }
            else if (command.Type == CommandType.Service)
            {
                var parts = command.Command.Split('|');
                if (parts.Length < 2) { backup.UnsupportedCommandCount++; continue; }
                CaptureValue(backup, captured,
                    $"HKLM\\SYSTEM\\CurrentControlSet\\Services\\{parts[0]}", "Start");
            }
            else
            {
                backup.UnsupportedCommandCount++;
            }
        }
        return backup;
    });

    public void Save(TweakBackup backup)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(BackupPath)!);
        var temporaryPath = BackupPath + ".tmp";
        File.WriteAllText(temporaryPath, JsonSerializer.Serialize(backup, new JsonSerializerOptions { WriteIndented = true }));
        File.Move(temporaryPath, BackupPath, overwrite: true);
    }

    public bool HasBackup => File.Exists(BackupPath);

    public Task<(bool Success, int Restored, string Message)> RestoreLastAsync() => Task.Run(() =>
    {
        if (!File.Exists(BackupPath))
            return (false, 0, "No previous apply backup was found.");

        try
        {
            var backup = JsonSerializer.Deserialize<TweakBackup>(File.ReadAllText(BackupPath));
            if (backup is null) return (false, 0, "The previous apply backup could not be read.");
            var restored = 0;
            foreach (var entry in backup.RegistryEntries)
            {
                _registry.RestoreValue(entry);
                restored++;
            }
            return (true, restored, $"Restored {restored} saved setting(s) from {backup.CreatedAt:g}.");
        }
        catch (Exception ex)
        {
            return (false, 0, ex.Message);
        }
    });

    private void CaptureValue(TweakBackup backup, HashSet<string> captured, string path, string name)
    {
        var identity = $"{path}|{name}";
        if (!captured.Add(identity)) return;

        var entry = new RegistryBackupEntry { Path = path, Name = name };
        if (_registry.TryGetValue(path, name, out var value, out var kind))
        {
            entry.Existed = true;
            entry.Kind = kind;
            entry.Value = value switch
            {
                byte[] bytes => Convert.ToBase64String(bytes),
                string[] strings => string.Join('\n', strings),
                _ => Convert.ToString(value, System.Globalization.CultureInfo.InvariantCulture)
            };
        }
        backup.RegistryEntries.Add(entry);
    }
}
