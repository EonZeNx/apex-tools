using System.Text;
using ApexTools.Core;
using ApexTools.Core.Extensions;

namespace ApexFormat.AAF.V01.Models;

public static class AafV01HeaderConstants
{
    public const EFourCc FourCc = EFourCc.AAF;
    public const uint Version = 0x01;
    public const string Comment = "AVALANCHEARCHIVEFORMATISCOOL";
}

public class AafV01Header
{
    public EFourCc FourCc = AafV01HeaderConstants.FourCc;
    public uint Version = AafV01HeaderConstants.Version;
    public string Comment = AafV01HeaderConstants.Comment;
    public uint TotalUncompressedSize;
    public uint MaxUncompressedChunkSize;
    public uint ChunkCount;
}

public static class AafV01HeaderExtensions
{
    public static AafV01Header ReadAafV01Header(this BinaryReader br)
    {
        var result = new AafV01Header
        {
            FourCc = (EFourCc) br.ReadUInt32().LittleEndian(),
            Version = br.ReadUInt32(),
            Comment = Encoding.UTF8.GetString(br.ReadBytes(AafV01HeaderConstants.Comment.Length))
        };

        if (result.FourCc != AafV01HeaderConstants.FourCc)
        {
            throw new FileLoadException($"{nameof(result.FourCc)} is {result.FourCc}, expected {nameof(AafV01Header.FourCc)}");
        }
        
        if (result.Version != AafV01HeaderConstants.Version)
        {
            throw new FileLoadException($"{nameof(result.Version)} is {result.Version}, expected {nameof(AafV01Header.Version)}");
        }
        
        if (!string.Equals(result.Comment, AafV01HeaderConstants.Comment))
        {
            throw new FileLoadException($"{nameof(result.Comment)} is {result.Comment}, expected {nameof(AafV01Header.Comment)}");
        }

        result.TotalUncompressedSize = br.ReadUInt32();
        result.MaxUncompressedChunkSize = br.ReadUInt32();
        result.ChunkCount = br.ReadUInt32();

        return result;
    }

    public static void Write(this BinaryWriter bw, AafV01Header header)
    {
        bw.Write(((uint) header.FourCc).LittleEndian());
        bw.Write(header.Version);
        bw.WriteString(header.Comment);
        bw.Write(header.TotalUncompressedSize);
        bw.Write(header.MaxUncompressedChunkSize);
        bw.Write(header.ChunkCount);
    }
}