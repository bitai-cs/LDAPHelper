using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LdapHelperLib
{
	public class LdapClientConfiguration
	{
		public string Profile { get; private set; }

		public LdapHelperLib.LdapServerSettings ServerSettings { get; private set; }

		public LdapHelperLib.LdapConnectionPipeline ConnectionPipeline { get; private set; }

		public LdapHelperLib.LdapUserCredentials UserCredentials { get; private set; }

		public string BaseDN { get; private set; }

		public bool UseGC { get; private set; }

		public LdapClientConfiguration(string profile, LdapHelperLib.LdapServerSettings serverSettings, LdapHelperLib.LdapConnectionPipeline connectionPipeline, LdapHelperLib.LdapUserCredentials userCredentials, string baseDN, bool useGC)
		{
			Profile = profile;
			ServerSettings = serverSettings;
			ConnectionPipeline = connectionPipeline;
			UserCredentials = userCredentials;
			BaseDN = baseDN;
			UseGC = useGC;
		}
	}
}
