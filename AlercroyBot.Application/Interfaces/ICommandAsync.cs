using Telegram.Bot;
using Telegram.Bot.Types;

namespace AlercroyBot.Application.Interfaces;

public interface ICommandAsync
{
    public Task ExecuteAsync(ITelegramBotClient botClient, Update update,
        String command, String[] arguments, CancellationToken token);
}