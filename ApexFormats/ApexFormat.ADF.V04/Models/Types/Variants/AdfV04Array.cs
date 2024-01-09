namespace ApexFormat.ADF.V04.Models.Types.Variants;

public class AdfV04Array : TypeDefV04
{
    public AdfV04Array()
    {
        VariantType = EVariantType.Array;
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