using ApexTools.Core.Abstractions.CombinedSerializable;
using ApexTools.Core.Extensions;
using ApexTools.Core.Interfaces;

namespace ApexFormat.AAF.V01.Models;

/// <summary>
/// Structure:
/// <br/>Header - <see cref="AafV01Header"/>
/// <br/>ChunkArray - <see cref="AafV01Chunk"/>
/// </summary>
public class AafV01File : IApexFile, ICustomFileSerializable
{
    public string ApexExtension { get; set; } = ".aaf";
    
    public AafV01Header Header = new();
    public AafV01Chunk[] ChunkArray { get; set; } = Array.Empty<AafV01Chunk>();

    #region ApexSerializable

    public void FromApex(BinaryReader br)
    {
        Header = br.ReadAafV01Header();
        
        ChunkArray = new AafV01Chunk[Header.ChunkCount];
        for (var i = 0; i < Header.ChunkCount; i++)
        {
            ChunkArray[i] = br.ReadApexAafV01Chunk();
        }
    }

    public void ToApex(BinaryWriter bw)
    {
        bw.Write(Header);
        
        foreach (var chunk in ChunkArray)
        {
            bw.WriteApex(chunk);
        }
    }

    #endregion


    #region CustomSerializable

    public void FromCustomFile(BinaryReader br)
    {
        var blockList = new List<AafV01Chunk>();
        while (br.Position() < br.BaseStream.Length)
        {
            var block = br.ReadCustomAafV01Chunk();
            blockList.Add(block);
        }
        
        ChunkArray = blockList.ToArray();
        
        Header = new AafV01Header
        {
            ChunkCount = (uint) blockList.Count,
            TotalUncompressedSize = (uint) ChunkArray.Sum(chunk => chunk.Header.UncompressedSize),
            MaxUncompressedChunkSize = ChunkArray.Length == 1
                ? ChunkArray[0].Header.UncompressedSize
                : AafV01ChunkConstants.MaxUncompressedSize
        };
    }

    public void ToCustomFile(BinaryWriter bw)
    {
        foreach (ref var chunk in ChunkArray.AsSpan())
        {
            bw.WriteCustom(chunk);
        }
    }

    #endregion
}