namespace UaskusTweaks.Models;

public class SystemInfo
{
    public string OsVersion { get; set; } = string.Empty;
    public string OsBuild { get; set; } = string.Empty;
    public string CpuName { get; set; } = string.Empty;
    public int CpuCores { get; set; }
    public int CpuThreads { get; set; }
    public double CpuSpeedGHz { get; set; }
    public double RamTotalGB { get; set; }
    public double RamAvailableGB { get; set; }
    public double RamUsagePercent { get; set; }
    public string GpuName { get; set; } = string.Empty;
    public double GpuVramGB { get; set; }
    public double StorageFreeGB { get; set; }
    public double StorageTotalGB { get; set; }
    public string PowerPlan { get; set; } = string.Empty;
    public TimeSpan Uptime { get; set; }
    public string DisplayResolution { get; set; } = string.Empty;
    public int RefreshRateHz { get; set; }
}
