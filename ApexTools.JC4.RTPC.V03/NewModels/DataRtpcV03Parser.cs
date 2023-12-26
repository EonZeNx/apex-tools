namespace ApexTools.JC4.RTPC.V03.NewModels;

public static class DataRtpcV03Parser
{
    public static RtpcV03File FromApex(string targetPath)
    {
        using var br = new BinaryReader(new FileStream(targetPath, FileMode.Open));
        
        var rtpcV03 = new RtpcV03File
        {
            ApexExtension = Path.GetExtension(targetPath)
        };

        rtpcV03.FromApex(br);
            
        return rtpcV03;
    }
    
    public static void ToXml(RtpcV03File rtpcV03, string targetPath)
    {
        rtpcV03.ToXml($"{targetPath}.xml");
    }
    
    public static RtpcV03File FromXml(string targetPath)
    {
        var rtpcV03 = new RtpcV03File();
        
        // using var xr = XmlReader.Create(targetPath);
        // rtpcV03.FromXml(xr);
        
        rtpcV03.FromXml(targetPath);

        return rtpcV03;
    }
    
    public static void ToApex(RtpcV03File rtpcV03, string targetPath)
    {
        using var bw = new BinaryWriter(new FileStream($"{targetPath}{rtpcV03.ApexExtension}", FileMode.Create));
        rtpcV03.ToApex(bw);
    }
}