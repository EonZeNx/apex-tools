using System.Xml;

namespace ApexFormat.ADF.V04.Models.Types.Variants;

public class Structure : TypeDefV04
{
    public uint MemberCount { get; set; }


    public Structure()
    {
        VariantType = EVariantType.Structure;
    }


    #region ApexSerializable

    public override void FromApex(BinaryReader br)
    {
        throw new NotImplementedException();
    }

    public override void ToApex(BinaryWriter bw)
    {
        throw new NotImplementedException();
    }

    #endregion
    
    
    #region LoadDefinitions
    
    public override void LoadDefinitionFromApex(BinaryReader br)
    {
        MemberCount = br.ReadUInt32();
    }

    public override void LoadDefinitionFromXml(XmlWriter xw)
    {
        throw new NotImplementedException();
    }
    
    #endregion
}