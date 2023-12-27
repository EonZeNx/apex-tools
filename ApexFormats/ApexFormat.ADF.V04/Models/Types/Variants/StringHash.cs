using ApexFormat.ADF.V04.Abstractions;
using ApexTools.Core.Utils;

namespace ApexFormat.ADF.V04.Models.Types.Variants;

public class StringHash : TypeDefV04
{
    public StringHash()
    {
        VariantType = EVariantType.StringHash;
    }

    public string Value { get; set; } = string.Empty;
    public ulong Hash { get; set; }


    #region ApexSerializable

    public override void FromApex(BinaryReader br)
    {
        Value = br.ReadStringZ();
        Hash = br.ReadUInt64();
    }

    public override void ToApex(BinaryWriter bw)
    {
        bw.Write(Value);
        bw.Write(Hash);
    }

    #endregion
}