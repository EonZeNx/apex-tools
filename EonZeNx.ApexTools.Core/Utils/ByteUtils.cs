using System.Globalization;

namespace EonZeNx.ApexTools.Core.Utils;

public static class ByteUtils
{
    public static uint ToBigEndian(this EFourCc fourCc)
    {
        var uintFourCc = (uint) fourCc;
        return (uintFourCc & 0x000000FFU) << 24 | (uintFourCc & 0x0000FF00U) << 8 |
               (uintFourCc & 0x00FF0000U) >> 8 | (uintFourCc & 0xFF000000U) >> 24;
    }

    
    #region Reverse

    public static ushort ReverseBytes(ushort value)
    {
        return (ushort)((value & 0xFFU) << 8 | (value & 0xFF00U) >> 8);
    }
        
    public static uint ReverseBytes(uint value)
    {
        return (value & 0x000000FFU) << 24 | (value & 0x0000FF00U) << 08 |
               (value & 0x00FF0000U) >> 08 | (value & 0xFF000000U) >> 24;
    }
        
    public static ulong ReverseBytes(ulong value)
    {
        return (value & 0x00000000000000FFUL) << 56 | (value & 0x000000000000FF00UL) << 40 |
               (value & 0x0000000000FF0000UL) << 24 | (value & 0x00000000FF000000UL) << 08 |
               (value & 0x000000FF00000000UL) >> 08 | (value & 0x0000FF0000000000UL) >> 24 |
               (value & 0x00FF000000000000UL) >> 40 | (value & 0xFF00000000000000UL) >> 56;
    }

    #endregion


    #region Aligment

    public static int Align(int value, int align)
    {
        if (value == 0) return align;
        if (align == 0) return value;
            
        return value + ((align - (value % align)) % align);
    }
        
    public static uint Align(uint value, uint align)
    {
        if (value == 0) return align;
        if (align == 0) return value;

        return value + ((align - (value % align)) % align);
    }
        
    public static long Align(long value, long align)
    {
        if (value == 0) return 0;
        if (align == 0) return value;

        return value + ((align - (value % align)) % align);
    }
        
    public static ulong Align(ulong value, ulong align)
    {
        if (value == 0) return align;
        if (align == 0) return value;

        return value + ((align - (value % align)) % align);
    }

    #endregion


    #region To Hex

    public static string BytesToHex(IEnumerable<byte> bytes)
    {
        return bytes.Aggregate("", (current, b) => current + b.ToString("X2"));
    }
    
    public static string ToHex(byte value)
    {
        var bytes = new[] { value };
        return BytesToHex(bytes);
    }
    
    public static string ToHex(ulong value)
    {
        var bytes = BitConverter.GetBytes(value);
        return BytesToHex(bytes);
    }
    
    public static string ToHex(long value)
    {
        var bytes = BitConverter.GetBytes(value);
        return BytesToHex(bytes);
    }
    
    public static string ToHex(uint value)
    {
        var bytes = BitConverter.GetBytes(value);
        return BytesToHex(bytes);
    }
    
    public static string ToHex(int value)
    {
        var bytes = BitConverter.GetBytes(value);
        return BytesToHex(bytes);
    }
    
    public static string ToHex(ushort value)
    {
        var bytes = BitConverter.GetBytes(value);
        return BytesToHex(bytes);
    }
    
    public static string ToHex(short value)
    {
        var bytes = BitConverter.GetBytes(value);
        return BytesToHex(bytes);
    }

    #endregion


    #region From Hex
    
    public static uint HexToUint(string value)
    {
        if (value.Length < 1) return 0;

        var reversedValue = "";
        for (var i = value.Length - 2; i >= 0; i -= 2)
        {
            reversedValue += value[i..(i + 2)];
        }
        
        return uint.Parse(reversedValue, NumberStyles.AllowHexSpecifier);
    }

    public static int HexToInt(string value)
    {
        if (value.Length < 1) return 0;

        var reversedValue = "";
        for (var i = value.Length - 2; i >= 0; i -= 2)
        {
            reversedValue += value[i..(i + 2)];
        }
        
        return int.Parse(reversedValue, NumberStyles.AllowHexSpecifier);
    }

    #endregion
}