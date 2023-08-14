using AlercroyBot.Application.Entity;
using AlercroyBot.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace AlercroyBot.Application.Extensions;

public static class ServiceProviderExtension
{
    public static ICommandAsync? GetCommand(this IServiceProvider serviceProvider, String command)
    {
        var commandsInfo = serviceProvider.GetRequiredService<List<BotCommandInfo>>();

        foreach (var info in commandsInfo)
        {
            if (info.BotCommand.Command.Contains(command))
            {
                return (ICommandAsync?)serviceProvider.GetRequiredService(info.CommandType);
            }
        }

        return null;
    }
    
    public static IServiceProvider RegisterCommand(this IServiceProvider serviceCollection,
        ITelegramBotClient botClient)
    {
        var botCommandsArray = serviceCollection.GetRequiredService<List<BotCommandInfo>>()
            .Select(info => info.BotCommand)
            .ToArray();

        botClient.SetMyCommandsAsync(botCommandsArray, BotCommandScope.Default());
        
        return serviceCollection;
    }
}