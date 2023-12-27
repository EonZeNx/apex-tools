using System.Text;
using System.Xml;
using ApexTools.Core.Utils;

namespace ApexFormat.IRTPC.V01.Models.Properties.Variants;

public class Str : BasePropertyV01
{
    public override string XmlName => "String";
    
    protected override EVariantType VariantType => EVariantType.String;
    protected string Value { get; set; } = string.Empty;


    public Str() { }
    public Str(PropertyHeaderV01 propertyHeaderV01) : base(propertyHeaderV01) { }
    
    
    #region ApexSerializable

    public override void FromApex(BinaryReader br)
    {
        var length = br.ReadUInt16();
        var byteString = new byte[length];
        
        for (var i = 0; i < length; i++)
        {
            byteString[i] = br.ReadByte();
        }
        
        Value = Encoding.UTF8.GetString(byteString);
    }

    public override void ToApex(BinaryWriter bw)
    {
        base.ToApex(bw);
        bw.Write((ushort) Value.Length);
        bw.Write(Encoding.UTF8.GetBytes(Value));
    }
    
    #endregion

    
    #region XmlSerializable

    public override void FromXml(XmlReader xr)
    {
        NameHash = XmlUtils.ReadNameIfValid(xr);
        Value = xr.ReadElementContentAsString();
    }

    public override void ToXml(XmlWriter xw)
    {
        xw.WriteStartElement(XmlName);
        xw.WriteAttributeString("Offset", ByteUtils.ToHex((uint) Offset));
            
        // Write Name if valid
        XmlUtils.WriteNameOrNameHash(xw, NameHash, Name);
            
        xw.WriteValue(Value);
        xw.WriteEndElement();
    }

    #endregion
}