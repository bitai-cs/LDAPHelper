using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Bitai.LDAPHelper.DTO
{
	public class LDAPSearchResult : LDAPOperationResult
	{
		/// <summary>
		/// LDAP entries found in search.
		/// </summary>
		public IEnumerable<LDAPHelper.DTO.LDAPEntry> Entries { get; set; }




		/// <summary>
		/// Defaulr constructor.
		/// </summary>
		public LDAPSearchResult()
		{
			//Do not remove this constructor, it is required to deserialize data.
		}

		public LDAPSearchResult(string requestTag = null, IEnumerable<LDAPHelper.DTO.LDAPEntry> entries = null, string operationMessage = "OK", bool isSuccessfulOperation = true) : base(requestTag, isSuccessfulOperation)
		{
			if (entries != null)
				Entries = entries;
			else
				Entries = new List<LDAPEntry>(); 

			OperationMessage = operationMessage;
		}

		public LDAPSearchResult(string operationMessage, Exception exception, string requestTag = null) : base(operationMessage, exception, requestTag)
		{
		}
	}
}