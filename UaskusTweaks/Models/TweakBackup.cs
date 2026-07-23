using Microsoft.Win32;

namespace UaskusTweaks.Models;

public sealed class TweakBackup
{
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public List<string> TweakNames { get; set; } = new();
    public List<RegistryBackupEntry> RegistryEntries { get; set; } = new();
    public int UnsupportedCommandCount { get; set; }
}

public sealed class RegistryBackupEntry
{
    public string Path { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool Existed { get; set; }
    public RegistryValueKind Kind { get; set; }
    public string? Value { get; set; }
}
