namespace EonZeNx.ApexFormats.RTPC.V01.Models.Properties;

/// <summary>
/// The base structure of an <see cref="RtpcV01PropertyHeader"/>
/// <br/> Name hash - <see cref="int"/>
/// <br/> Raw data - <see cref="byte"/>[]
/// <br/> Type - <see cref="sbyte"/>/<see cref="EVariantType"/>
/// </summary>
public class RtpcV01PropertyHeader
{
    public int NameHash { get; set; }
    public byte[] RawData { get; set; }
    public EVariantType VariantType { get; set; }


    public RtpcV01PropertyHeader(BinaryReader br)
    {
        NameHash = br.ReadInt32();
        RawData = br.ReadBytes(4);
        VariantType = (EVariantType) br.ReadByte();
    }
}