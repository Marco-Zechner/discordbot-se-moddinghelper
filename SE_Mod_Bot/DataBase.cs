namespace SE_Mod_Bot {
    public class DataBase {

        public static async Task<string> ReadFile(string fileName) {
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);

            if (!File.Exists(filePath)) {
                File.WriteAllText(filePath, "");
                Console.WriteLine($"File '{fileName}' created at {filePath}");
            }

            return await File.ReadAllTextAsync(filePath);
        }        
        
        public static async Task WriteFile(string fileName, string content) {
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);

            if (!File.Exists(filePath)) {
                File.WriteAllText(filePath, "");
                Console.WriteLine($"File '{fileName}' created at {filePath}");
            }

            await File.WriteAllTextAsync(filePath, content);
        }
    }
}
