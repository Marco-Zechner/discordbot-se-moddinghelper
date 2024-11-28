using Discord;
using Discord.Net;
using Newtonsoft.Json;

namespace SE_Mod_Bot {
    public class Commands {
        public static async Task Client_Ready() {
            var guild = ProgramGlobal.client.GetGuild(ProgramGlobal.guildId);

            var adoptCommand = new SlashCommandBuilder()
                .WithName("adopt-mod")
                .WithDescription("Mod-Idea will be marked as adopted, and you get forum where you can make a post for this mod.");

            var keenBugMod = new SlashCommandBuilder()
                .WithName("keen-ticket")
                .WithDescription("Mod-Idea will be marked with keen-ticket.")
                .AddOption("keen-ticket-url", ApplicationCommandOptionType.String, "https://support.keenswh.com/spaceengineers/pc/topic/12345", isRequired: true);

            //var globalCommand = new SlashCommandBuilder();
            //globalCommand.WithName("first-global-command");
            //globalCommand.WithDescription("This is my first global slash command");

            try {
                await guild.CreateApplicationCommandAsync(adoptCommand.Build());
                await guild.CreateApplicationCommandAsync(keenBugMod.Build());

                //await ProgramGlobal.client.CreateGlobalApplicationCommandAsync(globalCommand.Build());
            }
            catch (HttpException exception) {
                var json = JsonConvert.SerializeObject(exception.Errors, Formatting.Indented);

                Console.WriteLine(json);
            }
        }
    }
}
