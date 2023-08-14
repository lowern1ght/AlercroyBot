using Serilog.Core;
using AlercroyBot.Application.Interfaces;
using Timer = AlercroyBot.Application.Entity.Timer;

namespace AlercroyBot.Application.Services;

public class TimerService : ITimerService
{
    private readonly Logger _logger;
    private List<Timer> Timers { get; } = new List<Timer>();
    
    private readonly ReaderWriterLockSlim _readerWriterLockSlim = new ReaderWriterLockSlim();
    
    public TimerService(Logger logger)
    {
        _logger = logger;
    }
    
    public Task StartTimerAsync(long chatId, TimeSpan duration)
    {
        _readerWriterLockSlim.TryEnterWriteLock(100);

        try
        {
            var timer = new Timer(chatId, duration);
            
            this.Timers.Add(timer);
            
            _readerWriterLockSlim.ExitWriteLock();

            _logger.Information("Timer {Duration} started for user {Username}", duration, chatId);
            
            Task.Delay(duration);

            _readerWriterLockSlim.TryEnterWriteLock(100);

            this.Timers.Remove(timer);
        }
        finally
        {
            _readerWriterLockSlim.ExitWriteLock();
        }
        
        return Task.CompletedTask;
    }

    public async Task<IEnumerable<Timer>?> GetTimersListAsync(long? chatId)
    {
        _readerWriterLockSlim.TryEnterReadLock(100);
        
        try
        {
            if (!chatId.HasValue)
            {
                return this.Timers;
            }

            return this.Timers.Where(timer => timer.ChatId == chatId);
        }
        finally
        {
            _readerWriterLockSlim.ExitReadLock();
        }
    }
}