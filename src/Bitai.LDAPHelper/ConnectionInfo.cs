namespace Bitai.LDAPHelper
{
	public class ConnectionInfo
	{
		public ConnectionInfo(string server, int port, bool useSSL, short connectionTimeout)
		{
			Server = server;
			ServerPort = port;
			UseSSL = useSSL;
			ConnectionTimeout = connectionTimeout;
		}

		public string Server { get; }

		public int ServerPort { get; }

		public bool UseSSL { get; }

		/// <summary>
		/// Connection timeout in seconds
		/// </summary>
		public short ConnectionTimeout { get; }
	}
}
