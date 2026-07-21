using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitai.LDAPHelper.DTO
{
	/// <summary>
	/// Represents the result of an MS Active Directory user-account removal operation.
	/// </summary>
	public class LDAPRemoveMsADUserAccountResult : LDAPOperationResult
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public LDAPRemoveMsADUserAccountResult()
		{
			//Do not remove this constructor, it is required to deserialize data.
		}

		/// <summary>
		/// Initializes a remove-account operation result.
		/// </summary>
		/// <param name="requestLabel">Optional request label.</param>
		/// <param name="operationMessage">Operation message.</param>
		/// <param name="isSuccessfulOperation">Whether the operation is marked successful.</param>
		public LDAPRemoveMsADUserAccountResult(string requestLabel = null, string operationMessage = "Operation completed", bool isSuccessfulOperation = true) : base(requestLabel, isSuccessfulOperation)
		{
			OperationMessage = operationMessage;
		}

		/// <summary>
		/// Initializes an unsuccessful remove-account result from an exception.
		/// </summary>
		/// <param name="operationMessage">Operation message.</param>
		/// <param name="exception">Underlying error.</param>
		/// <param name="requestLabel">Optional request label.</param>
		public LDAPRemoveMsADUserAccountResult(string operationMessage, Exception exception, string requestLabel = null) : base(operationMessage, exception, requestLabel)
		{
		}
	}
}
