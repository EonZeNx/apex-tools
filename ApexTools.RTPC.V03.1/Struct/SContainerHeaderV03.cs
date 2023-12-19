using ApexTools.RTPC.V03._1.Abstractions;
using EonZeNx.ApexTools.Core.Utils;

namespace ApexTools.RTPC.V03._1.Struct;

/// <summary>
/// Format:<br/>
/// Name hash - <see cref="int"/><br/>
/// Offset - <see cref="uint"/><br/>
/// Property count - <see cref="ushort"/><br/>
/// Container count - <see cref="ushort"/>
/// </summary>
public class SContainerHeaderV03 : IFromApexHeader
{
    public int NameHash = 0;
    public uint BodyOffset = 0;
    public ushort PropertyCount = 0;
    public ushort ContainerCount = 0;
    
    public string HexNameHash => ByteUtils.ToHex(NameHash);
    public string Name { get; set; } = string.Empty;

    public void FromApexHeader(BinaryReader br)
    {
        NameHash = br.ReadInt32();
        BodyOffset = br.ReadUInt32();
        PropertyCount = br.ReadUInt16();
        ContainerCount = br.ReadUInt16();
    }
}