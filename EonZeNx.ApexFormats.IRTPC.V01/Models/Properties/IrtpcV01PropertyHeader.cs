using EonZeNx.ApexTools.Core.Utils;

namespace EonZeNx.ApexFormats.IRTPC.V01.Models.Properties;

public class IrtpcV01PropertyHeader
{
    public long Offset { get; }
    public int NameHash { get; }
    public EVariantType VariantType { get; }
    

    public IrtpcV01PropertyHeader(BinaryReader br)
    {
        Offset = br.Position();
        NameHash = br.ReadInt32();
        VariantType = (EVariantType) br.ReadByte();
    }
}