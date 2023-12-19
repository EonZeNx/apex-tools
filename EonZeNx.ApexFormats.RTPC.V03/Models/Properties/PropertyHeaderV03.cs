namespace EonZeNx.ApexFormats.RTPC.V03.Models.Properties;

/// <summary>
/// The base structure of an <see cref="PropertyHeaderV03"/>
/// <br/> Name hash - <see cref="int"/>
/// <br/> Raw data - <see cref="byte"/>[]
/// <br/> Type - <see cref="sbyte"/>/<see cref="EVariantType"/>
/// </summary>
public class PropertyHeaderV03
{
    public uint NameHash { get; set; }
    public byte[] RawData { get; set; }
    public EVariantType VariantType { get; set; }
    
    public PropertyHeaderV03(BinaryReader br)
    {
        NameHash = br.ReadUInt32();
        RawData = br.ReadBytes(4);
        VariantType = (EVariantType) br.ReadByte();
    }
}