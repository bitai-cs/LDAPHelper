using System;
using System.Collections.Generic;
using System.Text;

namespace LdapHelperLib
{
	public class LdapServerSettings
	{
		public LdapServerSettings(string server, int port /*, bool isGCPort*/)
		{
			ServerName = server;
			ServerPort = port;
			//IsGCPort = isGCPort;
		}

		public string ServerName { get; }

		public int ServerPort { get; }

		//public bool IsGCPort { get; }
	}
}
