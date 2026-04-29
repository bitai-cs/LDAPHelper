// NovellLdapSearchQueueAdapter.cs
using System.Collections.Generic;
using Novell.Directory.Ldap;

namespace Bitai.LDAPHelper.Adapters.Novell
{
    /// <summary>
    /// Adapter for Novell.Directory.Ldap.LdapSearchQueue
    /// </summary>
    public class NovellLdapSearchQueueAdapter : ILdapSearchQueueAdapter
    {
        private readonly LdapSearchQueue _searchQueue;

        public NovellLdapSearchQueueAdapter(LdapSearchQueue searchQueue) {
            _searchQueue = searchQueue;
        }

        public ILdapMessageAdapter GetResponse() {
            LdapMessage message = _searchQueue.GetResponse();
            return message != null ? new NovellLdapMessageAdapter(message) : null;
        }
    }
}