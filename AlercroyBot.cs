using Telegram.Bot.Types;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;

namespace AlercroyBot;

internal class AlercroyBot {
    private ITelegramBotClient TelegramBot { get; init; }
    private CancellationTokenSource CancellationTokenSource { get; init; }

    public AlercroyBot(String token) {
        TelegramBot = new TelegramBotClient(token);
        CancellationTokenSource = new CancellationTokenSource();
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .MinimumLevel.Debug()
            .CreateLogger();
    }

    public static async Task Main(String[] args) {
        var alercroyBot = new AlercroyBot(GetTokenFromArgs(args));
        await alercroyBot.StartBotAsync();
    }

    public static String GetTokenFromArgs(String[] args) {
        if (args.Length == 0)
            throw new Exception("Arguments list is null lenght");

        var param = args[0];
        if (param == "-t" || param == "--t" || param == "-token" || param == "--token")
            return args[1];
        else {
            throw new Exception($"Unknown argument name [{param}]\n" +
                $"Use -t, --t, --token -token");
        }
    }

    public async Task StartBotAsync() {
        var botInfo = await TelegramBot.GetMeAsync();
        ReceiverOptions receiverOptions = new() {
            AllowedUpdates = Array.Empty<UpdateType>()
        };

        TelegramBot.StartReceiving(
            updateHandler: UpdateReceiverHandler,
            pollingErrorHandler: PollingExceptionHandler,
            receiverOptions, CancellationTokenSource.Token);

        Log.Information("Telegram bot {0} is started [{1}]", botInfo.Username, botInfo.Id);
        Console.ReadKey();
    }

    public async Task UpdateReceiverHandler(ITelegramBotClient client, Update update, CancellationToken cancellationToken) {

    }

    public async Task PollingExceptionHandler(ITelegramBotClient client, Exception exception, CancellationToken cancellationToken) {

    }
}