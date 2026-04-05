using Microsoft.Win32;

namespace UaskusTweaks.Services;

public class RegistryService
{
    public void SetValue(string keyPath, string valueName, object value, RegistryValueKind kind)
    {
        var (hive, subKey) = ParsePath(keyPath);
        using var key = hive.CreateSubKey(subKey, writable: true)
            ?? throw new InvalidOperationException($"Cannot open/create registry key: {keyPath}");
        key.SetValue(valueName, value, kind);
    }

    public void DeleteValue(string keyPath, string valueName)
    {
        var (hive, subKey) = ParsePath(keyPath);
        using var key = hive.OpenSubKey(subKey, writable: true);
        key?.DeleteValue(valueName, throwOnMissingValue: false);
    }

    public void CreateKey(string keyPath)
    {
        var (hive, subKey) = ParsePath(keyPath);
        using var key = hive.CreateSubKey(subKey, writable: true);
    }

    public void BackupKey(string keyPath, string backupPath)
    {
        // reg export is handled outside; this is a placeholder hook
        var (hive, subKey) = ParsePath(keyPath);
        using var key = hive.OpenSubKey(subKey);
        if (key == null) return;
        // Deep copy of values into a simple text file for basic backup
        using var writer = new System.IO.StreamWriter(backupPath, append: true);
        writer.WriteLine($"[{keyPath}]");
        foreach (var name in key.GetValueNames())
        {
            var v = key.GetValue(name);
            var vk = key.GetValueKind(name);
            writer.WriteLine($"\"{name}\"={vk}:{v}");
        }
    }

    private static (RegistryKey hive, string subKey) ParsePath(string path)
    {
        if (path.StartsWith("HKLM\\", StringComparison.OrdinalIgnoreCase) ||
            path.StartsWith("HKEY_LOCAL_MACHINE\\", StringComparison.OrdinalIgnoreCase))
        {
            var sub = path.Substring(path.IndexOf('\\') + 1);
            return (Registry.LocalMachine, sub);
        }
        if (path.StartsWith("HKCU\\", StringComparison.OrdinalIgnoreCase) ||
            path.StartsWith("HKEY_CURRENT_USER\\", StringComparison.OrdinalIgnoreCase))
        {
            var sub = path.Substring(path.IndexOf('\\') + 1);
            return (Registry.CurrentUser, sub);
        }
        if (path.StartsWith("HKCR\\", StringComparison.OrdinalIgnoreCase) ||
            path.StartsWith("HKEY_CLASSES_ROOT\\", StringComparison.OrdinalIgnoreCase))
        {
            var sub = path.Substring(path.IndexOf('\\') + 1);
            return (Registry.ClassesRoot, sub);
        }
        if (path.StartsWith("HKU\\", StringComparison.OrdinalIgnoreCase) ||
            path.StartsWith("HKEY_USERS\\", StringComparison.OrdinalIgnoreCase))
        {
            var sub = path.Substring(path.IndexOf('\\') + 1);
            return (Registry.Users, sub);
        }
        throw new ArgumentException($"Unknown registry hive in path: {path}");
    }
}
