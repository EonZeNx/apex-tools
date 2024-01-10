using System.Globalization;
using ApexTools.Core.Extensions;
using ApexTools.Core.Hash;
using ApexTools.Core.Utils;

namespace ApexTools;

public enum ECommand
{
    Hash,
    Lookup,
    Exit
}

public static class ConsoleHash
{
    private static string SwitchMode => "-";
    
    public static void Start()
    {
        ConsoleUtils.Log("[- to switch between Hash and Lookup]", LogType.Info);
        ConsoleUtils.Log("[Leave empty to exit]", LogType.Info);

        var userCommand = ECommand.Hash;
        while (userCommand != ECommand.Exit)
        {
            userCommand = userCommand switch
            {
                ECommand.Hash => HashInput(),
                ECommand.Lookup => LookupInput(),
                _ => ECommand.Exit
            };
        }
    }

    private static ECommand HashInput()
    {
        var userInput = ConsoleUtils.GetInput("Enter string to hash: ");
        
        if (string.IsNullOrEmpty(userInput)) return ECommand.Exit;
        if (string.Equals(userInput, SwitchMode)) return ECommand.Lookup;

        var hash = userInput.HashJenkins();
        ConsoleUtils.Log($"Hex (big endian):    {hash:X8}", ConsoleColor.White);
        ConsoleUtils.Log($"Hex (little endian): {hash.LittleEndian():X8}", ConsoleColor.White);
        ConsoleUtils.Log($"UInt32:              {hash}", ConsoleColor.White);

        return ECommand.Hash;
    }

    private static ECommand LookupInput()
    {
        var userInput = ConsoleUtils.GetInput("Enter hash to lookup: ");
        
        if (string.IsNullOrEmpty(userInput)) return ECommand.Exit;
        if (string.Equals(userInput, SwitchMode)) return ECommand.Hash;

        var parseSuccess = uint.TryParse(userInput, out var hash);
        
        if (!parseSuccess)
        {
            parseSuccess = uint.TryParse(userInput, NumberStyles.HexNumber, null, out hash);
        }
        
        if (!parseSuccess)
        {
            ConsoleUtils.Log("Cannot hash string, is it a valid uint32?", LogType.Error);
            return ECommand.Lookup;
        }
        
        var result = LookupHashes.Get(hash);
        if (string.IsNullOrEmpty(result))
        {
            ConsoleUtils.Log("Hash not found in database", LogType.Warning);
        }
        else
        {
            ConsoleUtils.Log($"Found result: {result}", ConsoleColor.White);
        }
        
        return ECommand.Lookup;
    }
}