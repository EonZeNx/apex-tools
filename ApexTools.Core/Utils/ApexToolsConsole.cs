using ApexTools.Core.Config;

namespace ApexTools.Core.Utils;

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

public static class ApexToolsConsole
{
    /// <summary>
    /// Log function that calls the Log function in the LogUtils class and changes the colour based on LogType enum
    /// </summary>
    /// <param name="message"></param>
    /// <param name="logType"></param>
    /// <param name="newLine"></param>
    public static void Log(string message, LogType logType, bool newLine = true)
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
        
        Log($"[{logType.ToString().ToUpper()}]: {message}", consoleColor, newLine);
    }

    /// <summary>
    /// Public log function that writes a message with the specified color to the console.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="color"></param>
    /// <param name="newLine"></param>
    public static void Log(string message, ConsoleColor color, bool newLine = true)
    {
        Console.ForegroundColor = color;
        
        if (newLine) Console.WriteLine(message);
        else Console.Write(message);
        
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
        var input = Console.ReadLine();
        
        return input;
    }

    
    /// <summary>
    /// Log the loading step of the file.
    /// </summary>
    /// <param name="targetFilename"></param>
    /// <param name="fileType"></param>
    public static void LogLoading(string targetFilename, string fileType)
    {
        if (Settings.LogProgress.Value) Log($"Loading '{targetFilename}' as {fileType}", LogType.Info);
    }
    
    /// <summary>
    /// Log the processing step of the file.
    /// </summary>
    /// <param name="targetFilename"></param>
    public static void LogProcessing(string targetFilename)
    {
        if (Settings.LogProgress.Value) Log($"Processing '{targetFilename}'", LogType.Info);
    }
    
    /// <summary>
    /// Log the completion step of the file.
    /// </summary>
    /// <param name="targetFilename"></param>
    public static void LogComplete(string targetFilename)
    {
        if (Settings.LogProgress.Value) Log($"Completed '{targetFilename}'", LogType.Info);
    }
    
    /// <summary>
    /// Log the failed status of loading the file.
    /// </summary>
    /// <param name="targetFilename"></param>
    public static void LogFailedToLoadError(string targetFilename)
    {
        if (Settings.LogProgress.Value) Log($"Failed to load '{targetFilename}'", LogType.Error);
    }
}