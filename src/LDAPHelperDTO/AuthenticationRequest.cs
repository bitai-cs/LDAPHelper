using System;
using System.Collections.Generic;
using System.Text;

namespace LdapHelperDTO
{
	/// <summary>
	/// This class represents the credentials that the LDAP server will validate  
	/// </summary>
	public class AuthenticationRequest
	{
		/// <summary>
		/// Tag value used to identify the request
		/// </summary>
		public string RequestTag { get; set; }

		/// <summary>
		/// Profile configured in LDAP Web API Proxy service
		/// </summary>
		public string LdapServerProfile { get; set; }

		/// <summary>
		/// value. True: Search in global Catalog. False: Search in default directory service.
		/// </summary>
		public bool UseGC { get; set; }

		/// <summary>
		/// Domain of AccountName/>
		/// </summary>
		public string DomainName { get; set; }

		/// <summary>
		/// Network account. LDAP samAccountName.
		/// </summary>
		public string AccountName { get; set; }

		/// <summary>
		/// Network account password.
		/// </summary>
		public string Password { get; set; }
	}
}