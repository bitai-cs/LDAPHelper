using System;
using System.Collections.Generic;
using System.Security;
using System.Text;

namespace Bitai.LDAPHelper
{
	public class Credentials
	{
		public Credentials(string domainAccountName, string accountPassword)
		{
			DomainAccountName = domainAccountName;
			AccountPassword = accountPassword;
		}

		public string DomainAccountName { get; }
		public string AccountPassword { get; }
	}
}