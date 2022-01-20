namespace EonZeNx.ApexFormats.ADF.V04.Models.Types.Variants.Primitives;

public class PrimitiveLong : PrimitiveBase<long>
{
    public override long Value { get; set; }

    
    public PrimitiveLong()
    {
        ScalarType = EScalarType.Signed;
    }

    
    #region ApexSerializable

    public override void FromApex(BinaryReader br)
    {
        Value = br.ReadInt64();
    }

    public override void ToApex(BinaryWriter bw)
    {
        bw.Write(Value);
    }
    
    #endregion
}