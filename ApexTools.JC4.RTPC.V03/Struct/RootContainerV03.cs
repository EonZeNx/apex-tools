using ApexTools.JC4.RTPC.V03.Abstractions;

namespace ApexTools.JC4.RTPC.V03.Struct;

/// <summary>
/// Format:<br/>
/// Header - <see cref="SContainerHeaderV03"/><br/>
/// Property headers - <see cref="JC4PropertyHeaderV03"/><br/>
/// Container headers - <see cref="SContainerHeaderV03"/><br/>
/// Valid property count - <see cref="uint"/>
/// </summary>
public class RootContainerV03 : IFromApexHeader, IFromApex
{
    public SContainerHeaderV03 Header = new();
    public JC4PropertyHeaderV03[] PropertyHeaders = Array.Empty<JC4PropertyHeaderV03>();
    public SContainerV03[] Containers = Array.Empty<SContainerV03>();
    public uint ValidPropertyCount = 0;

    public void FromApexHeader(BinaryReader br)
    {
        PropertyHeaders = new JC4PropertyHeaderV03[Header.PropertyCount];
        if (Header.PropertyCount != 0)
        {
            for (var i = 0; i < Header.PropertyCount; i++)
            {
                PropertyHeaders[i].FromApexHeader(br);
            }
        }

        Containers = new SContainerV03[Header.ContainerCount];
        if (Header.ContainerCount != 0)
        {
            for (var i = 0; i < Header.ContainerCount; i++)
            {
                Containers[i].Header.FromApexHeader(br);
            }
        }
        
        // Exclude unassigned values
        ValidPropertyCount = br.ReadUInt32();
        
        for (var i = 0; i < Header.ContainerCount; i++)
        {
            Containers[i].FromApexHeader(br);
        }
    }

    public void FromApex(BinaryReader br)
    {
        // TODO
    }
}