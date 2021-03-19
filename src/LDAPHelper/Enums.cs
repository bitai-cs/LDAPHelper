﻿using System;
using System.Collections.Generic;
using System.Text;

namespace LDAPHelper
{
	public enum LdapServerCommonPorts : int
	{
		DefaultPort = Novell.Directory.Ldap.LdapConnection.DefaultPort,
		DefaultSslPort = Novell.Directory.Ldap.LdapConnection.DefaultSslPort,
		GlobalCatalogPort = 3268
	}
}