using System;

namespace Ridavei.Settings.Exceptions
{
    /// <summary>
    /// Exception throwed when there was no dictionary found.
    /// </summary>
    public sealed class DictionaryNotFoundException : Exception
    {
        private const string ExceptionMessageFormat = "The dictionary \"{0}\" was not found.";

        /// <summary>
        /// Default constructor for <see cref="ManagerNotExistsException"/>.
        /// </summary>
        /// <param name="dictionaryName">Name of the dictionary</param>
        public DictionaryNotFoundException(string dictionaryName) : base(string.Format(ExceptionMessageFormat)) { }
    }
}
