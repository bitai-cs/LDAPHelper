// MockLdapConnectionFactoryAdapter.cs
using System.Threading.Tasks;
using Bitai.LDAPHelper.Adapters;

namespace Bitai.LDAPHelper.Tests.Mocks
{
    public class MockLdapConnectionFactoryAdapter : ILdapConnectionFactoryAdapter
    {
        private readonly MockLdapConnectionAdapter _connection;

        public MockLdapConnectionFactoryAdapter(MockLdapConnectionAdapter connection) {
            _connection = connection;
        }

        public async Task<ILdapConnectionAdapter> CreateConnectionAsync(
            ConnectionInfo connectionInfo,
            string userAccount,
            string password,
            bool bindRequired = true) {

            _connection.ConnectionTimeout = connectionInfo.ConnectionTimeout;
            _connection.SecureSocketLayer = connectionInfo.UseSSL;

            await _connection.ConnectAsync(connectionInfo.Server, connectionInfo.ServerPort);

            try {
                await _connection.BindAsync(userAccount, password);
            }
            catch (Novell.Directory.Ldap.LdapException) {
                if (bindRequired)
                    throw;
            }
            catch (Exception) {
                throw;
            }

            return (ILdapConnectionAdapter)_connection;
        }
    }
}