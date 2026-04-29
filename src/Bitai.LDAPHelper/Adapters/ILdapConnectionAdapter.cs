// ILdapConnectionAdapter.cs
using System;
using System.Threading.Tasks;

namespace Bitai.LDAPHelper.Adapters
{
    /// <summary>
    /// Target interface for LDAP connection operations
    /// </summary>
    public interface ILdapConnectionAdapter : IDisposable
    {
        int ConnectionTimeout { get; set; }
        bool SecureSocketLayer { get; set; }
        bool IsBound { get; }

        void ServerCertificateValidationByPass(); 
        Task ConnectAsync(string host, int port);
        Task BindAsync(string userDN, string password);
        Task<ILdapSearchQueueAdapter> SearchAsync(
            string searchBase,
            int searchScope,
            string searchFilter,
            string[] attributeNames,
            bool typesOnly,
            ILdapSearchConstraintsAdapter constraints);
        void Disconnect();
    }
}