using System;
using System.Collections.Generic;

namespace Ridavei.Settings.Interfaces
{
    /// <summary>
    /// Interface for the settings classes. It can retrieve and set objects in the settings.
    /// </summary>
    public interface ISettings : IDisposable
    {
        /// <summary>
        /// Sets the value for the specific key and if UseCache is true stores it in the cache.
        /// </summary>
        /// <param name="key">Settings key</param>
        /// <param name="value">New value stored in the settings</param>
        /// <exception cref="ArgumentNullException">Throwed when the key or value are null, empty or whitespace.</exception>
        void Set(string key, string value);

        /// <summary>
        /// Sets the values for the keys and if UseCache is true stores them in the cache.
        /// </summary>
        /// <param name="keyValues">Settings keys and values</param>
        /// <exception cref="ArgumentNullException">Throwed when the dictionary, key or value are null, empty or whitespace.</exception>
        void Set(IDictionary<string, string> keyValues);

        /// <summary>
        /// Gets the value for the specific key or throws a <see cref="KeyNotFoundException"/> when not found.
        /// </summary>
        /// <param name="key">Settings key</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Throwed when the key is null, empty or whitespace.</exception>
        /// <exception cref="KeyNotFoundException">Throwed when the key was not found.</exception>
        string Get(string key);

        /// <summary>
        /// Gets the value for the specific key if found, else return the default value.
        /// </summary>
        /// <param name="key">Settings key</param>
        /// <param name="defaultValue">Default value that will be returned when the key was not found</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Throwed when the key is null, empty or whitespace.</exception>
        string Get(string key, string defaultValue);

        /// <summary>
        /// Retrieves all elements for the dictionary.
        /// </summary>
        /// <returns>Dictionary of keys and values.</returns>
        IReadOnlyDictionary<string, string> GetAll();
    }
}
