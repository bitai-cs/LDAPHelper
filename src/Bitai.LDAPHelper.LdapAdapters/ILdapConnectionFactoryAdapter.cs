namespace Bitai.LDAPHelper.LdapAdapters;

/// <summary>
/// Factory interface for creating LDAP connections
/// </summary>
public interface ILdapConnectionFactoryAdapter
{
    Task<ILdapConnectionAdapter> CreateConnectionAsync(
        IConnectionInfo connectionInfo,
        string userAccount,
        string password,
        bool bindRequired = true);
}
