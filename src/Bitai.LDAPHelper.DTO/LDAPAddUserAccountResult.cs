using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitai.LDAPHelper.DTO
{
	public class LDAPAddUserAccountResult: LDAPOperationResult
	{
		/// <summary>
		/// Defaulr constructor.
		/// </summary>
		public LDAPAddUserAccountResult() {
			//Do not remove this constructor, it is required to deserialize data.
		}

		public LDAPAddUserAccountResult(string requestLabel = null, bool isSuccessfulOperation = true) : base(requestLabel, isSuccessfulOperation)
		{
		}

		public LDAPAddUserAccountResult(string operationMessage, Exception exception, string requestLabel = null) : base(operationMessage, exception, requestLabel)
		{
		}
	}
}
