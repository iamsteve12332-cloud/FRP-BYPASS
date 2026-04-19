using System;
using System.Diagnostics;

public class AdbService {
    public string Execute(string args) {
        try {
            ProcessStartInfo psi = new ProcessStartInfo {
                FileName = "platform-tools/adb.exe",
                Arguments = args,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            using (Process p = Process.Start(psi)) {
                return p.StandardOutput.ReadToEnd();
            }
        } catch { return "ADB not found!"; }
    }

    public void RemoveFRP() {
        Execute("shell content insert --uri content://settings/secure --bind name:s:user_setup_complete --bind value:s:1");
        Execute("shell am start -n com.google.android.gsf.login/com.google.android.gsf.login.LoginActivity");
    }
}
