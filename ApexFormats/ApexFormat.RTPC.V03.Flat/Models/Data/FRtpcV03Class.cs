using ApexFormat.RTPC.V03.Models.Properties;
using ApexTools.Core.Utils;

namespace ApexFormat.RTPC.V03.Flat.Models.Data;

public struct FRtpcV03ClassMember
{
    public EVariantType VariantType = EVariantType.Unassigned;
    public uint NameHash = 0x0;

    public FRtpcV03ClassMember() {}
}

public struct FRtpcV03Class
{
    public uint ClassHash = 0x0;
    public List<FRtpcV03ClassMember> Members = new();

    public FRtpcV03Class() {}
}

public static class FRtpcV03ClassExtensions
{
    public static readonly List<FRtpcV03ClassMember> DefaultMemberHashes = new()
    {
        new FRtpcV03ClassMember { NameHash = ByteUtils.ReverseBytes(0xE65940D0), VariantType = EVariantType.UInteger32 }, // Class hash
        new FRtpcV03ClassMember { NameHash = ByteUtils.ReverseBytes(0x84B61AD3), VariantType = EVariantType.String }, // Name
        new FRtpcV03ClassMember { NameHash = ByteUtils.ReverseBytes(0x8C863A7D), VariantType = EVariantType.UInteger32 }, // Name hash
        new FRtpcV03ClassMember { NameHash = ByteUtils.ReverseBytes(0x0584FFCF), VariantType = EVariantType.UInteger32 } // Object ID
    };
    
    public static FRtpcV03Class FilterDefaultMembers(this FRtpcV03Class fRtpcV03Class)
    {
        var result = new FRtpcV03Class
        {
            ClassHash = fRtpcV03Class.ClassHash,
            Members = fRtpcV03Class.Members
                .Except(DefaultMemberHashes)
                .ToList()
        };

        return result;
    }
    
    public static IEnumerable<RtpcV03PropertyHeader> FilterDefaultMembers(IEnumerable<RtpcV03PropertyHeader> headers)
    {
        var hashes = DefaultMemberHashes.Select(m => m.NameHash);
        return headers.Where(h => !hashes.Contains(h.NameHash));
    }
}