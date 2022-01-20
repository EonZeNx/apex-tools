using EonZeNx.ApexFormats.ADF.V04.Abstractions;

namespace EonZeNx.ApexFormats.ADF.V04.Models.Types.Variants;

public class Deferred : TypeDefV04
{
    public Deferred()
    {
        VariantType = EVariantType.Deferred;
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