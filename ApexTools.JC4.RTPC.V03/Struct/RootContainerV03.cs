using System.Xml;
using ApexTools.JC4.RTPC.V03.Abstractions;
using ApexTools.JC4.RTPC.V03.Exceptions;
using ApexTools.JC4.RTPC.V03.Variants;
using EonZeNx.ApexFormats.RTPC.V03.Models.Properties;
using EonZeNx.ApexTools.Core.Utils;
using EonZeNx.ApexTools.Core.Utils.Hash;

namespace ApexTools.JC4.RTPC.V03.Struct;

/// <summary>
/// Format:<br/>
/// Header - <see cref="SContainerHeaderV03"/><br/>
/// Property headers - <see cref="JC4PropertyHeaderV03"/><br/>
/// Container headers - <see cref="SContainerHeaderV03"/><br/>
/// Valid property count - <see cref="uint"/>
/// </summary>
public class RootContainerV03 : IFromApexHeader, IFromApex, IToXml, IFromXml, IToApexHeader, IToApex
{
    public SContainerHeaderV03 Header = new();
    public JC4PropertyHeaderV03[] PropertyHeaders = Array.Empty<JC4PropertyHeaderV03>();
    public APropertyV03[] Properties = Array.Empty<APropertyV03>();
    public SContainerV03[] Containers = Array.Empty<SContainerV03>();
    public uint ValidPropertyCount = 0;

    public Dictionary<ulong, uint> OIdToParentIndex = new();
    public List<ulong> OIdList = new();
    public uint Unknown01 = 0;
    
    public string XmlName => "Root";

    public void FromApexUnflatSetup()
    {
        if (Header.PropertyCount != 4)
        {
            throw new RtpcContainerException($"Expected 4 properties, got {Header.PropertyCount}");
        }
        
        // 3EB4D7CF = parent container index
        // 5A6E1D8F = container object id
        // A0DDE26A = container class
        
        var flatContainerIndices = PropertyHeaders.Any(ph => ph.NameHash == 0xCFD7B43E);
        var flatContainerObjectIds = PropertyHeaders.Any(ph => ph.NameHash == 0x8F1D6E5A);
        var unknownProperty = PropertyHeaders.Any(ph => ph.NameHash == 0x95C1191D);
        var flatContainerClassHash = PropertyHeaders.Any(ph => ph.NameHash == 0x6AE2DDA0);

        if (!flatContainerIndices)
        {
            throw new RtpcContainerException($"Missing index array property ${ByteUtils.ToHex(0xCFD7B43E)}");
        }
        if (!flatContainerObjectIds)
        {
            throw new RtpcContainerException($"Missing object id array property ${ByteUtils.ToHex(0x8F1D6E5A)}");
        }
        if (!unknownProperty)
        {
            throw new RtpcContainerException($"Missing unknown property 01 ${ByteUtils.ToHex(0x95C1191D)}");
        }
        if (!flatContainerClassHash)
        {
            throw new RtpcContainerException($"Missing class hash array property ${ByteUtils.ToHex(0x6AE2DDA0)}");
        }
        
        var indices = ((VariantU32Array) Properties.First(p => p.Header.NameHash == 0xCFD7B43E)).Value;
        var byteObjectIds = ((VariantByteArray) Properties.First(p => p.Header.NameHash == 0x8F1D6E5A)).Value;
        Unknown01 = ((VariantU32) Properties.First(p => p.Header.NameHash == 0x95C1191D)).Value;
        
        OIdToParentIndex = new Dictionary<ulong, uint>(indices.Count);
        for (var i = 0; i < byteObjectIds.Count; i += 8)
        {
            var parentIndex = indices[i / 8];
            var objectIdBytes = byteObjectIds.GetRange(i, 8).ToArray();
            var objectId = BitConverter.ToUInt64(objectIdBytes, 0);
            
            OIdToParentIndex.Add(objectId, parentIndex);
            OIdList.Add(objectId);
        }
    }
    
    public void FromApexUnflat()
    {
        var objectIdHash = HashJenkinsL3.Hash("_object_id"u8.ToArray());
        var toRemove = new List<SContainerV03>();
        
        foreach (var container in Containers)
        {
            var oid = container.GetObjectId(objectIdHash);
            var moveList = new List<int>();
            
            RecurseFlatFromApex(oid, ref moveList);

            if (moveList.Count == 0) continue;

            container.Flat = true;

            var parentContainer = new SContainerV03();
            foreach (var move in moveList)
            {
                parentContainer = Containers[move];
            }
            
            var newParentContainers = parentContainer.Containers.ToList();
            newParentContainers.Add(container);

            parentContainer.Containers = newParentContainers.ToArray();
            
            toRemove.Add(container);
        }

        var newContainers = Containers.ToList();
        newContainers.RemoveAll(c => toRemove.Contains(c));

        Containers = newContainers.ToArray();
    }

    public void RecurseFlatFromApex(ulong oid, ref List<int> moveList)
    {
        while (true)
        {
            if (!OIdToParentIndex.ContainsKey(oid))
            {
                return;
            }
            
            var parentIndex = OIdToParentIndex[oid];
            if (parentIndex >= OIdList.Count)
            {
                return;
            }

            moveList.Add((int) parentIndex);
            oid = OIdList[(int) parentIndex];
        }
    }

    #region IApex

    public void FromApexHeader(BinaryReader br)
    {
        PropertyHeaders = new JC4PropertyHeaderV03[Header.PropertyCount];
        if (Header.PropertyCount != 0)
        {
            for (var i = 0; i < Header.PropertyCount; i++)
            {
                PropertyHeaders[i] = new JC4PropertyHeaderV03();
                PropertyHeaders[i].FromApexHeader(br);
            }
        }

        Containers = new SContainerV03[Header.ContainerCount];
        for (var i = 0; i < Header.ContainerCount; i++)
        {
            Containers[i] = new SContainerV03();
            Containers[i].Header.FromApexHeader(br);
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
        Properties = new APropertyV03[Header.PropertyCount];
        for (var i = 0; i < Properties.Length; i++)
        {
            var propertyHeader = PropertyHeaders[i];
            Properties[i] = propertyHeader.VariantType switch
            {
                EVariantType.UInteger32 => new VariantU32(propertyHeader),
                EVariantType.Float32 => new VariantF32(propertyHeader),
                EVariantType.String => new VariantStr(propertyHeader),
                EVariantType.Vec2 => new VariantVec2(propertyHeader),
                EVariantType.Vec3 => new VariantVec3(propertyHeader),
                EVariantType.Vec4 => new VariantVec4(propertyHeader),
                EVariantType.Mat3X3 => new VariantMat3X3(propertyHeader),
                EVariantType.Mat4X4 => new VariantMat4X4(propertyHeader),
                EVariantType.UInteger32Array => new VariantU32Array(propertyHeader),
                EVariantType.Float32Array => new VariantF32Array(propertyHeader),
                EVariantType.ByteArray => new VariantByteArray(propertyHeader),
                EVariantType.ObjectId => new VariantObjectId(propertyHeader),
                EVariantType.Event => new VariantEvent(propertyHeader),
                EVariantType.Unassigned => new VariantUnassigned(propertyHeader),
                EVariantType.Deprecated => new VariantUnassigned(propertyHeader),
                EVariantType.Total => new VariantUnassigned(propertyHeader),
                _ => throw new ArgumentOutOfRangeException()
            };

            Properties[i].FromApex(br);
        }

        FromApexUnflatSetup();
        
        for (var i = 0; i < Header.ContainerCount; i++)
        {
            Containers[i].FromApex(br);
        }

        FromApexUnflat();
    }

    public void ToApexHeader(BinaryWriter bw)
    {
        for (var i = 0; i < Header.PropertyCount; i++)
        {
            PropertyHeaders[i].ToApexHeader(bw);
        }
        
        for (var i = 0; i < Header.ContainerCount; i++)
        {
            Containers[i].Header.ToApexHeader(bw);
        }

        var validProperties = PropertyHeaders
            .Where(ph => ph.VariantType != EVariantType.Unassigned)
            .ToArray();
        bw.Write(validProperties.Length);
        
        for (var i = 0; i < Header.ContainerCount; i++)
        {
            Containers[i].ToApexHeader(bw);
        }
    }

    public void ToApex(BinaryWriter bw)
    {
        throw new NotImplementedException();
    }

    #endregion

    #region IXml

    public void ToXml(XmlWriter xw)
    {
        xw.WriteStartElement(XmlName);
        
        XmlUtils.WriteNameOrNameHash(xw, Header.HexNameHash, Header.Name);
        
        xw.WriteAttributeString("Flat", $"{true}");
        xw.WriteAttributeString(nameof(Unknown01), $"{Unknown01}");

        foreach (var container in Containers)
        {
            container.ToXml(xw);
        }
            
        xw.WriteEndElement();
    }

    public void FromXml(XmlReader xr)
    {
        Header.NameHash = XmlUtils.ReadNameIfValid(xr);
        Unknown01 = uint.Parse(xr.GetAttribute(nameof(Unknown01)) ?? "0");

        while (xr.Read())
        { if (xr.NodeType != XmlNodeType.Whitespace) break; }
            
        ContainersFromXml(xr);
    }

    #endregion
    
    #region XmlHelpers

    private void ContainersFromXml(XmlReader xr)
    {
        var containers = new List<SContainerV03>();

        do
        {
            var tag = xr.Name;
            var nodeType = xr.NodeType;
                
            if (tag == XmlName && nodeType == XmlNodeType.EndElement) break;
            if (nodeType != XmlNodeType.Element) continue;
                
            if (!xr.HasAttributes) throw new XmlException("Container missing attributes");

            var container = new SContainerV03();

            container.FromXml(xr);
            containers.Add(container);
        } 
        while (xr.Read());

        Containers = containers.ToArray();
        Header.ContainerCount = (ushort) Containers.Length;
    }

    #endregion
}