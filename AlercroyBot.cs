using System.Text.Json;
using AlercroyBot.Modules;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Serilog.Sinks.SystemConsole.Themes;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace AlercroyBot;

using static CommandsHandler;

public class AlercroyBot
{
    
    private Serilog.ILogger AlercroyLogger { get; init; }
    private IConfiguration Configuration { get; init; }
    private ITelegramBotClient TelegramBot { get; init; }
    
    public AlercroyBot(IConfiguration configuration)
    {
        Configuration = configuration;

        if (Configuration["Token"] is { } token)
        {
            AlercroyLogger = new LoggerConfiguration()
                .WriteTo.Console(theme: AnsiConsoleTheme.Code)
                .MinimumLevel.Debug()
                .CreateLogger();
            
            TelegramBot = new TelegramBotClient(token);
        }
        else
        {
            var messageError = "'Token' is not found or null in configuration manager";
            AlercroyLogger?.Error(messageError + "{0}", configuration);
            throw new ArgumentException(messageError, configuration.ToString());
        }
    }

    private async Task AddCommandsAsync()
    {
        var commands = new BotCommand[]
        {
            new BotCommand() { Command = "about", Description = "Write technical info about this bot" },
            new BotCommand() { Command = "help", Description = "Write helpful guide what used this bot" },
            new BotCommand() { Command = "timer", Description = "set timer on day, min, sec, millisecond" }
        }; 
        
        AlercroyLogger.Debug("Add commands list: {array}",  JsonSerializer.Serialize(commands), 
            commands.Length);
        
        await TelegramBot.SetMyCommandsAsync(commands);
    }

    public async Task StartBotAsync()
    {
        await AddCommandsAsync();
        
        ReceiverOptions receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = Array.Empty<UpdateType>()
        };

        this.TelegramBot.StartReceiving(
            HandleUpdateAsync,
            HandlePollingErrorAsync,
            receiverOptions, 
            new CancellationToken());

        var botInfo = await this.TelegramBot.GetMeAsync();
        
        AlercroyLogger.Information("Telegram bot {0} is started [{1}]", botInfo.Username, botInfo.Id);

        Console.ReadKey();
    }

    private async Task HandleUpdateAsync(ITelegramBotClient telegramBotClient, Update update, CancellationToken token)
    {
        if (update.Message is not { Text: { } messageText })
            return;

        var chatId = update.Message.Chat.Id;
        
        AlercroyLogger.Information("Receiver message '{message}' from id: [{chatId}]",
            messageText, chatId);
        
        var commandWithArgs = messageText.Split(" ");
        String command = commandWithArgs.Length >= 1 ? commandWithArgs[0] : String.Empty;

        var commandHandler = new CommandsHandler(telegramBotClient, new CancellationTokenSource(), update, command, AlercroyLogger);
        Task action = commandHandler.UpdateCommand();
        
        try
        {
            await action;
        }
        catch (Exception e)
        {
            AlercroyLogger.Error(e.Message);
        }
        
    }
    
    private async Task HandlePollingErrorAsync(ITelegramBotClient telegramBotClient, Exception exception, CancellationToken token)
    {
        AlercroyLogger.Fatal(exception.Message, exception.Data);
        await Task.CompletedTask;
    }
}