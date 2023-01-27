using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitai.LDAPHelper.DTO
{
	public class LDAPRemoveMsADUserAccountResult : LDAPOperationResult
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public LDAPRemoveMsADUserAccountResult()
		{
			//Do not remove this constructor, it is required to deserialize data.
		}

		public LDAPRemoveMsADUserAccountResult(string requestLabel = null, string operationMessage = "Operation completed", bool isSuccessfulOperation = true) : base(requestLabel, isSuccessfulOperation)
		{
			OperationMessage = operationMessage;
		}

		public LDAPRemoveMsADUserAccountResult(string operationMessage, Exception exception, string requestLabel = null) : base(operationMessage, exception, requestLabel)
		{
		}
	}
}
