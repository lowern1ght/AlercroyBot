namespace Alercroy.TelegramBot.Commands.Models;

public class CommandDescription
{
    public required string Command { get; init; }
    public required string Description { get; init; }
    public required Type CommandClass { get; set; }
}