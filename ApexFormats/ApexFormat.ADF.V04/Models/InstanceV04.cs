using System.Xml;
using ApexTools.Core.Abstractions.CombinedSerializable;

namespace ApexFormat.ADF.V04.Models;

/// <summary>
/// The structure for an <b><see cref="InstanceV04"/></b>.
/// <list type="table">
///     <listheader>
///         <term>Property</term>
///         <description>Type</description>
///     </listheader>
///     <item>
///         <term>Name hash</term>
///         <description><see cref="uint"/></description>
///     </item>
///     <item>
///         <term>Type hash</term>
///         <description><see cref="uint"/></description>
///     </item>
///     <item>
///         <term>Data offset</term>
///         <description><see cref="uint"/></description>
///     </item>
///     <item>
///         <term>Data size</term>
///         <description><see cref="uint"/></description>
///     </item>
///     <item>
///         <term>Name</term>
///         <description><see cref="ulong"/></description>
///     </item>
/// </list>
/// </summary>
public class InstanceV04 : XmlSerializable, IApexSerializable
{
    public override string XmlName => "Instance";

    public Dictionary<uint, TypeDefV04> TypeDefs { get; set; } = new();

    public uint NameHash { get; set; }
    public uint TypeHash { get; set; }
    public uint DataOffset { get; set; }
    public uint DataSize { get; set; }
    public ulong Name { get; set; }


    public InstanceV04() { }

    public InstanceV04(Dictionary<uint, TypeDefV04> typeDefs)
    {
        TypeDefs = typeDefs;
    }


    #region ApexSerializable

    public void FromApex(BinaryReader br)
    {
        NameHash = br.ReadUInt32();
        TypeHash = br.ReadUInt32();
        DataOffset = br.ReadUInt32();
        DataSize = br.ReadUInt32();
        Name = br.ReadUInt64();

        if (!TypeDefs.ContainsKey(TypeHash)) throw new KeyNotFoundException($"Type not found: {TypeHash}");
        
        var typeDef = TypeDefs[TypeHash];
        typeDef.FromApex(br);
    }

    public void ToApex(BinaryWriter bw)
    {
        throw new NotImplementedException();
    }

    #endregion
    
    
    #region XmlSerializable

    public override void FromXml(XmlReader xr)
    {
        throw new NotImplementedException();
    }

    public override void ToXml(XmlWriter xw)
    {
        throw new NotImplementedException();
    }

    #endregion
}