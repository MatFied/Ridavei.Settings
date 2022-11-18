using System;
using System.Collections.Generic;

using Ridavei.Settings.Cache;

namespace Ridavei.Settings.Base
{
    /// <summary>
    /// Abstract class for the settings classes. It can retrieve and set objects in the settings.
    /// </summary>
    public abstract class ASettings : IDisposable
    {
        private bool _initialized = false;

        private static readonly object _lock = new object();

        /// <summary>
        /// Name of the dictionary
        /// </summary>
        public readonly string DictionaryName;

        /// <summary>
        /// Defines if the settings class should use cache.
        /// </summary>
        public bool UseCache { get; private set; }

        /// <summary>
        /// Timeout for the cache in milliseconds.
        /// </summary>
        public int CacheTimeout { get; private set; }

        /// <summary>
        /// The default constructor for settings class.
        /// </summary>
        /// <param name="dictionaryName">Name of the dictionary</param>
        /// <exception cref="ArgumentNullException">Throwed when the name of the dictionary is null, empty or whitespace.</exception>
        public ASettings(string dictionaryName)
        {
            if (string.IsNullOrWhiteSpace(dictionaryName))
                throw new ArgumentNullException(nameof(dictionaryName), "The name of the dictionary cannot be null or empty or whitespace.");
            DictionaryName = dictionaryName;
        }

        /// <summary>
        /// Releases all resources used by the Settings object.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Sets the value for the specific key and if UseCache is true stores it in the cache.
        /// </summary>
        /// <param name="key">Settings key</param>
        /// <param name="value">New value stored in the settings</param>
        /// <exception cref="ArgumentNullException">Throwed when the key or value are null, empty or whitespace.</exception>
        public void Set(string key, string value)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key), "The settings key cannot be null or empty or whitespace.");
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentNullException(nameof(value), $"The settings value for \"{key}\" cannot be null or empty or whitespace.");
            lock (_lock)
            {
                SetValue(key, value);
                if (UseCache)
                    AddToCache(key, value, EvictPolicyGenerator.GetAbsoluteExpirationTime(CacheTimeout));
            }
        }

        /// <summary>
        /// Sets the values for the keys and if UseCache is true stores them in the cache.
        /// </summary>
        /// <param name="keyValues">Settings keys and values</param>
        /// <exception cref="ArgumentNullException">Throwed when the key or value are null, empty or whitespace.</exception>
        public void Set(IDictionary<string, string> keyValues)
        {
            foreach (var kvp in keyValues)
                Set(kvp.Key, kvp.Value);
        }

        /// <summary>
        /// Gets the value for the specific key or throws a <see cref="KeyNotFoundException"/> when not found.
        /// </summary>
        /// <param name="key">Settings key</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Throwed when the key is null, empty or whitespace.</exception>
        /// <exception cref="KeyNotFoundException">Throwed when the key was not found.</exception>
        public string Get(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key), "The settings key cannot be null or empty or whitespace.");

            if (!TryGet(key, out var val))
                throw new KeyNotFoundException($"The key \"{key}\" was not found");
            return val;
        }

        /// <summary>
        /// Gets the value for the specific key if found, else return the default value.
        /// </summary>
        /// <param name="key">Settings key</param>
        /// <param name="defaultValue">Default value that will be returned when the key was not found</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Throwed when the key is null, empty or whitespace.</exception>
        public string Get(string key, string defaultValue)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key), "The settings key cannot be null or empty or whitespace.");

            if (!TryGet(key, out var val))
                return defaultValue;
            return val;
        }

        /// <summary>
        /// Retrieves all elements for the dictionary.
        /// </summary>
        /// <returns>Dictionary of keys and values.</returns>
        public IReadOnlyDictionary<string, string> GetAll()
        {
            return UseCache ? GetAllFromCache() : GetAllValues();
        }

        /// <summary>
        /// Initializes values for the settings.
        /// </summary>
        /// <param name="useCache">Defines if the settings class should use cache</param>
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
        /// Sets a new value for the specific key.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        protected abstract void SetValue(string key, string value);

        /// <summary>
        /// Returns true and the value for the specific key if exists, else return false and null value.
        /// </summary>
        /// <param name="key">Settings key</param>
        /// <param name="value">Returned value</param>
        /// <returns>True if key exists, else false.</returns>
        protected abstract bool TryGetValue(string key, out string value);

        /// <summary>
        /// Returns all keys with their values.
        /// </summary>
        protected abstract IReadOnlyDictionary<string, string> GetAllValues();

        /// <summary>
        /// Tries to get the settings value from the cache or by method.
        /// </summary>
        /// <param name="key">Settings key</param>
        /// <param name="value">Retrieved settings value</param>
        /// <returns>True if the settings value exists or false if not.</returns>
        private bool TryGet(string key, out string value)
        {
            return UseCache ? TryGetUsingCache(key, out value) : TryGetValue(key, out value);
        }

        /// <summary>
        /// Tries to get the settings value from the cache.<para/>
        /// If it not exists in the cache then it tries to load it and stores it in the cache.<para/>
        /// if the <paramref name="replaceAbsoluteExpiration"/> is true then it replaces the absolute expiration time for the settings value in the cache.
        /// </summary>
        /// <param name="key">Settings key</param>
        /// <param name="value">Retrieved settings value</param>
        /// <param name="replaceAbsoluteExpiration">Flag to know if to replace the absolute expiration time for the object in the cache</param>
        /// <returns>True if the settings value exists or false if not.</returns>
        private bool TryGetUsingCache(string key, out string value, bool replaceAbsoluteExpiration = false)
        {
            value = GetFromCache(key, out var genKey);
            if (value == null || replaceAbsoluteExpiration)
            {
                lock (_lock)
                {
                    if (value == null)
                        if (!TryGetValue(key, out value))
                            return false;
                    CacheManager.Add(genKey, value, EvictPolicyGenerator.GetAbsoluteExpirationTime(CacheTimeout));
                }
            }
            return true;
        }

        private IReadOnlyDictionary<string, string> GetAllFromCache()
        {
            var keyNameForGetAllDictionary = KeyGenerator.GenerateForGetAllDictionary(DictionaryName);
            if (CacheManager.Get(keyNameForGetAllDictionary) is List<string> keys)
            {
                Dictionary<string, string> res = new Dictionary<string, string>();
                foreach (var key in keys)
                {
                    if (!TryGetUsingCache(key, out var val, true))
                        throw new KeyNotFoundException($"The key \"{key}\" was not found");
                    res.Add(key, val);
                }
                return res;
            }
            else
            {
                lock (_lock)
                {
                    var res = GetAllValues();
                    keys = new List<string>(res.Keys);
                    var absoluteExpiration = EvictPolicyGenerator.GetAbsoluteExpirationTime(CacheTimeout);
                    foreach (var key in keys)
                        AddToCache(key, res[key], absoluteExpiration);
                    CacheManager.Add(keyNameForGetAllDictionary, keys, absoluteExpiration);
                    return res;
                }
            }
        }

        /// <summary>
        /// Generates the key that will be used for cache and adds the object to cache.
        /// </summary>
        /// <param name="key">Settings key</param>
        /// <param name="value">Settings value</param>
        /// <param name="absoluteExpiration">Time when the object will expire from the cache.</param>
        private void AddToCache(string key, string value, DateTimeOffset absoluteExpiration)
        {
            CacheManager.Add(KeyGenerator.Generate(DictionaryName, key), value, absoluteExpiration);
        }

        /// <summary>
        /// Retrieves the settings value from cache or null if not exists.
        /// </summary>
        /// <param name="key">Settings key</param>
        /// <param name="genKey"></param>
        /// <returns>Settings value from cache or null if not exists.</returns>
        private string GetFromCache(string key, out string genKey)
        {
            genKey = KeyGenerator.Generate(DictionaryName, key);
            return CacheManager.Get(genKey) as string;
        }

        /// <summary>
        /// Releases all resources used by the Settings object.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing) { }
    }
}
