namespace AlercroyBot.Application;

static class CommandUtilities
{
    public static Boolean IsCommand(String message)
    {
        return message.Length > 1 && message[0] == '/';
    }
    
    public static (String command, String[] arguments) GetCommandTextAndArguments(String message)
    {
        var splitMessage = message.Split(' ');

        if (splitMessage.Length < 1)
        {
            throw new ArgumentNullException(nameof(message), "Uncorrected command");
        }
        
        return new(splitMessage[0], splitMessage[1..splitMessage.Length]);
    }
}