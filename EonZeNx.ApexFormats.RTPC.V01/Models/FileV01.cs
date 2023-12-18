﻿using System.Xml;
using EonZeNx.ApexFormats.RTPC.V01.Models.Properties.Variants;
using EonZeNx.ApexTools.Core;
using EonZeNx.ApexTools.Core.Abstractions;
using EonZeNx.ApexTools.Core.Abstractions.CombinedSerializable;
using EonZeNx.ApexTools.Core.Utils;

namespace EonZeNx.ApexFormats.RTPC.V01.Models;

/// <summary>
/// The structure for <see cref="FileV01"/> file
/// <br/> FourCc - <see cref="EFourCc"/>
/// <br/> Version - <see cref="uint"/>
/// <br/> Root container - <see cref="ContainerV01"/>
/// </summary>
public class FileV01 : XmlSerializable, IApexFile, IApexSerializable
{
    public override string XmlName => "RTPC";
    public EFourCc FourCc => EFourCc.Rtpc;
    public uint Version => 0x01;
    
    public ContainerV01 Root { get; set; } = new();
    
    
    #region ApexSerializable
    
    public void FromApex(BinaryReader br)
    {
        br.BaseStream.Seek(4 + 4, SeekOrigin.Begin);
        Root = new ContainerV01();
        Root.FromApexHeader(br);
        Root.FromApex(br);
        
        Str.StringMap.Clear();
    }

    public void ToApex(BinaryWriter bw)
    {
        bw.Write(FourCc.ToBigEndian());
        bw.Write(Version);

        var originalOffset = bw.Position();
        bw.Seek(ContainerV01.HeaderSize, SeekOrigin.Current);
        Root.ToApexDeferred(bw);
        
        bw.Seek((int) originalOffset, SeekOrigin.Begin);
        Root.ToApex(bw);
    }
    
    #endregion
    
    
    #region XmlSerializable
    
    public override void FromXml(XmlReader xr)
    {
        xr.ReadToDescendant(Root.XmlName);
        Root.FromXml(xr);
    }

    public override void ToXml(XmlWriter xw)
    {
        xw.WriteStartElement(XmlName);
        Root.ToXml(xw);
        xw.WriteEndElement();
    }

    #endregion
}