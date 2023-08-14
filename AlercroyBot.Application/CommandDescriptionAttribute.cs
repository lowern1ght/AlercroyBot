namespace AlercroyBot.Application;

public class CommandDescriptionAttribute : Attribute
{
    public String Command { get; }
    
    public String Description { get; }
    
    public CommandDescriptionAttribute(String command, String? description)
    {
        this.Command = command; 
        this.Description = description ?? String.Empty;
    }
}