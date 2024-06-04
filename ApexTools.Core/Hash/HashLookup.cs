using System.Data;
using System.Data.SQLite;
using ApexTools.Core.Config;

namespace ApexTools.Core.Hash;

[Flags]
public enum EHashType
{
    Unknown = 0,
    FilePath = 1,
    Property = 2,
    Class = 4,
    Misc = 8
}

public static class LookupHashes
{
    public static SQLiteConnection? DbConnection { get; set; } = null;
    public static bool TriedToOpenDb { get; set; } = false;
    
    public static readonly Dictionary<uint, string> KnownHashes = new();
    public static readonly HashSet<uint> UnknownHashes = new();
    
    public static readonly Dictionary<EHashType, string> HashTypeToTable = new()
    {
        { EHashType.FilePath, "filepaths" },
        { EHashType.Property, "properties" },
        { EHashType.Class, "classes" },
        { EHashType.Misc, "misc" },
    };
    
    # region Database
    
    public static void OpenDatabaseConnection()
    {
        TriedToOpenDb = true;
        
        var dbFile = Settings.DatabasePath.Value;
        if (!File.Exists(dbFile)) return;
            
        var dataSource = @$"Data Source={dbFile}";
        DbConnection = new SQLiteConnection(dataSource);
        DbConnection.Open();
    }
    
    public static void LoadAll()
    {
        if (DbConnection == null && !TriedToOpenDb)
        {
            OpenDatabaseConnection();
        }
        
        if (DbConnection?.State != ConnectionState.Open) return;
        
        var command = DbConnection.CreateCommand();
        var tables = new List<string>(HashTypeToTable.Values);
        
        foreach (var table in tables)
        {
            command.CommandText = $"SELECT Hash, Value FROM '{table}'";
            using var dbr = command.ExecuteReader();
            while (dbr.Read())
            {
                var hash = (uint) dbr.GetInt32(0);
                var value = dbr.GetString(1);
                
                KnownHashes.TryAdd(hash, value);
            }
        }
    }
    
    #endregion
    
    # region Hash
    
    public static bool Known(uint hash)
    {
        return KnownHashes.ContainsKey(hash);
    }
    
    public static bool Unknown(uint hash)
    {
        return !KnownHashes.ContainsKey(hash) || UnknownHashes.Contains(hash);
    }
    
    public static string Get(byte[] bytes, EHashType hashType = EHashType.Unknown)
    {
        return Get(BitConverter.ToUInt32(bytes), hashType);
    }
    
    public static string Get(uint hash, EHashType hashType = EHashType.Unknown)
    {
        if (!Settings.LookupHashes.Value) return string.Empty;
        
        if (Known(hash)) return KnownHashes[hash];
        if (Unknown(hash)) return string.Empty;

        if (DbConnection == null && !TriedToOpenDb) OpenDatabaseConnection();
        if (DbConnection?.State != ConnectionState.Open) return string.Empty;
        
        var tables = new List<string>();
        if (hashType.HasFlag(EHashType.FilePath))
        {
            tables.Add(HashTypeToTable[EHashType.FilePath]);
        }
        if (hashType.HasFlag(EHashType.Property))
        {
            tables.Add(HashTypeToTable[EHashType.Property]);
        }
        if (hashType.HasFlag(EHashType.Class))
        {
            tables.Add(HashTypeToTable[EHashType.Class]);
        }
        if (hashType.HasFlag(EHashType.Misc))
        {
            tables.Add(HashTypeToTable[EHashType.Misc]);
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
            AddKnown(hash, foundValue);
        }
        else
        {
            AddUnknown(hash);
        }
        
        return foundValue;
    }
    
    public static void AddKnown(uint hash, string value)
    {
        KnownHashes.TryAdd(hash, value);
    }
    
    public static void AddUnknown(uint hash)
    {
        UnknownHashes.Add(hash);
    }
    
    #endregion
}