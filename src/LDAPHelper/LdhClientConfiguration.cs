﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LDAPHelper
{
	public class LdhClientConfiguration
	{
		/// <summary>
		/// <see cref="LdhConnectionInfo"/>
		/// </summary>
		public LDAPHelper.LdhConnectionInfo ServerSettings { get; set; }

		/// <summary>
		/// <see cref="LdhCredentials"/>
		/// </summary>
		public LDAPHelper.LdhCredentials UserCredentials { get; set; }

		/// <summary>
		/// <see cref="LdhSearchLimits"/>
		/// </summary>
		public LDAPHelper.LdhSearchLimits SearchLimits { get; set; }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="serverSettings"><see cref="LdhConnectionInfo"/></param>
		/// <param name="userCredentials"><see cref="LdhCredentials"/></param>
		/// <param name="searchLimits"><see cref="LdhSearchLimits"/></param>
		public LdhClientConfiguration(LDAPHelper.LdhConnectionInfo serverSettings, LDAPHelper.LdhCredentials userCredentials, LDAPHelper.LdhSearchLimits searchLimits)
		{
			this.ServerSettings = serverSettings;
			this.UserCredentials = userCredentials;
			this.SearchLimits = searchLimits;
		}
	}
}
