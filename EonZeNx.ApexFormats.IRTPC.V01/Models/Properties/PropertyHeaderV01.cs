using EonZeNx.ApexTools.Core.Utils;

namespace EonZeNx.ApexFormats.IRTPC.V01.Models.Properties;

public class PropertyHeaderV01
{
    public long Offset { get; }
    public uint NameHash { get; }
    public EVariantType VariantType { get; }
    

    public PropertyHeaderV01(BinaryReader br)
    {
        Offset = br.Position();
        NameHash = br.ReadUInt32();
        VariantType = (EVariantType) br.ReadByte();
    }
}