using System.Globalization;

namespace ApexTools.Core.Utils;

public static class ByteUtils
{
    public static uint ToBigEndian(this EFourCc fourCc)
    {
        var uintFourCc = (uint) fourCc;
        return (uintFourCc & 0x000000FFU) << 24 | (uintFourCc & 0x0000FF00U) << 8 |
               (uintFourCc & 0x00FF0000U) >> 8 | (uintFourCc & 0xFF000000U) >> 24;
    }

    #region Aligment

    public static int Align(int value, int align, bool force = false)
    {
        if (value == 0) return align;
        if (align == 0) return value;
        
        var desiredAlignment = (align - (value % align)) % align;
        if (force && desiredAlignment == 0)
        {
            desiredAlignment = align;
        }

        return value + desiredAlignment;
    }
        
    public static uint Align(uint value, uint align, bool force = false)
    {
        if (value == 0) return align;
        if (align == 0) return value;
        
        var desiredAlignment = (align - (value % align)) % align;
        if (force && desiredAlignment == 0)
        {
            desiredAlignment = align;
        }

        return value + desiredAlignment;
    }
        
    public static long Align(long value, long align, bool force = false)
    {
        if (value == 0) return 0;
        if (align == 0) return value;
        
        var desiredAlignment = (align - (value % align)) % align;
        if (force && desiredAlignment == 0)
        {
            desiredAlignment = align;
        }

        return value + desiredAlignment;
    }
        
    public static ulong Align(ulong value, ulong align, bool force = false)
    {
        if (value == 0) return align;
        if (align == 0) return value;
        
        var desiredAlignment = (align - (value % align)) % align;
        if (force && desiredAlignment == 0)
        {
            desiredAlignment = align;
        }

        return value + desiredAlignment;
    }

    #endregion


    #region To Hex
    
    public static string ToHex(byte value)
    {
        return $"{value:X2}";
    }

    public static string BytesToHex(IEnumerable<byte> bytes, bool reverse = false)
    {
        var safeBytes = reverse ? bytes.Reverse() : bytes;
        return safeBytes.Aggregate("", (current, b) => current + ToHex(b));
    }
    
    public static string ToHex(ulong value, bool reverse = false)
    {
        var bytes = BitConverter.GetBytes(value);
        return BytesToHex(bytes, reverse);
    }
    
    public static string ToHex(long value, bool reverse = false)
    {
        var bytes = BitConverter.GetBytes(value);
        return BytesToHex(bytes, reverse);
    }
    
    public static string ToHex(uint value, bool reverse = false)
    {
        var bytes = BitConverter.GetBytes(value);
        return BytesToHex(bytes, reverse);
    }
    
    public static string ToHex(int value, bool reverse = false)
    {
        var bytes = BitConverter.GetBytes(value);
        return BytesToHex(bytes, reverse);
    }
    
    public static string ToHex(ushort value, bool reverse = false)
    {
        var bytes = BitConverter.GetBytes(value);
        return BytesToHex(bytes, reverse);
    }
    
    public static string ToHex(short value, bool reverse = false)
    {
        var bytes = BitConverter.GetBytes(value);
        return BytesToHex(bytes, reverse);
    }

    #endregion


    #region From Hex
    
    public static byte HexToByte(string value)
    {
        return value.Length < 1 ? (byte)0 : Convert.ToByte(value, 16);
    }
    
    public static uint HexToUInt(string value)
    {
        if (value.Length < 1) return 0;
        
        var safeValue = "";
        for (var i = value.Length - 2; i >= 0; i -= 2)
        {
            safeValue += value[i..(i + 2)];
        }
        
        return uint.Parse(safeValue, NumberStyles.AllowHexSpecifier);
    }

    public static int HexToInt(string value, bool reverse = false)
    {
        if (value.Length < 1) return 0;

        if (!reverse)
        {
            return int.Parse(value, NumberStyles.AllowHexSpecifier);
        }
        
        var safeValue = "";
        for (var i = value.Length - 2; i >= 0; i -= 2)
        {
            safeValue += value[i..(i + 2)];
        }
        
        return int.Parse(safeValue, NumberStyles.AllowHexSpecifier);
    }

    #endregion
}