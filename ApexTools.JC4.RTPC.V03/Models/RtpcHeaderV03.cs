using ApexTools.JC4.RTPC.V03.Abstractions;
using EonZeNx.ApexTools.Core;
using EonZeNx.ApexTools.Core.Utils;

namespace ApexTools.JC4.RTPC.V03.Models;

/// <summary>
/// Format:<br/>
/// FourCc - <see cref="EFourCc"/><br/>
/// Version - <see cref="uint"/><br/>
/// </summary>
public class RtpcHeaderV03 : IBinarySize, IFromApexHeader, IToApexHeader
{
    public EFourCc FourCc = EFourCc.Rtpc;
    public uint Version = 3;
    
    public static int BinarySize => 4 + 4;

    public void FromApexHeader(BinaryReader br)
    {
        FourCc = (EFourCc) br.ReadUInt32();
        Version = br.ReadUInt32();
    }

    public void ToApexHeader(BinaryWriter bw)
    {
        bw.Write(FourCc.ToBigEndian());
        bw.Write(Version);
    }
}