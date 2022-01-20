using EonZeNx.ApexFormats.ADF.V04.Abstractions;
using EonZeNx.ApexTools.Core.Utils;

namespace EonZeNx.ApexFormats.ADF.V04.Models.Types.Variants;

public class AdfV04String : TypeDefV04
{
    public AdfV04String()
    {
        VariantType = EVariantType.String;
    }
    public string Value { get; set; } = string.Empty;


    #region ApexSerializable

    public override void FromApex(BinaryReader br)
    {
        var offset = br.ReadUInt32();
        var originalPosition = br.Position();
        
        br.BaseStream.Seek(offset, SeekOrigin.Begin);
        Value = br.ReadStringZ();

        br.BaseStream.Seek(originalPosition, SeekOrigin.Begin);
    }

    public override void ToApex(BinaryWriter bw)
    {
        throw new NotImplementedException();
    }

    #endregion
}