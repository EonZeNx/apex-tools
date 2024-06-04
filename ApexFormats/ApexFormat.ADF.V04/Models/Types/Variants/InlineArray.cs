namespace ApexFormat.ADF.V04.Models.Types.Variants;

public class InlineArray : TypeDefV04
{
    public InlineArray()
    {
        VariantType = EVariantType.InlineArray;
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