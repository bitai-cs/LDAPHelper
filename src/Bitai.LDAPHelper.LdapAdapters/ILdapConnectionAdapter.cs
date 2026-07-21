using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bitai.LDAPHelper.LdapAdapters;

/// <summary>
/// Defines the operations required to manage and use an LDAP connection.
/// </summary>
public interface ILdapConnectionAdapter : IDisposable
{
    /// <summary>
    /// Gets or sets the connection timeout in milliseconds.
    /// </summary>
    int ConnectionTimeout { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether SSL is enabled.
    /// </summary>
    bool SecureSocketLayer { get; set; }

    /// <summary>
    /// Gets a value indicating whether the connection is authenticated (bound).
    /// </summary>
    bool IsBound { get; }

    /// <summary>
    /// Bypasses server certificate validation.
    /// </summary>
    /// <remarks>Use only in controlled environments.</remarks>
    void ServerCertificateValidationByPass(); 

    /// <summary>
    /// Connects to the LDAP server.
    /// </summary>
    /// <param name="host">Server host name or IP address.</param>
    /// <param name="port">Server port.</param>
    /// <returns>A task that completes when the connection is established.</returns>
    Task ConnectAsync(string host, int port);

    /// <summary>
    /// Authenticates the connection.
    /// </summary>
    /// <param name="userDN">User distinguished name or account identifier.</param>
    /// <param name="password">Account password.</param>
    /// <returns>A task that completes when bind finishes.</returns>
    Task BindAsync(string userDN, string password);

    /// <summary>
    /// Executes an LDAP search.
    /// </summary>
    /// <param name="searchLimits">Search boundaries and limits.</param>
    /// <param name="searchFilter">LDAP filter expression.</param>
    /// <param name="attributeNames">Attributes to return.</param>
    /// <param name="typesOnly">Whether only attribute names should be returned.</param>
    /// <returns>A queue adapter with server responses.</returns>
    Task<ILdapSearchQueueAdapter> SearchAsync(ISearchLimits searchLimits, string searchFilter, string[] attributeNames, bool typesOnly);

    /// <summary>
    /// Creates an empty attribute set for add/update operations.
    /// </summary>
    /// <returns>An attribute set adapter.</returns>
    ILdapAttributeSetAdapter CreateAttributeSet();

    /// <summary>
    /// Adds an entry to the directory.
    /// </summary>
    /// <param name="distinguishedName">Distinguished name for the new entry.</param>
    /// <param name="attributes">Attribute set for the new entry.</param>
    /// <returns>A task that completes when the entry is created.</returns>
    Task AddEntryAsync(string distinguishedName, ILdapAttributeSetAdapter attributes);

    /// <summary>
    /// Creates a modification descriptor.
    /// </summary>
    /// <param name="type">Modification type.</param>
    /// <param name="attributeName">Attribute name to modify.</param>
    /// <param name="value">New attribute value.</param>
    /// <returns>A modification adapter.</returns>
    ILdapModificationAdapter CreateModification(LdapModificationType type, string attributeName, object value);

    /// <summary>
    /// Applies modifications to an existing entry.
    /// </summary>
    /// <param name="distinguishedName">Target entry distinguished name.</param>
    /// <param name="modifications">Modifications to apply.</param>
    /// <returns>A task that completes when the update finishes.</returns>
    Task ModifyEntryAsync(string distinguishedName, IEnumerable<ILdapModificationAdapter> modifications);

    /// <summary>
    /// Deletes an entry from the directory.
    /// </summary>
    /// <param name="distinguishedName">Target entry distinguished name.</param>
    /// <returns>A task that completes when deletion finishes.</returns>
    Task DeleteEntryAsync(string distinguishedName);

    /// <summary>
    /// Disconnects the connection from the server.
    /// </summary>
    void Disconnect();
}
