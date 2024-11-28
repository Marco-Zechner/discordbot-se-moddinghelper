using Discord.Commands;
using Discord;

namespace SE_Mod_Bot {
    public class Debug {
        public static Task Log(LogMessage message) {
            switch (message.Severity) {
                case LogSeverity.Critical:
                case LogSeverity.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case LogSeverity.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case LogSeverity.Info:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case LogSeverity.Verbose:
                case LogSeverity.Debug:
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    break;
            }

            if (message.Exception is CommandException cmdException) {
                Console.WriteLine($"{DateTime.Now,-19} [Command/{message.Severity,-8}] {cmdException.Command.Aliases[0]}" +
                    $" failed to execute in {cmdException.Context.Channel}\n{cmdException}");
            }
            else {
                Console.WriteLine($"{DateTime.Now,-19} [General/{message.Severity,-8}] {message.Source}: {message.Message} {message.Exception}");
            }

            Console.ResetColor();

            return Task.CompletedTask;
        }
    }
}
