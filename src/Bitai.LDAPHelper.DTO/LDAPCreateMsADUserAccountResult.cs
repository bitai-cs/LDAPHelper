using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitai.LDAPHelper.DTO
{
	/// <summary>
	/// Represents the result of an MS Active Directory user-account creation operation.
	/// </summary>
	public class LDAPCreateMsADUserAccountResult : LDAPOperationResult
	{
		/// <summary>
		/// Gets or sets the created user account payload.
		/// </summary>
		public LDAPMsADUserAccount UserAccount { get; set; }




		/// <summary>
		/// Default constructor.
		/// </summary>
		public LDAPCreateMsADUserAccountResult()
		{
			//Do not remove this constructor, it is required to deserialize data.
		}

		/// <summary>
		/// Initializes a result for user-account creation.
		/// </summary>
		/// <param name="newUserAccount">Created user account data.</param>
		/// <param name="requestLabel">Optional request label.</param>
		/// <param name="operationMessage">Operation message.</param>
		/// <param name="isSuccessfulOperation">Whether the operation is marked successful.</param>
		public LDAPCreateMsADUserAccountResult(LDAPMsADUserAccount newUserAccount, string requestLabel = null, string operationMessage = "Operation completed", bool isSuccessfulOperation = true) : base(requestLabel, isSuccessfulOperation)
		{
			if (newUserAccount == null)
				throw new ArgumentNullException(nameof(newUserAccount));

			UserAccount = newUserAccount;
			OperationMessage = operationMessage;
		}

		/// <summary>
		/// Initializes an unsuccessful creation result from an exception.
		/// </summary>
		/// <param name="operationMessage">Operation message.</param>
		/// <param name="exception">Underlying error.</param>
		/// <param name="requestLabel">Optional request label.</param>
		public LDAPCreateMsADUserAccountResult(string operationMessage, Exception exception, string requestLabel = null) : base(operationMessage, exception, requestLabel)
		{
		}
	}
}
