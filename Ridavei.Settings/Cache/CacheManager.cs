using System;
using System.Collections.Generic;
using System.Text.Json;

using Ridavei.Settings.Internals;

using Microsoft.Extensions.Caching.Distributed;

namespace Ridavei.Settings.Cache
{
    /// <summary>
    /// Cache manager class to help add, retrieve and remove data from <see cref="IDistributedCache"/>.
    /// </summary>
    internal class CacheManager
    {
        private readonly IDistributedCache _cache;
        private readonly int _cacheTimeout;

        /// <summary>
        /// The default constructor for cache manager class.
        /// </summary>
        /// <param name="distributedCache">Distributed cache</param>
        /// <param name="cacheTimeout">Timeout for the cache in milliseconds</param>
        /// <exception cref="ArgumentNullException">Throwed when the <see cref="IDistributedCache"/> object is null.</exception>
        /// <exception cref="ArgumentException">Throwed when the cache timeout value is lower then <see cref="Consts.MinCacheTimeout"/>.</exception>
        public CacheManager(IDistributedCache distributedCache, int cacheTimeout)
        {
            if (distributedCache == null)
                throw new ArgumentNullException(nameof(distributedCache));
            if (cacheTimeout < Consts.MinCacheTimeout)
                throw new ArgumentException($"The cache timeout cannot be lower then {Consts.MinCacheTimeout}.", nameof(cacheTimeout));
            _cache = distributedCache;
            _cacheTimeout = cacheTimeout;
        }

        /// <summary>
        /// Adds value to the cache.
        /// </summary>
        /// <param name="genKey">Generated key for the cache manager</param>
        /// <param name="value">Cached value</param>
        public void AddString(string genKey, string value)
        {
            _cache.SetString(genKey, value, new DistributedCacheEntryOptions { AbsoluteExpiration = GetAbsoluteExpirationTime(_cacheTimeout) });
        }

        /// <summary>
        /// Adds <see cref="IReadOnlyDictionary{TKey, TValue}"/> to the cache.
        /// </summary>
        /// <param name="genKey">Generated key for the cache manager</param>
        /// <param name="dict">Read only dictionary</param>
        public void AddDictionary(string genKey, IReadOnlyDictionary<string, string> dict)
        {
            AddString(genKey, JsonSerializer.Serialize(dict));
        }

        /// <summary>
        /// Retrieves <see cref="string"/> from cache if still exists.
        /// </summary>
        /// <param name="genKey">Generated key for the cache manager</param>
        /// <returns></returns>
        public string GetString(string genKey)
        {
            return _cache.GetString(genKey);
        }

        /// <summary>
        /// Retrieves <see cref="IReadOnlyDictionary{TKey, TValue}"/> from cache if still exists.
        /// </summary>
        /// <param name="genKey">Generated key for the cache manager</param>
        /// <returns></returns>
        public IReadOnlyDictionary<string, string> GetDictionary(string genKey)
        {
            var res = GetString(genKey);
            if (res == null)
                return null;

            return JsonSerializer.Deserialize<IReadOnlyDictionary<string, string>>(res);
        }

        /// <summary>
        /// Removes data from cache if still exists.
        /// </summary>
        /// <param name="genKey">Generated key for the cache manager</param>
        public void Remove(string genKey)
        {
            _cache.Remove(genKey);
        }

        /// <summary>
        /// Generates the expire time for cached object.
        /// </summary>
        /// <param name="timeout">Timeout in milliseconds.</param>
        /// <returns></returns>
        private static DateTimeOffset GetAbsoluteExpirationTime(int timeout)
        {
            return DateTimeOffset.UtcNow.AddMilliseconds(timeout);
        }
    }
}
