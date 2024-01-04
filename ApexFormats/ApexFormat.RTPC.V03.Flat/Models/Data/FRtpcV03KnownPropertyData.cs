using ApexFormat.RTPC.V03.Models.Properties;

namespace ApexFormat.RTPC.V03.Flat.Models.Data;

public struct FRtpcV03KnownPropertyData
{
    public uint NameHash;
    public string Name;
    public EVariantType VariantType;
    public bool IsHash;
}