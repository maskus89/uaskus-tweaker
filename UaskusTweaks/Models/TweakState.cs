namespace UaskusTweaks.Models;

/// <summary>The degree to which the current machine matches a tweak's intended settings.</summary>
public enum TweakState
{
    Unknown,
    Enabled,
    PartiallyEnabled,
    NotEnabled
}
