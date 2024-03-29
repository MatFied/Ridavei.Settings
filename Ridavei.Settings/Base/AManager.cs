﻿using System;
using System.Collections.Generic;

using Ridavei.Settings.Cache;
using Ridavei.Settings.Exceptions;
using Ridavei.Settings.Interfaces;

namespace Ridavei.Settings.Base
{
    /// <summary>
    /// Abstract manager class to help retrieve <see cref="ISettings"/>.
    /// </summary>
    public abstract class AManager
    {
        private static readonly object _lock = new object();

        private readonly Dictionary<string, ASettings> _settings = new Dictionary<string, ASettings>();
        private bool _initialized = false;
        internal CacheManager CacheManager;

        /// <summary>
        /// Initializes values for the manager.
        /// </summary>
        /// <param name="cacheManager">Cache manager object</param>
        internal void Init(CacheManager cacheManager)
        {
            if (_initialized)
                return;

            CacheManager = cacheManager;
            _initialized = true;
        }

        /// <summary>
        /// Retrieves the <see cref="ISettings"/> object for the specifed dictionary name.<para />
        /// Settings objects are stored in cache if caching was in the <see cref="SettingsBuilder"/> enabled.
        /// </summary>
        /// <param name="dictionaryName">Name of the dictionary</param>
        /// <returns>Settings</returns>
        /// <exception cref="ArgumentNullException">Throwed when the name of the dictionary is null, empty or whitespace.</exception>
        /// <exception cref="DictionaryNotFoundException">Throwed when the name of the dictionary was not found.</exception>
        internal ISettings GetSettings(string dictionaryName)
        {
            if (string.IsNullOrWhiteSpace(dictionaryName))
                throw new ArgumentNullException(nameof(dictionaryName), "The name of the dictionary cannot be null or empty or whitespace.");

            if (!_settings.TryGetValue(dictionaryName, out var res))
                if (!TryGetSettingsObject(dictionaryName, out res))
                    throw new DictionaryNotFoundException(dictionaryName);
                else
                    _settings.Add(dictionaryName, res);
            res.Init(CacheManager);

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
            if (string.IsNullOrWhiteSpace(dictionaryName))
                throw new ArgumentNullException(nameof(dictionaryName), "The name of the dictionary cannot be null or empty or whitespace.");

            if (!_settings.TryGetValue(dictionaryName, out var res))
                if (!TryGetSettingsObject(dictionaryName, out res))
                {
                    res = CreateSettingsObject(dictionaryName);
                    _settings.Add(dictionaryName, res);
                }
            return res;
        }

        /// <summary>
        /// Tries to retrieve the <see cref="ASettings"/> object for the specifed dictionary name.
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
    }
}
