using HtmlAgilityPack;

namespace SE_Mod_Bot {
    public class ExtractKeenInfo {

        public static async Task<(string title, string creator, string status, string votes)> GetKeenInfo(string url) {
            string title, creator, status, votes;

            try {
                var htmlContent = await GetHtmlContent(url);

                (title, creator, status, votes) = ExtractStringsFromHtml(htmlContent);
            }
            catch (Exception ex) {
                title = "Error";
                creator = "N/A";
                status = ex.Message;
                votes = "-1";
            }

            return (title, creator, status, votes);
        }

        private static async Task<string> GetHtmlContent(string url) {
            using HttpClient client = new();
            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        // Method to extract the desired strings from the HTML
        private static (string title, string creator, string status, string votes) ExtractStringsFromHtml(string html) {
            HtmlDocument document = new();
            document.LoadHtml(html);

            // Locate the div with class "title-object-full"
            var titleDiv = document.DocumentNode.SelectSingleNode("//div[normalize-space(@class)='title-object-full']")
                ?? throw new Exception("Div with class 'title-object-full' not found.");

            // Extract strings
            string title = titleDiv.SelectSingleNode(".//h1[@itemprop='name']")?.InnerText.Trim() ?? "N/A";
            string creator = titleDiv.SelectSingleNode(".//span[@itemprop='creator']")?.InnerText.Trim() ?? "N/A";
            string status = titleDiv.SelectSingleNode(".//span[@class='']")?.InnerText.Trim() ?? "N/A";

            var buttonsDiv = document.DocumentNode.SelectSingleNode("//div[normalize-space(@class)='buttons-object-full']") 
                ?? throw new Exception("Div with class 'buttons-object-full' not found.");

            // Extract string from "buttons-object-full"
            string votes = buttonsDiv.SelectSingleNode(".//span[normalize-space(@class)='how-vote']")?.InnerText.Trim() ?? "N/A";

            return (title, creator, status, votes);
        }
    }
}
