using System;
using System.Management;

public class DeviceMonitor {
    public string GetCurrentMode() {
        var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity WHERE Caption LIKE '%Android%' OR Caption LIKE '%Samsung%' OR Caption LIKE '%MediaTek%'");
        foreach (var device in searcher.Get()) {
            string name = device["Caption"].ToString().ToLower();
            if (name.Contains("adb")) return "ADB MODE";
            if (name.Contains("bootloader")) return "FASTBOOT MODE";
            if (name.Contains("modem")) return "SAMSUNG DOWNLOAD";
            if (name.Contains("9008")) return "QUALCOMM EDL";
        }
        return "DISCONNECTED";
    }
}
