
using System.Buffers.Binary;
using System.Xml;

namespace EonZeNx.ApexTools.Core;

public static class LogUtils
{
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
    
    /// <summary>
    /// Log function that calls the Log function in the LogUtils class and changes the colour based on LogType enum
    /// </summary>
    /// <param name="message"></param>
    /// <param name="logType"></param>
    public static void Log(string message, LogType logType)
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
        
        Log($"{logType.ToString().ToUpper()}: {message}", consoleColor);
    }
    
    /// <summary>
    /// Public log function that writes a message with the specified color to the console.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="color"></param>
    public static void Log(string message, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(message);
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
        return Console.ReadLine();
    }
}

public static class Extensions
{
    public static long Position(this BinaryReader br)
    {
        return br.BaseStream.Position;
    }
    
    public static uint ReadBigUInt32(this BinaryReader br)
    {
        return BinaryPrimitives.ReadUInt32BigEndian(br.ReadBytes(4));
    }
    
    public static long Position(this BinaryWriter bw)
    {
        return bw.BaseStream.Position;
    }
}


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


    #region Stream Alignment

    public static void Align(BinaryReader br, uint align)
    {
        br.BaseStream.Seek(Align(br.BaseStream.Position, align), SeekOrigin.Begin);
    }
        
    public static void Align(BinaryWriter bw, long align, byte fill = 0x50)
    {
        var pos = bw.BaseStream.Position;
        var alignment = Align(pos, align);
        for (var i = 0; i < alignment - pos; i++)
        {
            bw.Write(fill);
        }
    }
        
    public static long Align(MemoryStream ms, long offset, long align)
    {
        var preAlign = Align(offset, align);
        var alignment = preAlign - offset;
        for (var i = 0; i < alignment; i++)
        {
            ms.WriteByte(0x50);
        }

        return preAlign;
    }

    #endregion
}


public static class XmlUtils
{
    public static string GetAttribute(XmlReader xr, string attribute)
    {
        if (!xr.HasAttributes) throw new XmlException("Missing attributes");

        return xr.GetAttribute(attribute) ?? "";
    }
}