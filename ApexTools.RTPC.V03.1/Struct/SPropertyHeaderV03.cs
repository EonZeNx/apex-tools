using ApexTools.RTPC.V03._1.Abstractions;
using EonZeNx.ApexFormats.RTPC.V03.Models.Properties;
using EonZeNx.ApexTools.Core.Utils;

namespace ApexTools.RTPC.V03._1.Struct;

/// <summary>
/// Format:<br/>
/// Name hash - <see cref="int"/><br/>
/// RawData - <see cref="byte"/>[]<br/>
/// VariantType - <see cref="EVariantType"/>
/// </summary>
public class SPropertyHeaderV03 : IFromApexHeader
{
    public int NameHash = 0;
    public byte[] RawData = Array.Empty<byte>();
    public EVariantType VariantType = EVariantType.Unassigned;
    
    public string HexNameHash => ByteUtils.ToHex(NameHash);
    public string Name { get; set; } = string.Empty;

    public void FromApexHeader(BinaryReader br)
    {
        NameHash = br.ReadInt32();
        RawData = br.ReadBytes(4);
        VariantType = (EVariantType)br.ReadByte();
    }
}