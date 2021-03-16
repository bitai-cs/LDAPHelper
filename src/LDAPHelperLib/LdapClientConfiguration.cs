using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LdapHelperLib
{
	public class LdapClientConfiguration
	{
		public LdapHelperLib.LdapConnectionInfo ServerSettings { get; private set; }

		public LdapHelperLib.LdapUserCredentials UserCredentials { get; private set; }

		public string BaseDN { get; private set; }

		public LdapClientConfiguration(LdapHelperLib.LdapConnectionInfo serverSettings, LdapHelperLib.LdapUserCredentials userCredentials, string baseDN)
		{
			this.ServerSettings = serverSettings;
			this.UserCredentials = userCredentials;
			this.BaseDN = baseDN;
		}
	}
}
