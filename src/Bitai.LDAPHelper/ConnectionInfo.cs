using System;
using System.Collections.Generic;
using System.Text;

namespace Bitai.LDAPHelper
{
	public class ConnectionInfo
	{
		public ConnectionInfo(string server, int port, bool useSSL, short connectionTimeout)
		{
			ServerName = server;
			ServerPort = port;
			UseSSL = useSSL;
			ConnectionTimeout = connectionTimeout;
		}

		public string ServerName { get; }

		public int ServerPort { get; }

		public bool UseSSL { get; }

		/// <summary>
		/// Connection timeout in seconds
		/// </summary>
		public short ConnectionTimeout { get; }
	}
}
