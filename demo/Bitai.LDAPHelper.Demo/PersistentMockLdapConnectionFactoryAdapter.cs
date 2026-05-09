using System.Threading.Tasks;
using Bitai.LDAPHelper.Adapters;

namespace Bitai.LDAPHelper.Demo
{
    public class PersistentMockLdapConnectionFactoryAdapter : ILdapConnectionFactoryAdapter
    {
        private readonly PersistentMockLdapConnectionAdapter _connection;

        public PersistentMockLdapConnectionFactoryAdapter()
        {
            _connection = new PersistentMockLdapConnectionAdapter();
        }

        public Task<ILdapConnectionAdapter> CreateConnectionAsync(
            ConnectionInfo connectionInfo,
            string userAccount,
            string password,
            bool bindRequired = true)
        {
            // Always succeed in mock mode
            _connection.BindAsync(userAccount, password);
            return Task.FromResult<ILdapConnectionAdapter>(_connection);
        }
    }
}