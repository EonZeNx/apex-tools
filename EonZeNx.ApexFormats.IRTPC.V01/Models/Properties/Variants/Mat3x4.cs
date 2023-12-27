using System.Xml;
using EonZeNx.ApexTools.Core.Utils;

namespace EonZeNx.ApexFormats.IRTPC.V01.Models.Properties.Variants;

public class Mat3X4 : FloatArray
{
    public override string XmlName => "Max3x4";
    protected override EVariantType VariantType => EVariantType.Mat3X4;
    
    public Mat3X4() { }
    public Mat3X4(PropertyHeaderV01 propertyHeaderV01) : base(propertyHeaderV01)
    {
        Num = 12;
    }


    #region XmlSerializable

    public override void FromXml(XmlReader xr)
    {
        NameHash = XmlUtils.ReadNameIfValid(xr);
            
        var floatString = xr.ReadElementContentAsString();
        var vectorString = floatString.Split(", ");

        var floats = new List<float>();
        foreach (var vector in vectorString)
        {
            var vecStr = vector.Split(",");
            var vecFloats = Array.ConvertAll(vecStr, float.Parse);
                
            floats.AddRange(vecFloats);
        }
            
        Value = floats.ToArray();
    }

    public override void ToXml(XmlWriter xw)
    {
        xw.WriteStartElement(XmlName);
        xw.WriteAttributeString("Offset", ByteUtils.ToHex((uint) Offset));
            
        // Write Name if valid
        XmlUtils.WriteNameOrNameHash(xw, NameHash, Name);

        var strArray = new string[3];
        for (var i = 0; i < strArray.Length; i++)
        {
            var startIndex = i * 4;
            var endIndex = (i + 1) * 4;
            var values = Value[startIndex..endIndex];
            strArray[i] = string.Join(",", values);
        }
        xw.WriteValue(string.Join(", ", strArray));
        xw.WriteEndElement();
    }

    #endregion
}