using System.Collections.ObjectModel;

namespace AlercroyBot.Application.Entity;

public class Timer
{
    public Int64 ChatId { get; }
    public TimeSpan Duration { get; }

    public Timer(long chatId, TimeSpan duration)
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
    
    public Boolean TryParse(Object o, out TimeSpan? duration)
    {
        if (o is String message)
        {
            try
            {
                duration = Parse(message);
                return true;
            }
            catch (Exception)
            {
                duration = null;
                return false;
            }
        }

        duration = null;
        return false;
    }
    
    public TimeSpan Parse(String message)
    {
        return Parse(message.Split(' '));
    }
    
    public TimeSpan Parse(String[] arguments)
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

    private Int64 ParseArgumentDuration(String argument)
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