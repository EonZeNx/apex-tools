namespace ApexTools.Core.Abstractions.Serializable;

public interface IToApexSerializableDeferred
{
    public void ToApexDeferred(BinaryWriter bw);
}