using AlercroyBot.Application.Interfaces;
using Serilog.Core;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace AlercroyBot.Application.Commands;

[CommandDescription("help", description: "write help information")]
public class HelpCommand : ICommandAsync
{
    private readonly Logger _logger;

    public HelpCommand(Logger logger)
    {
        _logger = logger;
    }

    private const String TextMessage 
        = @"- /**help**: write help to use this bot
            - /**timer:** set timer on time (s, m, h, d)
            - /**timers:** get ur timers list";

    public async Task ExecuteAsync(ITelegramBotClient botClient, Update update, string command, string[] arguments,
        CancellationToken token)
    {
        if (update.Message?.Chat.Id != null)
            await botClient.SendTextMessageAsync(
                update.Message!.Chat.Id,
                TextMessage, 
                parseMode: ParseMode.Markdown,
                cancellationToken: token);
    }
}