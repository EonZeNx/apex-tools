using ApexTools.JC4.RTPC.V03.Abstractions.New;
using ApexTools.JC4.RTPC.V03.Struct;
using EonZeNx.ApexTools.Core;

namespace ApexTools.JC4.RTPC.V03.Flat;

/// <summary>
/// Format:<br/>
/// Header - <see cref="SRtpcHeaderV03"/><br/>
/// Container - <see cref="SContainerV03"/><br/>
/// Variant value maps
/// </summary>
public class RtpcHeaderV03 : FileHeader, IFileHeaderStatic
{
    public static EFourCc FourCc => EFourCc.Rtpc;
    public static int Version => 3;
}