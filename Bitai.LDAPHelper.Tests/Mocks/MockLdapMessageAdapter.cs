// MockLdapMessageAdapter.cs
using Bitai.LDAPHelper.Adapters;

namespace Bitai.LDAPHelper.Tests.Mocks
{
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
}