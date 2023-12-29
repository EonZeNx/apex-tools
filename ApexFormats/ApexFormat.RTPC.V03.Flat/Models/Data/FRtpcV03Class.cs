using ApexFormat.RTPC.V03.Models.Properties;
using ApexTools.Core.Utils;

namespace ApexFormat.RTPC.V03.Flat.Models.Data;

public struct FRtpcV03ClassMember
{
    public EVariantType VariantType = EVariantType.Unassigned;
    public uint NameHash = 0x0;
    
    public string NameHashHex = string.Empty;
    public string Name = string.Empty;

    public FRtpcV03ClassMember() {}
}

public struct FRtpcV03Class
{
    public uint ClassHash = 0x0;
    public List<FRtpcV03ClassMember> Members = new();
    
    public string Name = string.Empty;

    public FRtpcV03Class() {}
}

public static class FRtpcV03ClassExtensions
{
    public static readonly List<uint> DefaultMemberHashes = new()
    {
        ByteUtils.ReverseBytes(0xE65940D0), // Class hash
        ByteUtils.ReverseBytes(0x84B61AD3), // Name
        ByteUtils.ReverseBytes(0x8C863A7D), // Name hash
        ByteUtils.ReverseBytes(0x0584FFCF) // Object ID
    };
    
    public static FRtpcV03Class FilterDefaultMembers(this FRtpcV03Class fRtpcV03Class)
    {
        var result = new FRtpcV03Class
        {
            ClassHash = fRtpcV03Class.ClassHash,
            Members = fRtpcV03Class.Members.ToList()
        };

        return result;
    }
    
    public static IEnumerable<RtpcV03PropertyHeader> FilterDefaultMembers(IEnumerable<RtpcV03PropertyHeader> headers)
    {
        var filteredHeaders = headers.Where(h => !DefaultMemberHashes.Contains(h.NameHash));
        
        return filteredHeaders;
    }
}