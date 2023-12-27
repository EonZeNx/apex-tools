using EonZeNx.ApexTools.Core.Utils.Hash;

namespace ApexTools.JC4.RTPC.V03.Models.Data;

public struct RtpcV03ContainerHeader
{
    public uint NameHash;
    public uint BodyOffset;
    public ushort PropertyCount;
    public ushort ContainerCount;
    
    public string Name = string.Empty;
    
    public static int SizeOf(bool withValid = false) => 4 + 4 + 2 + 2 + (withValid ? 4 : 0);

    public RtpcV03ContainerHeader()
    {
        NameHash = 0;
        BodyOffset = 0;
        PropertyCount = 0;
        ContainerCount = 0;
    }
}

public static class RtpcV03ContainerHeaderExtension
{
    public static void LookupNameHash(this ref RtpcV03ContainerHeader header)
    {
        header.Name = HashUtils.Lookup(header.NameHash);
    }
    
    public static RtpcV03ContainerHeader ReadRtpcV03ContainerHeader(this BinaryReader br)
    {
        var result = new RtpcV03ContainerHeader
        {
            NameHash = br.ReadUInt32(),
            BodyOffset = br.ReadUInt32(),
            PropertyCount = br.ReadUInt16(),
            ContainerCount = br.ReadUInt16()
        };

        return result;
    }
    
    public static void Write(this BinaryWriter bw, RtpcV03ContainerHeader header)
    {
        bw.Write(header.NameHash);
        bw.Write(header.BodyOffset);
        bw.Write(header.PropertyCount);
        bw.Write(header.ContainerCount);
    }
}