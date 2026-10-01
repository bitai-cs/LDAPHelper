using Bitai.LDAPHelper.LdapAdapters;
using Bitai.LDAPHelper.LdapAdapters.LdapHelperMock.LdapData;
using Microsoft.Extensions.Logging;

namespace Bitai.LDAPHelper.LdapAdapters.LdapHelperMock;

/// <summary>
/// Factory for creating persistent mock LDAP connections backed by shared in-memory data.
/// </summary>
public class MockLdapPersistentConnectionFactoryAdapter : ILdapConnectionFactoryAdapter
{
    //private readonly MockLdapPersistentConnectionAdapter _connection;
    private readonly ILogger<MockLdapPersistentConnectionFactoryAdapter> _logger;

    public MockLdapPersistentConnectionFactoryAdapter(ILogger<MockLdapPersistentConnectionFactoryAdapter> logger, ILogger<MockLdapDataSeeder> seederLogger)
    {
        _logger = logger;
        
        var _seeder = new MockLdapDataSeeder(seederLogger);
        _seeder.SeedAllData();
        _seeder.PrintAllData();
    }

    public Task<ILdapConnectionAdapter> CreateConnectionAsync(
        IConnectionInfo connectionInfo,
        string userAccount,
        string password,
        bool bindRequired = true)
    {
        // Always succeed in mock mode
        var _connection = new MockLdapPersistentConnectionAdapter();
        _connection.BindAsync(userAccount, password);

        return Task.FromResult<ILdapConnectionAdapter>(_connection);
    }
}
