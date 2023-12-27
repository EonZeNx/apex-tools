namespace ApexFormat.RTPC.V03.JC4.Abstractions;

public interface IApexFile : IFromApex, IToApex
{
    public string ApexExtension { get; set; }
}