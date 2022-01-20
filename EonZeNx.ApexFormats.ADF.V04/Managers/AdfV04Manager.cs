﻿using EonZeNx.ApexFormats.ADF.V04.Models;
using EonZeNx.ApexTools.Core.Abstractions;
using EonZeNx.ApexTools.Core.Utils;

namespace EonZeNx.ApexFormats.ADF.V04.Managers;

public class AdfV04Manager : IPathProcessor
{
    public string TargetPath { get; set; }
    public string TargetPathName => Path.GetFileName(TargetPath);

    public AdfV04Manager(string targetPath)
    {
        TargetPath = targetPath;
    }

    public void TryProcess()
    {
        if (Path.GetExtension(TargetPath) is ".wtunec" or ".vmodc") FromApexToCustomFile();
        else if (Path.GetExtension(TargetPath) == ".xml") FromCustomFileToApex();
        else LogUtils.LogFailedToLoadError(TargetPathName);
    }

    private void FromApexToCustomFile()
    {
        LogUtils.LogLoading(TargetPathName, "ApexFile");
        
        var adfV04File = new FileV04();

        using (var inBinaryReader = new BinaryReader(new FileStream(TargetPath, FileMode.Open)))
        {
            adfV04File.FromApex(inBinaryReader);
        }

        using var outBinaryWriter = new BinaryWriter(new FileStream($"{TargetPath}.sarc", FileMode.Create));
        adfV04File.ToCustomFile(outBinaryWriter);
    }
    
    private void FromCustomFileToApex()
    {
        LogUtils.LogLoading(TargetPathName, "CustomFile");
        
        var adfV04File = new FileV04();
        
        using var inFileStream = new FileStream(TargetPath, FileMode.Open);
        using var inBinaryReader = new BinaryReader(inFileStream);
        adfV04File.FromCustomFile(inBinaryReader);
        inBinaryReader.Dispose();
        inFileStream.Dispose();
        
        LogUtils.LogProcessing(TargetPathName);

        using var outFileStream = new FileStream($"{TargetPath}.ee", FileMode.Create);
        using var outBinaryWriter = new BinaryWriter(outFileStream);
        adfV04File.ToApex(outBinaryWriter);
        
        LogUtils.LogComplete(TargetPathName);
    }
}