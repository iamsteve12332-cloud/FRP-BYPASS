using System;
using System.Management; // Нужно добавить ссылку на System.Management в NuGet
using System.Windows.Forms;

public class PortEngine {
    public enum DeviceMode { None, MTP, ADB, Fastboot, MTK_BROM, Download, EDL }

    public DeviceMode CurrentMode { get; private set; }

    public void CheckDevice(Label statusLabel) {
        string searcherQuery = "SELECT * FROM Win32_PnPEntity WHERE Caption LIKE '%(COM%)' OR Caption LIKE '%Android%' OR Caption LIKE '%Samsung%'";
        using (var searcher = new ManagementObjectSearcher(searcherQuery)) {
            var devices = searcher.Get();
            CurrentMode = DeviceMode.None;

            foreach (var device in devices) {
                string name = device["Caption"].ToString().ToLower();

                if (name.Contains("adb")) CurrentMode = DeviceMode.ADB;
                else if (name.Contains("bootloader") || name.Contains("fastboot")) CurrentMode = DeviceMode.Fastboot;
                else if (name.Contains("mediatek") || name.Contains("preloader")) CurrentMode = DeviceMode.MTK_BROM;
                else if (name.Contains("samsung mobile usb modem")) CurrentMode = DeviceMode.Download;
                else if (name.Contains("9008")) CurrentMode = DeviceMode.EDL;
                else if (name.Contains("mtp")) CurrentMode = DeviceMode.MTP;
            }
        }
        UpdateStatusUI(statusLabel);
    }

    private void UpdateStatusUI(Label lbl) {
        lbl.Text = "Status: " + CurrentMode.ToString();
        lbl.ForeColor = (CurrentMode == DeviceMode.None) ? System.Drawing.Color.Red : System.Drawing.Color.Green;
    }
}
