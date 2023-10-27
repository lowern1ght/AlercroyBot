using Alercroy.TelegramBot.Commands;
using Alercroy.TelegramBot.Commands.Models;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Telegram.Bot.Types;

namespace Alercroy.TelegramBot.Services;

public class CommandExecuteService : ICommandExecuteService
{
    private readonly ILogger _logger;
    private readonly IReadOnlyCollection<CommandDescription> _commandDescriptions;

    public CommandExecuteService(IEnumerable<CommandDescription> commandDescriptions, ILogger logger)
    {
        _logger = logger;
        _commandDescriptions = (IReadOnlyCollection<CommandDescription>)commandDescriptions;
    }

    public async Task ExecuteCommand(string command, IServiceProvider serviceProvider, Update update, CancellationToken token)
    {
        var commandDescription = _commandDescriptions.FirstOrDefault(description
            => description.Command.Equals(command));

        if (commandDescription is null)
        {
            throw new ArgumentNullException(nameof(commandDescription));
        }

        await using var scope = serviceProvider.CreateAsyncScope();

        var constructorInfo = commandDescription.CommandClass.GetConstructor(Type.EmptyTypes);

        if (constructorInfo is null)
        {
            throw new ArgumentNullException(nameof(constructorInfo));
        }

        var types = new List<object>();
        foreach (var parameterInfo in constructorInfo.GetParameters())
        {
            types.Add(scope.ServiceProvider.GetRequiredService(parameterInfo.ParameterType));
        }

        await ((ICommand)constructorInfo.Invoke(new object?[] { types }))
            .ExecuteAsync(update, token);
    }
}