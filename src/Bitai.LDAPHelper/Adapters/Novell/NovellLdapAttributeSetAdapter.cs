// NovellLdapAttributeSetAdapter.cs
using Novell.Directory.Ldap;

namespace Bitai.LDAPHelper.Adapters.Novell
{
    /// <summary>
    /// Adapter for Novell.Directory.Ldap.LdapAttributeSet
    /// </summary>
    public class NovellLdapAttributeSetAdapter : ILdapAttributeSetAdapter
    {
        private readonly LdapAttributeSet _attributeSet;

        public NovellLdapAttributeSetAdapter(LdapAttributeSet attributeSet) {
            _attributeSet = attributeSet;
        }

        public bool ContainsKey(string attributeName) {
            return _attributeSet.ContainsKey(attributeName);
        }

        public ILdapAttributeAdapter GetAttribute(string attributeName) {
            LdapAttribute attribute = _attributeSet.GetAttribute(attributeName);
            return attribute != null ? new NovellLdapAttributeAdapter(attribute) : null;
        }
    }
}