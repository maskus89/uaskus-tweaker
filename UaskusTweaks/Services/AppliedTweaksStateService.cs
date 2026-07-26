using System.IO;
using System.Text.Json;

namespace UaskusTweaks.Services;

public sealed class AppliedTweaksStateService
{
    private static readonly string StatePath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "UaskusTweaks",
        "applied-tweaks.json");

    public HashSet<string> LoadAppliedTweakIds()
    {
        if (!File.Exists(StatePath))
            return new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        try
        {
            var ids = JsonSerializer.Deserialize<List<string>>(File.ReadAllText(StatePath)) ?? [];
            return new HashSet<string>(ids.Where(id => !string.IsNullOrWhiteSpace(id)), StringComparer.OrdinalIgnoreCase);
        }
        catch
        {
            return new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        }
    }

    public void SaveAppliedTweakIds(IEnumerable<string> tweakIds)
    {
        var normalized = tweakIds
            .Where(id => !string.IsNullOrWhiteSpace(id))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(id => id, StringComparer.OrdinalIgnoreCase)
            .ToList();

        Directory.CreateDirectory(Path.GetDirectoryName(StatePath)!);
        var temporaryPath = StatePath + ".tmp";
        File.WriteAllText(temporaryPath, JsonSerializer.Serialize(normalized, new JsonSerializerOptions { WriteIndented = true }));
        File.Move(temporaryPath, StatePath, overwrite: true);
    }
}
