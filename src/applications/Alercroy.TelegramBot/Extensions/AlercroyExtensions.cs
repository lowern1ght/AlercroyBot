using Alercroy.TelegramBot.Extensions.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using Telegram.Bot;

namespace Alercroy.TelegramBot.Extensions;

public static class AlercroyExtension
{
    #region IServiceCollectionExtensions

    public static IServiceCollection AddTimerService(
        this IServiceCollection serviceCollection,
        AlercroySettings alercroySettings)
    {
        throw new NotImplementedException();
    }

    public static IServiceCollection AddTimerStorage(this IServiceCollection serviceCollection)
    {
        throw new NotImplementedException();
    }

    public static IServiceCollection AddTelegramBot(this IServiceCollection serviceCollection, string? token)
    {
        if (token is null)
        {
            throw new ArgumentNullException(nameof(token));
        }

        return serviceCollection.AddSingleton<TelegramBotClient>(_ => new TelegramBotClient(token));
    }

    public static IServiceCollection AddConfiguration(
        this IServiceCollection serviceCollection,
        IConfiguration configuration)
    {
        return serviceCollection.AddSingleton<IConfiguration>(_ => configuration);
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