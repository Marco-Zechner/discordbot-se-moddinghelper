using Discord.WebSocket;

namespace SE_Mod_Bot {
    public class InteractionHandler {
        public static async Task ButtonHandler(SocketMessageComponent component) {
            switch (component.Data.CustomId) {
                case "refresh-ticket":
                    await RefreshTicket(component);
                    break;
            }
        }

        private static async Task RefreshTicket(SocketMessageComponent component) {
            var originalEmbed = component.Message.Embeds.First();
            var (embed, comp, error) = await ResponseBuilder.KeenTicketResponse(originalEmbed.Url);

            if (error != null)
                await component.RespondAsync(error, ephemeral: true);
            else if (embed != null && comp != null) {
                await component.UpdateAsync(x => {
                    x.Embed = embed.Build();
                    x.Components = comp.Build();
                });
            }
            else
                await component.RespondAsync("Unexpected Null from embed or comp for refresh-ticket", ephemeral: true);
        }
    }
}
