using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LdapHelperLib
{
	public class LdhClientConfiguration
	{
		/// <summary>
		/// <see cref="LdhConnectionInfo"/>
		/// </summary>
		public LdapHelperLib.LdhConnectionInfo ServerSettings { get; set; }

		/// <summary>
		/// <see cref="LdhUserCredentials"/>
		/// </summary>
		public LdapHelperLib.LdhUserCredentials UserCredentials { get; set; }

		/// <summary>
		/// <see cref="LdhSearchLimits"/>
		/// </summary>
		public LdapHelperLib.LdhSearchLimits SearchLimits { get; set; }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="serverSettings"><see cref="LdhConnectionInfo"/></param>
		/// <param name="userCredentials"><see cref="LdhUserCredentials"/></param>
		/// <param name="searchLimits"><see cref="LdhSearchLimits"/></param>
		public LdhClientConfiguration(LdapHelperLib.LdhConnectionInfo serverSettings, LdapHelperLib.LdhUserCredentials userCredentials, LdapHelperLib.LdhSearchLimits searchLimits)
		{
			this.ServerSettings = serverSettings;
			this.UserCredentials = userCredentials;
			this.SearchLimits = searchLimits;
		}
	}
}
