using ApexTools.Core.Abstractions.Serializable;

namespace ApexTools.Core.Abstractions.CombinedSerializable;

public interface IApexSerializable : IFromApexSerializable, IToApexSerializable
{
}