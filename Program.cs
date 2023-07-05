using Microsoft.Extensions.Configuration;

namespace AlercroyBot;

public static class Program
{
    private static IConfigurationRoot GetConfiguration(String[] args)
    {
        var pathToConfigFile 
            = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Properties", "appsettings.json");
        
        var builder = new ConfigurationBuilder();
        builder.AddJsonFile(pathToConfigFile, false, true);
        builder.AddCommandLine(args);
        return builder.Build();
    }
    
    public static async Task Main(String[]? args)
    {
        var alercroyBot = new AlercroyBot(
            GetConfiguration(args ?? new []{ String.Empty }));
        await alercroyBot.StartBotAsync();
    }
}