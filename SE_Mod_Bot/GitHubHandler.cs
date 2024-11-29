using System.Diagnostics;

namespace SE_Mod_Bot {
    public class GitHubHandler {
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

        public static async Task<bool> CheckBranchExistsAsync(string owner, string repo, string branch) {
            using HttpClient client = new();
            // GitHub API endpoint
            string url = $"https://api.github.com/repos/{owner}/{repo}/branches/{branch}";

            // Set User-Agent header (required by GitHub API)
            client.DefaultRequestHeaders.Add("User-Agent", "CSharpApp");

            try {
                // Send GET request
                HttpResponseMessage response = await client.GetAsync(url);

                // Return true if the branch exists (HTTP 200), otherwise false
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex) {
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }
        }
    }
}
