
using EonZeNx.ApexTools.Core;
using EonZeNx.ApexTools.Core.Utils;
using EonZeNx.ApexTools.Managers;

namespace EonZeNx.ApexTools;

// Main function for console application
public class Program
{
    /// <summary>
    /// Close function that calls LogUtils.Log() then closes the application
    /// </summary>
    /// <param name="message"></param>
    public static void Close(string message = "")
    {
        if (message.Length != 0) LogUtils.Log(message, LogUtils.LogType.Warning);
        if (!Config.Settings.AutoClose.Value) LogUtils.GetInput("Press any key to continue...");
        
        Environment.Exit(0);
    }
    
    
    /// <summary>
    /// FilterPaths which performs a LINQ query filtering files and directories if they exist, and do not have the extension ".exe"
    /// </summary>
    /// <param name="paths"></param>
    /// <returns></returns>
    public static IEnumerable<string> FilterPaths(IEnumerable<string> paths)
    {
        return paths.Where(path => File.Exists(path) || Directory.Exists(path) && !path.EndsWith(".exe"));
    }
    
    
    public static void Main(string[] args)
    {
        // If args length is zero, call Close
        if (args.Length == 0) Close("No arguments passed. Make sure to drag a supported file onto me.");
        
        // Set validPaths to result of FilterPaths. If length is zero, call Close
        var validPaths = FilterPaths(args).ToArray();
        if (!validPaths.Any()) Close("No valid paths found.");
        
        var manager = new ApexMultiPathManager(validPaths);
        manager.ProcessPaths();
        Close();
    }
}