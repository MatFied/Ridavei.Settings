using System;
using System.Collections.Generic;

using Ridavei.Settings.Interface;
using Ridavei.Settings.Internals;

namespace Ridavei.Settings.Base
{
    /// <summary>
    /// Abstract class for the settings classes. It can retrieve and set objects in the settings.
    /// </summary>
    public abstract class ASettings : ISettings, IDisposable
    {
        /// <summary>
        /// Name of the dictionary
        /// </summary>
        public readonly string DictionaryName;

        /// <summary>
        /// Defines if the settings class should use cache.
        /// </summary>
        public readonly bool UseCache;

        /// <summary>
        /// Timeout for the cache in seconds.
        /// </summary>
        public readonly int CacheTimeout = Consts.DefaultCacheTimeout;

        /// <summary>
        /// The default constructor for settings class.
        /// </summary>
        /// <param name="dictionaryName">Name of the dictionary</param>
        /// <param name="useCache">Defines if the manager or settings class should use cache</param>
        /// <param name="cacheTimeout">Timeout for the cache in seconds</param>
        /// <exception cref="ArgumentNullException">Throwed when the name of the dictionary is null, empty or whitespace.</exception>
        public ASettings(string dictionaryName, bool useCache = false, int cacheTimeout = 0)
        {
            if (string.IsNullOrWhiteSpace(dictionaryName))
                throw new ArgumentNullException(nameof(dictionaryName), "The name of the dictionary can not be null or empty or whitespace.");
            DictionaryName = dictionaryName;
            UseCache = useCache;
            if (cacheTimeout > 0)
                CacheTimeout = cacheTimeout;
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
        /// Sets the value for the specific key and if <see cref="UseCache"/> is true stores it in the cache.
        /// </summary>
        /// <param name="key">Settings key</param>
        /// <param name="value">New value stored in the settings</param>
        /// <exception cref="ArgumentNullException">Throwed when the key or value are null, empty or whitespace.</exception>
        public void Set(string key, string value)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key), "The settings key can not be null or empty or whitespace.");
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentNullException(nameof(value), $"The settings value for \"{key}\" can not be null or empty or whitespace.");
            SetValue(key, value);
            if (UseCache)
                AddToCache(key, value, CacheManager.GetAbsoluteExpiration(CacheTimeout));
        }

        /// <summary>
        /// Gets the value for the specific key or throws an <see cref="Exception"/> when not found.
        /// </summary>
        /// <param name="key">Settings key</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Throwed when the key is null, empty or whitespace.</exception>
        /// <exception cref="KeyNotFoundException">Throwed when the key was not found.</exception>
        public string Get(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key), "The settings key can not be null or empty or whitespace.");

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
                throw new ArgumentNullException(nameof(key), "The settings key can not be null or empty or whitespace.");

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
            if (UseCache)
            {
                var keyNameForGetAllDictionary = CacheManager.GenerateKeyNameForGetAllDictionary(DictionaryName);
                if (CacheManager.Get(keyNameForGetAllDictionary) is List<string> keys)
                {
                    Dictionary<string, string> res = new Dictionary<string, string>();
                    foreach (var key in keys)
                        if (TryGetUsingCache(key, out var val, true))
                            res.Add(key, val);
                    return res;
                }
                else
                {
                    var res = GetAllValues();
                    keys = new List<string>(res.Keys);
                    var absoluteExpiration = CacheManager.GetAbsoluteExpiration(CacheTimeout);
                    foreach (var key in keys)
                        AddToCache(key, res[key], absoluteExpiration);
                    CacheManager.Add(keyNameForGetAllDictionary, keys, absoluteExpiration);
                    return res;
                }
            }
            else
                return GetAllValues();
        }

        private bool TryGet(string key, out string value)
        {
            return UseCache ? TryGetUsingCache(key, out value) : TryGetValue(key, out value);
        }

        private bool TryGetUsingCache(string key, out string value, bool replaceAbsoluteExpiration = false)
        {
            var genKey = CacheManager.GenerateKeyName(DictionaryName, key);
            value = CacheManager.Get(genKey) as string;
            if (value == null || replaceAbsoluteExpiration)
            {
                if (value == null)
                    if (!TryGetValue(key, out value))
                        return false;
                CacheManager.Add(genKey, value, CacheManager.GetAbsoluteExpiration(CacheTimeout));
            }
            return true;
        }

        /// <summary>
        /// Generates the key that will be used for cache and adds the object to cache.
        /// </summary>
        /// <param name="key">Settings key</param>
        /// <param name="value">Settings value</param>
        /// <param name="absoluteExpiration">Time when the object will expire from the cache.</param>
        private void AddToCache(string key, string value, DateTimeOffset absoluteExpiration)
        {
            var genKey = CacheManager.GenerateKeyName(DictionaryName, key);
            CacheManager.Add(genKey, value, absoluteExpiration);
        }

        /// <summary>
        /// Releases all resources used by the Settings object.
        /// </summary>
        public virtual void Dispose() { }
    }
}
