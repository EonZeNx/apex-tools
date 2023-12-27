using ApexTools.Core.Abstractions.Serializable;

namespace ApexFormat.RTPC.V03.Models.Properties;

public abstract class PropertyBaseDeferredV03 : PropertyBaseV03, IToApexSerializableDeferred
{
    public long Offset { get; set; }
    public abstract int Alignment { get; }
    
    public abstract void ToApexDeferred(BinaryWriter bw);
}