using Serilog;
using Serilog.Core;
using Serilog.Events;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using AlercroyBot.Application.Extensions;
using Microsoft.Extensions.DependencyInjection;


namespace AlercroyBot.Application;

public class AlercroyBot
{
    private readonly Logger _logger;
    private readonly ITelegramBotClient _botClient;
    private readonly IServiceProvider _serviceProvider;
    
    public AlercroyBot(String token, 
        LogEventLevel eventLevel = LogEventLevel.Information)
    {
        _botClient = new TelegramBotClient(token);

        var serviceCollection = new ServiceCollection()
            .AddCommand();

        _serviceProvider = serviceCollection
            .AddSingleton(CreateLogger(eventLevel))
            .AddTimerService()
            .BuildServiceProvider()
            .RegisterCommand(_botClient);

        _logger = _serviceProvider.GetRequiredService<Logger>();
    }

    private Logger CreateLogger(LogEventLevel eventLevel)
    {
        return new LoggerConfiguration()
            .MinimumLevel.Is(eventLevel)
            .WriteTo.Console()
            .CreateLogger();
    }

    public void Run()
    {
        using var cts = new CancellationTokenSource();
        
        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = new [] { UpdateType.Message }
        };

        var botInfo = _botClient.GetMeAsync().Result;
        
        _logger.Information("Bot {Client} is {Status}", botInfo.Username, "Started");
        
        _botClient.StartReceiving(
            updateHandler: HandleUpdateAsync,
            pollingErrorHandler: HandlePollingErrorAsync,
            receiverOptions: receiverOptions,
            cancellationToken: cts.Token
        );
        
        _logger.Information("Press any key to cancel {BotName}", botInfo.Username);
        
        Console.ReadKey();

        cts.Cancel();
    }
    
    private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        if (update.Message is not { Text: { } messageText })
            return;
        
        if (CommandUtilities.IsCommand(messageText))
        {
            try
            {
                var textAndArguments = CommandUtilities.GetCommandTextAndArguments(messageText);

                await HandleCommandAsync(
                    botClient,
                    update,
                    textAndArguments.command,
                    textAndArguments.arguments,
                    cancellationToken);
            }
            catch (Exception exception) when (exception is ArgumentNullException or OperationCanceledException)
            {
                _logger.Error(exception.Message, exception.Data);
            }
        }
    }

    /// <summary>
    ///     Handler commands
    /// </summary>
    /// <param name="botClient"></param>
    /// <param name="update"></param>
    /// <param name="command"></param>
    /// <param name="arguments"></param>
    /// <param name="cancellationToken"></param>
    /// <exception cref="ArgumentNullException"></exception>
    private async Task HandleCommandAsync(ITelegramBotClient botClient, Update update,
        String command, String[] arguments, CancellationToken cancellationToken)
    {
        var commandController = _serviceProvider.GetCommand(command);

        if (commandController is null)
        {
            throw new ArgumentNullException(nameof(commandController));
        }

        _logger.Information("Receive {Command} handle", command);
        
        await commandController.ExecuteAsync(botClient, 
            update,
            command,
            arguments,
            cancellationToken);
    }
    
    private Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        _logger.Error(exception.Message);
        
        return Task.CompletedTask;
    }
}



















