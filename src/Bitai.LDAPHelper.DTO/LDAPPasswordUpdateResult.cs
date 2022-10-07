using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Bitai.LDAPHelper.DTO
{
	public class LDAPPasswordUpdateResult : LDAPOperationResult
	{
		public LDAPPasswordUpdateResult(string requesttag = null, string operationMessage = "OK", bool isSuccessfulOperation = true) : base(requesttag, isSuccessfulOperation)
		{
			OperationMessage = operationMessage;
		}

		public LDAPPasswordUpdateResult(string operationMessage, Exception ex, string requestTag = null) : base(operationMessage, ex, requestTag)
		{
		}
	}
}