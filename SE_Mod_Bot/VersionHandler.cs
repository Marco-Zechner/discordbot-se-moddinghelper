using System.Diagnostics;

namespace SE_Mod_Bot {
    public class VersionHandler {
        public static string GetVersion() {
            var processInfo = new ProcessStartInfo {
                FileName = "git",
                Arguments = "describe --tags",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(processInfo);
            
            return process?.StandardOutput.ReadToEnd().Trim() ?? "Unknown version";
        }
    }
}
