using UaskusTweaks.Models;
using UaskusTweaks.Services;

namespace UaskusTweaks.ViewModels;

public class SystemInfoViewModel : BaseViewModel
{
    private readonly SystemInfoService _service = new();
    private SystemInfo _info = new();

    public RelayCommand RefreshCommand { get; }

    public SystemInfoViewModel()
    {
        RefreshCommand = new RelayCommand(async () => await RefreshAsync());
    }

    public async Task RefreshAsync()
    {
        _info = await _service.GetSystemInfoAsync();
        OnPropertyChanged(string.Empty); // refresh all
    }

    public string OsVersion => _info.OsVersion;
    public string OsBuild => _info.OsBuild;
    public string CpuName => _info.CpuName;
    public int CpuCores => _info.CpuCores;
    public int CpuThreads => _info.CpuThreads;
    public double CpuSpeedGHz => _info.CpuSpeedGHz;
    public double RamTotalGB => _info.RamTotalGB;
    public double RamAvailableGB => _info.RamAvailableGB;
    public double RamUsagePercent => _info.RamUsagePercent;
    public string GpuName => _info.GpuName;
    public double GpuVramGB => _info.GpuVramGB;
    public double StorageFreeGB => _info.StorageFreeGB;
    public double StorageTotalGB => _info.StorageTotalGB;
    public string PowerPlan => _info.PowerPlan;
    public TimeSpan Uptime => _info.Uptime;
    public string DisplayResolution => _info.DisplayResolution;
    public int RefreshRateHz => _info.RefreshRateHz;

    public string RamDisplay =>
        $"{_info.RamAvailableGB:F1} GB / {_info.RamTotalGB:F1} GB ({_info.RamUsagePercent:F0}% used)";

    public string CpuDisplay =>
        $"{_info.CpuCores}C / {_info.CpuThreads}T  {_info.CpuSpeedGHz:F2} GHz";

    public string StorageDisplay =>
        $"{_info.StorageFreeGB:F1} GB free / {_info.StorageTotalGB:F1} GB";

    public string UptimeDisplay
    {
        get
        {
            var u = _info.Uptime;
            return $"{(int)u.TotalHours}h {u.Minutes}m {u.Seconds}s";
        }
    }
}
