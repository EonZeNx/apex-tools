using System.Runtime.InteropServices;
using EonZeNx.ApexTools.Core;

namespace ApexTools.JC4.RTPC.V03.NewModels.Data;

[StructLayout(LayoutKind.Sequential)]
public struct RtpcV03Header
{
    public EFourCc FourCc = EFourCc.Rtpc;
    public uint Version = 3;

    public RtpcV03Header() {}
}

public static class RtpcV03HeaderExtension
{
    public static RtpcV03Header ReadRtpcV03Header(this BinaryReader br)
    {
        var result = new RtpcV03Header
        {
            FourCc = (EFourCc) br.ReadUInt32(),
            Version = br.ReadUInt32()
        };

        return result;
    }
    
    public static void Write(this BinaryWriter bw, RtpcV03Header header)
    {
        bw.Write((uint) header.FourCc);
        bw.Write(header.Version);
    }
}