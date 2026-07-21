using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Bitai.LDAPHelper.DTO
{
	/// <summary>
	/// Represents the result of an LDAP password update operation.
	/// </summary>
	public class LDAPPasswordUpdateResult : LDAPOperationResult
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public LDAPPasswordUpdateResult() {
			//Do not remove this constructor, it is required to deserialize data.
		}

		/// <summary>
		/// Initializes a password-update result.
		/// </summary>
		/// <param name="requesttag">Optional request label.</param>
		/// <param name="operationMessage">Operation message.</param>
		/// <param name="isSuccessfulOperation">Whether the operation is marked successful.</param>
		public LDAPPasswordUpdateResult(string requesttag = null, string operationMessage = "Operation completed.", bool isSuccessfulOperation = true) : base(requesttag, isSuccessfulOperation)
		{
			OperationMessage = operationMessage;
		}

		/// <summary>
		/// Initializes an unsuccessful password-update result from an exception.
		/// </summary>
		/// <param name="operationMessage">Operation message.</param>
		/// <param name="ex">Underlying error.</param>
		/// <param name="requestLabel">Optional request label.</param>
		public LDAPPasswordUpdateResult(string operationMessage, Exception ex, string requestLabel = null) : base(operationMessage, ex, requestLabel)
		{
		}
	}
}
