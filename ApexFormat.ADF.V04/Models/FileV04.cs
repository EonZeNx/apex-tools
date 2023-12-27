using ApexTools.Core;
using ApexTools.Core.Abstractions;
using ApexTools.Core.Abstractions.CombinedSerializable;

namespace ApexFormat.ADF.V04.Models;

/// <summary>
/// The structure for a <b><see cref="FileV04"/></b>.
/// <list type="table">
///     <listheader>
///         <term>Property</term>
///         <description>Type</description>
///     </listheader>
///     <item>
///         <term>FourCc</term>
///         <description><see cref="EFourCc"/></description>
///     </item>
///     <item>
///         <term>Version</term>
///         <description><see cref="uint"/></description>
///     </item>
///     <item>
///         <term>ADF file header</term>
///         <description><see cref="FileHeaderV04"/></description>
///     </item>
/// </list>
/// </summary>
public class FileV04 : IApexFile, IApexSerializable, ICustomFileSerializable
{
    public EFourCc FourCc => EFourCc.Adf;
    public uint Version => 0x04;

    public FileHeaderV04 Header { get; set; } = new ();
    public StringsTableV04 Strings { get; set; } = new ();
    public StringHashTableV04 StringHashes { get; set; } = new ();
    public TypeDefTableV04 TypeDefs { get; set; } = new ();
    public InstanceV04[] Instances { get; set; } = Array.Empty<InstanceV04>();


    #region ApexSerializable

    public void FromApex(BinaryReader br)
    {
        br.BaseStream.Seek(4 + 4, SeekOrigin.Begin);
        Header.FromApex(br);

        StringHashesFromApex(br);
        StringsFromApex(br);
        TypeDefsFromApex(br);

        Instances = new InstanceV04[Header.InstanceCount];
        for (var i = 0; i < Header.InstanceCount; i++)
        {
            Instances[i] = new InstanceV04(TypeDefs.TypeDefs);
            Instances[i].FromApex(br);
        }
    }

    public void ToApex(BinaryWriter bw)
    {
        throw new NotImplementedException();
    }

    #region Apex Helpers

    private void StringsFromApex(BinaryReader br)
    {
        Strings.Offset = Header.FirstStringOffset;
        Strings.Count = Header.StringCount;
        Strings.FromApex(br);
    }
    
    private void StringHashesFromApex(BinaryReader br)
    {
        StringHashes.Offset = Header.FirstStringHashOffset;
        StringHashes.Count = Header.StringHashCount;
        StringHashes.FromApex(br);
    }
    
    private void TypeDefsFromApex(BinaryReader br)
    {
        TypeDefs.Offset = Header.FirstTypeDefOffset;
        TypeDefs.Count = Header.TypeDefCount;
        TypeDefs.FromApex(br);
    }

    #endregion

    #endregion
    

    #region XmlSerializable
    
    public void FromCustomFile(BinaryReader br)
    {
        throw new NotImplementedException();
    }

    public void ToCustomFile(BinaryWriter bw)
    {
        throw new NotImplementedException();
    }
    
    #endregion
}