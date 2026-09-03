using System.Windows.Media;
using UaskusTweaks.Models;

namespace UaskusTweaks.ViewModels;

public class TweakViewModel : BaseViewModel
{
    private bool _isSelected;
    private System.Windows.Media.Brush? _riskColor;
    private TweakState _state = TweakState.Unknown;

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

    public TweakState State
    {
        get => _state;
        set
        {
            if (SetProperty(ref _state, value))
            {
                OnPropertyChanged(nameof(StateText));
                OnPropertyChanged(nameof(StateColor));
            }
        }
    }

    public string StateText => State switch
    {
        TweakState.Enabled => "ALREADY ON",
        TweakState.PartiallyEnabled => "PARTLY ON",
        TweakState.NotEnabled => "NOT ON",
        _ => "UNKNOWN"
    };

    public Brush StateColor => State switch
    {
        TweakState.Enabled => new SolidColorBrush(Color.FromRgb(0x22, 0xc5, 0x5e)),
        TweakState.PartiallyEnabled => new SolidColorBrush(Color.FromRgb(0xf5, 0x9e, 0x0b)),
        TweakState.NotEnabled => new SolidColorBrush(Color.FromRgb(0x60, 0xa5, 0xfa)),
        _ => new SolidColorBrush(Color.FromRgb(0x60, 0x60, 0x80))
    };

    public bool IsSelected
    {
        get => _isSelected;
        set => SetProperty(ref _isSelected, value);
    }

    public System.Windows.Media.Brush RiskColor
    {
        get
        {
            _riskColor ??= RiskLevel switch
            {
                RiskLevel.Low => new SolidColorBrush(Color.FromRgb(0x4a, 0xde, 0x80)),
                RiskLevel.Medium => new SolidColorBrush(Color.FromRgb(0xfa, 0xcc, 0x15)),
                RiskLevel.High => new SolidColorBrush(Color.FromRgb(0xf9, 0x73, 0x16)),
                RiskLevel.Critical => new SolidColorBrush(Color.FromRgb(0xdc, 0x26, 0x26)),
                _ => System.Windows.Media.Brushes.Gray
            };
            return _riskColor;
        }
    }

    public string RiskText => RiskLevel switch
    {
        RiskLevel.Low => "SAFE",
        RiskLevel.Medium => "CAUTION",
        RiskLevel.High => "HIGH",
        RiskLevel.Critical => "EXTREME",
        _ => "?"
    };
}
