using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Text;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace AlercroyBot.Modules;

public class CommandsHandler
{
    private ITelegramBotClient BotClient { get; init; }
    private CancellationTokenSource TokenSource { get; init; }
    private Update Update { get; init; }
    private ILogger? Logger { get; init; }
    private string Command { get; init; }

    private enum Commands 
    {
        Timer, Help, About
    }
    
    public CommandsHandler(ITelegramBotClient botClient, CancellationTokenSource tokenSource, Update update, String command, Serilog.ILogger? logger)
    {
        BotClient = botClient;
        TokenSource = tokenSource;
        Update = update;
        Logger = logger;
        Command = command;
    }

    public Task UpdateCommand()
    {
        return Command switch
        {
            "/timer" => TimerCommandAsync(TokenSource),
            "/help" => HelpCommandAsync(new CancellationTokenSource()),
            _ => UnknownCommandAsync(new CancellationTokenSource())
        };
    }
    
    private String GetHelpInfoResource(String fileName, bool removeBrackets = true)
    {
        Assembly assembly = Assembly.GetExecutingAssembly();
        var resourcePath = assembly.GetManifestResourceNames()
            .Single(s => s.EndsWith(fileName));
        
        using var stream = assembly.GetManifestResourceStream(resourcePath);
        if (stream is null)
        {
            return String.Empty;
        }
        
        using StreamReader reader = new StreamReader(stream);

        StringBuilder builder = new StringBuilder();
        builder.Append(reader.ReadToEnd());
        
        if (removeBrackets)
        {
            builder.Replace("\r", "");
        }
        
        return builder.ToString();
    }
    
    private async Task UnknownCommandAsync(CancellationTokenSource tokenSource)
    {
        Logger?.Error("Unknown command '{message}', [{updateId}]", Update.Message?.Text, Update.Id);
    }

    private async Task TimerCommandAsync(CancellationTokenSource tokenSource)
    {
        Logger?.Information("Accepted 'timer' command, [{updateId}]", Update.Id);

        var spitedCommand = Update.Message?.Text?.Split(" ");

        if (spitedCommand is { Length: <= 1 })
        {
            await BotClient.SendTextMessageAsync(Update.Message!.Chat.Id,
                "**/timer** arguments is bad... use /help /timer to get info",
                parseMode: ParseMode.Markdown, 
                replyToMessageId: Update.Message.MessageId);
            
            throw new ArgumentException("/timer arguments is bad");
        }

        if (TimeSpan.TryParse(spitedCommand?[1], CultureInfo.InvariantCulture, out var span))
        {
            Logger?.Information("Start timer to {user}, on duration: {duration}", Update.Message?.Chat.Username, span);
            var timerService = new TimerService(span, Update.Message!.Chat.Id, BotClient, null, Logger);
        }
        else
        {
            throw new FormatException("bad format duration, example: /timer (12:20, 20:11:11, 32)");
        }
    }

    private async Task HelpCommandAsync(CancellationTokenSource tokenSource)
    {
        if (Update.Message is null or { Text: null })
        {
            throw new ArgumentNullException(nameof(Update.Message), "Update message is null");
        }

        Message message = Update.Message;
        
        var commandArgs = Update.Message?.Text?.Split(" ");
        
        if (commandArgs is { Length: 2 })
        {
            
            String messageHelp = String.Empty;
            var parseMod = ParseMode.MarkdownV2;
            
            var commandToHelp = commandArgs[1];

            if (commandToHelp.Contains(nameof(Commands.Timer).ToLower())) // timer
            {
                messageHelp = GetHelpInfoResource("timer_help.md");
                parseMod = ParseMode.Markdown;
            }
            
            if (commandToHelp.Contains(nameof(Commands.About).ToLower())) // about
            {
                messageHelp = " **/about** - write info about this _bot_";
            }

            if (messageHelp == String.Empty)
            {
                Logger?.Information("Uncorrected 'help' command, [{updateId}]", Update.Id);
                await BotClient.SendTextMessageAsync(message.Chat.Id, "uncorrected command args, use /help [command to help]");
            }
            else
            {
                Logger?.Information("Accepted 'help' command with args {args}, [{updateId}]", commandToHelp, Update.Id);
                await BotClient.SendTextMessageAsync(message.Chat.Id, messageHelp, parseMode: parseMod, 
                    replyToMessageId: message.MessageId);
            }
        }
        else
        {
            Logger?.Information("Accepted 'help' command, [{updateId}]", Update.Id);
            await BotClient.SendTextMessageAsync(message.Chat.Id, "empty args list, use **/help [command to help]**", parseMode: ParseMode.Markdown, 
                replyToMessageId: message.MessageId);
        }
    }
}





























