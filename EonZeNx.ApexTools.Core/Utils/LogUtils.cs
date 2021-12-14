namespace EonZeNx.ApexTools.Core.Utils;

public static class LogUtils
{
    /// <summary>
    /// Enum for logging, includes info, error, success, warning, and debug
    /// </summary>
    public enum LogType
    {
        Info,
        Error,
        Success,
        Warning,
        Debug
    }
    
    /// <summary>
    /// Log function that calls the Log function in the LogUtils class and changes the colour based on LogType enum
    /// </summary>
    /// <param name="message"></param>
    /// <param name="logType"></param>
    public static void Log(string message, LogType logType)
    {
        var consoleColor = logType switch
        {
            LogType.Error => ConsoleColor.Red,
            LogType.Success => ConsoleColor.Green,
            LogType.Warning => ConsoleColor.Yellow,
            LogType.Debug => ConsoleColor.Cyan,
            LogType.Info => ConsoleColor.White,
            _ => ConsoleColor.White
        };
        
        Log($"{logType.ToString().ToUpper()}: {message}", consoleColor);
    }
    
    /// <summary>
    /// Public log function that writes a message with the specified color to the console.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="color"></param>
    public static void Log(string message, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(message);
        Console.ResetColor();
    }
    
    /// <summary>
    /// GetInput function that writes a message to the console and returns the input from the user.
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public static string? GetInput(string message)
    {
        Console.Write(message);
        return Console.ReadLine();
    }
}