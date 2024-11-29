using Discord;
using Discord.WebSocket;
using static SE_Mod_Bot.ProgramGlobal;


namespace SE_Mod_Bot {
    
    public class Program {
        private static ulong channelId_restart = 0;
        private static bool restarted = false;

        // Program entry point
        static async Task Main(string[] args) {
            
            Console.WriteLine("Start with args: " + string.Join(", ", args));

            for (int i = 0; i < args.Length; i++) {
                if (args[i] == "-restarted") {
                    channelId_restart = ulong.Parse(args[i + 1]);
                    restarted = true;
                }
            }

            client = new DiscordSocketClient(new DiscordSocketConfig {
                LogLevel = LogSeverity.Info,
            });
            client.Log += Debug.Log;
            client.Ready += Commands.Client_Ready;
            client.SlashCommandExecuted += CommandHandler.SlashCommandHandler;
            client.ButtonExecuted += InteractionHandler.ButtonHandler;

            await MainAsync();
        }

        private static async Task MainAsync() {

            await DataBase.LoadEnv("token.env");

            // Login and connect.
            await client.LoginAsync(TokenType.Bot, Environment.GetEnvironmentVariable("Discord_SE_Bot_Token"));
            await client.StartAsync();

            if (restarted) {
                var channel = await client.GetChannelAsync(channelId_restart);

                await ((ITextChannel)channel).SendMessageAsync("Bot restarted with version: " + GitHubHandler.GetVersion());
            }


            // Wait infinitely so your bot actually stays connected.
            await Task.Delay(Timeout.Infinite);
        }
    }
}
