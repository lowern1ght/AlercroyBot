using Microsoft.Extensions.Configuration;

namespace AlercroyBot;

public static class Program
{
    private static IConfigurationRoot GetConfigurationFromArgumentLine(String[] args)
    {
        var builder = new ConfigurationBuilder();
        builder.AddCommandLine(args);
        return builder.Build();
    }
    
    public static async Task Main(String[] args)
    {
        var alercroyBot = new AlercroyBot(
            GetConfigurationFromArgumentLine(args));
        await alercroyBot.StartBotAsync();
    }
}