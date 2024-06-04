using System.Xml;
using ApexFormat.ADF.V04.Models.Types;

namespace ApexFormat.ADF.V04.Abstractions;

public abstract class AStructureTypeDefV04 : ATypeDefV04, ITypeDefV04
{
    public Dictionary<uint, ATypeDefV04> TypeDefs { get; set; } = new();

    public override EVariantType VariantType => EVariantType.Structure;
    public abstract uint Size { get; set; }
    public abstract uint Alignment { get; set; }
    public abstract uint TypeHash { get; set; }
    public abstract ulong NameHash { get; set; }
    public abstract ushort Flags { get; set; }
    public abstract EScalarType ScalarType { get; set; }
    public abstract uint SubTypeHash { get; set; }
    
    
    #region TypeDefSerializable
    public abstract void LoadDefinitionFromApex(BinaryReader br);
    public abstract void LoadDefinitionFromXml(XmlWriter xw);
    #endregion
}