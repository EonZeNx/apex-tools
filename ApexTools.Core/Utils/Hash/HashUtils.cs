using System.Data;
using System.Data.SQLite;
using ApexTools.Core.Config;

namespace ApexTools.Core.Utils.Hash;

[Flags]
public enum EHashType
{
    Unknown = 0,
    FilePath = 1,
    Property = 2,
    Class = 4,
    Misc = 8
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
        
        var tables = new List<string>();
        if (HasFlag(hashType, EHashType.FilePath))
        {
            tables.Add("filepaths");
        }
        if (HasFlag(hashType, EHashType.Property))
        {
            tables.Add("properties");
        }
        if (HasFlag(hashType, EHashType.Class))
        {
            tables.Add("classes");
        }
        if (HasFlag(hashType, EHashType.Misc))
        {
            tables.Add("misc");
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
    
    public static bool HasFlag<T>(T flags, T flag) where T : struct
    {
        int flagsValue = (int)(object)flags;
        int flagValue = (int)(object)flag;

        return (flagsValue & flagValue) != 0;
    }
    
    public static void Set<T>(ref T flags, T flag) where T : struct
    {
        int flagsValue = (int)(object)flags;
        int flagValue = (int)(object)flag;

        flags = (T)(object)(flagsValue | flagValue);
    }

    public static void Unset<T>(ref T flags, T flag) where T : struct
    {
        int flagsValue = (int)(object)flags;
        int flagValue = (int)(object)flag;

        flags = (T)(object)(flagsValue & (~flagValue));
    }
}