// ILdapSearchQueueAdapter.cs
using System;

namespace Bitai.LDAPHelper.Adapters
{
    /// <summary>
    /// Target interface for LDAP search queue operations
    /// </summary>
    public interface ILdapSearchQueueAdapter
    {
        ILdapMessageAdapter GetResponse();
    }
}