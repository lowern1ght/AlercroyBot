namespace AlercroyBot.Application.Entity;

public class Timer
{
    public Int64 ChatId { get; }
    public TimeSpan Duration { get; }

    public Timer(long chatId, ref TimeSpan duration)
    {
        this.ChatId = chatId;
        this.Duration = duration;
    }

    private static Dictionary<Char, Int32> _dictionary = new Dictionary<Char, Int32>
    {
        { 's', TimeSpan.FromSeconds(1).Seconds },
        { 'm', TimeSpan.FromMinutes(1).Seconds },
        { 'h', TimeSpan.FromHours(1).Seconds },
        { 'd', TimeSpan.FromDays(1).Seconds },
        { 'y', TimeSpan.FromDays(364).Seconds },
    };
    
    public static Boolean TryParse(String message, out TimeSpan duration)
    {
        return TryParse(message.Split(' '), out duration);
    }
    
    public static Boolean TryParse(String[] arguments, out TimeSpan duration)
    {
        try
        {
            duration = Parse(arguments);
            return true;
        }
        catch (Exception)
        {
            duration = default;
            return false;
        } 
    }

    public static TimeSpan Parse(String message)
    {
        return Parse(message.Split(' '));
    }
    
    public static TimeSpan Parse(String[] arguments)
    {
        var seconds = Int64.MinValue;
        
        foreach (var argument in arguments)
        {
            foreach (var pair in _dictionary)
            {
                if (argument.EndsWith(pair.Key))
                {
                    seconds = ParseArgumentDuration(argument) * pair.Value;
                }
            }
        }

        return TimeSpan.FromSeconds(seconds);
    }

    private static Int64 ParseArgumentDuration(String argument)
    {
        List<Char> chars = new List<char>();

        foreach (var c in argument)
        {
            if (Char.IsDigit(c))
            {
                chars.Add(c);
            }
        }

        return Int64.TryParse(new string(chars.ToArray()), out var digits) ? digits : Int64.MinValue;
    }
}