using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bitai.LDAPHelper.LdapAdapters;

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
    Task<ILdapSearchQueueAdapter> SearchAsync(SearchLimits searchLimits, string searchFilter, string[] attributeNames, bool typesOnly);
    ILdapAttributeSetAdapter CreateAttributeSet();
    Task AddEntryAsync(string distinguishedName, ILdapAttributeSetAdapter attributes);
    ILdapModificationAdapter CreateModification(LdapModificationType type, string attributeName, object value);
    Task ModifyEntryAsync(string distinguishedName, IEnumerable<ILdapModificationAdapter> modifications);
    Task DeleteEntryAsync(string distinguishedName);
    void Disconnect();
}