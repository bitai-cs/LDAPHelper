using System;
using System.Collections.Generic;
using System.Security;
using System.Text;

namespace LdapHelperLib
{
	public class LdapUserCredentials
	{
		public LdapUserCredentials(string username, string password)
		{
			Username = username;
			Password = password;
		}

		public string Username { get; }
		public string Password { get; }
	}
}
