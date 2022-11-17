using System;
using System.Runtime.Caching;

namespace Ridavei.Settings.Cache
{
    internal static class EvictPolicyGenerator
    {
        /// <summary>
        /// Generates the expire time for cached object.
        /// </summary>
        /// <param name="cacheTimeout">Cache timeout in seconds.</param>
        /// <returns></returns>
        public static DateTimeOffset GetAbsoluteExpirationTime(int cacheTimeout)
        {
            return DateTimeOffset.UtcNow.AddSeconds(cacheTimeout);
        }

        /// <summary>
        /// Creates the <see cref="CacheItemPolicy"/> with the expiration time.
        /// </summary>
        /// <param name="absoluteExpiration">Expiration time</param>
        /// <returns></returns>
        public static CacheItemPolicy CreatePolicyItem(DateTimeOffset absoluteExpiration)
        {
            return new CacheItemPolicy
            {
                AbsoluteExpiration = absoluteExpiration,
                RemovedCallback = RemovedCallback
            };
        }

        /// <summary>
        /// Callback method for the removed cached objects.
        /// </summary>
        private static void RemovedCallback(CacheEntryRemovedArguments arg)
        {
            var item = arg.CacheItem.Value as IDisposable;
            if (item != null)
                item.Dispose();
        }
    }
}
