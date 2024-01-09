namespace ApexFormat.RTPC.V03.Inline.Models.Data;

public enum EVariantType
{
    Unassigned = 0,
    UInteger32 = 1,
    Float32 = 2,
    String = 3,
    Vector2 = 4,
    Vector3 = 5,
    Vector4 = 6,
    Matrix3X4 = 8,
    Events = 14
}

public static class VariantTypeExtensions
{
    public static readonly Dictionary<EVariantType, string> VariantToXml = new()
    {
        {EVariantType.Unassigned, "Unassigned"},
        {EVariantType.UInteger32, "UInt32"},
        {EVariantType.Float32, "Float"},
        {EVariantType.String, "String"},
        {EVariantType.Vector2, "Vector2"},
        {EVariantType.Vector3, "Vector3"},
        {EVariantType.Vector4, "Vector4"},
        {EVariantType.Matrix3X4, "Matrix3X4"},
        {EVariantType.Events, "Events"},
    };
    public static readonly Dictionary<string, EVariantType> XmlNameVariant = VariantToXml.ToDictionary(x => x.Value, x => x.Key);
    
    public static string GetXmlName(this EVariantType variantType)
    {
        return VariantToXml[variantType];
    }
    
    public static EVariantType GetVariantType(string str)
    {
        return XmlNameVariant[str];
    }
}