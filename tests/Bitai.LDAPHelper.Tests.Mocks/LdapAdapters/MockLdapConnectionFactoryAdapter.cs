using Bitai.LDAPHelper.LdapAdapters;

namespace Bitai.LDAPHelper.Tests.Mocks.LdapAdapters;

/// <summary>
/// Mock implementation of <see cref="ILdapConnectionFactoryAdapter"/> that returns a provided mock connection.
/// </summary>
public class MockLdapConnectionFactoryAdapter : ILdapConnectionFactoryAdapter
{
    private readonly MockLdapConnectionAdapter _connection;

    public MockLdapConnectionFactoryAdapter(MockLdapConnectionAdapter connection) {
        _connection = connection;
    }

    public async Task<ILdapConnectionAdapter> CreateConnectionAsync(
        IConnectionInfo connectionInfo,
        string userAccount,
        string password,
        bool bindRequired = true) {

        _connection.ConnectionTimeout = connectionInfo.ConnectionTimeout;
        _connection.SecureSocketLayer = connectionInfo.UseSSL;

        await _connection.ConnectAsync(connectionInfo.Server, connectionInfo.ServerPort);

        try {
            await _connection.BindAsync(userAccount, password);
        }
        catch (LdapOperationException) {
            if (bindRequired)
                throw;
        }
        catch (Exception) {
            throw;
        }

        return (ILdapConnectionAdapter)_connection;
    }
}
