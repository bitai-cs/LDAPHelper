// ILdapConnectionFactoryAdapter.cs
using System.Threading.Tasks;

namespace Bitai.LDAPHelper.Adapters
{
    /// <summary>
    /// Factory interface for creating LDAP connections
    /// </summary>
    public interface ILdapConnectionFactoryAdapter
    {
        Task<ILdapConnectionAdapter> CreateConnectionAsync(
            ConnectionInfo connectionInfo,
            string userAccount,
            string password,
            bool bindRequired = true);
    }
}