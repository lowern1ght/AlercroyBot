using Telegram.Bot.Types;

namespace Alercroy.TelegramBot.Commands;

public interface ICommand
{
    Task ExecuteAsync(Update update, CancellationToken token);
}