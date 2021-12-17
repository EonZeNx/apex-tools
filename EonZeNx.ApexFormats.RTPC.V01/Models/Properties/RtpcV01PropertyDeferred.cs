using System.Xml;
using EonZeNx.ApexTools.Core.Abstractions.CombinedSerializable;
using EonZeNx.ApexTools.Core.Abstractions.Serializable;

namespace EonZeNx.ApexFormats.RTPC.V01.Models.Properties;

public abstract class RtpcV01PropertyDeferred : RtpcV01PropertyBase, IToApexSerializableDeferred
{
    public long Offset { get; set; }
    public abstract int Alignment { get; }
    
    public abstract void ToApexDeferred(BinaryWriter bw);
}