namespace EonZeNx.ApexFormats.ADF.V04.Models.Types.Variants.Primitives;

public class PrimitiveSByte : PrimitiveBase<sbyte>
{
    public override sbyte Value { get; set; }

    
    public PrimitiveSByte()
    {
        ScalarType = EScalarType.Signed;
    }

    
    #region ApexSerializable

    public override void FromApex(BinaryReader br)
    {
        Value = br.ReadSByte();
    }

    public override void ToApex(BinaryWriter bw)
    {
        bw.Write(Value);
    }
    
    #endregion
}