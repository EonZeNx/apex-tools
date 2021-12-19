using System.Text;
using EonZeNx.ApexTools.Core;
using EonZeNx.ApexTools.Core.Abstractions;
using EonZeNx.ApexTools.Core.Abstractions.CombinedSerializable;
using EonZeNx.ApexTools.Core.Utils;

namespace EonZeNx.ApexFormats.AAF.V01.Models;


/// <summary>
/// The structure for <see cref="FileV01"/>.
/// <br/> FourCc - <see cref="EFourCc"/>
/// <br/> Version - <see cref="uint"/>
/// <br/> Comment (Length 28, UTF8) - <see cref="string"/>
/// <br/> Uncompressed Size - <see cref="uint"/>
/// <br/> Compressed Size - <see cref="uint"/>
/// <br/> Block count - <see cref="uint"/>
/// <br/> Block array - <see cref="BlockV01"/>
/// </summary>
public class FileV01 : IApexFile, IApexSerializable, ICustomFileSerializable
{
    public EFourCc FourCc => EFourCc.Aaf;
    public uint Version => 0x01;
    public string Comment { get; set; } = "AVALANCHEARCHIVEFORMATISCOOL";
    public uint UncompressedSize { get; set; } = 0;
    public uint CompressedSize { get; set; } = 0;
    public uint BlockCount { get; set; } = 0;
    public BlockV01[] BlockArray { get; set; } = Array.Empty<BlockV01>();


    #region ApexSerializable

    public void FromApex(BinaryReader br)
    {
        var readFourCc = br.ReadBigUInt32();
        if (!FileHeaderUtils.IsCharacterCode(readFourCc, FourCc)) throw new IOException($"Character code was not valid (Expected '{(uint) FourCc}' got '{readFourCc}')");
        
        var readVersion = br.ReadUInt32();
        if (readVersion != Version) throw new IOException($"Version was not valid (Expected '{Version}' got '{readVersion}')");

        Comment = Encoding.UTF8.GetString(br.ReadBytes(8 + 16 + 4));
        
        UncompressedSize = br.ReadUInt32();
        CompressedSize = br.ReadUInt32();
        BlockCount = br.ReadUInt32();
        
        BlockArray = new BlockV01[BlockCount];
        for (var i = 0; i < BlockCount; i++)
        {
            BlockArray[i] = new BlockV01();
            BlockArray[i].FromApex(br);
        }
    }

    public void ToApex(BinaryWriter bw)
    {
        bw.Write(FourCc.ToBigEndian());
        bw.Write(Version);
        bw.Write(Encoding.UTF8.GetBytes(Comment));
        
        var uncompressedSize = (uint) BlockArray.Sum(block => block.UncompressedSize);
        bw.Write(uncompressedSize);
        
        bw.Write(
            BlockArray.Length == 1
                ? BlockArray[0].BlockSize
                : Math.Min(BlockArray.Max(block => block.BlockSize), BlockV01.MaxBlockSize)
        );
        bw.Write(BlockCount);
        
        foreach (var block in BlockArray)
        {
            block.ToApex(bw);
        }
    }

    #endregion


    #region CustomSerializable

    public void FromCustomFile(BinaryReader br)
    {
        var blockList = new List<BlockV01>();
        while (br.Position() < br.BaseStream.Length)
        {
            var block = new BlockV01();
            block.FromCustomFile(br);
            blockList.Add(block);
        }

        BlockArray = blockList.ToArray();
        BlockCount = (uint) BlockArray.Length;
    }

    public void ToCustomFile(BinaryWriter bw)
    {
        for (var i = 0; i < BlockCount; i++)
        {
            BlockArray[i].ToCustomFile(bw);
        }
    }

    #endregion
}