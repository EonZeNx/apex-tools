using ApexFormat.ADF.V04.Abstractions;

namespace ApexFormat.ADF.V04.Models.Types.Variants;

public class Enumeration : TypeDefV04
{
    public Enumeration()
    {
        VariantType = EVariantType.Enumeration;
    }


    #region ApexSerializable

    public override void FromApex(BinaryReader br)
    {
        throw new NotImplementedException();
    }

    public override void ToApex(BinaryWriter bw)
    {
        throw new NotImplementedException();
    }

    #endregion
}