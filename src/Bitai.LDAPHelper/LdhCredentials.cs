using System;
using System.Collections.Generic;
using System.Security;
using System.Text;

namespace LDAPHelper
{
	public class LdhCredentials
	{
		public LdhCredentials(string domainAccountName, string accountPassword)
		{
			DomainAccountName = domainAccountName;
			AccountPassword = accountPassword;
		}

		public string DomainAccountName { get; }
		public string AccountPassword { get; }
	}
}