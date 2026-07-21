namespace Bitai.LDAPHelper.LdapAdapters;

/// <summary>
/// Defines LDAP server connection settings required to open an LDAP connection.
/// </summary>
public interface IConnectionInfo
{
    /// <summary>
    /// Gets the connection timeout in seconds.
    /// </summary>
    short ConnectionTimeout { get; }

    /// <summary>
    /// Gets the LDAP server host name or IP address.
    /// </summary>
    string Server { get; }

    /// <summary>
    /// Gets the LDAP server port.
    /// </summary>
    int ServerPort { get; }

    /// <summary>
    /// Gets a value indicating whether SSL must be used.
    /// </summary>
    bool UseSSL { get; }
}
