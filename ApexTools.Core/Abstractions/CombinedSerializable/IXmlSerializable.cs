using ApexTools.Core.Abstractions.Serializable;

namespace ApexTools.Core.Abstractions.CombinedSerializable;

public interface IXmlSerializable : IFromXmlSerializable, IToXmlSerializable
{
}