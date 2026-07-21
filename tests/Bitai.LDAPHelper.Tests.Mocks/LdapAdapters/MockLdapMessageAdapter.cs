using Bitai.LDAPHelper.LdapAdapters;

namespace Bitai.LDAPHelper.Tests.Mocks.LdapAdapters;

/// <summary>
/// In-memory mock LDAP message wrapper used in search queue responses.
/// </summary>
public class MockLdapMessageAdapter : ILdapMessageAdapter
{
    public MockLdapMessageAdapter(ILdapEntryAdapter entry) {
        Entry = entry;
        IsSearchResult = true;
        IsSearchDone = false;
    }

    public ILdapEntryAdapter Entry { get; }
    public bool IsSearchResult { get; set; }
    public bool IsSearchDone { get; set; }
}
