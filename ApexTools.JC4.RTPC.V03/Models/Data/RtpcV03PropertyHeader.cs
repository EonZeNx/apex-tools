using System.Runtime.InteropServices;
using EonZeNx.ApexFormats.RTPC.V03.Models.Properties;

namespace ApexTools.JC4.RTPC.V03.Models.Data;

[StructLayout(LayoutKind.Sequential)]
public struct RtpcV03PropertyHeader
{
    public uint NameHash = 0;
    public byte[] RawData = new byte[4];
    public EVariantType VariantType = EVariantType.Unassigned;

    public RtpcV03PropertyHeader(){}
}

public static class RtpcV03PropertyHeaderExtension
{
    public static RtpcV03PropertyHeader ReadRtpcV03PropertyHeader(this BinaryReader br)
    {
        var result = new RtpcV03PropertyHeader
        {
            NameHash = br.ReadUInt32(),
            RawData = br.ReadBytes(4),
            VariantType = (EVariantType) br.ReadByte()
        };

        return result;
    }
    
    public static void Write(this BinaryWriter bw, RtpcV03PropertyHeader header)
    {
        bw.Write(header.NameHash);
        bw.Write(header.RawData);
        bw.Write((byte) header.VariantType);
    }
}