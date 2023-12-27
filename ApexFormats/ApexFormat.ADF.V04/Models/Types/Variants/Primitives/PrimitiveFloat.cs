namespace ApexFormat.ADF.V04.Models.Types.Variants.Primitives;

public class PrimitiveFloat : PrimitiveBase<float>
{
    public override float Value { get; set; }

    
    public PrimitiveFloat()
    {
        ScalarType = EScalarType.Unsigned;
    }

    
    #region ApexSerializable

    public override void FromApex(BinaryReader br)
    {
        Value = br.ReadSingle();
    }

    public override void ToApex(BinaryWriter bw)
    {
        bw.Write(Value);
    }
    
    #endregion
}