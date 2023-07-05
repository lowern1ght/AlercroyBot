using Telegram.Bot.Types;

namespace AlercroyBot.Services;

public class TimerService
{
    private long ChatId { get; }
    private TimeSpan Duration { get; }
    private ITelegramBotClient Client { get; }
    private ILogger? Logger { get; }

    public TimerService(TimeSpan duration, Int64 chatId, ITelegramBotClient client, Action<String>? actionOnUnSleep, Serilog.ILogger? logger = null)
    {
        this.Duration = duration;
        this.ChatId = chatId;
        this.Client = client;
        this.Logger = logger;

        var activityTimerHandler = ActivityTimerHandler(actionOnUnSleep);
        activityTimerHandler.Start();
    }

    private async Task ActivityTimerHandler(Action<String>? actionForUnSleep)
    {
        Thread.Sleep(Duration);

        actionForUnSleep ??= message 
            => { this.Client.SendTextMessageAsync(this.ChatId, message); };

        String message = $"Time {Duration:dd\\.hh\\:mm\\:ss} is over, [{ChatId}]";
        actionForUnSleep.Invoke(message);
        
        Logger?.Information("Timer service duration {duration} is end [{chatId}]", Duration, ChatId);
        await Task.CompletedTask;
    }
}