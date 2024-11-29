using Discord;
using Discord.WebSocket;

namespace SE_Mod_Bot {
    
    public class Program {
        // Program entry point
        static async Task Main(string[] args) {
            Console.WriteLine("Start with args: " + string.Join(", ", args));
            
            ProgramGlobal.client = new DiscordSocketClient(new DiscordSocketConfig {
                LogLevel = LogSeverity.Info,
            });
            ProgramGlobal.client.Log += Debug.Log;
            ProgramGlobal.client.Ready += Commands.Client_Ready;
            ProgramGlobal.client.SlashCommandExecuted += CommandHandler.SlashCommandHandler;
            ProgramGlobal.client.ButtonExecuted += InteractionHandler.ButtonHandler;

            await MainAsync();
        }

        private static async Task MainAsync() {

            await DataBase.LoadEnv("token.env");

            // Login and connect.
            await ProgramGlobal.client.LoginAsync(TokenType.Bot, Environment.GetEnvironmentVariable("Discord_SE_Bot_Token"));
            await ProgramGlobal.client.StartAsync();

            // Wait infinitely so your bot actually stays connected.
            await Task.Delay(Timeout.Infinite);
        }
    }
}
