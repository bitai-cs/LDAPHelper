using System.Threading.Tasks;

namespace Bitai.LDAPHelper.LdapAdapters;

/// <summary>
/// Defines a factory for creating initialized LDAP connections.
/// </summary>
public interface ILdapConnectionFactoryAdapter
{
    /// <summary>
    /// Creates a new LDAP connection and optionally binds it with credentials.
    /// </summary>
    /// <param name="connectionInfo">Server connection settings.</param>
    /// <param name="userAccount">Account identifier used for bind.</param>
    /// <param name="password">Account password used for bind.</param>
    /// <param name="bindRequired">
    /// <see langword="true"/> to fail when bind cannot be completed; otherwise <see langword="false"/>.
    /// </param>
    /// <returns>A task with an initialized LDAP connection adapter.</returns>
    Task<ILdapConnectionAdapter> CreateConnectionAsync(
        IConnectionInfo connectionInfo,
        string userAccount,
        string password,
        bool bindRequired = true);
}
