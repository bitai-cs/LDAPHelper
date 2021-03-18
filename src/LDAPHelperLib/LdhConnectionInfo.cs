using System;
using System.Collections.Generic;
using System.Text;

namespace LdapHelperLib
{
	public class LdhConnectionInfo
	{
		public LdhConnectionInfo(string server, int port, bool useSSL, short connectionTimeout, LdhUserCredentials userCredentials)
		{
			ServerName = server;
			ServerPort = port;
			UseSSL = useSSL;
			ConnectionTimeout = connectionTimeout;
			UserCredentials = userCredentials;
		}

		public string ServerName { get; }

		public int ServerPort { get; }

		public bool UseSSL { get; }

		/// <summary>
		/// Connection timeout in seconds
		/// </summary>
		public short ConnectionTimeout { get; }

		public LdhUserCredentials UserCredentials { get; }
	}
}
