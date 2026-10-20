using UaskusTweaks.Models;

namespace UaskusTweaks.Services;

public static class TweakSelectionValidator
{
    public static List<string> FindConflicts(IEnumerable<Tweak> selection)
    {
        var tweaks = selection.ToList();
        var conflicts = tweaks.Where(t => t.ExclusiveGroup is not null)
            .GroupBy(t => t.ExclusiveGroup, StringComparer.OrdinalIgnoreCase)
            .Where(group => group.Count() > 1)
            .Select(group => $"{group.Key}: {string.Join(", ", group.Select(t => t.Name))}")
            .ToList();
        var targets = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);
        foreach (var command in tweaks.SelectMany(t => t.Commands))
        {
            var parts = command.Command.Split('|');
            string? target = null, value = null;
            if (command.Type == CommandType.Registry && parts.Length == 4)
            {
                target = $"{parts[0]}\\{parts[1]}";
                value = $"{parts[2]}|{parts[3]}";
            }
            else if (command.Type == CommandType.Service && parts.Length == 2)
            {
                target = $"Service:{parts[0]}";
                value = parts[1].ToLowerInvariant();
            }
            if (target is null || value is null) continue;
            if (!targets.TryGetValue(target, out var values))
                targets[target] = values = new HashSet<string>(StringComparer.Ordinal);
            values.Add(value);
        }
        conflicts.AddRange(targets.Where(pair => pair.Value.Count > 1).Select(pair => pair.Key));
        return conflicts;
    }
}
