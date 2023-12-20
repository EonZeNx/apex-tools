using ApexTools.JC4.RTPC.V03.Abstractions;
using ApexTools.JC4.RTPC.V03.Models;
using ApexTools.JC4.RTPC.V03.Variants;

namespace ApexTools.JC4.RTPC.V03;

public static class XmlUtilsV03
{
    public static readonly Dictionary<string, Func<APropertyV03>> XmlNameToBaseProperty = new()
    {
        { new VariantUnassigned().XmlName, () => new VariantUnassigned() },
        { new VariantU32().XmlName, () => new VariantU32() },
        { new VariantF32().XmlName, () => new VariantF32() },
        { new VariantStr().XmlName, () => new VariantStr() },
        { new VariantVec2().XmlName, () => new VariantVec2() },
        { new VariantVec3().XmlName, () => new VariantVec3() },
        { new VariantVec4().XmlName, () => new VariantVec4() },
        { new VariantMat3X3().XmlName, () => new VariantMat3X3() },
        { new VariantMat4X4().XmlName, () => new VariantMat4X4() },
        { new VariantU32Array().XmlName, () => new VariantU32Array() },
        { new VariantF32Array().XmlName, () => new VariantF32Array() },
        { new VariantByteArray().XmlName, () => new VariantByteArray() },
        { new VariantObjectId().XmlName, () => new VariantObjectId() },
        { new VariantEvent().XmlName, () => new VariantEvent() },
    };
}