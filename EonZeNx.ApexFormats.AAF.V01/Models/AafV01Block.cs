using System.Buffers.Binary;
using EonZeNx.ApexTools.Core;
using EonZeNx.ApexTools.Core.Abstractions.CombinedSerializable;
using EonZeNx.ApexTools.Core.Utils;
using Ionic.Zlib;

namespace EonZeNx.ApexFormats.AAF.V01.Models;


/// <summary>
/// The structure for a Block in an AAF file.
/// <br/> Compressed Size - <see cref="uint"/>
/// <br/> Uncompressed Size - <see cref="uint"/>
/// <br/> Next block offset (From start of block) - <see cref="uint"/>
/// <br/> FourCC - <see cref="EFourCc"/>
/// <br/> Compressed Data : ZLib uncompress Level 6
/// </summary>
public class AafV01Block : IApexSerializable, ICustomFileSerializable
{
    public uint CompressedSize { get; set; }
    public uint UncompressedSize { get; set; }
    public long NextBlockOffset { get; set; }
    public EFourCc FourCc => EFourCc.Mawe;
    // public byte[] CompressedData { get; set; } = Array.Empty<byte>();
    public byte[] UncompressedData { get; set; } = Array.Empty<byte>();
    public long DataOffset { get; set; } = 0;
    
    
    
    private byte[] _compressedData = Array.Empty<byte>();
    public byte[] CompressedData
    {
        get
        {
            if (_compressedData.Length != 0) return _compressedData;

            using var ms = new MemoryStream();
            using (var zs = new ZlibStream(ms, CompressionMode.Compress, CompressionLevel.Level6))
            {
                zs.Write(UncompressedData, 0, UncompressedData.Length);
            }
                            
            _compressedData = ms.ToArray();
            return _compressedData;
        }
    }
    

    
    // 33,554,432 is 32MB
    public static uint MaxBlockSize => 33554432;
    public uint BlockSize { get; set; } = MaxBlockSize;


    #region From Binary

    public void FromApex(BinaryReader br)
    {
        DataOffset = br.Position();
        CompressedSize = br.ReadUInt32();
        UncompressedSize = br.ReadUInt32();
        
        NextBlockOffset = br.ReadUInt32() + DataOffset;
        var readFourCc = br.ReadBigUInt32();
        if (readFourCc != (uint) FourCc) throw new IOException($"Character code was not valid (Expected '{(uint) FourCc}' got '{readFourCc}')");

        var compressedData = br.ReadBytes((int) CompressedSize);
        using (var ms = new MemoryStream())
        {
            // Write valid header for ZLib
            ms.WriteByte(0x78);
            ms.WriteByte(0x01);
            
            ms.Write(compressedData);

            compressedData = ms.ToArray();
        }
        
        UncompressedData = ZlibStream.UncompressBuffer(compressedData);
        if (UncompressedData.Length != UncompressedSize) throw new IOException("Uncompressed data size was not valid");
        
        br.BaseStream.Seek(NextBlockOffset, SeekOrigin.Begin);
    }

    public void ToApex(BinaryWriter bw)
    {
        var blockStartPosition = (uint) bw.Position();
        bw.Write(CompressedSize - 2);
        bw.Write(UncompressedSize);
        
        // Absolute offset of next block
        bw.Write(blockStartPosition + 4 + 4 + 4 + 4 + CompressedSize - 2);
        
        bw.Write(FourCc.ToBigEndian());
        bw.Write(CompressedData[2..]);
    }

    #endregion


    #region From Custom File

    public void FromCustomFile(BinaryReader br)
    {
        UncompressedData = br.ReadBytes((int) BlockSize);
        UncompressedSize = (uint) UncompressedData.Length;
        BlockSize = UncompressedSize;
        CompressedSize = (uint) CompressedData.Length;
    }

    public void ToCustomFile(BinaryWriter bw)
    {
        bw.Write(UncompressedData);
    }

    #endregion
}