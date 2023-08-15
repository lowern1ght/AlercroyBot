using CommandLine;

namespace AlercroyBot.Application;

public class ArgumentOption
{
    [Option('t', "token", Required = true, HelpText = "write token for telegram bot")]
    public String Token { get; }

    public ArgumentOption(String token)
    {
        Token = token;
    }
}

public static class Program
{
    public static void Main(String[] args)
    {
        Parser.Default.ParseArguments<ArgumentOption>(args)
            .WithParsed(Action);
    }

    private static void Action(ArgumentOption argumentOption)
    {
        new AlercroyBot(argumentOption.Token).Run();
    }
}