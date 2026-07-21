using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitai.LDAPHelper.DTO
{
	/// <summary>
	/// Represents the result of a user-account disable operation.
	/// </summary>
	public class LDAPDisableUserAccountOperationResult : LDAPOperationResult
	{
		/// <summary>
		/// Default constructor
		/// </summary>
		public LDAPDisableUserAccountOperationResult()
		{
			//Do not remove this constructor, it is required to deserialize data.
		}

		/// <summary>
		/// Initializes a result for a disable operation.
		/// </summary>
		/// <param name="requestLabel">Optional request label.</param>
		/// <param name="isSuccessfulOperation">Whether the operation is marked successful.</param>
		public LDAPDisableUserAccountOperationResult(string requestLabel = null, bool isSuccessfulOperation = true) : base(requestLabel, isSuccessfulOperation)
		{
		}

		/// <summary>
		/// Initializes an unsuccessful disable-operation result from an exception.
		/// </summary>
		/// <param name="operationMessage">Operation message.</param>
		/// <param name="exception">Underlying error.</param>
		/// <param name="requestLabel">Optional request label.</param>
		public LDAPDisableUserAccountOperationResult(string operationMessage, Exception exception, string requestLabel = null) : base(operationMessage, exception, requestLabel)
		{
		}
	}
}
