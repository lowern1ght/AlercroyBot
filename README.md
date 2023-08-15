# AlercroyBot

![Banner](Repository/banner.png)

---

**AlercroyBot** - is a telegram bot, on .NET with [Telegram.Bot](https://github.com/TelegramBots/Telegram.Bot) 
the bot has a number of features: 

- DI container
- Quickly add new commands without explicitly specifying dependencies
- _80%_ asynchronous code.
- Built-in Logger from Serilog
- A timer service that supports multiple tasks for a different number of users. Competitively independent code

---

### Add new command

> The first step is to create a new class inheriting the **ICommandAsync** interface and 
add Attribute: **CommandDescription**

- You can also get the added built-in services in the constructor because this bot uses MsDI

```csharp

[CommandDescription("test", "the description for test command")]
public class TestCommand : ICommandAsync 
{
    ....
}
```

> After that, the bot itself will register in the telegram API all the commands found in the assembly

---

### Development

 - [ ] Add humazizer translate and formating to more language if can set to command */language*
 - [ ] Transfer to ConcurrentDictionary TimerService