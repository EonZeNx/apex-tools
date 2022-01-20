namespace EonZeNx.ApexFormats.ADF.V04.Models.Types.Variants.Primitives;

public class PrimitiveUInt : PrimitiveBase<uint>
{
    public override uint Value { get; set; }

    
    public PrimitiveUInt()
    {
        ScalarType = EScalarType.Unsigned;
    }

    
    #region ApexSerializable

    public override void FromApex(BinaryReader br)
    {
        Value = br.ReadUInt32();
    }

    public override void ToApex(BinaryWriter bw)
    {
        bw.Write(Value);
    }
    
    #endregion
}