using System;
using System.Runtime.Caching;

namespace Ridavei.Settings
{
    /// <summary>
    /// The cache manager class.
    /// </summary>
    internal static class CacheManager
    {
        private const string DictionaryStartName = "Ridavei_Settings_Dict_";

        /// <summary>
        /// Adds value to the cache.
        /// </summary>
        /// <param name="genKey">Generated key to store it in cache</param>
        /// <param name="value">Cached value</param>
        /// <param name="absoluteExpiration">Time when the object will expire from the cache.</param>
        public static void Add(string genKey, object value, DateTimeOffset absoluteExpiration)
        {
            MemoryCache.Default.Set(genKey, value, CreatePolicyItem(absoluteExpiration));
        }

        /// <summary>
        /// Retrieves data from cache if still exists.
        /// </summary>
        /// <param name="genKey">Generated key to store it in cache</param>
        /// <returns></returns>
        public static object Get(string genKey)
        {
            return MemoryCache.Default.Get(genKey);
        }

        /// <summary>
        /// Generates the expire time for cached object.
        /// </summary>
        /// <param name="cacheTimeout">Cache timeout in seconds.</param>
        /// <returns></returns>
        public static DateTimeOffset GetAbsoluteExpiration(int cacheTimeout)
        {
            return DateTimeOffset.UtcNow.AddSeconds(cacheTimeout);
        }

        /// <summary>
        /// Generates the key name of the key used to cache object.
        /// </summary>
        /// <param name="dictionaryName">Name of the dictionary</param>
        /// <param name="key">Settings key</param>
        /// <returns></returns>
        public static string GenerateKeyName(string dictionaryName, string key)
        {
            return string.Concat(GenerateDictionaryName(dictionaryName), "_", key);
        }

        /// <summary>
        /// Generates the key name for the GetAll method used to cache settings keys.
        /// </summary>
        /// <param name="dictionaryName">Name of the dictionary</param>
        /// <returns></returns>
        public static string GenerateKeyNameForGetAllDictionary(string dictionaryName)
        {
            return string.Concat(GenerateDictionaryName(dictionaryName), "GetAllDictionary");
        }

        /// <summary>
        /// Generates the key name for the dictionary.
        /// </summary>
        /// <param name="dictionaryName">Name of the dictionary</param>
        /// <returns></returns>
        private static string GenerateDictionaryName(string dictionaryName)
        {
            return string.Concat(DictionaryStartName, dictionaryName);
        }

        /// <summary>
        /// Creates the <see cref="CacheItemPolicy"/> with the expiration time.
        /// </summary>
        /// <param name="absoluteExpiration">Expiration time</param>
        /// <returns></returns>
        private static CacheItemPolicy CreatePolicyItem(DateTimeOffset absoluteExpiration)
        {
            return new CacheItemPolicy
            {
                AbsoluteExpiration = absoluteExpiration
            };
        }
    }
}
