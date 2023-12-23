using System.Xml;
using EonZeNx.ApexFormats.Debug.IRTPC.V01.Models.Properties.CustomArrays;
using EonZeNx.ApexTools.Core.Utils;

namespace EonZeNx.ApexFormats.Debug.IRTPC.V01.Models.Properties.Variants;

public class F32Array : BaseArray<float>
{
    public override string XmlName => "FloatArray";
    public override EVariantType VariantType => EVariantType.Float32Array;
    public override float[] Values { get; set; } = Array.Empty<float>();
    
    
    public F32Array() { }


    #region ApexSerializable

    public override void FromApex(BinaryReader br)
    {
        var length = Count;
        if (length == -1) length = br.ReadInt32();
        
        var values = new float[length];
        for (var i = 0; i < length; i++)
        {
            values[i] = br.ReadSingle();
        }

        Values = values;
    }

    #endregion
    

    #region XmlSerializable
    
    public override void FromXml(XmlReader xr)
    {
        NameHash = XmlUtils.ReadNameIfValid(xr);
            
        var floatString = xr.ReadElementContentAsString();
        if (floatString.Length == 0)
        {
            Values = Array.Empty<float>();
            return;
        }
            
        var floats = floatString.Split(",");
        Values = Array.ConvertAll(floats, float.Parse);
    }
    
    #endregion
}