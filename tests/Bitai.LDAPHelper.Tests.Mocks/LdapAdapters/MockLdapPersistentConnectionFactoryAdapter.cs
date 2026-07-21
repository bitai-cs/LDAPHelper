using Bitai.LDAPHelper.LdapAdapters;

namespace Bitai.LDAPHelper.Tests.Mocks.LdapAdapters;

/// <summary>
/// Factory for creating persistent mock LDAP connections backed by shared in-memory data.
/// </summary>
public class MockLdapPersistentConnectionFactoryAdapter : ILdapConnectionFactoryAdapter
{
    private readonly MockLdapPersistentConnectionAdapter _connection;

    public MockLdapPersistentConnectionFactoryAdapter()
    {
        _connection = new MockLdapPersistentConnectionAdapter();
    }

    public Task<ILdapConnectionAdapter> CreateConnectionAsync(
        IConnectionInfo connectionInfo,
        string userAccount,
        string password,
        bool bindRequired = true)
    {
        // Always succeed in mock mode
        _connection.BindAsync(userAccount, password);
        return Task.FromResult<ILdapConnectionAdapter>(_connection);
    }
}
