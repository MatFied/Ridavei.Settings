using System;
using System.Runtime.Caching;

namespace Ridavei.Settings.Cache
{
    internal static class EvictPolicyGenerator
    {
        /// <summary>
        /// Generates the expire time for cached object.
        /// </summary>
        /// <param name="timeout">Timeout in milliseconds.</param>
        /// <returns></returns>
        public static DateTimeOffset GetAbsoluteExpirationTime(int timeout)
        {
            if (timeout < 0)
                return DateTimeOffset.UtcNow;
            return DateTimeOffset.UtcNow.AddMilliseconds(timeout);
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
