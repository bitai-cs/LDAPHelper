using Novell.Directory.Ldap;

namespace Bitai.LDAPHelper.LdapAdapters.Novell;

/// <summary>
/// Adapter for Novell.Directory.Ldap.LdapMessage
/// </summary>
public class NovellLdapMessageAdapter : ILdapMessageAdapter
{
    private readonly LdapMessage _message;

    public NovellLdapMessageAdapter(LdapMessage message) {
        _message = message;
    }

    public ILdapEntryAdapter Entry {
        get {
            if (_message is LdapSearchResult searchResult && searchResult.Entry != null)
                return new NovellLdapEntryAdapter(searchResult.Entry);
            return null;
        }
    }

    public bool IsSearchResult => _message is LdapSearchResult;

    public bool IsSearchDone {
        get {
            throw new System.Exception();
            //_message is LdapSearchResultDone;
        }
    }
}