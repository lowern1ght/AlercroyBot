using System.Globalization;
using System.Text;
using Newtonsoft.Json.Linq;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace AlercroyBot.Services;

public class TimerService
{
    private Guid Guid { get; init; } = Guid.NewGuid();
    private static Object LockTimeServicesObject { get; set; } = new object();
    private static List<TimerService> TimerServices { get; set; } = new List<TimerService>();
    private long ChatId { get; }
    private TimeSpan Duration { get; }
    private DateTime BeginDuration { get; set; }
    private ITelegramBotClient Client { get; }
    private ILogger? Logger { get; }

    public TimerService(TimeSpan duration, Int64 chatId, ITelegramBotClient client, Action<String>? actionOnUnSleep, Serilog.ILogger? logger = null)
    {
        ChatId = chatId;
        Client = client;
        Logger = logger;
        Duration = duration;
        BeginDuration = DateTime.Now;

        async void ThreadStart()
        {
            await ActivityTimerHandler(null);
        }

        Thread threadDuration 
            = new Thread(new ThreadStart(ThreadStart));
        
        threadDuration.Start();
    }

    private async Task ActivityTimerHandler(Action<String>? actionForUnSleep = null)
    {
        BeginDuration = DateTime.Now;

        lock (LockTimeServicesObject)
        {
            TimerServices.Add(this);
        }

        await Client.SendTextMessageAsync(ChatId, 
            $"**timer** on {Duration} started...", parseMode: ParseMode.Markdown);
        
        Thread.Sleep(Duration);

        actionForUnSleep ??= message 
            => { this.Client.SendTextMessageAsync(this.ChatId, message); };

        String message = $"Time {Duration:dd\\.hh\\:mm\\:ss} is over, [{ChatId}]";
        actionForUnSleep.Invoke(message);
        
        Logger?.Information("Timer service duration {duration} is end [{chatId}]", Duration, ChatId);

        lock (LockTimeServicesObject)
        {
            TimerServices.Remove(this);
        }
    }

    public static async Task GetHowTimeIsLeftTimers(ITelegramBotClient botClient, Int64 chatId)
    {
        var timersThisUser = TimerServices
            .Where(service => service.ChatId == chatId).ToArray();

        if (timersThisUser.Any())
        {
            StringBuilder builder = new StringBuilder();
            builder.Append($"timers **count: {timersThisUser.Length}** \n");

            foreach (var service in timersThisUser)
            {
                var leftIs = (service.BeginDuration + service.Duration) - DateTime.Now;
                builder.Append($"timer duration: (**{service.Duration:g}**) remained: {leftIs}\n");
            }

            await botClient.SendTextMessageAsync(chatId, builder.ToString(), parseMode: ParseMode.Markdown);
        }
        else
        {
            await botClient.SendTextMessageAsync(chatId, "You not have a **timers**", parseMode: ParseMode.Markdown);
        }
    }

    public static async Task<TimeSpan> ParseCommandLineToDurationAsync(String[] strings)
    {
        (int sec, int min, int hour, int days) tupleTime = (0, 0, 0, 0);
        
        TimeSpan result = TimeSpan.Zero;
        
        foreach (var s in strings)
        {

            if (s.Contains("s") && 
                UInt16.TryParse(s.Replace("s", ""), NumberStyles.Integer, CultureInfo.InvariantCulture, out var second))
            {
                tupleTime.sec += second;
            }
            
            if (s.Contains("m") && 
                UInt16.TryParse(s.Replace("m", ""), NumberStyles.Integer, CultureInfo.InvariantCulture, out var minute))
            {
                tupleTime.min += minute;
            }
            
            if (s.Contains("h") && 
                UInt16.TryParse(s.Replace("h", ""), NumberStyles.Integer, CultureInfo.InvariantCulture, out var hour))
            {
                tupleTime.hour += hour;
            }
            
            if (s.Contains("d") && 
                UInt16.TryParse(s.Replace("d", ""), NumberStyles.Integer, CultureInfo.InvariantCulture, out var days))
            {
                tupleTime.days += days;
            }

            result = new TimeSpan(tupleTime.days, tupleTime.hour, tupleTime.min, tupleTime.sec);
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
}