using ApexTools.Core.Extensions;
using ApexTools.Core.Utils;
using Ionic.Zlib;

namespace ApexFormat.AAF.V01.Models;

public static class AafV01ChunkConstants
{
    public const uint MaxUncompressedSize = 33_554_432; // 32MB
    public const uint Alignment = 16;
}

/// <summary>
/// Structure:
/// <br/>Header - <see cref="AafV01ChunkHeader"/>
/// <br/>Compressed Data : ZLib uncompress Level 6
/// </summary>
public class AafV01Chunk
{
    public AafV01ChunkHeader Header = new();
    public byte[] CompressedData = Array.Empty<byte>();
}

public static class AafV01ChunkExtensions
{
    public static AafV01Chunk ReadApexAafV01Chunk(this BinaryReader br)
    {
        var preReadPosition = br.Position();
        
        var result = new AafV01Chunk
        {
            Header = br.ReadApexAafV01ChunkHeader()
        };

        result.CompressedData = br.ReadBytes((int) result.Header.CompressedSize);
        
        br.Seek(preReadPosition + result.Header.DataSize);

        return result;
    }

    public static void WriteCustom(this BinaryWriter bw, AafV01Chunk chunk)
    {
        byte[] uncompressedData;
        using (var ms = new MemoryStream())
        {
            // Write valid header for ZLib
            ms.WriteByte(0x78);
            ms.WriteByte(0x01);
            
            ms.Write(chunk.CompressedData);

            uncompressedData = ZlibStream.UncompressBuffer(ms.ToArray());
        }

        if (uncompressedData.Length != chunk.Header.UncompressedSize)
        {
            throw new IOException("Uncompressed data size was not valid");
        }

        bw.Write(uncompressedData);
    }
    
    public static AafV01Chunk ReadCustomAafV01Chunk(this BinaryReader br)
    {
        var uncompressedSize = Math.Min(br.BaseStream.Length - br.Position(), AafV01ChunkConstants.MaxUncompressedSize);
        var uncompressedData = br.ReadBytes((int) uncompressedSize);
        
        using var ms = new MemoryStream();
        using (var zs = new ZlibStream(ms, CompressionMode.Compress, CompressionLevel.Level6))
        {
            zs.Write(uncompressedData, 0, uncompressedData.Length);
        }

        var unalignedPosition = br.Position();
        var padding = ByteUtils.Align(unalignedPosition, AafV01ChunkConstants.Alignment) - unalignedPosition;
        
        var result = new AafV01Chunk
        {
            CompressedData = ms.ToArray()[2..],
            Header = new AafV01ChunkHeader()
        };
        
        result.Header.UncompressedSize = (uint) uncompressedSize;
        result.Header.CompressedSize = (uint) result.CompressedData.Length;
        result.Header.DataSize = AafV01ChunkHeaderConstants.Size + (uint) result.CompressedData.Length + (uint) padding;
        
        return result;
    }

    public static void WriteApex(this BinaryWriter bw, AafV01Chunk chunk)
    {
        bw.WriteApex(chunk.Header);
        bw.Write(chunk.CompressedData);
        bw.Align(AafV01ChunkConstants.Alignment, 0x30);
    }
}