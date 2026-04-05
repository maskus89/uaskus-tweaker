using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace UaskusTweaks.Models;

public class Tweak : INotifyPropertyChanged
{
    private bool _isEnabled;

    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public RiskLevel RiskLevel { get; set; } = RiskLevel.Low;
    public bool IsAdminRequired { get; set; }
    public bool RequiresRestart { get; set; }
    public string Tooltip { get; set; } = string.Empty;
    public List<TweakCommand> Commands { get; set; } = new();

    public bool IsEnabled
    {
        get => _isEnabled;
        set
        {
            if (_isEnabled != value)
            {
                _isEnabled = value;
                OnPropertyChanged();
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
