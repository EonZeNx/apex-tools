using System.Data.SQLite;
using EonZeNx.ApexTools.Config;

namespace EonZeNx.ApexTools.Core.Utils.Hash;

public static class HashUtils
{
    public static HashCache Cache { get; set; } = new(Settings.HashCacheSize.Value);
    
    public static string Lookup(SQLiteConnection con, int hash)
    {
        if (!Settings.PerformHashLookUp.Value) return "";

        if (Cache.Contains(hash)) return Cache.Get(hash);
            
        var command = con.CreateCommand();
        command.CommandText = $"SELECT Value FROM properties WHERE Hash = {hash}";
        
        using var dbr = command.ExecuteReader();
        if (!dbr.Read()) return "";
            
        var value = dbr.GetString(0);
        Cache.Add(hash, value);
                    
        return value;
    }
}