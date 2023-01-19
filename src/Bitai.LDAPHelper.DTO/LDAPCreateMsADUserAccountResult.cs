using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitai.LDAPHelper.DTO
{
	public class LDAPCreateMsADUserAccountResult : LDAPOperationResult
	{
		public LDAPMsADUserAccount UserAccount { get; set; }




		/// <summary>
		/// Defaulr constructor.
		/// </summary>
		public LDAPCreateMsADUserAccountResult()
		{
			//Do not remove this constructor, it is required to deserialize data.
		}

		public LDAPCreateMsADUserAccountResult(LDAPMsADUserAccount newUserAccount, string requestLabel = null, string operationMessage = "Operation completed", bool isSuccessfulOperation = true) : base(requestLabel, isSuccessfulOperation)
		{
			if (newUserAccount == null)
				throw new ArgumentNullException(nameof(newUserAccount));

			UserAccount = newUserAccount;
			OperationMessage = operationMessage;
		}

		public LDAPCreateMsADUserAccountResult(string operationMessage, Exception exception, string requestLabel = null) : base(operationMessage, exception, requestLabel)
		{
		}
	}
}
