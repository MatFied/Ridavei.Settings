using System;

namespace Ridavei.Settings.Exceptions
{
    /// <summary>
    /// Exception throwed when there is no manager object added in the Builder class.
    /// </summary>
    public sealed class ManagerNotExistsException : Exception
    {
        private const string ExceptionMessage = "Manager object was not added.";

        /// <summary>
        /// Default constructor for <see cref="ManagerNotExistsException"/>.
        /// </summary>
        public ManagerNotExistsException() : base(ExceptionMessage) { }
    }
}
