// NovellLdapConnectionFactoryAdapter.cs
using System;
using System.Threading.Tasks;
using Novell.Directory.Ldap;

namespace Bitai.LDAPHelper.Adapters.Novell
{
    /// <summary>
    /// Factory for creating Novell LDAP connection adapters
    /// </summary>
    public class NovellLdapConnectionFactoryAdapter : ILdapConnectionFactoryAdapter
    {
        public async Task<ILdapConnectionAdapter> CreateConnectionAsync(
            ConnectionInfo connectionInfo,
            string userAccount,
            string password,
            bool bindRequired = true) {
            var ldapConnection = new LdapConnection {
                ConnectionTimeout = connectionInfo.ConnectionTimeout * 1000
            };

            var adapter = new NovellLdapConnectionAdapter(ldapConnection);

            if (connectionInfo.UseSSL) {
                adapter.SecureSocketLayer = true;
                adapter.ServerCertificateValidationByPass();
            }

            await adapter.ConnectAsync(connectionInfo.Server, connectionInfo.ServerPort);

            try {
                await adapter.BindAsync(userAccount, password);
            }
            catch (LdapException) {
                if (bindRequired)
                    throw;
            }
            catch (Exception) {
                throw;
            }

            return adapter;
        }
    }
}