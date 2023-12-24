using ApexTools.JC4.RTPC.V03.Abstractions;

namespace ApexTools.JC4.RTPC.V03.NewModels;

public interface IApexFile : IFromApex, IToApex
{
    public string ApexExtension { get; set; }
}