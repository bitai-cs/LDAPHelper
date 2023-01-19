using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Bitai.LDAPHelper.DTO
{
	public class LDAPPasswordUpdateResult : LDAPOperationResult
	{
		/// <summary>
		/// Defaulr constructor.
		/// </summary>
		public LDAPPasswordUpdateResult() {
			//Do not remove this constructor, it is required to deserialize data.
		}

		public LDAPPasswordUpdateResult(string requesttag = null, string operationMessage = "Operation completed.", bool isSuccessfulOperation = true) : base(requesttag, isSuccessfulOperation)
		{
			OperationMessage = operationMessage;
		}

		public LDAPPasswordUpdateResult(string operationMessage, Exception ex, string requestLabel = null) : base(operationMessage, ex, requestLabel)
		{
		}
	}
}