using System.Reflection;
using AlercroyBot.Application.Entity;
using AlercroyBot.Application.Interfaces;
using AlercroyBot.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.Types;

namespace AlercroyBot.Application.Extensions;

public static class ServiceCollectionExtension
{ 
    public static IServiceCollection AddCommand(this IServiceCollection serviceCollection)
    {
        var commandsInfo = new List<BotCommandInfo>(); 
        
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            foreach (var type in assembly.GetTypes())
            {
                if (type.IsAssignableTo(typeof(ICommandAsync)) && 
                    type.GetCustomAttribute<CommandDescriptionAttribute>() is {} attribute)
                {

                    var commandInfo = new BotCommandInfo(
                        type,
                        new BotCommand { Command = attribute.Command, Description = attribute.Description });

                    serviceCollection.AddTransient(type);
                    
                    commandsInfo.Add(commandInfo);
                }
            }
        }

        serviceCollection.AddSingleton(commandsInfo);
        
        return serviceCollection;
    }

    public static IServiceCollection AddTimerService(this IServiceCollection serviceCollection)
    {
        return serviceCollection.AddTransient<ITimerService, TimerService>();
    }
}