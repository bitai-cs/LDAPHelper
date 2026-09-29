namespace Bitai.LDAPHelper.LdapAdapters.LdapHelperMock;

/// <summary>
/// Mock implementation of <see cref="ILdapConnectionFactoryAdapter"/> that returns a provided mock connection.
/// </summary>
public class MockLdapConnectionFactoryAdapter : ILdapConnectionFactoryAdapter
{
    //public MockLdapConnectionFactoryAdapter(MockLdapConnectionAdapter connection) {
    //    connection = connection;
    //}

    public async Task<ILdapConnectionAdapter> CreateConnectionAsync(
        IConnectionInfo connectionInfo,
        string userAccount,
        string password,
        bool bindRequired = true) {

        var connection = new MockLdapConnectionAdapter()
        {
            ConnectionTimeout = connectionInfo.ConnectionTimeout,
            SecureSocketLayer = connectionInfo.UseSSL
        };

        await connection.ConnectAsync(connectionInfo.Server, connectionInfo.ServerPort);

        try {
            await connection.BindAsync(userAccount, password);
        }
        catch (LdapOperationException) {
            if (bindRequired)
                throw;
        }
        catch (Exception) {
            throw;
        }

        return (ILdapConnectionAdapter)connection;
    }
}
