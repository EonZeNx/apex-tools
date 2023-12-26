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
        var targetFilePath = Path.GetDirectoryName(targetPath);
        var targetFileName = Path.GetFileNameWithoutExtension(targetPath);

        var targetXmlFilePath = Path.Join(targetFilePath, $"{targetFileName}.xml");
        rtpcV03.ToXml(targetXmlFilePath);
    }
    
    public static RtpcV03File FromXml(string targetPath)
    {
        var rtpcV03 = new RtpcV03File();
        
        rtpcV03.FromXml(targetPath);

        return rtpcV03;
    }
    
    public static void ToApex(RtpcV03File rtpcV03, string targetPath)
    {
        var targetFilePath = Path.GetDirectoryName(targetPath);
        var targetFileName = Path.GetFileNameWithoutExtension(targetPath);

        var targetApexFilePath = Path.Join(targetFilePath, $"{targetFileName}{rtpcV03.ApexExtension}");
        using var bw = new BinaryWriter(new FileStream(targetApexFilePath, FileMode.Create));
        
        rtpcV03.ToApex(bw);
    }
}