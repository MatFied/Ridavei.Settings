using System;

using Ridavei.Settings.Base;
using Ridavei.Settings.Exceptions;
using Ridavei.Settings.Interface;
using Ridavei.Settings.Internals;

namespace Ridavei.Settings
{
    /// <summary>
    /// Builder to receive settings.
    /// </summary>
    public class SettingsBuilder : IDisposable
    {
        private bool _useCache;
        private int _cacheTimeout = Consts.DefaultCacheTimeout;
        private AManager _manager;

        /// <summary>
        /// Static method to create the builder.
        /// </summary>
        /// <returns>Builder</returns>
        public static SettingsBuilder CreateBuilder()
        {
            return new SettingsBuilder();
        }

        private SettingsBuilder() { }

        /// <summary>
        /// Sets the flag if to cache settings objects.
        /// </summary>
        /// <param name="use"></param>
        /// <returns>Builder</returns>
        public SettingsBuilder SetCache(bool use)
        {
            _useCache = use;
            return this;
        }

        /// <summary>
        /// Sets the timeout for cache
        /// </summary>
        /// <param name="cacheItemTimeout">Timeout for the cache in seconds</param>
        /// <returns>Builder</returns>
        public SettingsBuilder SetCacheTimeout(int cacheItemTimeout)
        {
            _cacheTimeout = cacheItemTimeout;
            return this;
        }

        /// <summary>
        /// Sets the manager object used to retrieve settings.
        /// <para>The manager will be disposed by the <see cref="SettingsBuilder"/>.</para>
        /// <para>If there was a previously setted manager then it will be disposed.</para>
        /// </summary>
        /// <param name="manager">Manager object</param>
        /// <returns>Builder</returns>
        /// <exception cref="ArgumentNullException">Throwed when the manager object is null.</exception>
        public SettingsBuilder SetManager(AManager manager)
        {
            if (manager == null)
                throw new ArgumentNullException(nameof(manager), "The manager object cannot be null.");

            DisposeManager();
            _manager = manager;
            return this;
        }

        /// <summary>
        /// Retrieves the <see cref="ISettings"/> object for the specifed dictionary name.
        /// </summary>
        /// <param name="dictionaryName">Name of the dictionary</param>
        /// <exception cref="ArgumentNullException">Throwed when the name of the dictionary is null, empty or whitespace.</exception>
        /// <exception cref="ManagerNotExistsException">Throwed when the manager object was not initialized.</exception>
        /// <returns>Settings</returns>
        public ISettings GetSettings(string dictionaryName)
        {
            if (string.IsNullOrWhiteSpace(dictionaryName))
                throw new ArgumentNullException(nameof(dictionaryName), "The name of the dictionary can not be null or empty or whitespace.");
            if (_manager == null)
                throw new ManagerNotExistsException();
            _manager.Init(_useCache, _cacheTimeout);
            return _manager.GetSettings(dictionaryName);
        }

        /// <summary>
        /// Releases all resources used by the Settings builder object.
        /// </summary>
        public void Dispose()
        {
            DisposeManager();
        }

        private void DisposeManager()
        {
            if (_manager != null)
                _manager.Dispose();
        }
    }
}
