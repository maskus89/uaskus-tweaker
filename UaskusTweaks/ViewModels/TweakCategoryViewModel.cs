using System.Collections.ObjectModel;
using System.Windows.Input;

namespace UaskusTweaks.ViewModels;

public class TweakCategoryViewModel : BaseViewModel
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;

    public ObservableCollection<TweakViewModel> Tweaks { get; } = new();

    public ICommand SelectAllCommand { get; }
    public ICommand DeselectAllCommand { get; }

    public int SelectedCount => Tweaks.Count(t => t.IsSelected);
    public int TotalCount => Tweaks.Count;

    public TweakCategoryViewModel()
    {
        SelectAllCommand = new RelayCommand(_ =>
        {
            foreach (var t in Tweaks) t.IsSelected = true;
            NotifyCounts();
        });
        DeselectAllCommand = new RelayCommand(_ =>
        {
            foreach (var t in Tweaks) t.IsSelected = false;
            NotifyCounts();
        });
    }

    public void NotifyCounts()
    {
        OnPropertyChanged(nameof(SelectedCount));
        OnPropertyChanged(nameof(TotalCount));
    }
}
