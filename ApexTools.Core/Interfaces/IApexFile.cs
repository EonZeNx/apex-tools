namespace ApexTools.Core.Interfaces;

public interface IApexFile : IFromApex, IToApex
{
    public string ApexExtension { get; set; }
}