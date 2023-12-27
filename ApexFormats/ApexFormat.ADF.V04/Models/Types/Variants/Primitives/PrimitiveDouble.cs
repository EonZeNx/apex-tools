namespace ApexFormat.ADF.V04.Models.Types.Variants.Primitives;

public class PrimitiveDouble : PrimitiveBase<double>
{
    public override double Value { get; set; }

    
    public PrimitiveDouble()
    {
        ScalarType = EScalarType.Unsigned;
    }
    
    
    #region ApexSerializable

    public override void FromApex(BinaryReader br)
    {
        Value = br.ReadDouble();
    }

    public override void ToApex(BinaryWriter bw)
    {
        bw.Write(Value);
    }
    
    #endregion
}