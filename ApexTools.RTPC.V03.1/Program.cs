// See https://aka.ms/new-console-template for more information

using ApexTools.RTPC.V03._1.Struct;

using (var br = new BinaryReader(new FileStream(args[0], FileMode.Open)))
{
    var rtpcV03 = RtpcV03Parser.FromApex(br);
    RtpcV03Parser.ToXml(rtpcV03, args[0]);
}