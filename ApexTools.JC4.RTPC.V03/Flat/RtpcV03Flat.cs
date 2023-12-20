using ApexTools.JC4.RTPC.V03.Abstractions.New;
using ApexTools.JC4.RTPC.V03.Struct;

namespace ApexTools.JC4.RTPC.V03.Flat;

/// <summary>
/// Format:<br/>
/// Header - <see cref="SRtpcHeaderV03"/><br/>
/// Container - <see cref="SContainerV03"/><br/>
/// Variant value maps
/// </summary>
public class RtpcV03Flat : AFile
{
    public override string Extension { get; set; }
    public override RtpcHeaderV03 Header => new();
}