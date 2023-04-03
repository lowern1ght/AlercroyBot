namespace AlercroyBot;

internal class AlecroyBot {
    private TelegramBotClient TelegramBot { get; init; }

    public AlecroyBot(String token) {
        TelegramBot = new(token);
    }

    public static async Task Main(String[] args) {
        var alecroyBot = new AlecroyBot(args[2]);
        await alecroyBot.StartBotAsync();
    }

    public async Task StartBotAsync() {

    }
}