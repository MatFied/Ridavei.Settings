using System;
using System.Collections.Generic;

using Ridavei.Settings.Interface;

namespace Ridavei.Settings.Base
{
    /// <summary>
    /// Abstract manager class to help retrieve <see cref="ISettings"/>.
    /// </summary>
    public abstract class AManager : IDisposable
    {
        private readonly Dictionary<string, ASettings> _dictionaries = new Dictionary<string, ASettings>();
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
        /// Retrieves the <see cref="ISettings"/> object for the specifed dictionary name. Settings objects are stored in a <see cref="Dictionary{TKey, TValue}"/>.
        /// </summary>
        /// <param name="dictionaryName">Name of the dictionary</param>
        /// <returns>Settings</returns>
        /// <exception cref="ArgumentNullException">Throwed when the name of the dictionary is null, empty or whitespace.</exception>
        internal ISettings GetSettings(string dictionaryName)
        {
            if (string.IsNullOrWhiteSpace(dictionaryName))
                throw new ArgumentNullException(nameof(dictionaryName), "The name of the dictionary can not be null or empty or whitespace.");
            if (!_dictionaries.TryGetValue(dictionaryName, out var res))
            {
                res = GetSettingsObject(dictionaryName);
                _dictionaries.Add(dictionaryName, res);
            }
            return res;
        }

        /// <summary>
        /// Retrieves the <see cref="ISettings"/> object for the specifed dictionary name.
        /// </summary>
        /// <param name="dictionaryName">Name of the dictionary</param>
        /// <returns>Settings</returns>
        protected abstract ASettings GetSettingsObject(string dictionaryName);

        /// <summary>
        /// Releases all resources used by the Manager object.
        /// </summary>
        public virtual void Dispose()
        {
            foreach (var dictionary in _dictionaries)
                dictionary.Value.Dispose();
        }
    }
}
