namespace ApexTools.Core.Utils;

public enum LogType
{
    Error,
    Warning,
    Success,
    Info,
    Debug
}

public static class ConsoleUtils
{
    public static void Log(string message, LogType logType, bool displayLogLevel = false, bool newLine = true)
    {
        var consoleColor = logType switch
        {
            LogType.Error => ConsoleColor.Red,
            LogType.Warning => ConsoleColor.Yellow,
            LogType.Success => ConsoleColor.Green,
            LogType.Info => ConsoleColor.White,
            LogType.Debug => ConsoleColor.Cyan,
            _ => ConsoleColor.White
        };

        var safeMessage = message;
        if (displayLogLevel)
        {
            safeMessage = $"[{logType.ToString().ToUpper()}]: {message}";
        }
        
        Log(safeMessage, consoleColor, newLine);
    }

    public static void Log(string message, ConsoleColor color, bool newLine = true)
    {
        Console.ForegroundColor = color;
        
        if (newLine)
        {
            Console.WriteLine(message);
        }
        else
        {
            Console.Write(message);
        }
        
        Console.ResetColor();
    }
    
    public static string? GetInput(string message)
    {
        Console.Write(message);
        var input = Console.ReadLine();
        
        return input;
    }
}
