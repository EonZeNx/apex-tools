namespace ApexFormat.ADF.V04.Models.Types.Variants.Primitives;

public class PrimitiveUShort : PrimitiveBase<ushort>
{
    public override ushort Value { get; set; }

    
    public PrimitiveUShort()
    {
        ScalarType = EScalarType.Unsigned;
    }

    
    #region ApexSerializable

    public override void FromApex(BinaryReader br)
    {
        Value = br.ReadUInt16();
    }

    public override void ToApex(BinaryWriter bw)
    {
        bw.Write(Value);
    }
    
    #endregion
}