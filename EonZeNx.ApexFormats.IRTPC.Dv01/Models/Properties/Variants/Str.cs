using System.Text;
using System.Xml;
using EonZeNx.ApexTools.Core.Utils;

namespace EonZeNx.ApexFormats.Debug.IRTPC.V01.Models.Properties.Variants;

public class Str : PropertyBase
{
    public override string XmlName => "String";
    public override EVariantType VariantType => EVariantType.String;

    public static readonly Dictionary<string, long> StringMap = new();
    public string Value { get; set; } = "";


    public Str() { }


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
        Value = xr.ReadString();
    }

    public override void ToXml(XmlWriter xw)
    {
        xw.WriteStartElement(XmlName);
            
        // Write Name if valid
        XmlUtils.WriteNameOrNameHash(xw, NameHash, Name);
            
        xw.WriteValue(Value);
        xw.WriteEndElement();
    }

    #endregion
}