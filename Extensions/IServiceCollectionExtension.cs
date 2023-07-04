using Microsoft.Extensions.DependencyInjection;

namespace AlercroyBot.Extensions;

public static class IServiceCollectionExtension
{
    public static IServiceCollection AddSerilog(this IServiceCollection collection, Serilog.ILogger logger) 
        => collection.AddSingleton<Serilog.ILogger>(logger);
}