namespace EonZeNx.ApexFormats.RTPC.V03.Models.Properties;

/// <summary>
/// Assume 1-byte padding unless stated otherwise
/// </summary>
public enum EVariantType
{
    Unassigned = 0, // No pad
    UInteger32 = 1, // No pad
    Float32 = 2, // No pad
    String = 3, // No pad
    Vec2 = 4,
    Vec3 = 5,
    Vec4 = 6, // 4-byte pad
    Mat3X3 = 7,
    Mat4X4 = 8, // 4-byte pad
    UInteger32Array = 9,
    Float32Array = 10,
    ByteArray = 11,
    Deprecated = 12,
    ObjectId = 13,
    Event = 14,
    Total = 15
}