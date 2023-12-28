namespace ApexFormat.RTPC.V03.Flat.Abstractions;

public interface IApexFile : IFromApex, IToApex
{
    public string ApexExtension { get; set; }
}