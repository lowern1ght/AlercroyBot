using Timer = AlercroyBot.Application.Entity.Timer;

namespace AlercroyBot.Application.Interfaces;

public interface ITimerService
{
    Task StartTimerAsync(Int64 chatId, TimeSpan duration);
    Task<IEnumerable<Timer>?> GetTimersListAsync(Int64? chatId);
}