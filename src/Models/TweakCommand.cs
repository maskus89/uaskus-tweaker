namespace UaskusTweaks.Models;

public class TweakCommand
{
    public CommandType Type { get; set; }
    public string Command { get; set; } = string.Empty;
    public string? Description { get; set; }
}
