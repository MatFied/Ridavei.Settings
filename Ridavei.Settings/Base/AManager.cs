using System;

using Ridavei.Settings.Cache;
using Ridavei.Settings.Exceptions;

namespace Ridavei.Settings.Base
{
    /// <summary>
    /// Abstract manager class to help retrieve <see cref="ASettings"/>.
    /// </summary>
    public abstract class AManager
    {
        private bool _initialized = false;

        private static readonly object _lock = new object();

        /// <summary>
        /// Defines if the manager or settings class should use cache.
        /// </summary>
        public bool UseCache { get; private set; }

        /// <summary>
        /// Timeout for the cache in milliseconds.
        /// </summary>
        public int CacheTimeout { get; private set; }

        /// <summary>
        /// Initializes values for the manager.
        /// </summary>
        /// <param name="useCache">Defines if the manager or settings class should use cache</param>
        /// <param name="cacheTimeout">Timeout for the cache in milliseconds</param>
        internal void Init(bool useCache, int cacheTimeout)
        {
            if (_initialized)
                return;

            UseCache = useCache;
            CacheTimeout = cacheTimeout;
            _initialized = true;
        }

        /// <summary>
        /// Retrieves the <see cref="ASettings"/> object for the specifed dictionary name.<para />
        /// Settings objects are stored in cache if caching was in the <see cref="SettingsBuilder"/> enabled.
        /// </summary>
        /// <param name="dictionaryName">Name of the dictionary</param>
        /// <returns>Settings</returns>
        /// <exception cref="ArgumentNullException">Throwed when the name of the dictionary is null, empty or whitespace.</exception>
        /// <exception cref="DictionaryNotFoundException">Throwed when the name of the dictionary was not found.</exception>
        internal ASettings GetSettings(string dictionaryName)
        {
            if (string.IsNullOrWhiteSpace(dictionaryName))
                throw new ArgumentNullException(nameof(dictionaryName), "The name of the dictionary cannot be null or empty or whitespace.");

            var exists = UseCache ? TryGetCachedSettings(dictionaryName, out ASettings res) : TryGetSettingsObject(dictionaryName, out res);
            if (!exists)
                throw new DictionaryNotFoundException(dictionaryName);
            res.Init(UseCache, CacheTimeout);

            return res;
        }

        /// <summary>
        /// Retrieves or creates the <see cref="ASettings"/> object for the specifed dictionary name.<para />
        /// Settings objects are stored in cache if caching was in the <see cref="SettingsBuilder"/> enabled.
        /// </summary>
        /// <param name="dictionaryName">Name of the dictionary</param>
        /// <returns>Settings</returns>
        /// <exception cref="ArgumentNullException">Throwed when the name of the dictionary is null, empty or whitespace.</exception>
        internal ASettings GetOrCreateSettings(string dictionaryName)
        {
            if (string.IsNullOrWhiteSpace(dictionaryName))
                throw new ArgumentNullException(nameof(dictionaryName), "The name of the dictionary cannot be null or empty or whitespace.");

            lock (_lock)
            {
                var exists = UseCache ? TryGetCachedSettings(dictionaryName, out var res) : TryGetSettingsObject(dictionaryName, out res);
                if (!exists)
                {
                    res = CreateSettingsObject(dictionaryName);
                    if (UseCache)
                        CacheManager.Add(KeyGenerator.GenerateForDictionary(dictionaryName), res, EvictPolicyGenerator.GetAbsoluteExpirationTime(CacheTimeout));
                }
                return res;
            }
        }

        /// <summary>
        /// Retrieves the <see cref="ASettings"/> object for the specifed dictionary name.
        /// </summary>
        /// <param name="dictionaryName">Name of the dictionary</param>
        /// <param name="settings">Retrieved Settings</param>
        /// <returns>True if the Settings exists or false if not.</returns>
        protected abstract bool TryGetSettingsObject(string dictionaryName, out ASettings settings);

        /// <summary>
        /// Creates the <see cref="ASettings"/> object for the specifed dictionary name.
        /// </summary>
        /// <param name="dictionaryName">Name of the dictionary</param>
        /// <returns>Settings</returns>
        protected abstract ASettings CreateSettingsObject(string dictionaryName);

        /// <summary>
        /// Returns the cached <see cref="ASettings"/> object if it exists in the cache.<para />
        /// If not it's run the basic method to get the <see cref="ASettings"/> and stores it in the cache.
        /// </summary>
        /// <param name="dictionaryName">Name of the dictionary</param>
        /// <param name="settings">Retrieved Settings</param>
        /// <returns>True if the Settings exists or false if not.</returns>
        private bool TryGetCachedSettings(string dictionaryName, out ASettings settings)
        {
            var genKey = KeyGenerator.GenerateForDictionary(dictionaryName);
            lock (_lock)
            {
                settings = CacheManager.Get(genKey) as ASettings;
                if (settings == null)
                {
                    if (!TryGetSettingsObject(dictionaryName, out settings))
                        return false;
                    CacheManager.Add(genKey, settings, EvictPolicyGenerator.GetAbsoluteExpirationTime(CacheTimeout));
                }
            }
            return true;
        }
    }
}
