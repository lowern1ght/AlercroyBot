using System.Globalization;
using Telegram.Bot.Types;

namespace AlercroyBot.Modules;

public static class CommandsHandler
{
    public static async Task TimerAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken, 
        Serilog.ILogger logger)
    {
        await Task.CompletedTask;
    }

    public static async Task TimerCommandAsync(ITelegramBotClient botClient, Update update,
        CancellationToken cancellationToken, Serilog.ILogger logger)
    {
        logger.Information("Accepted 'timer' command, [{updateId}]", update.Id);

        var spitedCommand = update.Message.Text.Split(" ");

        if (spitedCommand.Length <= 1)
        {
            throw new ArgumentException("/timer arguments is bad");
        }

        if (TimeSpan.TryParse(spitedCommand[1], CultureInfo.InvariantCulture, out var span))
        {
            logger.Information("Start timer to {user}, on duration: {duration:h:mm:ss tt zz}", update.Message.Chat.Username, span);
            var timerService = new TimerService(span, update.Message.Chat.Id, botClient, null, logger);
        }
        else
        {
            throw new FormatException("bad format duration, example: /timer (12:20, 20:11:11, 32)");
        }
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