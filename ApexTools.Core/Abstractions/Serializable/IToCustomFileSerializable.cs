namespace ApexTools.Core.Abstractions.Serializable;

public interface IToCustomFileSerializable
{
    public void ToCustomFile(BinaryWriter bw);
}