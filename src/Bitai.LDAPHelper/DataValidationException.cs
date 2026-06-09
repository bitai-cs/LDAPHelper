using System;

namespace Bitai.LDAPHelper
{
    [Serializable]
    internal class DataValidationException : Exception
    {
        public DataValidationException()
        {
        }

        public DataValidationException(string message) : base(message)
        {
        }

        public DataValidationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
