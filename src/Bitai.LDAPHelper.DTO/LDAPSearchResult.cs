using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Bitai.LDAPHelper.DTO
{
	/// <summary>
	/// Represents the result of an LDAP search operation.
	/// </summary>
	public class LDAPSearchResult : LDAPOperationResult
	{
		/// <summary>
		/// LDAP entries found in search.
		/// </summary>
		public IEnumerable<LDAPHelper.DTO.LDAPEntry> Entries { get; set; }




		/// <summary>
		/// Default constructor.
		/// </summary>
		public LDAPSearchResult()
		{
			//Do not remove this constructor, it is required to deserialize data.
		}

		/// <summary>
		/// Initializes a successful or unsuccessful search result with entries.
		/// </summary>
		/// <param name="requestLabel">Optional request label.</param>
		/// <param name="entries">Entries returned by the search.</param>
		/// <param name="operationMessage">Operation message.</param>
		/// <param name="isSuccessfulOperation">Whether the operation is marked successful.</param>
		public LDAPSearchResult(string requestLabel = null, IEnumerable<LDAPHelper.DTO.LDAPEntry> entries = null, string operationMessage = "Operation completed.", bool isSuccessfulOperation = true) : base(requestLabel, isSuccessfulOperation)
		{
			if (entries != null)
				Entries = entries;
			else
				Entries = new List<LDAPEntry>(); 

			OperationMessage = operationMessage;
		}

		/// <summary>
		/// Initializes an unsuccessful search result from an exception.
		/// </summary>
		/// <param name="operationMessage">Operation message.</param>
		/// <param name="exception">Underlying error.</param>
		/// <param name="requestLabel">Optional request label.</param>
		public LDAPSearchResult(string operationMessage, Exception exception, string requestLabel = null) : base(operationMessage, exception, requestLabel)
		{
		}
	}
}
