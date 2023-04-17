namespace Ridavei.Settings.Interfaces
{
    public interface IManager
    {
        /// <summary>
        /// Tries to retrieve the <see cref="ISettings"/> object for the specifed dictionary name.
        /// </summary>
        /// <param name="dictionaryName">Name of the dictionary</param>
        /// <param name="settings">Retrieved Settings</param>
        /// <returns>True if the Settings exists or false if not.</returns>
        protected abstract bool TryGetSettingsObject(string dictionaryName, out ISettings settings);

        /// <summary>
        /// Creates the <see cref="ISettings"/> object for the specifed dictionary name.
        /// </summary>
        /// <param name="dictionaryName">Name of the dictionary</param>
        /// <returns>Settings</returns>
        protected abstract ISettings CreateSettingsObject(string dictionaryName);
    } 
}
