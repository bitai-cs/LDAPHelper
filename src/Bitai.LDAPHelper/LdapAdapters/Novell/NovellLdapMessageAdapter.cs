using Novell.Directory.Ldap;

namespace Bitai.LDAPHelper.LdapAdapters.Novell;

/// <summary>
/// Adapter for <see cref="LdapMessage"/>.
/// </summary>
public class NovellLdapMessageAdapter : ILdapMessageAdapter
{
    private readonly LdapMessage _message;

    /// <summary>
    /// Initializes a new instance of the <see cref="NovellLdapMessageAdapter"/> class.
    /// </summary>
    /// <param name="message">Wrapped LDAP message.</param>
    public NovellLdapMessageAdapter(LdapMessage message) {
        _message = message;
    }

    /// <inheritdoc/>
    public ILdapEntryAdapter Entry {
        get {
            if (_message is LdapSearchResult searchResult && searchResult.Entry != null)
                return new NovellLdapEntryAdapter(searchResult.Entry);
            return null;
        }
    }

    /// <inheritdoc/>
    public bool IsSearchResult => _message is LdapSearchResult;

    /// <inheritdoc/>
    public bool IsSearchDone {
        get {
            throw new System.Exception();
            //_message is LdapSearchResultDone;
        }
    }
}
