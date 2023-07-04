using Microsoft.Extensions.Configuration;

namespace AlercroyBot;

public static class Program
{
    private static IConfigurationRoot GetConfigurationFromArgumentLine(String[] args)
    {
        var pathToConfigFile 
            = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Properties", "appsettings.json");
        
        var builder = new ConfigurationBuilder();
        builder.AddCommandLine(args);
        builder.AddJsonFile(pathToConfigFile, false, true);
        return builder.Build();
    }
    
    public static async Task Main(String[]? args)
    {
        var alercroyBot = new AlercroyBot(
            GetConfigurationFromArgumentLine(args ?? new []{ String.Empty }));
        await alercroyBot.StartBotAsync();
    }
}