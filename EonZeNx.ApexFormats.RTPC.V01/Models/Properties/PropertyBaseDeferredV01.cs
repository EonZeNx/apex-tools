using EonZeNx.ApexTools.Core.Abstractions.Serializable;

namespace EonZeNx.ApexFormats.RTPC.V01.Models.Properties;

public abstract class PropertyBaseDeferredV01 : PropertyBaseV01, IToApexSerializableDeferred
{
    public long Offset { get; set; }
    public abstract int Alignment { get; }
    
    public abstract void ToApexDeferred(BinaryWriter bw);
}