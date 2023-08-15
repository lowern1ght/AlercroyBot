using Telegram.Bot;
using Timer = AlercroyBot.Application.Entity.Timer;

namespace AlercroyBot.Application.Interfaces;

public interface ITimerService
{
    Task<IEnumerable<Timer>?> GetTimersListAsync(Int64? chatId);
    Task StartTimerAsync(Int64 chatId, TimeSpan duration, ITelegramBotClient botClient);
}