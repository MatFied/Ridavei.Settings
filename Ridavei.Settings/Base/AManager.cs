using System;
using System.Collections.Generic;

using Ridavei.Settings.Cache;
using Ridavei.Settings.Interface;

namespace Ridavei.Settings.Base
{
    /// <summary>
    /// Abstract manager class to help retrieve <see cref="ISettings"/>.
    /// </summary>
    public abstract class AManager : IDisposable
    {
        private readonly List<string> _cachedSettingsList = new List<string>();
        private bool _initialized = false;

        /// <summary>
        /// Defines if the manager or settings class should use cache.
        /// </summary>
        public bool UseCache { get; private set; }

        /// <summary>
        /// Timeout for the cache in seconds.
        /// </summary>
        public int CacheTimeout { get; private set; }

        /// <summary>
        /// Releases all resources used by the Manager object.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            if (UseCache)
                foreach (var cachedSettingsName in _cachedSettingsList)
                    CacheManager.Remove(KeyGenerator.GenerateForDictionary(cachedSettingsName));
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Initializes values for the manager.
        /// </summary>
        /// <param name="useCache">Defines if the manager or settings class should use cache</param>
        /// <param name="cacheTimeout">Timeout for the cache in seconds</param>
        internal void Init(bool useCache, int cacheTimeout)
        {
            if (_initialized)
                return;

            UseCache = useCache;
            CacheTimeout = cacheTimeout;
            _initialized = true;
        }

        /// <summary>
        /// Retrieves the <see cref="ISettings"/> object for the specifed dictionary name.<para />
        /// Settings objects are stored in cache if caching was in the <see cref="SettingsBuilder"/> enabled.
        /// </summary>
        /// <param name="dictionaryName">Name of the dictionary</param>
        /// <returns>Settings</returns>
        /// <exception cref="ArgumentNullException">Throwed when the name of the dictionary is null, empty or whitespace.</exception>
        internal ISettings GetSettings(string dictionaryName)
        {
            if (string.IsNullOrWhiteSpace(dictionaryName))
                throw new ArgumentNullException(nameof(dictionaryName), "The name of the dictionary cannot be null or empty or whitespace.");
            var res = UseCache ? GetCachedSettings(dictionaryName) : GetSettingsObject(dictionaryName);
            res.Init(UseCache, CacheTimeout);

            return res;
        }

        /// <summary>
        /// Retrieves the <see cref="ISettings"/> object for the specifed dictionary name.
        /// </summary>
        /// <param name="dictionaryName">Name of the dictionary</param>
        /// <returns>Settings</returns>
        protected abstract ASettings GetSettingsObject(string dictionaryName);

        /// <summary>
        /// Releases all resources used by the derived Manager object.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing) { }

        /// <summary>
        /// Returns the cached <see cref="ISettings"/> object if it exists in the cache.<para />
        /// If not it's run the basic method to get the <see cref="ISettings"/> and stores it in the cache.
        /// </summary>
        /// <param name="dictionaryName">Name of the dictionary</param>
        /// <returns>Settings</returns>
        private ASettings GetCachedSettings(string dictionaryName)
        {
            var genKey = KeyGenerator.GenerateForDictionary(dictionaryName);
            var res = CacheManager.Get(genKey) as ASettings;
            if (res == null)
            {
                res = GetSettingsObject(dictionaryName);
                if (res != null)
                    CacheManager.Add(genKey, res, EvictPolicyGenerator.GetAbsoluteExpirationTime(CacheTimeout));
            }
            _cachedSettingsList.Add(dictionaryName);
            return res;
        }
    }
}
