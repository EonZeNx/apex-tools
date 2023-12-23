using System.Xml;
using ApexTools.JC4.RTPC.V03.Abstractions;
using ApexTools.JC4.RTPC.V03.Exceptions;
using ApexTools.JC4.RTPC.V03.Variants;
using EonZeNx.ApexFormats.RTPC.V03.Models.Properties;
using EonZeNx.ApexTools.Core.Utils;
using EonZeNx.ApexTools.Core.Utils.Hash;

namespace ApexTools.JC4.RTPC.V03.Models;

public class RootContainerV03 : ContainerV03
{
    public Dictionary<ulong, uint> OIdToParentIndex = new();
    public List<ulong> OIdList = new();
    public uint Unknown01 = 0;
    
    public override string XmlName => "Root";

    public RootContainerV03()
    {
        Header = new ContainerHeaderV03
        {
            BodyOffset = 0x14
        };
        Flat = true;
    }

    #region Unflat

    public void UnflatSetup()
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
    
    public void Unflat()
    {
        var objectIdHash = HashJenkinsL3.Hash("_object_id"u8.ToArray());
        var toRemove = new List<ContainerV03>();
        
        foreach (var container in Containers)
        {
            var oid = container.GetObjectId(objectIdHash);
            var moveList = new List<int>();
            
            UnflatRecurse(oid, ref moveList);

            if (moveList.Count == 0) continue;

            container.Flat = true;

            var parentContainer = new ContainerV03();
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

    public void UnflatRecurse(ulong oid, ref List<int> moveList)
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

    #endregion

    #region Flatten

    public void Flatten()
    {
        CreateFlatContainers();
        CreateFlatProperties();
    }

    public void CreateFlatContainers()
    {
        uint flatCount = 0;
        foreach (var container in Containers)
        {
            flatCount += container.GetContainerCountFlat();
        }

        for (uint i = 0; i < flatCount; i++)
        {
            i += FlattenRecurse(Containers[i], i, true);
        }
        
        var strappedContainers = new List<ContainerV03>(Containers);
        foreach (var container in Containers)
        {
            var containersFlat = container.GetAllContainersFlat();
            strappedContainers.AddRange(containersFlat);
        }

        Containers = strappedContainers.ToArray();
        Header.ContainerCount = (ushort) Containers.Length;
    }
    
    public void CreateFlatProperties()
    {
        PropertyHeaders = new PropertyHeaderV03[4];
        Properties = new APropertyV03[4];
        
        var orderedIndices = new List<uint>(OIdList.Count);
        orderedIndices.AddRange(OIdList.Select(oid => OIdToParentIndex[oid]));
        
        // Indices
        PropertyHeaders[0] = new PropertyHeaderV03
        {
            NameHash = 0xCFD7B43E,
            RawData = new byte[4],
            VariantType = EVariantType.UInteger32Array
        };
        Properties[0] = new VariantU32Array
        {
            Header = PropertyHeaders[0],
            Value = orderedIndices,
            Count = (ushort) orderedIndices.Count
        };

        // Flat object ID array (byte array)
        var oidByteArray = new List<byte>(OIdList.Count * 8);
        foreach (var oid in OIdList)
        {
            oidByteArray.AddRange(BitConverter.GetBytes(oid));
        }
        
        PropertyHeaders[1] = new PropertyHeaderV03
        {
            NameHash = 0x8F1D6E5A,
            RawData = new byte[4],
            VariantType = EVariantType.ByteArray
        };
        Properties[1] = new VariantByteArray
        {
            Header = PropertyHeaders[1],
            Value = oidByteArray,
            Count = (ushort) oidByteArray.Count
        };

        // ???
        PropertyHeaders[2] = new PropertyHeaderV03
        {
            NameHash = 0x95C1191D,
            RawData = BitConverter.GetBytes(Unknown01),
            VariantType = EVariantType.UInteger32
        };
        Properties[2] = new VariantU32
        {
            Header = PropertyHeaders[2],
            Value = Unknown01
        };

        // Class hash array (uint32 array)
        PropertyHeaders[3] = new PropertyHeaderV03
        {
            NameHash = 0x6AE2DDA0,
            RawData = BitConverter.GetBytes(Unknown01),
            VariantType = EVariantType.UInteger32
        };
        Properties[3] = new VariantU32Array(PropertyHeaders[3]);

        Header.PropertyCount = 4;
    }
    
    public uint FlattenRecurse(ContainerV03 container, uint currentIndex, bool noParent = false)
    {
        uint flattenedChildren = 0;
        var objectIdHash = HashJenkinsL3.Hash("_object_id");
        
        foreach (var subContainer in container.Containers)
        {
            if (!noParent && !subContainer.Flat)
            {
                continue;
            }

            var objectId = subContainer.GetObjectId(objectIdHash);
            
            OIdToParentIndex.Add(objectId, noParent ? 0xFFFFFFFF : currentIndex);
            OIdList.Add(objectId);

            flattenedChildren += 1;
            flattenedChildren += FlattenRecurse(subContainer, currentIndex + flattenedChildren);
        }
        

        return flattenedChildren;
    }

    #endregion

    #region IApex

    public override void FromApex(BinaryReader br)
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
                EVariantType.Vector2 => new VariantVec2(propertyHeader),
                EVariantType.Vector3 => new VariantVec3(propertyHeader),
                EVariantType.Vector4 => new VariantVec4(propertyHeader),
                EVariantType.Matrix3X3 => new VariantMat3X3(propertyHeader),
                EVariantType.Matrix4X4 => new VariantMat4X4(propertyHeader),
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

        UnflatSetup();
        
        for (var i = 0; i < Header.ContainerCount; i++)
        {
            Containers[i].FromApex(br);
        }

        Unflat();
    }

    #endregion

    #region IXml

    public override void ToXml(XmlWriter xw)
    {
        xw.WriteStartElement(XmlName);
        
        XmlUtils.WriteNameOrNameHash(xw, Header.HexNameHash, Header.Name);
        
        xw.WriteAttributeString(nameof(Flat), $"{Flat}");
        xw.WriteAttributeString(nameof(Unknown01), $"{Unknown01}");

        foreach (var container in Containers)
        {
            container.ToXml(xw);
        }
            
        xw.WriteEndElement();
    }

    public override void FromXml(XmlReader xr)
    {
        Header.NameHash = ByteUtils.ReverseBytes(XmlUtils.ReadNameIfValid(xr));
        Unknown01 = uint.Parse(xr.GetAttribute(nameof(Unknown01)) ?? "0");

        while (xr.Read())
        { if (xr.NodeType != XmlNodeType.Whitespace) break; }
            
        ContainersFromXml(xr);
    }

    #endregion
}