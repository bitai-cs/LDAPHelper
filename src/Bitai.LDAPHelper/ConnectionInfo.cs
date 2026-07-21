using Bitai.LDAPHelper.LdapAdapters;

namespace Bitai.LDAPHelper
{
    /// <summary>
    /// Represents LDAP server connection settings.
    /// </summary>
    public class ConnectionInfo : IConnectionInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionInfo"/> class.
        /// </summary>
        /// <param name="server">LDAP server host name or IP address.</param>
        /// <param name="port">LDAP server port.</param>
        /// <param name="useSSL">Whether SSL is enabled.</param>
        /// <param name="connectionTimeout">Connection timeout in seconds.</param>
        public ConnectionInfo(string server, int port, bool useSSL, short connectionTimeout)
        {
            Server = server;
            ServerPort = port;
            UseSSL = useSSL;
            ConnectionTimeout = connectionTimeout;
        }

        /// <summary>
        /// Gets the LDAP server host name or IP address.
        /// </summary>
        public string Server { get; }

        /// <summary>
        /// Gets the LDAP server port.
        /// </summary>
        public int ServerPort { get; }

        /// <summary>
        /// Gets a value indicating whether SSL is enabled.
        /// </summary>
        public bool UseSSL { get; }

        /// <summary>
        /// Connection timeout in seconds
        /// </summary>
        public short ConnectionTimeout { get; }
    }
}
