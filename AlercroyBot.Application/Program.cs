using CommandLine;

namespace AlercroyBot.Application;

class Option
{
    [Option('t', "token", Required = true, HelpText = "write token for telegram bot")]
    public String Token { get; set; } = String.Empty;
}

public static class Program 
{
    public static void Main(String[] args)
    {
        Parser.Default.ParseArguments<Option>(args)
            .WithParsed(Action);
    }

    private static void Action(Option option)
    {
        new AlercroyBot(option.Token).Run();
    }
}