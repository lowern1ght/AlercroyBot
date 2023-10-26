namespace Alercroy.Services.Timer.Models;

public class Timer
{
    public Guid Id { get; set; }
    public uint ChatId { get; set; }
    public TimeSpan Duration { get; set; }
    public DateTimeOffset BeginDate { get; set; }
}