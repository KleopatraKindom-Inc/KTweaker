using System;
using System.Collections.Generic;
using System.Management;

namespace KleopatraKindomTweaker.Services;

public record SystemInfoSnapshot(
    string OsName,
    string OsVersion,
    string CpuName,
    int CpuCores,
    int CpuLogicalProcessors,
    double TotalRamGb,
    double AvailableRamGb,
    string GpuName,
    string MotherboardModel,
    IReadOnlyList<DiskInfo> Disks);

public record DiskInfo(string Drive, double TotalGb, double FreeGb);

public class SystemInfoService
{
    public SystemInfoSnapshot GetSnapshot()
    {
        string osName = "Unknown", osVersion = "", cpuName = "Unknown", gpu = "Unknown", board = "Unknown";
        int cores = 0, logical = 0;
        double totalRam = 0, availRam = 0;
        var disks = new List<DiskInfo>();

        try
        {
            using var osSearcher = new ManagementObjectSearcher("SELECT Caption, Version, FreePhysicalMemory, TotalVisibleMemorySize FROM Win32_OperatingSystem");
            foreach (var o in osSearcher.Get())
            {
                osName = o["Caption"]?.ToString()?.Trim() ?? osName;
                osVersion = o["Version"]?.ToString() ?? "";
                var totalKb = Convert.ToDouble(o["TotalVisibleMemorySize"] ?? 0);
                var freeKb = Convert.ToDouble(o["FreePhysicalMemory"] ?? 0);
                totalRam = Math.Round(totalKb / 1024 / 1024, 1);
                availRam = Math.Round(freeKb / 1024 / 1024, 1);
            }

            using var cpuSearcher = new ManagementObjectSearcher("SELECT Name, NumberOfCores, NumberOfLogicalProcessors FROM Win32_Processor");
            foreach (var c in cpuSearcher.Get())
            {
                cpuName = c["Name"]?.ToString()?.Trim() ?? cpuName;
                cores = Convert.ToInt32(c["NumberOfCores"] ?? 0);
                logical = Convert.ToInt32(c["NumberOfLogicalProcessors"] ?? 0);
            }

            using var gpuSearcher = new ManagementObjectSearcher("SELECT Name FROM Win32_VideoController");
            var gpuNames = new List<string>();
            foreach (var g in gpuSearcher.Get())
                if (g["Name"] is { } n) gpuNames.Add(n.ToString()!);
            gpu = gpuNames.Count > 0 ? string.Join(" / ", gpuNames) : gpu;

            using var boardSearcher = new ManagementObjectSearcher("SELECT Manufacturer, Product FROM Win32_BaseBoard");
            foreach (var b in boardSearcher.Get())
                board = $"{b["Manufacturer"]} {b["Product"]}".Trim();

            using var diskSearcher = new ManagementObjectSearcher("SELECT DeviceID, Size, FreeSpace FROM Win32_LogicalDisk WHERE DriveType=3");
            foreach (var d in diskSearcher.Get())
            {
                var totalB = Convert.ToDouble(d["Size"] ?? 0);
                var freeB = Convert.ToDouble(d["FreeSpace"] ?? 0);
                disks.Add(new DiskInfo(
                    d["DeviceID"]?.ToString() ?? "?",
                    Math.Round(totalB / 1024 / 1024 / 1024, 1),
                    Math.Round(freeB / 1024 / 1024 / 1024, 1)));
            }
        }
        catch
        {
            // WMI can be unavailable in locked-down environments; fall back to whatever we gathered.
        }

        return new SystemInfoSnapshot(osName, osVersion, cpuName, cores, logical, totalRam, availRam, gpu, board, disks);
    }
}
