// See https://aka.ms/new-console-template for more information

using ApexTools.JC4.RTPC.V03.Struct;

var ext = new[]
{
    ".blo",
    ".epe"
};

var targetPath = args[0];
var targetExtension = Path.GetExtension(targetPath);

if (ext.Contains(targetExtension))
{
    var rtpcV03 = RtpcV03Parser.FromApex(targetPath);
    RtpcV03Parser.ToXml(rtpcV03, targetPath);
}
else if (targetExtension == ".xml")
{
    var rtpcV03 = RtpcV03Parser.FromXml(targetPath);
    RtpcV03Parser.ToApex(rtpcV03, targetPath);
}
