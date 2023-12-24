using System.Runtime.InteropServices;

namespace ApexTools.JC4.RTPC.V03.NewModels.Data;

[StructLayout(LayoutKind.Sequential)]
public struct RtpcV03Container
{
    public RtpcV03ContainerHeader Header = new();
    public RtpcV03PropertyHeader[] PropertyHeaders = Array.Empty<RtpcV03PropertyHeader>();
    public RtpcV03ContainerHeader[] ContainerHeaders = Array.Empty<RtpcV03ContainerHeader>();
    public uint ValidProperties = 0;

    public RtpcV03Container[] Containers = Array.Empty<RtpcV03Container>();

    public RtpcV03Container() {}
}

public static class RtpcV03ContainerExtension
{
    // Container header and body are separate
    public static RtpcV03Container ReadRtpcV03Container(this BinaryReader br, RtpcV03ContainerHeader header)
    {
        var result = new RtpcV03Container
        {
            Header = header,
            PropertyHeaders = new RtpcV03PropertyHeader[header.PropertyCount],
            ContainerHeaders = new RtpcV03ContainerHeader[header.ContainerCount],
            Containers = new RtpcV03Container[header.ContainerCount]
        };

        for (var i = 0; i < header.PropertyCount; i++)
        {
            result.PropertyHeaders[i] = br.ReadRtpcV03PropertyHeader();
        }

        for (var i = 0; i < header.ContainerCount; i++)
        {
            result.ContainerHeaders[i] = br.ReadRtpcV03ContainerHeader();
        }

        result.ValidProperties = br.ReadUInt32();

        for (var i = 0; i < header.ContainerCount; i++)
        {
            result.Containers[i] = br.ReadRtpcV03Container(result.ContainerHeaders[i]);
        }

        return result;
    }
    
    public static void Write(this BinaryWriter bw, RtpcV03Container container)
    {
        bw.Write(container.Header);

        foreach (var propertyHeader in container.PropertyHeaders)
        {
            bw.Write(propertyHeader);
        }

        foreach (var containerHeader in container.ContainerHeaders)
        {
            bw.Write(containerHeader);
        }
        
        bw.Write(container.ValidProperties);
    }
}