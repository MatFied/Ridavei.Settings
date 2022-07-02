using System;

namespace Ridavei.Settings.Exceptions
{
    /// <summary>
    /// Exception throwed when there is not manager object initialized in the Builder class.
    /// </summary>
    public class ManagerNotExistsException : Exception
    {
        private const string ExceptionMessage = "Manager object was not initialized.";

        /// <summary>
        /// Default constructor for <see cref="ManagerNotExistsException"/>.
        /// </summary>
        public ManagerNotExistsException() : base(ExceptionMessage) { }
    }
}
