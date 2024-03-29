﻿using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Bitai.LDAPHelper.DTO
{
    public abstract class LDAPOperationResult
    {
		#region Properties
		public bool IsSuccessfulOperation { get; set; }

		/// <summary>
		/// Error message of <see cref="ErrorObject"/>
		/// </summary>
		public string OperationMessage { get; set; }

		/// <summary>
		/// Intercepted error while searching for LDAP entries.
		/// This property must not be serialized.
		/// </summary>
		[IgnoreDataMember]
		public Exception ErrorObject { get; protected set; }

		/// <summary>
		/// Type name of <see cref="ErrorObject"/>.
		/// </summary>
		public string ErrorType { get; set; }

		/// <summary>
		/// Whether or not we have information on an intercepted error.
		/// </summary>
		public bool HasErrorObject { get => ErrorObject != null; }

		/// <summary>
		/// Custom label to tag tihs object.
		/// </summary>
		public string RequestLabel { get; set; }
		#endregion




		#region Constructors
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="requestLabel">Optional tag to label this object.</param>
		public LDAPOperationResult(string requestLabel = null, bool isSuccessfulOperation = true)
        {
			RequestLabel = requestLabel;

			if (isSuccessfulOperation)
                SetSuccessfullOperation("OK");
            else
                SetUnsuccessfullOperation("Error");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="requestLabel"></param>
        public LDAPOperationResult(string operationMessage, Exception exception, string requestLabel = null)
        {
            if (exception == null)
                throw new ArgumentNullException(nameof(exception));

            RequestLabel = requestLabel;

            SetUnsuccessfullOperation(operationMessage, exception);
        }
		#endregion




		#region Public methods
		public void SetSuccessfullOperation(string operationMessage)
		{
			IsSuccessfulOperation = true;

			ErrorObject = null;
			ErrorType = null;

			OperationMessage = operationMessage;
		}

		/// <summary>
		/// Store Error information.
		/// </summary>
		/// <param name="exception"></param>
		public void SetUnsuccessfullOperation(string operationMessage, Exception exception)
        {
            IsSuccessfulOperation = false;

            ErrorObject = exception;
            ErrorType = exception.GetType().FullName;

            OperationMessage = operationMessage;
        }

        public void SetUnsuccessfullOperation(string operationMessage)
        {
            IsSuccessfulOperation = false;

            ErrorObject = null;
            ErrorType = null;

            OperationMessage = operationMessage;
        }
		#endregion
	}
}