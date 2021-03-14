using System;
using System.Collections.Generic;
using System.Text;

namespace LdapHelperLib
{
	public class LdapUserCredentials
	{
		public LdapUserCredentials(string userName, string password)
		{
			UserName = userName;
			Password = password;
		}

		public string UserName { get; }
		public string Password { get; }
	}
}
