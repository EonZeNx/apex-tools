﻿using System.Data;
using System.Data.SQLite;
using ApexTools.Core.Config;

namespace ApexTools.Core.Utils.Hash;

public enum EHashType
{
    Unknown,
    FilePath,
    Property,
    Class,
    Misc,
}

public static class HashUtils
{
    public static SQLiteConnection? DbConnection { get; set; } = null;
    public static HashCache Hashes { get; set; } = new();
    public static bool TriedToOpenDb { get; set; } = false;
    
    
    public static void OpenDatabaseConnection()
    {
        TriedToOpenDb = true;
        
        var dbFile = Settings.DatabasePath.Value;
        if (!File.Exists(dbFile)) return;
            
        var dataSource = @$"Data Source={dbFile}";
        DbConnection = new SQLiteConnection(dataSource);
        DbConnection.Open();
    }
    
    public static string Lookup(uint hash, EHashType hashType = EHashType.Unknown)
    {
        if (!Settings.PerformHashLookUp.Value) return string.Empty;
        
        if (Hashes.InCache(hash)) return Hashes.Get(hash);
        if (Hashes.IsUnknown(hash)) return string.Empty;

        if (DbConnection == null && !TriedToOpenDb) OpenDatabaseConnection();
        if (DbConnection?.State != ConnectionState.Open) return string.Empty;
        
        var tables = new[]{ "properties", "classes", "misc", "filepaths" };
        switch (hashType)
        {
            case EHashType.FilePath:
                tables = new[] { "filepaths" };
                break;
            case EHashType.Property:
                tables = new[] { "properties" };
                break;
            case EHashType.Class:
                tables = new[] { "classes" };
                break;
            case EHashType.Misc:
                tables = new[] { "misc" };
                break;
            case EHashType.Unknown:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(hashType), hashType, null);
        }
        
        var command = DbConnection.CreateCommand();
        var foundValue = string.Empty;
        foreach (var table in tables)
        {
            command.CommandText = $"SELECT Value FROM '{table}' WHERE Hash = {(int) hash}";
            using var dbr = command.ExecuteReader();
            if (!dbr.Read()) continue;
            
            foundValue = dbr.GetString(0);
            break;
        }

        if (!string.IsNullOrEmpty(foundValue))
        {
            Hashes.Add(hash, foundValue);
        }
        else
        {
            Hashes.AddUnknown(hash);
        }
        
        return foundValue;
    }
}