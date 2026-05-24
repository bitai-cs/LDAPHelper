using Bitai.LDAPHelper.LdapAdapters;

namespace Bitai.LDAPHelper.Tests.Mocks.LdapAdapters;

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