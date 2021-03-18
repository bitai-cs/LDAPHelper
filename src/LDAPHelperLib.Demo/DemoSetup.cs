using System;
using System.Collections.Generic;
using System.Text;

namespace LDAPHelperLib.Demo
{
	public class DemoSetup
	{
		public string DomainAccountName { get; set; }
		public Ldapserver[] LdapServers { get; set; }
		public Basedn[] BaseDNs { get; set; }
		public short ConnectionTimeout { get; set; }
		public bool UseSSL { get; set; }
	}

	public class Ldapserver
	{
		public string Address { get; set; }
	}

	public class Basedn
	{
		public string DN { get; set; }
	}

}
