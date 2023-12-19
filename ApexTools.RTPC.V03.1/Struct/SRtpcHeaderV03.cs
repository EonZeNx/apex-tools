using ApexTools.RTPC.V03._1.Abstractions;
using EonZeNx.ApexTools.Core;

namespace ApexTools.RTPC.V03._1.Struct;

/// <summary>
/// Format:<br/>
/// FourCc - <see cref="EFourCc"/><br/>
/// Version - <see cref="uint"/><br/>
/// </summary>
public class SRtpcHeaderV03 : IFromApexHeader
{
    public EFourCc FourCc = EFourCc.Rtpc;
    public uint Version = 3;

    public void FromApexHeader(BinaryReader br)
    {
        FourCc = (EFourCc)br.ReadUInt32();
        Version = br.ReadUInt32();
    }
}