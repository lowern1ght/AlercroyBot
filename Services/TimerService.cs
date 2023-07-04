using Telegram.Bot.Types;

namespace AlercroyBot.Services;

public class TimerService
{
    public long ChatId { get; }
    public TimeSpan Duration { get; }
    public ITelegramBotClient Client { get; }
    public ILogger? Logger { get; }

    public TimerService(TimeSpan duration, Int64 chatId, ITelegramBotClient client, Action<String>? actionOnUnSleep, Serilog.ILogger? logger = null)
    {
        this.Duration = duration;
        this.ChatId = chatId;
        this.Client = client;
        this.Logger = logger;
        
        ActivityTimerHandler(actionOnUnSleep);
    }

    private async Task ActivityTimerHandler(Action<String>? actionForUnSleep)
    {
        Thread.Sleep(Duration);

        actionForUnSleep ??= message 
            => { this.Client.SendTextMessageAsync(this.ChatId, message); };

        String message = $"Time {Duration:dd\\.hh\\:mm\\:ss} is over, [{ChatId}]";
        actionForUnSleep.Invoke(message);
        
        Logger?.Information("Timer service duration {duration} is end [{chatId}]", Duration, ChatId);
    }
}