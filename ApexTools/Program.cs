using ApexTools.Console.Managers;
using ApexTools.Core.Config;
using ApexTools.Core.Hash;
using ApexTools.Core.Utils;

namespace ApexTools;

public class Program
{
    public static void Close(string message = "")
    {
        if (!string.IsNullOrEmpty(message))
        {
            ConsoleUtils.Log(message, LogType.Warning);
        }
        
        Environment.Exit(0);
    }
    
    public static IEnumerable<string> FilterPaths(IEnumerable<string> paths)
    {
        return paths.Where(path => File.Exists(path) || Directory.Exists(path) && !path.EndsWith(".exe"));
    }
    
    public static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            ConsoleHash.Start();
            return;
        }
        
        AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
        
        Settings.Load();

        if (Settings.PreloadHashes.Value)
        {
            ConsoleUtils.Log("Loading hashes into memory...", LogType.Info);
            LookupHashes.LoadAll();
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
        if (!Settings.AutoClose.Value)
        {
            ConsoleUtils.GetInput("Press any key to continue...");
        }
    }
}