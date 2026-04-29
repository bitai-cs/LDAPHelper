// NovellLdapConnectionAdapter.cs
using System;
using System.Threading.Tasks;
using Novell.Directory.Ldap;

namespace Bitai.LDAPHelper.Adapters.Novell
{
    /// <summary>
    /// Adapter for Novell.Directory.Ldap.LdapConnection
    /// </summary>
    public class NovellLdapConnectionAdapter : ILdapConnectionAdapter
    {
        private readonly LdapConnection _ldapConnection;
        private bool _disposed = false;

        public NovellLdapConnectionAdapter(LdapConnection ldapConnection) {
            _ldapConnection = ldapConnection ?? throw new ArgumentNullException(nameof(ldapConnection));
        }

        public int ConnectionTimeout {
            get => _ldapConnection.ConnectionTimeout;
            set => _ldapConnection.ConnectionTimeout = value;
        }

        public bool SecureSocketLayer {
            get => _ldapConnection.SecureSocketLayer;
            set => _ldapConnection.SecureSocketLayer = value;
        }

        public bool IsBound => _ldapConnection.Bound;

        public void ServerCertificateValidationByPass() {
            _ldapConnection.UserDefinedServerCertValidationDelegate += (sender, certificate, chain, sslPolicyErrors) => true;
        }

        public async Task ConnectAsync(string host, int port) {
            await _ldapConnection.ConnectAsync(host, port);
        }

        public async Task BindAsync(string userDN, string password) {
            await _ldapConnection.BindAsync(userDN, password);
        }

        public async Task<ILdapSearchQueueAdapter> SearchAsync(
            string searchBase,
            int searchScope,
            string searchFilter,
            string[] attributeNames,
            bool typesOnly,
            ILdapSearchConstraintsAdapter constraints) {
            var novellConstraints = constraints != null
                ? ((NovellLdapSearchConstraintsAdapter)constraints).GetWrappedConstraints()
                : null;

            LdapSearchQueue searchQueue = await _ldapConnection.SearchAsync(
                searchBase,
                searchScope,
                searchFilter,
                attributeNames,
                typesOnly,
                (LdapSearchQueue)null,
                novellConstraints);

            return new NovellLdapSearchQueueAdapter(searchQueue);
        }

        public void Disconnect() {
            _ldapConnection.Disconnect();
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) {
            if (!_disposed) {
                if (disposing) {
                    _ldapConnection?.Dispose();
                }
                _disposed = true;
            }
        }        
    }
}