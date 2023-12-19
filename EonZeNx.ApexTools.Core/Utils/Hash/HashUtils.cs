using System.Data;
using System.Data.SQLite;
using EonZeNx.ApexTools.Config;

namespace EonZeNx.ApexTools.Core.Utils.Hash;

public static class HashUtils
{
    public static SQLiteConnection? DbConnection { get; set; } = null;
    public static HashCache Cache { get; set; } = new(Settings.HashCacheSize.Value);
    
    
    public static void OpenDatabaseConnection()
    {
        var dbFile = Settings.DatabasePath.Value;
        if (!File.Exists(dbFile)) return;
            
        var dataSource = @$"Data Source={dbFile}";
        DbConnection = new SQLiteConnection(dataSource);
        DbConnection.Open();
    }
    
    public static string Lookup(uint hash)
    {
        if (!Settings.PerformHashLookUp.Value) return string.Empty;
        if (Cache.Contains(hash)) return Cache.Get(hash);

        if (DbConnection == null) OpenDatabaseConnection();
        if (DbConnection?.State != ConnectionState.Open) return string.Empty;
            
        var command = DbConnection.CreateCommand();
        command.CommandText = $"SELECT Value FROM 'Global' WHERE Hash = {hash}";
        
        using var dbr = command.ExecuteReader();
        if (!dbr.Read()) return string.Empty;
            
        var value = dbr.GetString(0);
        Cache.Add(hash, value);
                    
        return value;
    }
}