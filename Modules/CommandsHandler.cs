using Telegram.Bot.Types;

namespace AlercroyBot.Modules;

public static class CommandsHandler
{
    public static async Task TimerAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken, 
        Serilog.ILogger logger)
    {
        await Task.CompletedTask;
    }

    public static async Task HelpCommandAsync(ITelegramBotClient botClient, Update update,
        CancellationToken cancellationToken, Serilog.ILogger logger)
    {
        logger.Information("Accepted 'help' command, [{updateId}]", update.Id);
    }
    
    public static async Task UnknownCommandAsync(ITelegramBotClient botClient, Update update,
        CancellationToken cancellationToken, Serilog.ILogger logger)
    {
        logger.Error("Unknown command '{message}', [{updateId}]", update.Message.Text, update.Id);
    }
}