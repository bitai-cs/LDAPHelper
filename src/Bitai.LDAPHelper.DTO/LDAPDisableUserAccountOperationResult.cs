using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitai.LDAPHelper.DTO
{
	public class LDAPDisableUserAccountOperationResult : LDAPOperationResult
	{
		/// <summary>
		/// Default constructor
		/// </summary>
		public LDAPDisableUserAccountOperationResult()
		{
			//Do not remove this constructor, it is required to deserialize data.
		}

		public LDAPDisableUserAccountOperationResult(string requestLabel = null, bool isSuccessfulOperation = true) : base(requestLabel, isSuccessfulOperation)
		{
		}

		public LDAPDisableUserAccountOperationResult(string operationMessage, Exception exception, string requestLabel = null) : base(operationMessage, exception, requestLabel)
		{
		}
	}
}
