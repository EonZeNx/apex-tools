using ApexTools.Core.Utils;

namespace ApexFormat.IRTPC.V01.Debug.Models.Properties;

public class PropertyHeader
{
    public long Offset { get; }
    public int NameHash { get; }
    public EVariantType VariantType { get; }
    

    public PropertyHeader(BinaryReader br)
    {
        Offset = br.Position();
        NameHash = br.ReadInt32();
        VariantType = (EVariantType) br.ReadByte();
    }
}