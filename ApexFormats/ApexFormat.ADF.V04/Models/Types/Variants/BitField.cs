using ApexFormat.ADF.V04.Abstractions;

namespace ApexFormat.ADF.V04.Models.Types.Variants;

public class BitField : TypeDefV04
{
    public BitField()
    {
        VariantType = EVariantType.BitField;
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