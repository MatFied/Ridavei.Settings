using System;

using Ridavei.Settings.Base;
using Ridavei.Settings.Cache;
using Ridavei.Settings.Exceptions;
using Ridavei.Settings.Internals;

using Microsoft.Extensions.Caching.Distributed;

namespace Ridavei.Settings
{
    /// <summary>
    /// Builder to receive or create settings.
    /// </summary>
    public sealed class SettingsBuilder
    {
        private AManager _manager;
        private CacheManager _cacheManager;

        /// <summary>
        /// Static method to create the builder.
        /// </summary>
        /// <returns>Builder</returns>
        public static SettingsBuilder CreateBuilder() => new SettingsBuilder();

        private SettingsBuilder() { }

        /// <summary>
        /// Enables to cache settings objects.
        /// </summary>
        /// <param name="distributedCache">Distributed cache</param>
        /// <param name="cacheTimeout">Timeout for the cache in milliseconds</param>
        /// <returns>Builder</returns>
        /// <exception cref="ArgumentNullException">Throwed when the <see cref="IDistributedCache"/> object is null.</exception>
        /// <exception cref="ArgumentException">Throwed when the cache timeout value is lower then <see cref="Consts.MinCacheTimeout"/>.</exception>
        public SettingsBuilder SetDistributedCache(IDistributedCache distributedCache, int cacheTimeout = Consts.DefaultCacheTimeout)
        {
            _cacheManager = new CacheManager(distributedCache, cacheTimeout);
            return this;
        }

        /// <summary>
        /// Sets the manager object used to retrieve settings.
        /// </summary>
        /// <param name="manager">Manager object</param>
        /// <returns>Builder</returns>
        /// <exception cref="ArgumentNullException">Throwed when the manager object is null.</exception>
        public SettingsBuilder SetManager(AManager manager)
        {
            _manager = manager ?? throw new ArgumentNullException(nameof(manager), "The manager object cannot be null.");
            return this;
        }

        /// <summary>
        /// Retrieves the <see cref="ASettings"/> object for the specifed dictionary name.
        /// </summary>
        /// <param name="dictionaryName">Name of the dictionary</param>
        /// <exception cref="ArgumentNullException">Throwed when the name of the dictionary is null, empty or whitespace.</exception>
        /// <exception cref="ManagerNotExistsException">Throwed when the manager object was not added.</exception>
        /// <returns>Settings</returns>
        public ASettings GetSettings(string dictionaryName)
        {
            VerifyDictionaryName(dictionaryName);

            InitManager();
            return _manager.GetSettings(dictionaryName);
        }

        /// <summary>
        /// Retrieves or creates the <see cref="ASettings"/> object for the specifed dictionary name.
        /// </summary>
        /// <param name="dictionaryName">Name of the dictionary</param>
        /// <exception cref="ArgumentNullException">Throwed when the name of the dictionary is null, empty or whitespace.</exception>
        /// <exception cref="ManagerNotExistsException">Throwed when the manager object was not added.</exception>
        /// <returns>Settings</returns>
        public ASettings GetOrCreateSettings(string dictionaryName)
        {
            VerifyDictionaryName(dictionaryName);

            InitManager();
            return _manager.GetOrCreateSettings(dictionaryName);
        }

        /// <summary>
        /// Initializes the Manager class.
        /// </summary>
        /// <exception cref="ManagerNotExistsException">Throwed when the manager object was not added.</exception>
        private void InitManager()
        {
            if (_manager == null)
                throw new ManagerNotExistsException();
            _manager.Init(_cacheManager);
        }

        /// <summary>
        /// Checks the dictionary name.
        /// </summary>
        /// <param name="dictionaryName">Name of the dictionary</param>
        /// <exception cref="ArgumentNullException">Throwed when the name of the dictionary is null, empty or whitespace.</exception>
        private static void VerifyDictionaryName(string dictionaryName)
        {
            if (string.IsNullOrWhiteSpace(dictionaryName))
                throw new ArgumentNullException(nameof(dictionaryName), "The name of the dictionary cannot be null or empty or whitespace.");
        }
    }
}
