namespace Alercroy.TelegramBot.Commands;

public class CommandAttribute : Attribute
{
    public required string Command { get; set; }
    public required string Description { get; set; }
}