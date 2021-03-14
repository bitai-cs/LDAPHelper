using System;
using System.Collections.Generic;
using System.Text;

namespace LdapHelperLib
{
	public enum DefaultServerPorts : int
	{
		DefaultPort = Novell.Directory.Ldap.LdapConnection.DEFAULT_PORT,
		SslPort = Novell.Directory.Ldap.LdapConnection.DEFAULT_SSL_PORT,
		GlobalCatalogPort = 3268
	}
}