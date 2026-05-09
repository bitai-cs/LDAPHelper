using Bitai.LDAPHelper.Adapters;
using Bitai.LDAPHelper.Tests.Mocks;

namespace Bitai.LDAPHelper.Demo
{
    public class PersistentMockLdapModificationAdapter : ILdapModificationAdapter
    {
        public PersistentMockLdapModificationAdapter(LdapModificationType type, string attributeName, object value)
        {
            ModificationType = type;
            AttributeName = attributeName;
            Value = value;
        }
        
        public LdapModificationType ModificationType { get; }
        public string AttributeName { get; }
        public object Value { get; }
        public ILdapAttributeAdapter Attribute => new MockLdapAttributeAdapter(Value);
    }
}