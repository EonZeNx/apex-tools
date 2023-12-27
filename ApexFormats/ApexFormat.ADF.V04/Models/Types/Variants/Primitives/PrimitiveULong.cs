namespace ApexFormat.ADF.V04.Models.Types.Variants.Primitives;

public class PrimitiveULong : PrimitiveBase<ulong>
{
    public override ulong Value { get; set; }

    
    public PrimitiveULong()
    {
        ScalarType = EScalarType.Unsigned;
    }

    
    #region ApexSerializable

    public override void FromApex(BinaryReader br)
    {
        Value = br.ReadUInt64();
    }

    public override void ToApex(BinaryWriter bw)
    {
        bw.Write(Value);
    }
    
    #endregion
}