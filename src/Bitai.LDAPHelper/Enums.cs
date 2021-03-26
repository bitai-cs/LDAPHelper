using System;
using System.Collections.Generic;
using System.Text;

namespace LDAPHelper
{
	public enum LdapServerDefaultPorts : int
	{
		DefaultPort = Novell.Directory.Ldap.LdapConnection.DefaultPort,
		DefaultSslPort = Novell.Directory.Ldap.LdapConnection.DefaultSslPort,
		DefaultGlobalCatalogPort = 3268,
		DefaultGlobalCatalogSslPort = 3269
	}
}