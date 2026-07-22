using Microsoft.Win32;

namespace UaskusTweaks.Services;

public class RegistryService
{
    public bool TryGetValue(string keyPath, string valueName, out object? value, out RegistryValueKind kind)
    {
        var (hive, subKey) = ParsePath(keyPath);
        using var key = hive.OpenSubKey(subKey, writable: false);
        if (key is null || !key.GetValueNames().Contains(valueName, StringComparer.OrdinalIgnoreCase))
        {
            value = null;
            kind = default;
            return false;
        }

        value = key.GetValue(valueName, null, RegistryValueOptions.DoNotExpandEnvironmentNames);
        kind = key.GetValueKind(valueName);
        return value is not null;
    }

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
        if (path.StartsWith("HKLM\\", StringComparison.OrdinalIgnoreCase))
        {
            return (Registry.LocalMachine, path[5..]);
        }
        if (path.StartsWith("HKEY_LOCAL_MACHINE\\", StringComparison.OrdinalIgnoreCase))
        {
            return (Registry.LocalMachine, path[19..]);
        }
        if (path.StartsWith("HKCU\\", StringComparison.OrdinalIgnoreCase))
        {
            return (Registry.CurrentUser, path[5..]);
        }
        if (path.StartsWith("HKEY_CURRENT_USER\\", StringComparison.OrdinalIgnoreCase))
        {
            return (Registry.CurrentUser, path[18..]);
        }
        if (path.StartsWith("HKCR\\", StringComparison.OrdinalIgnoreCase))
        {
            return (Registry.ClassesRoot, path[5..]);
        }
        if (path.StartsWith("HKEY_CLASSES_ROOT\\", StringComparison.OrdinalIgnoreCase))
        {
            return (Registry.ClassesRoot, path[18..]);
        }
        if (path.StartsWith("HKU\\", StringComparison.OrdinalIgnoreCase))
        {
            return (Registry.Users, path[4..]);
        }
        if (path.StartsWith("HKEY_USERS\\", StringComparison.OrdinalIgnoreCase))
        {
            return (Registry.Users, path[10..]);
        }
        throw new ArgumentException($"Unknown registry hive in path: {path}");
    }
}
