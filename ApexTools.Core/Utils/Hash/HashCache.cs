﻿namespace ApexTools.Core.Utils.Hash;

public class HashCache
    {
        public readonly Dictionary<uint, HashCacheEntry> Cache = new();
        public readonly HashSet<uint> Unknown = new();

        /// <summary>
        /// Whether or not a given hash exists in the cache
        /// </summary>
        /// <param name="hash">Hash to lookup</param>
        /// <returns></returns>
        public bool InCache(uint hash)
        {
            return Cache.ContainsKey(hash);
        }
        
        /// <summary>
        /// Whether or not a given hash exists in the Unknown list
        /// </summary>
        /// <param name="hash">Hash to lookup</param>
        /// <returns></returns>
        public bool IsUnknown(uint hash)
        {
            return Unknown.Contains(hash);
        }

        /// <summary>
        /// Get a value from the cache.
        /// </summary>
        /// <param name="hash">Hash to search for.</param>
        /// <returns>Non-nullable string</returns>
        public string Get(uint hash)
        {
            return InCache(hash) ? Cache[hash].Value : string.Empty;
        }

        /// <summary>
        /// Gets the lowest count hash in the cache and returns it.
        /// </summary>
        /// <returns>Nullable HashCacheEntry</returns>
        public uint Lowest()
        {
            if (Cache.Count == 0) return 0;
            
            var low = Cache.Keys.ElementAt(0);
            var lowestValue = 999;
            foreach (var key in Cache.Keys.Where(key => Cache[key].Count < lowestValue))
            {
                low = key;
                lowestValue = Cache[key].Count;
            }

            return low;
        }

        /// <summary>
        /// Adds a value to the cache
        /// </summary>
        /// <param name="hash"></param>
        /// <param name="value"></param>
        public void Add(uint hash, string value)
        {
            // Check if exists in cache already
            if (InCache(hash))
            {
                Cache[hash].Count++;
                return;
            }

            Cache[hash] = new HashCacheEntry(value);
        }
        
        /// <summary>
        /// Adds a value to the unknown hashset
        /// </summary>
        /// <param name="hash"></param>
        public void AddUnknown(uint hash)
        {
            // Check if exists in cache already
            if (IsUnknown(hash))
            {
                return;
            }

            Unknown.Add(hash);
        }
    }