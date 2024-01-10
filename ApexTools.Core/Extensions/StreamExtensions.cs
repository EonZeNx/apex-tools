using System.Buffers.Binary;
using System.Text;
using ApexTools.Core.Utils;

namespace ApexTools.Core.Extensions;

public static class StreamExtensions
{
    public static uint ReadBigUInt32(this BinaryReader br)
    {
        return BinaryPrimitives.ReadUInt32BigEndian(br.ReadBytes(4));
    }
    
    #region StringZ
    
    public static string ReadStringZ(this BinaryReader br)
    {
        var fullString = "";
        var character = "";
            
        while (character != "\0")
        {
            fullString += character;
            character = Encoding.UTF8.GetString(br.ReadBytes(1));
        }

        return fullString;
    }
    
    public static void WriteString(this BinaryWriter bw, string value)
    {
        foreach (var character in value)
        {
            bw.Write(character);
        }
    }
        
    public static void WriteStringZ(this BinaryWriter bw, string value)
    {
        bw.WriteString(value);

        if (value.EndsWith(char.MinValue)) return;
            
        bw.Write(char.MinValue);
    }

    #endregion
    
    #region Stream
    
    public static void Seek(this BinaryReader br, long offset)
    {
        br.BaseStream.Seek(offset, SeekOrigin.Begin);
    }

    public static long Position(this BinaryReader br)
    {
        return br.BaseStream.Position;
    }
    
    public static long Position(this BinaryWriter bw)
    {
        return bw.BaseStream.Position;
    }

    #endregion
    
    #region Alignment
    
    public static void Align(this BinaryReader br, int align, bool force = false)
    {
        br.Align((uint) align, force);
    }

    public static void Align(this BinaryReader br, uint align, bool force = false)
    {
        br.BaseStream.Seek(ByteUtils.Align(br.Position(), align, force), SeekOrigin.Begin);
    }
        
    public static void Align(this BinaryWriter bw, long align, byte fill = 0x50)
    {
        var pos = bw.Position();
        var alignment = ByteUtils.Align(pos, align);
        for (var i = 0; i < alignment - pos; i++)
        {
            bw.Write(fill);
        }
    }
        
    public static long Align(this MemoryStream ms, long offset, long align)
    {
        var preAlign = ByteUtils.Align(offset, align);
        var alignment = preAlign - offset;
        for (var i = 0; i < alignment; i++)
        {
            ms.WriteByte(0x50);
        }

        return preAlign;
    }

    #endregion
}