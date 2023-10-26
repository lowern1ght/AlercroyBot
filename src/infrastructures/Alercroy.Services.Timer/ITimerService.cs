namespace Alercroy.Services.Timer;

public interface ITimerService
{
    Task<Models.Timer> TimerById(Guid timerId, CancellationToken token);
    Task<IEnumerable<Models.Timer>> TimersByChat(ulong chatId, CancellationToken token);

    Task DeleteTimer(Guid timerId, CancellationToken token);
    Task AddTimer(Models.Timer timer, CancellationToken token);
}