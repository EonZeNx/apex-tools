// See https://aka.ms/new-console-template for more information

using ApexTools.JC4.RTPC.V03.Models;
using ApexTools.JC4.RTPC.V03.NewModels;
using EonZeNx.ApexTools.Config;
using EonZeNx.ApexTools.Core.Utils;

Settings.LoadSettings();

var ext = new[]
{
    ".blo",
    ".epe"
};

var targetPath = args[0];
var targetFileName = Path.GetFileName(targetPath);
var targetExtension = Path.GetExtension(targetPath);

if (ext.Contains(targetExtension))
{
    LogUtils.LogLoading(targetFileName, "RTPC");
    var rtpcV03 = RtpcV03Parser.FromApex(targetPath);

    // var dataRtpcV03 = DataRtpcV03Parser.FromApex(targetPath);
    
    LogUtils.LogProcessing(targetFileName);
    RtpcV03Parser.ToXml(rtpcV03, targetPath);
    
    LogUtils.LogComplete(targetFileName);
}
else if (targetExtension == ".xml")
{
    LogUtils.LogLoading(targetFileName, "XML");
    var rtpcV03 = RtpcV03Parser.FromXml(targetPath);
    
    LogUtils.LogProcessing(targetFileName);
    RtpcV03Parser.ToApex(rtpcV03, targetPath);
    
    LogUtils.LogComplete(targetFileName);
}
