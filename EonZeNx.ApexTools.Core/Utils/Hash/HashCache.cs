namespace EonZeNx.ApexTools.Core.Utils.Hash;

public class HashCache
    {
        public readonly Dictionary<int, HashCacheEntry> Cache = new();
        private int MaxSize { get; }

        public HashCache(int size)
        {
            MaxSize = size;
        }

        /// <summary>
        /// Whether or not a given hash exists in the cache
        /// </summary>
        /// <param name="hash">Hash to lookup</param>
        /// <returns></returns>
        public bool Contains(int hash)
        {
            return Cache.ContainsKey(hash);
        }

        /// <summary>
        /// Get a value from the cache.
        /// </summary>
        /// <param name="hash">Hash to search for.</param>
        /// <returns>Non-nullable string</returns>
        public string Get(int hash)
        {
            return Contains(hash) ? Cache[hash].Value : string.Empty;
        }

        /// <summary>
        /// Gets the lowest count hash in the cache and returns it.
        /// </summary>
        /// <returns>Nullable HashCacheEntry</returns>
        public int Lowest()
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
        public void Add(int hash, string value)
        {
            // Check if exists in cache already
            if (Contains(hash))
            {
                Cache[hash].Count++;
                return;
            }

            // Trim cache if at limit
            if (Cache.Count >= MaxSize)
            {
                var lowestHash = Lowest();
                if (lowestHash != 0) Cache.Remove(lowestHash);
            }

            Cache[hash] = new HashCacheEntry(value);
        }
    }