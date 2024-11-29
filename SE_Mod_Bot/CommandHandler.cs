using Discord;
using Discord.WebSocket;
using System.ComponentModel;

namespace SE_Mod_Bot {
    public class CommandHandler {
        private const ulong MOD_IDEAS_ID = 1311284523069083679;
        private const string NOT_MOD_IDEAS = "This Command only works for posts in Forum: https://discord.com/channels/1311081154316603452/1311284523069083679";
        private const ulong KEEN_TICKET_TAG_ID = 1311285272079761409;
        private const ulong ADOPTED_TAG_ID = 1311285445547524176;
        private const ulong MOD_CATEGORY_ID = 1311284411991461888;
        private const string MODDER_FILE = "modder.txt";

        public static async Task SlashCommandHandler(SocketSlashCommand command) {
            switch (command.Data.Name) {
                case "keen-ticket":
                    await HandleKeenTicketCommand(command);
                    break;
                case "adopt-mod":
                    await HandleAdoptModCommand(command);
                    break;
                case "version":
                    await HandleVersionCommand(command);
                    break;
                case "restart":
                    await HandleRestartCommand(command);
                    break;
                default:
                    await command.RespondAsync($"You executed {command.Data.Name}. No behaviour found.");
                    break;
            }
        }

        private static async Task HandleRestartCommand(SocketSlashCommand command) {
            await command.DeferAsync();
            
            string branch = "main";
            if (command.Data.Options.Count != 0)
                branch = (string)command.Data.Options.First().Value;

            if (!await GitHubHandler.CheckBranchExistsAsync("Marco-Zechner", "discordbot-se-moddinghelper", branch)) {
                await command.ModifyOriginalResponseAsync(msg => {
                    msg.Content = $"Can't find branch: {branch}. Bot NOT restarting.";
                });
            }

            await command.ModifyOriginalResponseAsync(msg => {
                msg.Content = $"Restarting bot with {branch} now, please wait for a moment";
            });

            var channel = (IThreadChannel)command.Channel;

            await DataBase.SaveRestartArgs(channel.Id, branch);
            
            Environment.Exit(1);
        }

        private static async Task HandleVersionCommand(SocketSlashCommand command) {
            await command.RespondAsync("Currently Running Version: " + GitHubHandler.GetVersion());
        }

        private static async Task HandleAdoptModCommand(SocketSlashCommand command) {
            if (command.Channel.GetChannelType() is not ChannelType.PublicThread) {
                await command.RespondAsync(NOT_MOD_IDEAS, ephemeral: true);
                return;
            }

            IThreadChannel channel = (IThreadChannel)command.Channel;
            var id = channel.CategoryId;
            if (id != MOD_IDEAS_ID) {
                await command.RespondAsync(NOT_MOD_IDEAS, ephemeral: true);
                return;
            }

            if (command.Channel is SocketThreadChannel { ParentChannel: SocketForumChannel forumChannel } threadChannel && forumChannel.Tags.Count != 0) {
                var tagToApply = forumChannel.Tags.FirstOrDefault(f => f.Id == ADOPTED_TAG_ID);
                if (tagToApply != null) {
                    await threadChannel.ModifyAsync(m => m.AppliedTags = new List<ulong> { tagToApply.Id });
                }
            }

            string content = await DataBase.ReadFile(MODDER_FILE);
            int foundModder = -1;
            bool foundMod = false;
            List<string> lines = [.. content.Split('\n')];
            for (int i = 0; i < lines.Count; i++) {
                string line = lines[i];
                string modderSet = line.Split(':')[0];
                if (modderSet.Split('=')[0] == command.User.ToString()) {
                    foundModder = i;
                    foreach (string modSet in line.Split(':')[1].Split(';', StringSplitOptions.RemoveEmptyEntries)) {
                        if (modSet == command.ChannelId.ToString()) {
                            foundMod = true;
                        }
                    }
                    break;
                }
            }

            ulong modForumId = 0;
            if (foundModder == -1) {
                await channel.Guild.CreateNewsChannelAsync(command.User.Username + "-news", n => {
                    n.CategoryId = MOD_CATEGORY_ID;
                });
                IForumChannel newChannel = await channel.Guild.CreateForumChannelAsync(command.User.Username + "-mods", f => {
                    f.CategoryId = MOD_CATEGORY_ID;
                });
                lines.Add($"{command.User}={newChannel.Id}:");
                foundModder = lines.Count-1;
                modForumId = newChannel.Id;
            }
            else {
                modForumId = ulong.Parse(lines[foundModder].Split(':')[0].Split('=')[1]);
            }

            IForumChannel forum = await channel.Guild.GetForumChannelAsync(modForumId);
            if (foundMod == false && forum != null) {
                await forum.CreatePostAsync(command.Channel.Name, text: $"Created from: https://discord.com/channels/1311081154316603452/{command.ChannelId}");
                lines[foundModder] += $"{command.ChannelId};";
            }

            await DataBase.WriteFile(MODDER_FILE, string.Join('\n', lines));

            await command.ModifyOriginalResponseAsync(m => m.Content = $"foundModder: {foundModder}, foundMod: {foundMod}");
        }

        private static async Task HandleKeenTicketCommand(SocketSlashCommand command) {
            if (command.Channel.GetChannelType() is not ChannelType.PublicThread) {
                await command.RespondAsync(NOT_MOD_IDEAS, ephemeral: true);
                return;
            }

            IThreadChannel channel = (IThreadChannel)command.Channel;
            var id = channel.CategoryId;
            if (id != MOD_IDEAS_ID) {
                await command.RespondAsync(NOT_MOD_IDEAS, ephemeral: true);
                return;
            }

            if (command.Channel is SocketThreadChannel { ParentChannel: SocketForumChannel forumChannel } threadChannel && forumChannel.Tags.Count != 0) {
                var tagToApply = forumChannel.Tags.FirstOrDefault(f => f.Id == KEEN_TICKET_TAG_ID);
                if (tagToApply != null) {
                    await threadChannel.ModifyAsync(m => m.AppliedTags = new List<ulong> { tagToApply.Id });
                }
            }

            var keenUrl = (string)command.Data.Options.First().Value;

            var (embed, comp, error) = await ResponseBuilder.KeenTicketResponse(keenUrl);

            if (error != null)
                await command.RespondAsync(error, ephemeral: true);
            else if (embed != null && comp != null) {
                await command.RespondAsync(embed: embed.Build(), components: comp.Build());
            }
            else
                await command.RespondAsync("Unexpected Null from embed or comp for refresh-ticket", ephemeral: true);
        }
    }
}
