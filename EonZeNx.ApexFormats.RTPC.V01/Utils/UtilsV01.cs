using EonZeNx.ApexFormats.RTPC.V01.Models.Properties;
using EonZeNx.ApexFormats.RTPC.V01.Models.Properties.Variants;

namespace EonZeNx.ApexFormats.RTPC.V01.Utils;

public static class UtilsV01
{
    // This is horrible but Google isn't finding any alternatives
    public static readonly Dictionary<string, Func<PropertyBaseV01>> XmlNameToBaseProperty = new()
    {
        { new Unassigned().XmlName, () => new Unassigned() },
        { new F32().XmlName, () => new F32() },
        { new UnsignedInt32().XmlName, () => new UnsignedInt32() },
        { new Str().XmlName, () => new Str() },
        { new Event().XmlName, () => new Event() },
        { new ObjectId().XmlName, () => new ObjectId() },
        { new Vec2().XmlName, () => new Vec2() },
        { new Vec3().XmlName, () => new Vec3() },
        { new Vec4().XmlName, () => new Vec4() },
        { new Mat3X3().XmlName, () => new Mat3X3() },
        { new Mat4X4().XmlName, () => new Mat4X4() },
        { new FloatArray().XmlName, () => new FloatArray() },
        { new UInt32Array().XmlName, () => new UInt32Array() },
        { new ByteArray().XmlName, () => new ByteArray() },
    };
}