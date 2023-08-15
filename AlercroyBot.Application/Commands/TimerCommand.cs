using Serilog.Core;
using Telegram.Bot;
using Telegram.Bot.Types;
using AlercroyBot.Application.Interfaces;
using Telegram.Bot.Types.Enums;
using Timer = AlercroyBot.Application.Entity.Timer;

namespace AlercroyBot.Application.Commands;

[CommandDescription("timer",  "set timer for this user")]
public class TimerCommand : ICommandAsync
{
    private readonly Logger _logger;
    private readonly ITimerService _timerService;

    public TimerCommand(ITimerService timerService, Logger logger)
    {
        _logger = logger;
        _timerService = timerService;
    }
    
    public async Task ExecuteAsync(ITelegramBotClient botClient, Update update, string command, 
        string[] arguments, CancellationToken token)
    {
        if (arguments.Length == 0)
        {
            _logger.Error("Arguments is null");
            return;
        }
        
        if (!Timer.TryParse(arguments, out var duration))
        {
            _logger.Error("Can not parse arguments to {Type}", nameof(TimeSpan));
            await Task.CompletedTask;
        }

        var chatId = update.Message!.Chat.Id;

        try
        {
            await _timerService.StartTimerAsync(chatId, duration, botClient);
        }
        catch (Exception exception)
        {
            _logger.Error("Timer service drop with exception: {Exception}",
                exception.Message);
        }
    }

    private async void SendMessageOnTimerEnd(Int64 chatId, String message, ITelegramBotClient botClient)
    {
        await botClient.SendTextMessageAsync(chatId, message, parseMode: ParseMode.Markdown);
    }
}