using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bitai.LDAPHelper
{
	public class ClientConfiguration
	{
		/// <summary>
		/// <see cref="ConnectionInfo"/>
		/// </summary>
		public LDAPHelper.ConnectionInfo ServerSettings { get; set; }

		/// <summary>
		/// <see cref="Credentials"/>
		/// </summary>
		public LDAPHelper.Credentials UserCredentials { get; set; }

        /// <summary>
        /// <see cref="LDAPHelper.SearchLimits"/>
        /// </summary>
        public LDAPHelper.SearchLimits SearchLimits { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="serverSettings"><see cref="ConnectionInfo"/></param>
        /// <param name="userCredentials"><see cref="Credentials"/></param>
        /// <param name="searchLimits"><see cref="LDAPHelper.SearchLimits"/></param>
        public ClientConfiguration(LDAPHelper.ConnectionInfo serverSettings, LDAPHelper.Credentials userCredentials, LDAPHelper.SearchLimits searchLimits)
		{
			this.ServerSettings = serverSettings;
			this.UserCredentials = userCredentials;
			this.SearchLimits = searchLimits;
		}
	}
}
