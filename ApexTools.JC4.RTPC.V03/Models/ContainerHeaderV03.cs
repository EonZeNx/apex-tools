using ApexTools.JC4.RTPC.V03.Abstractions;
using EonZeNx.ApexTools.Core.Utils;

namespace ApexTools.JC4.RTPC.V03.Models;

/// <summary>
/// Format:<br/>
/// Name hash - <see cref="int"/><br/>
/// Offset - <see cref="uint"/><br/>
/// Property count - <see cref="ushort"/><br/>
/// Container count - <see cref="ushort"/>
/// </summary>
public class ContainerHeaderV03 : IBinarySize, IFromApexHeader, IToApexHeader
{
    public uint NameHash = 0;
    public uint BodyOffset = 0;
    public ushort PropertyCount = 0;
    public ushort ContainerCount = 0;

    public static int BinarySize => 4 + 4 + 2 + 2;

    public string HexNameHash => ByteUtils.ToHex(NameHash, true);
    public string Name { get; set; } = string.Empty;

    public void FromApexHeader(BinaryReader br)
    {
        NameHash = br.ReadUInt32();
        BodyOffset = br.ReadUInt32();
        PropertyCount = br.ReadUInt16();
        ContainerCount = br.ReadUInt16();
    }

    public void ToApexHeader(BinaryWriter bw)
    {
        bw.Write(NameHash);
        bw.Write(BodyOffset);
        bw.Write(PropertyCount);
        bw.Write(ContainerCount);
    }
}