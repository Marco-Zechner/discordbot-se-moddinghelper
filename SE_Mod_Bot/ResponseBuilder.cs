using Discord;

namespace SE_Mod_Bot {
    public class ResponseBuilder {
        public static async Task<(EmbedBuilder? embed, ComponentBuilder? comp, string? error)> KeenTicketResponse(string keenUrl) {

            if (!keenUrl.StartsWith("https://support.keenswh.com/spaceengineers/pc/topic/")) {
                return (null, null, "please provide a valid keen report");
            }

            var (title, creator, status, votes) = await ExtractKeenInfo.GetKeenInfo(keenUrl);

            string description = $"Created by: {creator}" +
                $"\n\nUpvote the ticked!!" +
                $"\n{keenUrl}" +
                $"\n\nRefresh with Button below:" +
                $"\nStatus: {status}" +
                $"\nVotes: {votes}";

            if (int.TryParse(votes.Trim(), out int votesCount)) {
                if (votesCount < 50)
                    description += $"\n\nKeen need at **LEAST 50** Votes to consider a ticket." +
                        $"\nPlease VOTE! Only {50 - votesCount} more votes to reach the minimum.";
                else
                    description += $"\n\nWe **reached the minimum** Votes for Keen!" +
                        $"\nPlease continue to vote. The more Votes the better. :)";
            }

            var embedBuiler = new EmbedBuilder()
            .WithAuthor("Click here to open the browser", "https://external-content.duckduckgo.com/ip3/www.keenswh.com.ico", keenUrl)
            .WithUrl(keenUrl)
            .WithTitle(title)
            .WithDescription(description)
            .WithColor(Color.Red)
            .WithCurrentTimestamp();

            var builder = new ComponentBuilder()
                .WithButton("Refresh", "refresh-ticket");

            return (embedBuiler, builder, null);
        }
    }
}
