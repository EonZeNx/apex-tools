using System.Xml;
using EonZeNx.ApexFormats.RTPC.V03.Models.Properties.CustomArrays;
using EonZeNx.ApexTools.Core.Utils;

namespace EonZeNx.ApexFormats.RTPC.V03.Models.Properties.Variants;

public class FloatArray : BaseArray<float>
{
    public override string XmlName => "FloatArray";
    public override EVariantType VariantType => EVariantType.Float32Array;
    public override bool Primitive => false;
    public override int Alignment => 4;
    public override float[] Values { get; set; } = Array.Empty<float>();
    
    
    public FloatArray() { }
    public FloatArray(PropertyHeaderV03 header) : base(header) { }


    #region ApexSerializable

    public override void FromApex(BinaryReader br)
    {
        var dataOffset = BitConverter.ToUInt32(RawData);
            
        br.BaseStream.Seek(dataOffset, SeekOrigin.Begin);

        var length = Count;
        if (length == -1) length = br.ReadInt32();
        
        var values = new float[length];
        for (var i = 0; i < length; i++)
        {
            values[i] = br.ReadSingle();
        }

        Values = values;
    }

    public override void ToApexDeferred(BinaryWriter bw)
    {
        base.ToApexDeferred(bw);
        foreach (var value in Values)
        {
            bw.Write(value);
        }
    }

    #endregion
    

    #region XmlSerializable
    
    public override void FromXml(XmlReader xr)
    {
        NameHash = XmlUtils.ReadNameIfValid(xr);
            
        var floatString = xr.ReadString();
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