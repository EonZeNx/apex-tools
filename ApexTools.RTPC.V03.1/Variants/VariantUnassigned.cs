using System.Xml;
using ApexTools.RTPC.V03._1.Struct;
using EonZeNx.ApexTools.Core.Utils;

namespace ApexTools.RTPC.V03._1.Variants;

public class VariantUnassigned : SPropertyV03
{
    public override string XmlName => "Un";

    public VariantUnassigned() { }
    public VariantUnassigned(SPropertyHeaderV03 header) : base(header)
    { }

    public override void FromApex(BinaryReader br)
    {}

    public override void ToXml(XmlWriter xw)
    {
        xw.WriteStartElement(XmlName);
        
        XmlUtils.WriteNameOrNameHash(xw, Header.NameHash, Header.Name);

        xw.WriteEndElement();
    }
}