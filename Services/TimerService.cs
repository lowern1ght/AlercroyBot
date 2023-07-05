using System.Globalization;
using Telegram.Bot.Types;

namespace AlercroyBot.Services;

public class TimerService
{
    private static Object LockRemoveObject { get; set; } = new object();

    private static List<TimeServiceInfo> TimeServiceInfos { get; set; } = new List<TimeServiceInfo>();
    
    private long ChatId { get; }
    private TimeSpan Duration { get; }
    private ITelegramBotClient Client { get; }
    private ILogger? Logger { get; }
    
    private TimeServiceInfo ServiceInfo { get; set; }
    
    public TimerService(TimeSpan duration, Int64 chatId, ITelegramBotClient client, Action<String>? actionOnUnSleep, Serilog.ILogger? logger = null)
    {
        this.ChatId = chatId;
        this.Client = client;
        this.Logger = logger;
        this.Duration = duration;

        ServiceInfo = new TimeServiceInfo { Duration  = Duration, ChatId = ChatId, BeginDateTime = DateTime.Now };
        
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

    public static async Task GetHowTimeIsLeftTimers(Int64 chatId)
    {
        
        
        
        
    }

    public static async Task<TimeSpan> ParseCommandLineToDurationAsync(String[] strings)
    {
        TimeSpan result = TimeSpan.Zero;
        foreach (var s in strings)
        {

            if (s.Contains("s") && 
                UInt16.TryParse(s.Replace("s", ""), NumberStyles.Integer, CultureInfo.InvariantCulture, out var second))
            {
                result.Add(TimeSpan.FromSeconds(second));
            }
            
            if (s.Contains("m") && 
                UInt16.TryParse(s.Replace("m", ""), NumberStyles.Integer, CultureInfo.InvariantCulture, out var minute))
            {
                result.Add(TimeSpan.FromMinutes(minute));
            }
            
            if (s.Contains("h") && 
                UInt16.TryParse(s.Replace("h", ""), NumberStyles.Integer, CultureInfo.InvariantCulture, out var hour))
            {
                result.Add(TimeSpan.FromHours(hour));
            }
            
            if (s.Contains("d") && 
                UInt16.TryParse(s.Replace("d", ""), NumberStyles.Integer, CultureInfo.InvariantCulture, out var days))
            {
                result.Add(TimeSpan.FromDays(days));
            }
        }

        return result;
    }

    public static Boolean TryParseCommandLineToDuration(String[] strings, out TimeSpan? span)
    {
        try
        {
            span = ParseCommandLineToDurationAsync(strings).GetAwaiter().GetResult();
            return true;
        }
        catch (Exception e)
        {
            span = null;
        }

        return false;
    }
    
    private struct TimeServiceInfo
    {
        public Guid Guid { get; init; }
        public Int64 ChatId { get; init; }
        public TimeSpan Duration { get; init; }
        public DateTime BeginDateTime { get; init; }

        public TimeServiceInfo(long chatId, TimeSpan duration, DateTime beginDateTime)
        {
            this.Guid = Guid.NewGuid();
            this.ChatId = chatId;
            this.Duration = duration;
            this.BeginDateTime = beginDateTime;
        }
    }
}