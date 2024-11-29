using Discord;
using Discord.Net;
using Discord.WebSocket;
using Newtonsoft.Json;

namespace SE_Mod_Bot {
    public class Commands {
        public static async Task Client_Ready() {
            var factory = new CommandFactory();

            SetUpCommands(factory);

            try {
                await factory.CreateCommands();
            }
            catch (HttpException exception) {
                var json = JsonConvert.SerializeObject(exception.Errors, Formatting.Indented);

                Console.WriteLine(json);
            }
        }

        private static void SetUpCommands(CommandFactory factory) {
            factory.Add("adopt-mod")
                .WithDescription("Mod-Idea will be marked as adopted, and you get forum where you can make a post for this mod.");

            factory.Add("keen-ticket")
                .WithDescription("Mod-Idea will be marked with keen-ticket.")
                .AddOption("keen-ticket-url", ApplicationCommandOptionType.String, "https://support.keenswh.com/spaceengineers/pc/topic/12345", isRequired: true);

            factory.Add("version")
                .WithDescription("Returns the current semantic version of the bot.");

            factory.Add("restart")
                .WithDescription("Exits the Console App, and docker will restart it.");
        }

        private class CommandFactory {
            private readonly SocketGuild guild;
            private readonly List<SlashCommandBuilder> commands;
            public CommandFactory() {
                this.guild = ProgramGlobal.client.GetGuild(ProgramGlobal.guildId);
                this.commands = [];
            }

            public SlashCommandBuilder Add(string name) {
                var command = new SlashCommandBuilder().WithName(name);
                commands.Add(command);
                return command;
            }

            public async Task CreateCommands() {
                foreach (var command in commands) {
                    await guild.CreateApplicationCommandAsync(command.Build());
                }
            }
        }
    }
}
