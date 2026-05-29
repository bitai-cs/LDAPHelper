using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Bitai.LDAPHelper.DTO
{
    /// <summary>
    /// Represents the base result of an LDAP operation.
    /// </summary>
    public abstract class LDAPOperationResult
    {
		#region Properties
		/// <summary>
		/// Gets or sets a value indicating whether the LDAP operation was successful.
		/// </summary>
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
		/// Custom label to tag this object.
		/// </summary>
		public string RequestLabel { get; set; }
		#endregion




		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="LDAPOperationResult"/> class.
		/// </summary>
		/// <param name="requestLabel">Optional tag to label this object.</param>
		/// <param name="isSuccessfulOperation">Indicates if the operation should be initially set as successful.</param>
		public LDAPOperationResult(string requestLabel = null, bool isSuccessfulOperation = true)
        {
			RequestLabel = requestLabel;

			if (isSuccessfulOperation)
                SetSuccessfulOperation("OK");
            else
                SetUnsuccessfulOperation("Error");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LDAPOperationResult"/> class representing an unsuccessful operation due to an exception.
        /// </summary>
        /// <param name="operationMessage">The descriptive message of the operation failure.</param>
        /// <param name="exception">The intercepted exception causing the failure.</param>
        /// <param name="requestLabel">Optional tag to label this object.</param>
        public LDAPOperationResult(string operationMessage, Exception exception, string requestLabel = null)
        {
            if (exception == null)
                throw new ArgumentNullException(nameof(exception));

            RequestLabel = requestLabel;

            SetUnsuccessfulOperation(operationMessage, exception);
        }
		#endregion




		#region Public methods
		/// <summary>
		/// Sets the state of the result to represent a successful operation.
		/// </summary>
		/// <param name="operationMessage">The success message to associate with the operation.</param>
		public void SetSuccessfulOperation(string operationMessage)
		{
			IsSuccessfulOperation = true;

			ErrorObject = null;
			ErrorType = null;

			OperationMessage = operationMessage;
		}

		/// <summary>
		/// Sets the state of the result to represent an unsuccessful operation with error details.
		/// </summary>
		/// <param name="operationMessage">The error message to associate with the operation.</param>
		/// <param name="exception">The intercepted exception containing error details.</param>
		public void SetUnsuccessfulOperation(string operationMessage, Exception exception)
        {
            IsSuccessfulOperation = false;

            ErrorObject = exception;
            ErrorType = exception.GetType().FullName;

            OperationMessage = operationMessage;
        }

		/// <summary>
		/// Sets the state of the result to represent an unsuccessful operation without an associated exception.
		/// </summary>
		/// <param name="operationMessage">The error message to associate with the operation.</param>
        public void SetUnsuccessfulOperation(string operationMessage)
        {
            IsSuccessfulOperation = false;

            ErrorObject = null;
            ErrorType = null;

            OperationMessage = operationMessage;
        }
		#endregion
	}
}