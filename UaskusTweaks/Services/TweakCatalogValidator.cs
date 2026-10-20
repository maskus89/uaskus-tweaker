using Microsoft.Win32;
using UaskusTweaks.Models;

namespace UaskusTweaks.Services;

public static class TweakCatalogValidator
{
    public static List<string> Validate(IEnumerable<Tweak> catalog)
    {
        var tweaks = catalog.ToList();
        var errors = new List<string>();
        foreach (var group in tweaks.GroupBy(t => t.Id, StringComparer.OrdinalIgnoreCase).Where(g => g.Count() > 1))
            errors.Add($"Duplicate tweak ID: {group.Key}");
        foreach (var group in tweaks.GroupBy(t => string.Join("\n",
                     t.Commands.Select(c => $"{c.Type}:{c.Command}").OrderBy(c => c, StringComparer.Ordinal)))
                 .Where(g => g.Count() > 1))
            errors.Add($"Duplicate tweak commands: {string.Join(", ", group.Select(t => t.Id))}");

        foreach (var tweak in tweaks)
        {
            if (string.IsNullOrWhiteSpace(tweak.Id) || string.IsNullOrWhiteSpace(tweak.Category) || tweak.Commands.Count == 0)
                errors.Add($"{tweak.Id}: missing ID, category, or commands.");
            foreach (var command in tweak.Commands)
            {
                var error = ValidateCommand(command);
                if (error is not null) errors.Add($"{tweak.Id}: {error}");
            }
        }
        return errors;
    }

    public static string? ValidateCommand(TweakCommand command)
    {
        if (!Enum.IsDefined(command.Type) || string.IsNullOrWhiteSpace(command.Command))
            return "Unknown command type or empty command.";
        var parts = command.Command.Split('|');
        if (command.Type == CommandType.Registry)
        {
            if (parts.Length != 4 || !Enum.TryParse<RegistryValueKind>(parts[3], out var kind) ||
                !Enum.IsDefined(kind) || kind is RegistryValueKind.Unknown or RegistryValueKind.None)
                return "Invalid registry command format or kind.";
            if (!new[] { "HKLM\\", "HKCU\\", "HKCR\\", "HKU\\", "HKEY_LOCAL_MACHINE\\",
                    "HKEY_CURRENT_USER\\", "HKEY_CLASSES_ROOT\\", "HKEY_USERS\\" }
                .Any(prefix => parts[0].StartsWith(prefix, StringComparison.OrdinalIgnoreCase)))
                return "Unknown registry hive.";
            try
            {
                if (kind == RegistryValueKind.DWord) _ = int.Parse(parts[2], System.Globalization.CultureInfo.InvariantCulture);
                if (kind == RegistryValueKind.QWord) _ = long.Parse(parts[2], System.Globalization.CultureInfo.InvariantCulture);
                if (kind == RegistryValueKind.Binary) _ = Convert.FromBase64String(parts[2]);
            }
            catch (Exception ex) when (ex is FormatException or OverflowException)
            {
                return $"Invalid registry value: {ex.Message}";
            }
        }
        if (command.Type == CommandType.Service &&
            (parts.Length != 2 || string.IsNullOrWhiteSpace(parts[0]) ||
             !new[] { "auto", "demand", "disabled", "boot", "system" }.Contains(parts[1].ToLowerInvariant())))
            return "Invalid service name or startup type.";
        return null;
    }
}
