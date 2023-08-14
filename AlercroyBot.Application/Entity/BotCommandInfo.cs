using Telegram.Bot.Types;

namespace AlercroyBot.Application.Entity;

public class BotCommandInfo
{
    public BotCommandInfo(Type commandType, BotCommand botCommand)
    {
        this.CommandType = commandType;
        this.BotCommand = botCommand;
    }

    public Type CommandType { get; }
    public BotCommand BotCommand { get; }
}