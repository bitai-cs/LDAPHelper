using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LdapHelperLib
{
	public class LdapClientConfiguration
	{
		/// <summary>
		/// <see cref="LdapConnectionInfo"/>
		/// </summary>
		public LdapHelperLib.LdapConnectionInfo ServerSettings { get; set; }

		/// <summary>
		/// <see cref="LdapUserCredentials"/>
		/// </summary>
		public LdapHelperLib.LdapUserCredentials UserCredentials { get; set; }

		/// <summary>
		/// <see cref="LdapSearchLimits"/>
		/// </summary>
		public LdapHelperLib.LdapSearchLimits SearchLimits { get; set; }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="serverSettings"><see cref="LdapConnectionInfo"/></param>
		/// <param name="userCredentials"><see cref="LdapUserCredentials"/></param>
		/// <param name="searchLimits"><see cref="LdapSearchLimits"/></param>
		public LdapClientConfiguration(LdapHelperLib.LdapConnectionInfo serverSettings, LdapHelperLib.LdapUserCredentials userCredentials, LdapHelperLib.LdapSearchLimits searchLimits)
		{
			this.ServerSettings = serverSettings;
			this.UserCredentials = userCredentials;
			this.SearchLimits = searchLimits;
		}
	}
}
