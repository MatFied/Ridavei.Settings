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

        private static readonly object _lock = new object();

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
                    CacheManager.Remove(cachedSettingsName);
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
        /// Creates the <see cref="ISettings"/> object for the specifed dictionary name.<para />
        /// Settings objects are stored in cache if caching was in the <see cref="SettingsBuilder"/> enabled.
        /// </summary>
        /// <param name="dictionaryName">Name of the dictionary</param>
        /// <returns>Settings</returns>
        /// <exception cref="ArgumentNullException">Throwed when the name of the dictionary is null, empty or whitespace.</exception>
        internal ISettings CreateSettings(string dictionaryName)
        {
            if (string.IsNullOrWhiteSpace(dictionaryName))
                throw new ArgumentNullException(nameof(dictionaryName), "The name of the dictionary cannot be null or empty or whitespace.");

            lock (_lock)
            {
                var res = CreateSettingsObject(dictionaryName);
                if (UseCache)
                    CacheManager.Add(KeyGenerator.GenerateForDictionary(dictionaryName), res, EvictPolicyGenerator.GetAbsoluteExpirationTime(CacheTimeout));
                return res;
            }
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
            ASettings res;
            var exists = UseCache ? TryGetCachedSettings(dictionaryName, out res) : TryGetSettingsObject(dictionaryName, out res);
            res.Init(UseCache, CacheTimeout);

            return res;
        }

        /// <summary>
        /// Retrieves or creates the <see cref="ISettings"/> object for the specifed dictionary name.<para />
        /// Settings objects are stored in cache if caching was in the <see cref="SettingsBuilder"/> enabled.
        /// </summary>
        /// <param name="dictionaryName">Name of the dictionary</param>
        /// <returns>Settings</returns>
        /// <exception cref="ArgumentNullException">Throwed when the name of the dictionary is null, empty or whitespace.</exception>
        internal ISettings GetOrCreateSettings(string dictionaryName)
        {
            lock(_lock)
            {
                var res = GetSettings(dictionaryName);
                if (res == null)
                    res = CreateSettings(dictionaryName);
                return res;
            }
        }

        /// <summary>
        /// Retrieves the <see cref="ISettings"/> object for the specifed dictionary name.
        /// </summary>
        /// <param name="dictionaryName">Name of the dictionary</param>
        /// <param name="settings">Retrieved Settings</param>
        /// <returns>True if the Settings exists or false if not.</returns>
        protected abstract bool TryGetSettingsObject(string dictionaryName, out ASettings settings);

        /// <summary>
        /// Creates the <see cref="ISettings"/> object for the specifed dictionary name.
        /// </summary>
        /// <param name="dictionaryName">Name of the dictionary</param>
        /// <returns>Settings</returns>
        protected abstract ASettings CreateSettingsObject(string dictionaryName);

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
        /// <param name="settings">Retrieved Settings</param>
        /// <returns>True if the Settings exists or false if not.</returns>
        private bool TryGetCachedSettings(string dictionaryName, out ASettings settings)
        {
            var genKey = KeyGenerator.GenerateForDictionary(dictionaryName);
            settings = CacheManager.Get(genKey) as ASettings;
            if (settings == null)
            {
                lock (_lock)
                {
                    if (!TryGetSettingsObject(dictionaryName, out settings))
                        return false;
                    CacheManager.Add(genKey, settings, EvictPolicyGenerator.GetAbsoluteExpirationTime(CacheTimeout));
                }
            }
            if (!_cachedSettingsList.Contains(genKey))
                _cachedSettingsList.Add(genKey);
            return true;
        }
    }
}
