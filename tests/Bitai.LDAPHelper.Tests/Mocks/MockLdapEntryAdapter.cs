using Bitai.LDAPHelper.Adapters;

namespace Bitai.LDAPHelper.Tests.Mocks
{
    public class MockLdapEntryAdapter : ILdapEntryAdapter
    {
        private readonly MockLdapAttributeSetAdapter _attributeSet;

        public MockLdapEntryAdapter(string distinguishedName) {
            DistinguishedName = distinguishedName;

            _attributeSet = new MockLdapAttributeSetAdapter();
            _attributeSet.AddAttribute("distinguishedName", distinguishedName);
        }

        public MockLdapEntryAdapter(string distinguishedName, ILdapAttributeSetAdapter attributeSet) {
            DistinguishedName = distinguishedName;

            _attributeSet = (MockLdapAttributeSetAdapter)attributeSet;
        }

        public string DistinguishedName { get; }

        public void AddAttribute(string name, string value) {
            _attributeSet.AddAttribute(name, value);
        }

        public void AddAttribute(string name, string[] value) {
            _attributeSet.AddAttribute(name, value);
        }

        public void AddAttribute(string name, byte[] value) {
            _attributeSet.AddAttribute(name, value);
        }

        public ILdapAttributeSetAdapter GetAttributeSet() {
            return _attributeSet;
        }
    }
}