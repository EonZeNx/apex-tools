namespace ApexFormat.AAF.V01.Models;

public static class AafV01ChunkHeaderConstants
{
    public const uint Magic = 0x4D415745; // "EWAM"
    public const uint Size = 4 + 4 + 4 + 4;
}

/// <summary>
/// Structure:
/// <br/>CompressedSize - <see cref="uint"/>
/// <br/>UncompressedSize - <see cref="uint"/>
/// <br/>DataSize - <see cref="uint"/> (Header.Size + CompressedSize + 16-byte padding)
/// <br/>Magic - <see cref="uint"/>
/// </summary>
public class AafV01ChunkHeader
{
    public uint CompressedSize;
    public uint UncompressedSize;
    public uint DataSize;
    public uint Magic = AafV01ChunkHeaderConstants.Magic;
}

public static class AafV01ChunkHeaderExtensions
{
    public static AafV01ChunkHeader ReadApexAafV01ChunkHeader(this BinaryReader br)
    {
        var result = new AafV01ChunkHeader
        {
            CompressedSize = br.ReadUInt32(),
            UncompressedSize = br.ReadUInt32(),
            DataSize = br.ReadUInt32(),
            Magic = br.ReadUInt32()
        };

        if (result.Magic != AafV01ChunkHeaderConstants.Magic)
        {
            throw new FileLoadException($"{nameof(result.Magic)} = {result.Magic}, expected {AafV01ChunkHeaderConstants.Magic}");
        }

        return result;
    }

    public static void WriteApex(this BinaryWriter bw, AafV01ChunkHeader header)
    {
        bw.Write(header.CompressedSize);
        bw.Write(header.UncompressedSize);
        bw.Write(header.DataSize);
        bw.Write(header.Magic);
    }
}