namespace EonZeNx.ApexFormats.ADF.V04.Models.Types.Variants.Primitives;

public class PrimitiveShort : PrimitiveBase<short>
{
    public override short Value { get; set; }

    
    public PrimitiveShort()
    {
        ScalarType = EScalarType.Signed;
    }

    
    #region ApexSerializable

    public override void FromApex(BinaryReader br)
    {
        Value = br.ReadInt16();
    }

    public override void ToApex(BinaryWriter bw)
    {
        bw.Write(Value);
    }
    
    #endregion
}