using System.Runtime.InteropServices;

namespace ApexTools.JC4.RTPC.V03.NewModels.Data;

[StructLayout(LayoutKind.Sequential)]
public struct RtpcV03ContainerHeader
{
    public uint NameHash = 0;
    public uint BodyOffset = 0;
    public ushort PropertyCount = 0;
    public ushort ContainerCount = 0;

    public RtpcV03ContainerHeader() {}
}

public static class RtpcV03ContainerHeaderExtension
{
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