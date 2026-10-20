using UaskusTweaks.Models;

// Data-only adapters let the production catalog run on macOS/Linux too.
// WPF rendering and actual view-model bindings are covered by --smoke-test on Windows.
namespace UaskusTweaks.ViewModels;

public sealed class TweakViewModel
{
    public Tweak Model { get; }
    public TweakViewModel(Tweak model) => Model = model;
}

public sealed class TweakCategoryViewModel
{
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public string Icon { get; set; } = "";
    public List<TweakViewModel> Tweaks { get; } = new();
    public void NotifyCounts() { }
}
