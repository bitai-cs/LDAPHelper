using System;

namespace Bitai.LDAPHelper
{
    /// <summary>
    /// Represents errors caused by invalid or inconsistent business data supplied to LDAP operations.
    /// </summary>
    [Serializable]
    public class DataValidationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataValidationException"/> class.
        /// </summary>
        public DataValidationException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataValidationException"/> class with a message.
        /// </summary>
        /// <param name="message">Validation error message.</param>
        public DataValidationException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataValidationException"/> class with a message and inner exception.
        /// </summary>
        /// <param name="message">Validation error message.</param>
        /// <param name="innerException">Underlying cause of this exception.</param>
        public DataValidationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
