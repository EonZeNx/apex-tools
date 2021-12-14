﻿using EonZeNx.ApexFormats.IRTPC.V01.Models.Properties;
using EonZeNx.ApexFormats.IRTPC.V01.Models.Properties.Variants;

namespace EonZeNx.ApexFormats.IRTPC.V01.Utils;

public static class IrtpcV01Utils
{
    // This is horrible but Google isn't finding any alternatives
    public static readonly Dictionary<string, Func<IrtpcV01BaseProperty>> XmlNameToBaseProperty = new()
    {
        { new F32().XmlName, () => new F32() },
        { new UnsignedInt32().XmlName, () => new UnsignedInt32() },
        { new Str().XmlName, () => new Str() },
        { new Event().XmlName, () => new Event() },
        { new Vec2().XmlName, () => new Vec2() },
        { new Vec3().XmlName, () => new Vec3() },
        { new Vec4().XmlName, () => new Vec4() },
        { new Mat3X4().XmlName, () => new Mat3X4() },
    };
}