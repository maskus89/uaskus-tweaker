using System.Diagnostics;
using System.IO;
using System.Management;
using UaskusTweaks.Models;

namespace UaskusTweaks.Services;

public class SystemInfoService
{
    public async Task<SystemInfo> GetSystemInfoAsync()
        => await Task.Run(Gather);

    private static SystemInfo Gather()
    {
        var info = new SystemInfo();

        try
        {
            using var osSearch = new ManagementObjectSearcher("SELECT Caption, BuildNumber, Version FROM Win32_OperatingSystem");
            foreach (ManagementObject obj in osSearch.Get())
            {
                info.OsVersion = obj["Caption"]?.ToString() ?? string.Empty;
                info.OsBuild = obj["BuildNumber"]?.ToString() ?? string.Empty;
            }
        }
        catch { info.OsVersion = Environment.OSVersion.VersionString; }

        try
        {
            using var cpuSearch = new ManagementObjectSearcher("SELECT Name, NumberOfCores, NumberOfLogicalProcessors, MaxClockSpeed FROM Win32_Processor");
            foreach (ManagementObject obj in cpuSearch.Get())
            {
                info.CpuName = obj["Name"]?.ToString()?.Trim() ?? string.Empty;
                info.CpuCores = Convert.ToInt32(obj["NumberOfCores"] ?? 0);
                info.CpuThreads = Convert.ToInt32(obj["NumberOfLogicalProcessors"] ?? 0);
                var mhz = Convert.ToDouble(obj["MaxClockSpeed"] ?? 0);
                info.CpuSpeedGHz = Math.Round(mhz / 1000.0, 2);
            }
        }
        catch { info.CpuName = "Unknown"; }

        try
        {
            using var ramSearch = new ManagementObjectSearcher("SELECT TotalPhysicalMemory, FreePhysicalMemory FROM Win32_ComputerSystem");
            foreach (ManagementObject obj in ramSearch.Get())
            {
                var total = Convert.ToDouble(obj["TotalPhysicalMemory"] ?? 0);
                info.RamTotalGB = Math.Round(total / (1024 * 1024 * 1024), 1);
            }

            using var osSearch2 = new ManagementObjectSearcher("SELECT FreePhysicalMemory FROM Win32_OperatingSystem");
            foreach (ManagementObject obj in osSearch2.Get())
            {
                var freeKb = Convert.ToDouble(obj["FreePhysicalMemory"] ?? 0);
                info.RamAvailableGB = Math.Round(freeKb / (1024 * 1024), 1);
            }

            if (info.RamTotalGB > 0)
                info.RamUsagePercent = Math.Round((1 - info.RamAvailableGB / info.RamTotalGB) * 100, 1);
        }
        catch { }

        try
        {
            using var gpuSearch = new ManagementObjectSearcher("SELECT Name, AdapterRAM FROM Win32_VideoController");
            foreach (ManagementObject obj in gpuSearch.Get())
            {
                var name = obj["Name"]?.ToString() ?? string.Empty;
                if (!string.IsNullOrWhiteSpace(name) && string.IsNullOrWhiteSpace(info.GpuName))
                {
                    info.GpuName = name;
                    var vram = Convert.ToDouble(obj["AdapterRAM"] ?? 0);
                    info.GpuVramGB = Math.Round(vram / (1024 * 1024 * 1024), 1);
                }
            }
        }
        catch { info.GpuName = "Unknown"; }

        try
        {
            var drive = DriveInfo.GetDrives().FirstOrDefault(d => d.IsReady && d.DriveType == DriveType.Fixed);
            if (drive != null)
            {
                info.StorageTotalGB = Math.Round(drive.TotalSize / (1024.0 * 1024 * 1024), 1);
                info.StorageFreeGB = Math.Round(drive.AvailableFreeSpace / (1024.0 * 1024 * 1024), 1);
            }
        }
        catch { }

        try
        {
            var uptimeMs = Environment.TickCount64;
            info.Uptime = TimeSpan.FromMilliseconds(uptimeMs);
        }
        catch { }

        try
        {
            var psi = new ProcessStartInfo("powercfg", "/getactivescheme")
            {
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            using var proc = Process.Start(psi);
            if (proc != null)
            {
                var output = proc.StandardOutput.ReadToEnd();
                proc.WaitForExit();
                // Parse: "Power Scheme GUID: ... (scheme name)"
                var match = System.Text.RegularExpressions.Regex.Match(output, @"\((.+)\)");
                info.PowerPlan = match.Success ? match.Groups[1].Value.Trim() : "Unknown";
            }
        }
        catch { info.PowerPlan = "Unknown"; }

        try
        {
            using var screenSearch = new ManagementObjectSearcher("SELECT CurrentHorizontalResolution, CurrentVerticalResolution, CurrentRefreshRate FROM Win32_VideoController");
            foreach (ManagementObject obj in screenSearch.Get())
            {
                var w = Convert.ToInt32(obj["CurrentHorizontalResolution"] ?? 0);
                var h = Convert.ToInt32(obj["CurrentVerticalResolution"] ?? 0);
                var hz = Convert.ToInt32(obj["CurrentRefreshRate"] ?? 0);
                if (w > 0 && h > 0)
                {
                    info.DisplayResolution = $"{w}x{h}";
                    info.RefreshRateHz = hz;
                    break;
                }
            }
        }
        catch { info.DisplayResolution = "Unknown"; }

        return info;
    }
}
