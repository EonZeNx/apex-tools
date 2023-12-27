using ApexTools.Console.Managers;
using ApexTools.Core.Config;
using ApexTools.Core.Utils;

namespace ApexTools.Console;

// Main function for console application
public class Program
{
    public static void Close(string message = "")
    {
        if (!string.IsNullOrEmpty(message))
        {
            ApexToolsConsole.Log(message, LogType.Warning);
        }

        if (!Settings.AutoClose.Value)
        {
            ApexToolsConsole.GetInput("Press any key to continue...");
        }
        
        Environment.Exit(0);
    }
    
    /// <summary>
    /// Filtering files and directories if they exist, and do not have the extension ".exe"
    /// </summary>
    /// <param name="paths"></param>
    /// <returns></returns>
    public static IEnumerable<string> FilterPaths(IEnumerable<string> paths)
    {
        return paths.Where(path => File.Exists(path) || Directory.Exists(path) && !path.EndsWith(".exe"));
    }
    
    public static void Main(string[] args)
    {
        Settings.Load();
        
        if (args.Length == 0)
        {
            Close("No arguments passed. Make sure to drag a supported file onto this.");
        }
        
        var validPaths = FilterPaths(args).ToArray();
        if (!validPaths.Any())
        {
            Close("No valid paths found.");
        }
        
        var manager = new ApexMultiPathManager(validPaths);
        manager.ProcessPaths();
        Close();
    }
}