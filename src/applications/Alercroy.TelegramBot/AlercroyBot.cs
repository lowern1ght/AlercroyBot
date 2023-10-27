using Alercroy.TelegramBot.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog.Events;
using Telegram.Bot;

namespace Alercroy.TelegramBot;

public class AlercroyBot
{
    private readonly IServiceProvider _serviceProvider;

    public AlercroyBot(string[]? args)
    {
        var serviceCollection = new ServiceCollection();
        var configurationBuilder = new ConfigurationBuilder();

        configurationBuilder
            .AddEnvironmentVariables()
            .AddCommandLine(args ?? Array.Empty<string>())
            .AddAlercroyConfig();

        IConfiguration configuration = configurationBuilder.Build();

        #if DEBUG
            var eventLevel = LogEventLevel.Debug;
        #else
            var eventLevel = LogEventLevel.Information;
        #endif

        var alercroySettings = configuration.GetAlercroySettings();

        serviceCollection
            .AddDefaultLogger(eventLevel)
            .AddConfiguration(configuration)
            .AddTelegramBot(alercroySettings.Token)
            .AddTimerService(alercroySettings);

        _serviceProvider = serviceCollection
            .BuildServiceProvider();
    }

    public void Run()
    {
        var telegramBot = _serviceProvider.GetRequiredService<ITelegramBotClient>();

        throw new NotImplementedException();

        Console.ReadKey();
    }
}