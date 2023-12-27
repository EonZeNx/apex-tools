namespace ApexFormat.ADF.V04.Models.Types.Variants.Primitives;

public class PrimitiveByte : PrimitiveBase<byte>
{
    public override byte Value { get; set; }

    public PrimitiveByte()
    {
        ScalarType = EScalarType.Unsigned;
    }
    
    #region ApexSerializable

    public override void FromApex(BinaryReader br)
    {
        Value = br.ReadByte();
    }

    public override void ToApex(BinaryWriter bw)
    {
        bw.Write(Value);
    }
    
    #endregion
}