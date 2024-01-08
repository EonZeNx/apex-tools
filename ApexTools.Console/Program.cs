using ApexTools.Console.Managers;
using ApexTools.Core.Config;
using ApexTools.Core.Utils;
using ApexTools.Core.Utils.Hash;

namespace ApexTools.Console;

// Main function for console application
public class Program
{
    public static bool ConsoleExit { get; set; } = false;
    
    public static void Close(string message = "")
    {
        if (!string.IsNullOrEmpty(message))
        {
            ApexToolsConsole.Log(message, LogType.Warning);
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
        AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
        
        Settings.Load();

        if (Settings.LoadAllHashes.Value)
        {
            ApexToolsConsole.Log("Loading hashes into memory...", LogType.Info);
            HashUtils.LoadAll();
        }
        
        if (args.Length == 0)
        {
            ConsoleHashing.Loop();
            ConsoleExit = true;
            
            return;
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

    public static void CurrentDomain_ProcessExit(object? sender, EventArgs e)
    {
        if (!Settings.AutoClose.Value && !ConsoleExit)
        {
            ApexToolsConsole.GetInput("Press any key to continue...");
        }
    }
}