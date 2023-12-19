using ApexTools.JC4.RTPC.V03.Abstractions;
using EonZeNx.ApexFormats.RTPC.V03.Models.Properties;
using EonZeNx.ApexTools.Core.Utils;

namespace ApexTools.JC4.RTPC.V03.Struct;

/// <summary>
/// Format:<br/>
/// Name hash - <see cref="int"/><br/>
/// RawData - <see cref="byte"/>[]<br/>
/// VariantType - <see cref="EVariantType"/>
/// </summary>
public class JC4PropertyHeaderV03 : IBinarySize, IFromApexHeader, IToApexHeader
{
    public int NameHash = 0;
    public byte[] RawData = new byte[4];
    public EVariantType VariantType = EVariantType.Unassigned;
    
    public string HexNameHash => ByteUtils.ToHex(NameHash, true);
    public string Name { get; set; } = string.Empty;

    public static int BinarySize => 4 + 4 + 1;

    public void FromApexHeader(BinaryReader br)
    {
        NameHash = br.ReadInt32();
        RawData = br.ReadBytes(4);
        VariantType = (EVariantType)br.ReadByte();
    }

    public void ToApexHeader(BinaryWriter bw)
    {
        bw.Write(NameHash);
        bw.Write(RawData);
        bw.Write((byte) VariantType);
    }
}