using UaskusTweaks.Models;
using UaskusTweaks.Services;

namespace UaskusTweaks.ViewModels;

public class SystemInfoViewModel : BaseViewModel
{
    private readonly SystemInfoService _service = new();
    private SystemInfo _info = new();
    
    private string _osVersion = string.Empty;
    private string _osBuild = string.Empty;
    private string _cpuName = string.Empty;
    private int _cpuCores;
    private int _cpuThreads;
    private double _cpuSpeedGHz;
    private double _ramTotalGB;
    private double _ramAvailableGB;
    private double _ramUsagePercent;
    private string _gpuName = string.Empty;
    private double _gpuVramGB;
    private double _storageFreeGB;
    private double _storageTotalGB;
    private string _powerPlan = string.Empty;
    private TimeSpan _uptime;
    private string _displayResolution = string.Empty;
    private int _refreshRateHz;

    public AsyncRelayCommand RefreshCommand { get; }

    public SystemInfoViewModel()
    {
        RefreshCommand = new AsyncRelayCommand(async () => await RefreshAsync());
    }

    public async Task RefreshAsync()
    {
        _info = await _service.GetSystemInfoAsync();
        OsVersion = _info.OsVersion;
        OsBuild = _info.OsBuild;
        CpuName = _info.CpuName;
        CpuCores = _info.CpuCores;
        CpuThreads = _info.CpuThreads;
        CpuSpeedGHz = _info.CpuSpeedGHz;
        RamTotalGB = _info.RamTotalGB;
        RamAvailableGB = _info.RamAvailableGB;
        RamUsagePercent = _info.RamUsagePercent;
        GpuName = _info.GpuName;
        GpuVramGB = _info.GpuVramGB;
        StorageFreeGB = _info.StorageFreeGB;
        StorageTotalGB = _info.StorageTotalGB;
        PowerPlan = _info.PowerPlan;
        Uptime = _info.Uptime;
        DisplayResolution = _info.DisplayResolution;
        RefreshRateHz = _info.RefreshRateHz;
    }

    public string OsVersion { get => _osVersion; set => SetProperty(ref _osVersion, value); }
    public string OsBuild { get => _osBuild; set => SetProperty(ref _osBuild, value); }
    public string CpuName { get => _cpuName; set => SetProperty(ref _cpuName, value); }
    public int CpuCores { get => _cpuCores; set => SetProperty(ref _cpuCores, value); }
    public int CpuThreads { get => _cpuThreads; set => SetProperty(ref _cpuThreads, value); }
    public double CpuSpeedGHz { get => _cpuSpeedGHz; set => SetProperty(ref _cpuSpeedGHz, value); }
    public double RamTotalGB { get => _ramTotalGB; set => SetProperty(ref _ramTotalGB, value); }
    public double RamAvailableGB { get => _ramAvailableGB; set => SetProperty(ref _ramAvailableGB, value); }
    public double RamUsagePercent { get => _ramUsagePercent; set => SetProperty(ref _ramUsagePercent, value); }
    public string GpuName { get => _gpuName; set => SetProperty(ref _gpuName, value); }
    public double GpuVramGB { get => _gpuVramGB; set => SetProperty(ref _gpuVramGB, value); }
    public double StorageFreeGB { get => _storageFreeGB; set => SetProperty(ref _storageFreeGB, value); }
    public double StorageTotalGB { get => _storageTotalGB; set => SetProperty(ref _storageTotalGB, value); }
    public string PowerPlan { get => _powerPlan; set => SetProperty(ref _powerPlan, value); }
    public TimeSpan Uptime { get => _uptime; set => SetProperty(ref _uptime, value); }
    public string DisplayResolution { get => _displayResolution; set => SetProperty(ref _displayResolution, value); }
    public int RefreshRateHz { get => _refreshRateHz; set => SetProperty(ref _refreshRateHz, value); }

    public string RamDisplay =>
        $"{_ramAvailableGB:F1} GB / {_ramTotalGB:F1} GB ({_ramUsagePercent:F0}% used)";

    public string CpuDisplay =>
        $"{_cpuCores}C / {_cpuThreads}T  {_cpuSpeedGHz:F2} GHz";

    public string StorageDisplay =>
        $"{_storageFreeGB:F1} GB free / {_storageTotalGB:F1} GB";

    public string UptimeDisplay
    {
        get
        {
            var u = _uptime;
            return $"{(int)u.TotalHours}h {u.Minutes}m {u.Seconds}s";
        }
    }
}
