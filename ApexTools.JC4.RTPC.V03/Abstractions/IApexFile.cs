namespace ApexTools.JC4.RTPC.V03.Abstractions;

public interface IApexFile : IFromApex, IToApex
{
    public string ApexExtension { get; set; }
}