using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Bitai.LDAPHelper.DTO
{
    public class LDAPSearchResult
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="customTag">Optional tag to label this object.</param>
        public LDAPSearchResult(string customTag = null)
        {
            CustomTag = customTag;
        }


        /// <summary>
        /// LDAP entries found in search.
        /// </summary>
        public IEnumerable<LDAPHelper.DTO.LDAPEntry> Entries { get; set; }

        /// <summary>
        /// Intercepted error while searching for LDAP entries.
        /// This property must not be serialized.
        /// </summary>
        [IgnoreDataMember]
        public Exception ErrorObject { get; private set; }

        /// <summary>
        /// Type name of <see cref="ErrorObject"/>.
        /// </summary>
        public string ErrorType { get; private set; }

        /// <summary>
        /// Error message of <see cref="ErrorObject"/>
        /// </summary>
        public string ErrorMessage { get; private set; }

        /// <summary>
        /// Whether or not we have information on an intercepted error.
        /// </summary>
        public bool HasErrorInfo { get => ErrorObject != null; }

        /// <summary>
        /// Custom label to tag tihs object.
        /// </summary>
        public string CustomTag { get; private set; }


        /// <summary>
        /// Store Error information.
        /// </summary>
        /// <param name="exception"></param>
        public void SetError(Exception exception)
        {
            ErrorObject = exception;
            ErrorType = exception.GetType().FullName;
            ErrorMessage = exception.Message;
        }
    }
}