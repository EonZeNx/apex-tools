using System.Xml;
using ApexTools.JC4.RTPC.V03.Abstractions;
using EonZeNx.ApexFormats.RTPC.V03.Models.Properties;
using EonZeNx.ApexTools.Config;
using EonZeNx.ApexTools.Core.Utils;
using PropertyHeaderV03 = ApexTools.JC4.RTPC.V03.Models.PropertyHeaderV03;

namespace ApexTools.JC4.RTPC.V03.Variants;

public class VariantUnassigned : APropertyV03
{
    public override string XmlName => "Un";

    public VariantUnassigned()
    {
        Header.VariantType = EVariantType.Unassigned;
    }
    public VariantUnassigned(PropertyHeaderV03 header) : base(header)
    { }

    public override void FromApex(BinaryReader br)
    {}

    public override void ToXml(XmlWriter xw)
    {
        if (Settings.SkipUnassignedRtpcProperties.Value) return;
        
        xw.WriteStartElement(XmlName);
        
        XmlUtils.WriteNameOrNameHash(xw, Header.HexNameHash, Header.Name);

        xw.WriteEndElement();
    }

    public override void FromXml(XmlReader xr)
    {
        Header.NameHash = ByteUtils.ReverseBytes(XmlUtils.ReadNameIfValid(xr));
    }

    public override void ToApex(BinaryWriter bw)
    {}
}