using System;
using System.Collections.Generic;
using System.Text;

namespace LDAPHelper
{
	public class LdhSearchLimits
	{
		/// <summary>
		/// The base distinguished name (DN) to search from.
		/// </summary>
		public string BaseDN { get; set; } = "DC=com";

		/// <summary>
		/// Maximum number of search results (entries) to be returned for a search operation.
		/// A value of 0 means no limit. Default: 1000 
		/// The search operation will be terminated with an LdapException.SIZE_LIMIT_EXCEEDED if the number of results exceed the maximum.
		/// </summary>
		public int MaxSearchResults { get; set; } = 1000;

		/// <summary>
		///  Maximum number of seconds that the server waits when returning search results. 
		///  The search operation will be terminated with an LdapException.TIME_LIMIT_EXCEEDED if the operation exceeds the time limit.
		/// </summary>
		public int MaxSearchTimeout { get; set; } = 60;

		public LdhSearchLimits(string baseDN) {
			this.BaseDN = baseDN;
		}
	}
}
