namespace SE_Mod_Bot {
    public class DataBase {

        public static async Task<string> ReadFile(string fileName) {
            string dataDir = Environment.GetEnvironmentVariable("DATA_DIR") ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data");
            string filePath = Path.Combine(dataDir, fileName);

            if (!File.Exists(filePath)) {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{DateTime.Now,-19} File '{fileName}' at {filePath} not found.");
                Console.ResetColor();
                return "";
            }

            return await File.ReadAllTextAsync(filePath);
        }        
        
        public static async Task WriteFile(string fileName, string content) {
            string dataDir = Environment.GetEnvironmentVariable("DATA_DIR") ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data");
            string filePath = Path.Combine(dataDir, fileName);

            if (!File.Exists(filePath)) {
                File.WriteAllText(filePath, "");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"{DateTime.Now,-19} File '{fileName}' created at {filePath}");
                Console.ResetColor();
            }

            await File.WriteAllTextAsync(filePath, content);
        }

        private static async Task<string> ReadFromConfig(string fileName) {
            string configDir = Environment.GetEnvironmentVariable("CONFIG_DIR") ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config");

            string filePath = Path.Combine(configDir, fileName);

            if (!File.Exists(filePath)) {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{DateTime.Now,-19} File '{fileName}' at {filePath} not found.");
                Console.ResetColor();
                return "";
            }

            return await File.ReadAllTextAsync(filePath);
        }

        public static async Task LoadEnv(string fileName) {
            var lines = (await ReadFromConfig(fileName)).Split('\n');
            foreach (var line in lines) {
                var parts = line.Split('=', 2);
                if (parts.Length == 2) {
                    Environment.SetEnvironmentVariable(parts[0], parts[1]);
                }
            }
        }

        public static async Task SaveRestartArgs(ulong channelID, string branch) {
            string fileName = "config.env";
            string configDir = Environment.GetEnvironmentVariable("CONFIG_DIR") ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config");

            string filePath = Path.Combine(configDir, fileName);

            if (!File.Exists(filePath)) {
                File.WriteAllText(filePath, "");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"{DateTime.Now,-19} File '{fileName}' created at {filePath}");
                Console.ResetColor();
            }

            string content = $"BRANCH={branch}                   # The GitHub branch to pull\n" +
                $"ARGS=-restarted {channelID}                 # Arguments for the .NET console app";

            await File.WriteAllTextAsync(filePath, content);
        }
    }
}
