using System.Runtime.InteropServices;

namespace ApexTools.JC4.RTPC.V03.Models.Data;

[StructLayout(LayoutKind.Sequential)]
public struct RtpcV03Container
{
    public RtpcV03ContainerHeader Header = new();
    public RtpcV03PropertyHeader[] PropertyHeaders = Array.Empty<RtpcV03PropertyHeader>();
    public RtpcV03ContainerHeader[] ContainerHeaders = Array.Empty<RtpcV03ContainerHeader>();

    public RtpcV03Container() {}
}

public static class RtpcV03ContainerExtension
{
    public static RtpcV03Container ReadRtpcV03Container(this BinaryReader br)
    {
        // Marshal.SizeOf<RtpcV03ContainerHeader>();
        var result = new RtpcV03Container
        {
            Header = br.ReadRtpcV03ContainerHeader()
        };

        result.PropertyHeaders = new RtpcV03PropertyHeader[result.Header.PropertyCount];
        for (var i = 0; i < result.Header.PropertyCount; i++)
        {
            result.PropertyHeaders[i] = br.ReadRtpcV03PropertyHeader();
        }

        result.ContainerHeaders = new RtpcV03ContainerHeader[result.Header.ContainerCount];
        for (var i = 0; i < result.Header.PropertyCount; i++)
        {
            result.ContainerHeaders[i] = br.ReadRtpcV03ContainerHeader();
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
    }
}