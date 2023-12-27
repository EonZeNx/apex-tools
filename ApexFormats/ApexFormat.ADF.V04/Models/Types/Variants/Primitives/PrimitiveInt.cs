namespace ApexFormat.ADF.V04.Models.Types.Variants.Primitives;

public class PrimitiveInt : PrimitiveBase<int>
{
    public override int Value { get; set; }

    
    public PrimitiveInt()
    {
        ScalarType = EScalarType.Signed;
    }

    
    #region ApexSerializable

    public override void FromApex(BinaryReader br)
    {
        Value = br.ReadInt32();
    }

    public override void ToApex(BinaryWriter bw)
    {
        bw.Write(Value);
    }
    
    #endregion
}