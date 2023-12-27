using System.Xml;
using ApexTools.Core.Utils;

namespace ApexFormat.IRTPC.V01.Debug.Models.Properties.Variants;

public class Mat3X4 : F32Array
{
    public override string XmlName => "Mat3x4";
    public override EVariantType VariantType => EVariantType.Mat3X4;
    
    public override int Count { get; set; } = 12;


    public Mat3X4() { }


    public override void ToXml(XmlWriter xw)
    {
        xw.WriteStartElement(XmlName);
            
        // Write Name if valid
        XmlUtils.WriteNameOrNameHash(xw, NameHash, Name);

        var strArray = new string[3];
        for (var i = 0; i < strArray.Length; i++)
        {
            var startIndex = i * 4;
            var endIndex = (i + 1) * 4;
            var values = Values[startIndex..endIndex];
            strArray[i] = string.Join(",", values);
        }
        xw.WriteValue(string.Join(", ", strArray));
        xw.WriteEndElement();
    }
    
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
            
        Values = floats.ToArray();
    }
}