using System;
using System.Drawing;
using System.Diagnostics;
using System.Management;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace FRP_Tool_Pro
{
    public partial class MainForm : Form
    {
        // Цвета в стиле Dark Mode (как у профи софта)
        private Color primaryDark = Color.FromArgb(28, 28, 28);
        private Color accentBlue = Color.FromArgb(0, 122, 204);
        private RichTextBox logBox;
        private Label statusLabel;
        private Timer deviceTimer;

        public MainForm()
        {
            SetupUI();
            StartDeviceMonitor();
        }

        private void SetupUI()
        {
            this.Text = "Universal FRP & Tool Pro v1.0";
            this.Size = new Size(900, 600);
            this.BackColor = primaryDark;
            this.ForeColor = Color.White;
            this.Font = new Font("Segoe UI", 10);

            // Панель бокового меню
            Panel sidePanel = new Panel { Dock = DockStyle.Left, Width = 200, BackColor = Color.FromArgb(35, 35, 35) };
            this.Controls.Add(sidePanel);

            // Заголовок меню
            Label logo = new Label { Text = "FRP TOOL", Font = new Font("Segoe UI", 14, FontStyle.Bold), Dock = DockStyle.Top, Height = 60, TextAlign = ContentAlignment.MiddleCenter };
            sidePanel.Controls.Add(logo);

            // Кнопки управления (примеры)
            AddMenuButton(sidePanel, "MTP BYPASS", 60, (s, e) => RunMTPBypass());
            AddMenuButton(sidePanel, "ADB OPERATIONS", 110, (s, e) => RunADBUnlock());
            AddMenuButton(sidePanel, "FASTBOOT INFO", 160, (s, e) => RunFastbootInfo());
            AddMenuButton(sidePanel, "MTK / BROM", 210, (s, e) => Log("MTK Mode selected. Waiting for device..."));

            // Окно логов
            logBox = new RichTextBox { Dock = DockStyle.Right, Width = 680, BackColor = Color.Black, ForeColor = Color.Lime, Font = new Font("Consolas", 9), ReadOnly = true };
            this.Controls.Add(logBox);

            // Статус-бар снизу
            statusLabel = new Label { Dock = DockStyle.Bottom, Height = 30, Text = "Status: No Device Detected", BackColor = Color.FromArgb(45, 45, 48), TextAlign = ContentAlignment.MiddleLeft };
            this.Controls.Add(statusLabel);

            Log("Application started. Ready for work.");
        }

        private void AddMenuButton(Panel p, string text, int top, EventHandler action)
        {
            Button btn = new Button { Text = text, Top = top, Width = 180, Left = 10, Height = 40, FlatStyle = FlatStyle.Flat, BackColor = accentBlue };
            btn.FlatAppearance.BorderSize = 0;
            btn.Click += action;
            p.Controls.Add(btn);
        }

        // --- ЛОГИКА МОНИТОРИНГА ПОРТОВ ---
        private void StartDeviceMonitor()
        {
            deviceTimer = new Timer { Interval = 2000 };
            deviceTimer.Tick += (s, e) => {
                string mode = "No Device Detected";
                using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity"))
                {
                    foreach (var device in searcher.Get())
                    {
                        string name = device["Caption"]?.ToString().ToLower() ?? "";
                        if (name.Contains("adb")) mode = "MODE: ADB (Authorized)";
                        else if (name.Contains("bootloader") || name.Contains("fastboot")) mode = "MODE: FASTBOOT";
                        else if (name.Contains("mediatek") || name.Contains("preloader")) mode = "MODE: MTK BROM";
                        else if (name.Contains("samsung mobile usb modem")) mode = "MODE: SAMSUNG DOWNLOAD";
                        else if (name.Contains("9008")) mode = "MODE: QUALCOMM EDL 9008";
                    }
                }
                statusLabel.Text = "Status: " + mode;
                statusLabel.ForeColor = (mode.Contains("No")) ? Color.Red : Color.Lime;
            };
            deviceTimer.Start();
        }

        // --- ФУНКЦИИ ---
        private void Log(string message)
        {
            logBox.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}\n");
            logBox.ScrollToCaret();
        }

        private void RunMTPBypass()
        {
            Log("Starting MTP Browser Launch...");
            Log("Step 1: Connecting to MTP Port...");
            // Здесь вызывается твоя логика отправки MTP-пакета
            Log("Done! Check phone screen for popup.");
        }

        private void RunADBUnlock()
        {
            Log("Executing ADB FRP Bypass...");
            // Вызов adb.exe
            RunCmd("adb shell content insert --uri content://settings/secure --bind name:s:user_setup_complete --bind value:s:1");
            Log("Command sent. Device should skip Setup Wizard.");
        }

        private void RunFastbootInfo()
        {
            Log("Reading Fastboot Info...");
            RunCmd("fastboot getvar all");
        }

        private void RunCmd(string command)
        {
            try {
                ProcessStartInfo psi = new ProcessStartInfo("cmd.exe", "/c " + command) {
                    RedirectStandardOutput = true, UseShellExecute = false, CreateNoWindow = true
                };
                Process p = Process.Start(psi);
                string output = p.StandardOutput.ReadToEnd();
                Log(output);
            } catch (Exception ex) { Log("Error: " + ex.Message); }
        }
    }
}
