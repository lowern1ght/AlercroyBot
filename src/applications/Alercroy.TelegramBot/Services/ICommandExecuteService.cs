using Telegram.Bot.Types;

namespace Alercroy.TelegramBot.Services;

public interface ICommandExecuteService
{
    public Task ExecuteCommand(string command, IServiceProvider serviceProvider, Update update, CancellationToken token);
}