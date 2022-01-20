namespace EonZeNx.ApexFormats.ADF.V04.Models.Types;

public enum EVariantType : uint
{
    Primitive   = 0,
    Structure   = 1,
    Pointer     = 2,
    Array       = 3,
    InlineArray = 4,
    String      = 5,
    Unknown     = 6,
    BitField    = 7,
    Enumeration = 8,
    StringHash  = 9,
    Deferred    = 10
}