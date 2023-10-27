using System.Reflection;
using Alercroy.Services.Timer;
using Alercroy.TelegramBot.Commands;
using Alercroy.TelegramBot.Commands.Models;
using Alercroy.TelegramBot.Extensions.Models;
using Alercroy.TelegramBot.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using Telegram.Bot;
using Telegram.Bot.Types;
using Timer = Alercroy.Services.Timer.Models.Timer;

namespace Alercroy.TelegramBot.Extensions;

public static class AlercroyExtension
{
    #region TelegramClientExtensions

    private static IEnumerable<CommandDescription> GetCommandDescriptionsInAssembly(this Assembly assembly)
    {
        var result = new List<CommandDescription>();

        foreach (var type in assembly.GetTypes())
        {
            if (type.GetCustomAttribute(typeof(CommandAttribute)) is CommandAttribute attribute &&
                typeof(ICommand).IsAssignableFrom(type))
            {
                result.Add(new CommandDescription
                {
                    Command = attribute.Command,
                    Description = attribute.Description,
                    CommandClass = type
                });
            }
        }

        return result;
    }

    public static IServiceCollection RegisterAlercroyCommands(this IServiceCollection serviceCollection)
    {
        var serviceProvider = serviceCollection.BuildServiceProvider();

        var commandDescriptions = Assembly
            .GetExecutingAssembly()
            .GetCommandDescriptionsInAssembly()
            .ToArray();

        serviceCollection.AddSingleton<ICommandExecuteService>();

        serviceProvider
            .GetRequiredService<ITelegramBotClient>()
            .AddAlercroyCommands(commandDescriptions);

        return serviceCollection;
    }

    private static ITelegramBotClient AddAlercroyCommands(
        this ITelegramBotClient telegramBotClient,
        IEnumerable<CommandDescription> commandDescriptions)
    {
        var botCommands = commandDescriptions.Select(description => new BotCommand
        {
            Command = description.Command,
            Description = description.Description
        });

        telegramBotClient.SetMyCommandsAsync(botCommands)
            .RunSynchronously();

        return telegramBotClient;
    }

    #endregion

    #region IServiceCollectionExtensions

    public static IServiceCollection AddTimerService(
        this IServiceCollection serviceCollection,
        AlercroySettings alercroySettings)
    {
        return serviceCollection
            .AddScoped<ITimerService, TimerService>()
            .AddTimerServiceMapper();
    }

    private static IServiceCollection AddTimerServiceMapper(this IServiceCollection serviceCollection)
    {
        return serviceCollection.AddAutoMapper(mapperConfiguration =>
        {
            mapperConfiguration.CreateMap<Database.Timer.Entities.Timer, Timer>()
                .ReverseMap();
        });
    }

    public static IServiceCollection AddTelegramBot(this IServiceCollection serviceCollection, string? token)
    {
        if (token is null)
        {
            throw new ArgumentNullException(nameof(token));
        }

        return serviceCollection.AddSingleton<ITelegramBotClient>(new TelegramBotClient(token));
    }

    public static IServiceCollection AddConfiguration(
        this IServiceCollection serviceCollection,
        IConfiguration configuration)
    {
        return serviceCollection.AddSingleton(configuration);
    }

    private static readonly string DefaultLoggerFileName = "log.txt";

    private static ILogger CreateDefaultLogger(LogEventLevel eventLevel = LogEventLevel.Debug)
    {
        return new LoggerConfiguration()
            .MinimumLevel.Is(eventLevel)
            .WriteTo
                .Console(theme: AnsiConsoleTheme.Code)
            .WriteTo
                .File(DefaultLoggerFileName, rollingInterval: RollingInterval.Month)
            .CreateLogger();
    }

    public static IServiceCollection AddDefaultLogger(this IServiceCollection serviceCollection, LogEventLevel eventLevel)
    {
        return serviceCollection.AddSingleton<ILogger>(_ => CreateDefaultLogger(eventLevel));
    }

    #endregion

    #region IConfigurationBuilderExtensions

    private static readonly string DefaultConfigName = "botsettings.json";

    public static IConfigurationBuilder AddAlercroyConfig(this IConfigurationBuilder configurationBuilder)
    {
        return configurationBuilder.AddJsonFile(DefaultConfigName, false, true);
    }

    #endregion

    #region IConfigurationExtensions

    public static AlercroySettings GetAlercroySettings(this IConfiguration configuration)
    {
        var alercroySettings = configuration
            .GetSection(nameof(AlercroySettings))
            .Get<AlercroySettings>();

        if (alercroySettings is null)
        {
            throw new ArgumentNullException(nameof(alercroySettings));
        }

        return alercroySettings;
    }

    #endregion
}