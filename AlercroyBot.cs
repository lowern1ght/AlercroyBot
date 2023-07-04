using AlercroyBot.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog.Sinks.SystemConsole.Themes;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace AlercroyBot;

public class AlercroyBot
{
    
    private ILogger Logger { get; init; }
    private IConfiguration Configuration { get; init; }
    private ITelegramBotClient TelegramBot { get; init; }
    private IServiceCollection ServiceCollection { get; init; }
    private CancellationTokenSource CancellationTokenSource { get; }
    
    public AlercroyBot(IConfiguration configuration)
    {
        Configuration = configuration;

        if (Configuration["Token"] is String token)
        {
            var logger = new LoggerConfiguration()
                .WriteTo.Console(theme: AnsiConsoleTheme.Code)
                .MinimumLevel.Debug()
                .CreateLogger();
            
            ServiceCollection.AddSerilog(logger);
            
            this.TelegramBot = new TelegramBotClient(token);
            this.CancellationTokenSource = new CancellationTokenSource();
        }
        else
        {
            throw new ArgumentException("'Token' is not found or null in configuration manager", configuration.ToString());
        }
    }

    public async Task StartBotAsync()
    {
        var botInfo = await this.TelegramBot.GetMeAsync();

        ReceiverOptions receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = Array.Empty<UpdateType>()
        };

        this.TelegramBot.StartReceiving(
            UpdateReceiverHandler,
            PollingExceptionHandler,
            receiverOptions, this.CancellationTokenSource.Token);

        Log.Information("Telegram bot {0} is started [{1}]", botInfo.Username, botInfo.Id);
        
        await ExitLoop(new [] {"exit", "quit"});
    }

    private async Task ExitLoop(String commandToExit)
        => await ExitLoop(new[] { commandToExit });
    
    private async Task ExitLoop(String[] commandToExit)
    {
        String command;
        do
        {
            Console.Write("Await command to exit" + Environment.NewLine + " >");
            command = Console.ReadLine().Trim();
        } 
        while (!commandToExit.Contains(command));
        await Task.CompletedTask;
    }

    public async Task UpdateReceiverHandler(ITelegramBotClient client, Update update,
        CancellationToken cancellationToken)
    {

    }

    public async Task PollingExceptionHandler(ITelegramBotClient client, Exception exception,
        CancellationToken cancellationToken)
    {

    }
}