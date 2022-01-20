using System.Xml;
using EonZeNx.ApexTools.Core.Abstractions.CombinedSerializable;

namespace EonZeNx.ApexFormats.ADF.V04.Abstractions;

public interface ITypeDefV04
{
    #region TypeDefSerializable
    void LoadDefinitionFromApex(BinaryReader br);
    void LoadDefinitionFromXml(XmlWriter xw);
    #endregion
}