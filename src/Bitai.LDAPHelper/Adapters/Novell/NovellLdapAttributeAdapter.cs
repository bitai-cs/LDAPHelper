// NovellLdapAttributeAdapter.cs
using Novell.Directory.Ldap;

namespace Bitai.LDAPHelper.Adapters.Novell
{
    /// <summary>
    /// Adapter for Novell.Directory.Ldap.LdapAttribute
    /// </summary>
    public class NovellLdapAttributeAdapter : ILdapAttributeAdapter
    {
        private readonly LdapAttribute _attribute;

        public NovellLdapAttributeAdapter(LdapAttribute attribute) {
            _attribute = attribute;
        }

        public object ByteValue => _attribute.ByteValue;

        public string StringValue => _attribute.StringValue;

        public string[] StringValueArray => _attribute.StringValueArray;
    }
}