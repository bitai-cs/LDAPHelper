// NovellLdapEntryAdapter.cs
using Novell.Directory.Ldap;

namespace Bitai.LDAPHelper.Adapters.Novell
{
    /// <summary>
    /// Adapter for Novell.Directory.Ldap.LdapEntry
    /// </summary>
    public class NovellLdapEntryAdapter : ILdapEntryAdapter
    {
        private readonly LdapEntry _entry;

        public NovellLdapEntryAdapter(LdapEntry entry) {
            _entry = entry;
        }

        public string DistinguishedName => _entry.Dn;

        public ILdapAttributeSetAdapter GetAttributeSet() {
            return new NovellLdapAttributeSetAdapter(_entry.GetAttributeSet());
        }
    }
}