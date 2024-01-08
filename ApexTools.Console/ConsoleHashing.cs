using System.Globalization;
using ApexTools.Core.Utils;
using ApexTools.Core.Utils.Hash;

namespace ApexTools.Console;

public enum EConsoleInputMode
{
    Hash,
    Lookup,
    Exit
}

public static class ConsoleHashing
{
    public static string SwitchMode => "-";
    
    public static void Loop()
    {
        ApexToolsConsole.Log("[- to switch between Hash and Lookup]", LogType.Info);
        ApexToolsConsole.Log("[Leave empty to exit]", LogType.Info);

        var userCommand = EConsoleInputMode.Hash;
        while (userCommand != EConsoleInputMode.Exit)
        {
            userCommand = userCommand switch
            {
                EConsoleInputMode.Hash => InputHash(),
                EConsoleInputMode.Lookup => InputLookup(),
                _ => EConsoleInputMode.Exit
            };
        }
    }

    public static EConsoleInputMode InputHash()
    {
        var userInput = ApexToolsConsole.GetInput("Enter string to hash: ");
        
        if (string.IsNullOrEmpty(userInput)) return EConsoleInputMode.Exit;
        if (string.Equals(userInput, SwitchMode)) return EConsoleInputMode.Lookup;

        var hash = userInput.HashJenkins();
        ApexToolsConsole.Log($"Hex (big endian):\t{hash:X8}", ConsoleColor.White);
        ApexToolsConsole.Log($"Hex (little endian):\t{ByteUtils.ReverseBytes(hash):X8}", ConsoleColor.White);
        ApexToolsConsole.Log($"UInt32:\t\t\t{hash}", ConsoleColor.White);

        return EConsoleInputMode.Hash;
    }

    public static EConsoleInputMode InputLookup()
    {
        var userInput = ApexToolsConsole.GetInput("Enter hash to lookup: ");
        
        if (string.IsNullOrEmpty(userInput)) return EConsoleInputMode.Exit;
        if (string.Equals(userInput, SwitchMode)) return EConsoleInputMode.Hash;

        var parseSuccess = uint.TryParse(userInput, out var hash);
        
        if (!parseSuccess)
        {
            parseSuccess = uint.TryParse(userInput, NumberStyles.HexNumber, null, out hash);
        }
        
        if (!parseSuccess)
        {
            ApexToolsConsole.Log("Cannot hash string, is it a valid uint32?", LogType.Error);
            return EConsoleInputMode.Lookup;
        }
        
        var result = HashUtils.Lookup(hash);
        if (string.IsNullOrEmpty(result))
        {
            ApexToolsConsole.Log("Hash not found in database", LogType.Warning);
        }
        else
        {
            ApexToolsConsole.Log($"Found result: {result}", ConsoleColor.White);
        }
        
        return EConsoleInputMode.Lookup;
    }
}