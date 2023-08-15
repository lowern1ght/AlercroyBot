using Serilog.Core;
using AlercroyBot.Application.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Timer = AlercroyBot.Application.Entity.Timer;

namespace AlercroyBot.Application.Services;

public class TimerService : ITimerService
{
    private readonly Logger _logger;
    private static List<Timer> Timers { get; } = new();
    
    private readonly ReaderWriterLockSlim _readerWriterLockSlim = new ReaderWriterLockSlim();
    
    public TimerService(Logger logger)
    {
        _logger = logger;
    }
    
    public Task StartTimerAsync(long chatId, TimeSpan duration, ITelegramBotClient botClient)
    {
        new Thread(() => StartTimerWatcher(ref chatId, ref duration, botClient)).Start();
        return Task.CompletedTask;
    }

    private void StartTimerWatcher(ref long chatId, ref TimeSpan duration, ITelegramBotClient botClient)
    {
        _readerWriterLockSlim.TryEnterWriteLock(100);

        try
        {
            var timer = new Timer(chatId, ref duration);
            
            Timers.Add(timer);
            
            _readerWriterLockSlim.ExitWriteLock();

            _logger.Information("Timer {Duration} started for user {Username}", duration, chatId);
            
            botClient.SendTextMessageAsync(
                chatId, 
                $"Timer **{duration:g}** started",
                parseMode: ParseMode.Markdown, 
                cancellationToken: new CancellationToken())
                .GetAwaiter().GetResult();
            
            Thread.Sleep(duration);
            
            _logger.Information("Timer {Duration} is end for user {Username}", duration, chatId);
            
            botClient.SendTextMessageAsync(
                chatId, 
                $"Timer **{duration:g}** is end",
                parseMode: ParseMode.Markdown, 
                cancellationToken: new CancellationToken())
                .GetAwaiter().GetResult();;

            _readerWriterLockSlim.TryEnterWriteLock(100);

            Timers.Remove(timer);
        }
        finally
        {
            _readerWriterLockSlim.ExitWriteLock();
        }
        
    }

    public async Task<IEnumerable<Timer>?> GetTimersListAsync(long? chatId)
    {
        _readerWriterLockSlim.TryEnterReadLock(100);
        
        try
        {
            if (!chatId.HasValue)
            {
                return Timers;
            }

            return Timers.Where(timer => timer.ChatId == chatId);
        }
        finally
        {
            _readerWriterLockSlim.ExitReadLock();
        }
    }
}