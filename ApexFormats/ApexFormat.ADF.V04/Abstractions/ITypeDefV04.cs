using System.Xml;
using ApexTools.Core.Abstractions.CombinedSerializable;

namespace ApexFormat.ADF.V04.Abstractions;

public interface ITypeDefV04
{
    #region TypeDefSerializable
    void LoadDefinitionFromApex(BinaryReader br);
    void LoadDefinitionFromXml(XmlWriter xw);
    #endregion
}