using AlercroyBot.Application.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace AlercroyBot.Application.Commands;

[CommandDescription("timers", "get active timers for this chat")]
public class TimersCommand : ICommandAsync
{
    public Task ExecuteAsync(ITelegramBotClient botClient, Update update, string command, 
        string[] arguments, CancellationToken token)
    {
        return Task.CompletedTask;
    }
}