using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Bitai.LDAPHelper.DTO
{
	public class LDAPSearchResult : LDAPOperationResult
	{
		public LDAPSearchResult(IEnumerable<LDAPHelper.DTO.LDAPEntry> entries, string requestTag = null, string operationMessage = "OK", bool isSuccessfulOperation = true) : base(requestTag, isSuccessfulOperation)
		{
			Entries = entries;
			OperationMessage = operationMessage;
		}

		public LDAPSearchResult(Exception exception, string requestTag = null) : base(exception, requestTag)
		{
		}



		/// <summary>
		/// LDAP entries found in search.
		/// </summary>
		public IEnumerable<LDAPHelper.DTO.LDAPEntry> Entries { get; private set; }
	}
}