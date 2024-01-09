namespace ApexTools.Core.Utils;

public static class FileHeaderUtils
{
    public static readonly Dictionary<uint, EFourCc> FourCcUintMap = Enum.GetValues<EFourCc>()
        .ToArray()
        .ToDictionary(val => (uint) val, val => val);
    
    public static readonly Dictionary<string, EFourCc> FourCcStringMap = Enum.GetValues<EFourCc>()
        .ToArray()
        .ToDictionary(val => val.ToString().ToUpper(), val => val);
    
    
    /// <summary>
    /// GetFileHeader function that checks a file exists then returns the first 16 bytes of the file
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    /// <exception cref="FileNotFoundException"></exception>
    public static byte[] GetFileHeader(string filePath)
    {
        if (!File.Exists(filePath)) throw new FileNotFoundException("File not found", filePath);

        // Use a binary reader to read the first 16 bytes of the file
        using var br = new BinaryReader(File.OpenRead(filePath));
        return br.ReadBytes(16);
    }
    
    public static bool IsSupportedCharacterCode(uint input)
    {
        return FourCcUintMap.ContainsKey(input);
    }
    
    /// <summary>
    /// Check the file header to see if it contains a supported character code.
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static EFourCc ValidCharacterCode(byte[] input)
    {
        if (input.Length < 4) throw new ArgumentException("Input must be at least 4 bytes long");
        
        for (var i = 4; i <= input.Length; i++)
        {
            var lastI = i - 4;
            var inputArray = input.AsSpan()[lastI..i];
            var value = BitConverter.ToUInt32(inputArray);
                
            if (IsSupportedCharacterCode(value)) return FourCcUintMap[value];

            var reversedInputArray = inputArray.ToArray();
            Array.Reverse(reversedInputArray);
            var reversedValue = BitConverter.ToUInt32(reversedInputArray);

            if (IsSupportedCharacterCode(reversedValue)) return FourCcUintMap[reversedValue];
        }

        return EFourCc.IRTPC;
    }
    
    /// <summary>
    /// Gets the file header then checks it for a supported character code.
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public static EFourCc ValidCharacterCode(string filePath)
    {
        var header = GetFileHeader(filePath);
        return ValidCharacterCode(header);
    }

    public static bool IsCharacterCode(uint input, EFourCc fourCc)
    {
        if (input == (uint) fourCc) return true;

        var inputBytes = BitConverter.GetBytes(input);
        Array.Reverse(inputBytes);
        
        var reversedInput = BitConverter.ToUInt32(inputBytes);
        return reversedInput == (uint) fourCc;
    }
}