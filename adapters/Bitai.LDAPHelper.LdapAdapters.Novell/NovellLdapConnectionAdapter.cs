using Novell.Directory.Ldap;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Bitai.LDAPHelper.LdapAdapters.Novell;

/// <summary>
/// Adapter that exposes a Novell.Directory.Ldap.LdapConnection through the project's <see cref="ILdapConnectionAdapter"/> abstraction.
/// </summary>
/// <remarks>
/// This adapter forwards calls to the wrapped <see cref="LdapConnection"/> instance and translates
/// between the project's adapter types and Novell.Directory.Ldap types where necessary.
/// The adapter is responsible for the lifetime of the wrapped connection when disposed.
/// </remarks>
public class NovellLdapConnectionAdapter : ILdapConnectionAdapter
{
    private readonly LdapConnection _ldapConnection;
    private bool _disposed = false;

    /// <summary>
    /// Initializes a new instance of the <see cref="NovellLdapConnectionAdapter"/> class.
    /// </summary>
    /// <param name="ldapConnection">The <see cref="LdapConnection"/> instance to wrap. Must not be <c>null</c>.</param>
    /// <exception cref="ArgumentNullException"><paramref name="ldapConnection"/> is <c>null</c>.</exception>
    public NovellLdapConnectionAdapter(LdapConnection ldapConnection) {
        _ldapConnection = ldapConnection ?? throw new ArgumentNullException(nameof(ldapConnection));
    }

    /// <summary>
    /// Gets or sets the connection timeout (in milliseconds) for operations on the wrapped connection.
    /// </summary>
    public int ConnectionTimeout {
        get => _ldapConnection.ConnectionTimeout;
        set => _ldapConnection.ConnectionTimeout = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether Secure Socket Layer (SSL) is enabled on the wrapped connection.
    /// </summary>
    public bool SecureSocketLayer {
        get => _ldapConnection.SecureSocketLayer;
        set => _ldapConnection.SecureSocketLayer = value;
    }

    /// <summary>
    /// Gets a value indicating whether the wrapped connection is currently bound (authenticated).
    /// </summary>
    public bool IsBound => _ldapConnection.Bound;

    /// <summary>
    /// Adds a user-defined server certificate validation delegate that unconditionally accepts the server certificate.
    /// </summary>
    /// <remarks>
    /// Use with caution: this bypasses standard SSL server certificate validation for the wrapped connection instance.
    /// It attaches a delegate that returns <c>true</c> for any certificate validation request.
    /// </remarks>
    public void ServerCertificateValidationByPass() {
        _ldapConnection.UserDefinedServerCertValidationDelegate += (sender, certificate, chain, sslPolicyErrors) => true;
    }

    /// <summary>
    /// Asynchronously connects to the specified LDAP server.
    /// </summary>
    /// <param name="host">The LDAP server host name or IP address.</param>
    /// <param name="port">The LDAP server port number.</param>
    /// <returns>A task that represents the asynchronous connect operation.</returns>
    public async Task ConnectAsync(string host, int port) {
        await _ldapConnection.ConnectAsync(host, port);
    }

    /// <summary>
    /// Asynchronously binds (authenticates) the wrapped connection using the supplied distinguished name and password.
    /// </summary>
    /// <param name="userDN">The distinguished name (DN) of the user to bind as.</param>
    /// <param name="password">The password for the user.</param>
    /// <returns>A task that represents the asynchronous bind operation.</returns>
    public async Task BindAsync(string userDN, string password) {
        await _ldapConnection.BindAsync(userDN, password);
    }

    /// <summary>
    /// Performs an asynchronous LDAP search and returns a search queue adapter.
    /// </summary>
    /// <param name="searchBase">The base DN from which to start the search.</param>
    /// <param name="searchScope">The scope of the search (base, one-level, or subtree) as expected by the Novell API.</param>
    /// <param name="searchFilter">The LDAP search filter string.</param>
    /// <param name="attributeNames">An array of attribute names to return. Passing <c>null</c> requests all attributes.</param>
    /// <param name="typesOnly">Whether to return only attribute types (no values).</param>
    /// <param name="constraints">Optional search constraints adapter which will be translated to the Novell constraints type.</param>
    /// <returns>A task that produces an <see cref="ILdapSearchQueueAdapter"/> wrapping the resulting <see cref="LdapSearchQueue"/>.</returns>
    public async Task<ILdapSearchQueueAdapter> SearchAsync(ISearchLimits searchLimits, string searchFilter, string[] attributeNames, bool typesOnly) {
        var novellConstraints = new LdapSearchConstraints() {
            ServerTimeLimit = searchLimits.MaxSearchTimeout,
            MaxResults = searchLimits.MaxSearchResults
        };

        int novellSearchScope = searchLimits.LdapSearchScope switch {
            LdapSearchScope.ScopeBase => LdapConnection.ScopeBase,
            LdapSearchScope.ScopeOne => LdapConnection.ScopeOne,
            LdapSearchScope.ScopeSub => LdapConnection.ScopeSub,
            _ => throw new ArgumentOutOfRangeException(nameof(searchLimits.LdapSearchScope))
        };

        LdapSearchQueue searchQueue = await _ldapConnection.SearchAsync(searchLimits.BaseDN, novellSearchScope, searchFilter, attributeNames, typesOnly, null, novellConstraints);

        return new NovellLdapSearchQueueAdapter(searchQueue);
    }

    /// <summary>
    /// Creates a new <see cref="ILdapAttributeSetAdapter"/> instance wrapping an empty <see cref="LdapAttributeSet"/>.
    /// </summary>
    /// <returns>An adapter for constructing attribute sets to supply to add/modify operations.</returns>
    public ILdapAttributeSetAdapter CreateAttributeSet() {
        return new NovellLdapAttributeSetAdapter(new LdapAttributeSet());
    }

    /// <summary>
    /// Adds an LDAP entry with the specified distinguished name and attributes.
    /// </summary>
    /// <param name="distinguishedName">The distinguished name (DN) of the new entry.</param>
    /// <param name="attributes">An attribute set adapter providing attributes for the new entry.</param>
    /// <returns>A task that represents the asynchronous add operation.</returns>
    /// <exception cref="InvalidCastException">If <paramref name="attributes"/> is not a <see cref="NovellLdapAttributeSetAdapter"/> instance.</exception>
    public async Task AddEntryAsync(string distinguishedName, ILdapAttributeSetAdapter attributes) {
        var novellAttributeSet = ((NovellLdapAttributeSetAdapter)attributes).GetWrappedAttributeSet();
        var entry = new LdapEntry(distinguishedName, novellAttributeSet);

        await _ldapConnection.AddAsync(entry);
    }

    /// <summary>
    /// Modifies an existing LDAP entry using the supplied modifications.
    /// </summary>
    /// <param name="distinguishedName">The distinguished name (DN) of the entry to modify.</param>
    /// <param name="modifications">A collection of modification adapters describing the changes to apply.</param>
    /// <returns>A task that represents the asynchronous modify operation.</returns>
    /// <exception cref="InvalidCastException">If any element of <paramref name="modifications"/> is not a <see cref="NovellLdapModificationAdapter"/>.</exception>
    public async Task ModifyEntryAsync(string distinguishedName, IEnumerable<ILdapModificationAdapter> modifications) {
        var novellModifications = modifications
            .Select(m => ((NovellLdapModificationAdapter)m).GetWrappedModification())
            .ToArray();

        await _ldapConnection.ModifyAsync(distinguishedName, novellModifications);
    }

    /// <summary>
    /// Deletes the LDAP entry with the specified distinguished name.
    /// </summary>
    /// <param name="distinguishedName">The distinguished name (DN) of the entry to delete.</param>
    /// <returns>A task that represents the asynchronous delete operation.</returns>
    public async Task DeleteEntryAsync(string distinguishedName) {
        await _ldapConnection.DeleteAsync(distinguishedName);
    }

    /// <summary>
    /// Creates an <see cref="ILdapModificationAdapter"/> instance representing a single modification operation.
    /// </summary>
    /// <param name="type">The modification type (Add, Delete, Replace) defined by <see cref="LdapModificationType"/>.</param>
    /// <param name="attributeName">The name of the attribute to modify.</param>
    /// <param name="value">
    /// The value to use for the modification. Supported runtime types:
    /// <list type="bullet">
    /// <item><description><see cref="byte[]"/> - binary attribute value</description></item>
    /// <item><description><see cref="string[]"/> - multi-valued string attribute</description></item>
    /// <item><description>other - will be converted to string via <c>ToString()</c></description></item>
    /// </list>
    /// </param>
    /// <returns>An <see cref="ILdapModificationAdapter"/> wrapping the underlying <see cref="LdapModification"/>.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="type"/> is not one of the supported modification types.</exception>
    public ILdapModificationAdapter CreateModification(LdapModificationType type, string attributeName, object value) {
        LdapAttribute attribute;

        if (value is byte[] bytes)
            attribute = new LdapAttribute(attributeName, bytes);
        else if (value is string[] strings)
            attribute = new LdapAttribute(attributeName, strings);
        else
            attribute = new LdapAttribute(attributeName, value?.ToString());

        var novellModificationType = type == LdapModificationType.Add ? LdapModification.Add : type == LdapModificationType.Delete ? LdapModification.Delete : type == LdapModificationType.Replace ? LdapModification.Replace : throw new ArgumentOutOfRangeException("Unkown type of modification.");
        var modification = new LdapModification(novellModificationType, attribute);

        return new NovellLdapModificationAdapter(modification);
    }

    /// <summary>
    /// Disconnects the wrapped LDAP connection.
    /// </summary>
    /// <remarks>
    /// After calling <see cref="Disconnect"/>, the connection object may need to be reconnected before reuse.
    /// This method does not dispose the wrapped connection; call <see cref="Dispose"/> to release resources.
    /// </remarks>
    public void Disconnect() {
        _ldapConnection.Disconnect();
    }

    /// <summary>
    /// Releases all resources used by the adapter and the wrapped <see cref="LdapConnection"/>.
    /// </summary>
    public void Dispose() {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Protected implementation of the standard disposal pattern.
    /// </summary>
    /// <param name="disposing">
    /// <c>true</c> when called from <see cref="Dispose"/> to dispose managed resources;
    /// <c>false</c> when called from a finalizer.
    /// </param>
    protected virtual void Dispose(bool disposing) {
        if (!_disposed) {
            if (disposing) {
                _ldapConnection?.Dispose();
            }
            _disposed = true;
        }
    }
}
