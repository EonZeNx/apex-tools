namespace EonZeNx.ApexFormats.RTPC.V03.Models.Properties;

/// <summary>
/// Assume 4-byte padding unless stated otherwise
/// </summary>
public enum EVariantType
{
    Unassigned = 0, // No pad
    UInteger32 = 1, // No pad
    Float32 = 2, // No pad
    String = 3, // No pad
    Vector2 = 4,
    Vector3 = 5,
    Vector4 = 6, // 16-byte pad
    Matrix3X3 = 7,
    Matrix4X4 = 8, // 16-byte pad
    UInteger32Array = 9,
    Float32Array = 10,
    ByteArray = 11, // 16-byte pad
    Deprecated = 12,
    ObjectId = 13,
    Event = 14,
    Total = 15
}