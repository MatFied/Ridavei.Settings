using System;
using System.Runtime.Caching;

namespace Ridavei.Settings.Cache
{
    internal static class CacheManager
    {
        private static readonly MemoryCache _cache = new MemoryCache("Ridavei_Settings_Cache");

        /// <summary>
        /// Adds value to the cache.
        /// </summary>
        /// <param name="genKey">Generated key for the cache manager</param>
        /// <param name="value">Cached value</param>
        /// <param name="absoluteExpiration">Time when the object will expire from the cache.</param>
        public static void Add(string genKey, object value, DateTimeOffset absoluteExpiration)
        {
            _cache.Set(genKey, value, EvictPolicyGenerator.CreatePolicyItem(absoluteExpiration));
        }

        /// <summary>
        /// Retrieves data from cache if still exists.
        /// </summary>
        /// <param name="genKey">Generated key for the cache manager</param>
        /// <returns></returns>
        public static object Get(string genKey)
        {
            return _cache.Get(genKey);
        }

        /// <summary>
        /// Removes data from cache if still exists.
        /// </summary>
        /// <param name="genKey">Generated key for the cache manager</param>
        public static void Remove(string genKey)
        {
            _cache.Remove(genKey);
        }
    }
}
