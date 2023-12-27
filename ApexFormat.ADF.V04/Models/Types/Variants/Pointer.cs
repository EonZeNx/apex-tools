
namespace ApexFormat.ADF.V04.Models.Types.Variants;

public class Pointer : TypeDefV04
{
    public Pointer()
    {
        VariantType = EVariantType.Pointer;
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