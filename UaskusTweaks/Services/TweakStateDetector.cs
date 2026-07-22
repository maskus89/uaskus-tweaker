using Microsoft.Win32;
using UaskusTweaks.Models;

namespace UaskusTweaks.Services;

/// <summary>
/// Checks declarative tweaks without making any changes. Registry and service commands
/// can be checked reliably; one-shot commands (for example flush DNS) deliberately
/// report as unavailable rather than guessing.
/// </summary>
public sealed class TweakStateDetector
{
    private readonly RegistryService _registry = new();

    public Task<TweakState> CheckAsync(Tweak tweak) => Task.Run(() => Check(tweak));

    private TweakState Check(Tweak tweak)
    {
        var checkedCommands = 0;
        var matches = 0;

        foreach (var command in tweak.Commands)
        {
            bool? result;
            try
            {
                result = command.Type switch
                {
                    CommandType.Registry => CheckRegistry(command),
                    CommandType.Service => CheckService(command),
                    _ => null
                };
            }
            catch (Exception)
            {
                // A protected or unavailable key should not prevent the rest of
                // the list from being inspected.
                result = null;
            }

            if (!result.HasValue)
                continue;

            checkedCommands++;
            if (result.Value)
                matches++;
        }

        if (checkedCommands == 0)
            return TweakState.Unknown;

        return matches == checkedCommands && checkedCommands == tweak.Commands.Count
            ? TweakState.Enabled
            : matches > 0
                ? TweakState.PartiallyEnabled
                : TweakState.NotEnabled;
    }

    private bool? CheckRegistry(TweakCommand command)
    {
        var parts = command.Command.Split('|');
        if (parts.Length < 4 || !Enum.TryParse<RegistryValueKind>(parts[3], out var kind))
            return null;

        if (!_registry.TryGetValue(parts[0], parts[1], out var actual, out var actualKind) || actualKind != kind)
            return false;

        try
        {
            object expected = kind switch
            {
                RegistryValueKind.DWord => int.Parse(parts[2], System.Globalization.CultureInfo.InvariantCulture),
                RegistryValueKind.QWord => long.Parse(parts[2], System.Globalization.CultureInfo.InvariantCulture),
                RegistryValueKind.Binary => Convert.FromBase64String(parts[2]),
                RegistryValueKind.MultiString => parts[2].Split('\n'),
                _ => parts[2]
            };
            return ValuesMatch(actual, expected);
        }
        catch (FormatException)
        {
            return null;
        }
    }

    private bool? CheckService(TweakCommand command)
    {
        var parts = command.Command.Split('|');
        if (parts.Length < 2)
            return null;

        var expected = parts[1].ToLowerInvariant() switch
        {
            "boot" => 0,
            "system" => 1,
            "auto" => 2,
            "demand" => 3,
            "disabled" => 4,
            _ => -1
        };
        if (expected < 0)
            return null;

        var path = $"HKLM\\SYSTEM\\CurrentControlSet\\Services\\{parts[0]}";
        return _registry.TryGetValue(path, "Start", out var actual, out _) &&
               Convert.ToInt32(actual, System.Globalization.CultureInfo.InvariantCulture) == expected;
    }

    private static bool ValuesMatch(object actual, object expected) =>
        actual is byte[] actualBytes && expected is byte[] expectedBytes
            ? actualBytes.SequenceEqual(expectedBytes)
            : actual is string[] actualStrings && expected is string[] expectedStrings
                ? actualStrings.SequenceEqual(expectedStrings, StringComparer.OrdinalIgnoreCase)
                : Equals(actual, expected);
}
