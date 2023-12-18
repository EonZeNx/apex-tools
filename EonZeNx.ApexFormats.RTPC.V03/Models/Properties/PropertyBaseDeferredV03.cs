using EonZeNx.ApexTools.Core.Abstractions.Serializable;

namespace EonZeNx.ApexFormats.RTPC.V03.Models.Properties;

public abstract class PropertyBaseDeferredV03 : PropertyBaseV03, IToApexSerializableDeferred
{
    public long Offset { get; set; }
    public abstract int Alignment { get; }
    
    public abstract void ToApexDeferred(BinaryWriter bw);
}