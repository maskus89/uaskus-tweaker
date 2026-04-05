using System.Windows.Media;
using UaskusTweaks.Models;

namespace UaskusTweaks.ViewModels;

public class TweakViewModel : BaseViewModel
{
    private bool _isSelected;

    public Tweak Model { get; }

    public TweakViewModel(Tweak model) => Model = model;

    public string Id => Model.Id;
    public string Name => Model.Name;
    public string Description => Model.Description;
    public string Category => Model.Category;
    public RiskLevel RiskLevel => Model.RiskLevel;
    public bool IsAdminRequired => Model.IsAdminRequired;
    public bool RequiresRestart => Model.RequiresRestart;
    public string Tooltip => Model.Tooltip;

    public bool IsSelected
    {
        get => _isSelected;
        set => SetProperty(ref _isSelected, value);
    }

    public System.Windows.Media.Brush RiskColor => RiskLevel switch
    {
        RiskLevel.Low => new SolidColorBrush(Color.FromRgb(0x4a, 0xde, 0x80)),
        RiskLevel.Medium => new SolidColorBrush(Color.FromRgb(0xfa, 0xcc, 0x15)),
        RiskLevel.High => new SolidColorBrush(Color.FromRgb(0xf9, 0x73, 0x16)),
        RiskLevel.Critical => new SolidColorBrush(Color.FromRgb(0xdc, 0x26, 0x26)),
        _ => System.Windows.Media.Brushes.Gray
    };

    public string RiskText => RiskLevel switch
    {
        RiskLevel.Low => "LOW",
        RiskLevel.Medium => "MED",
        RiskLevel.High => "HIGH",
        RiskLevel.Critical => "CRIT",
        _ => "?"
    };
}
