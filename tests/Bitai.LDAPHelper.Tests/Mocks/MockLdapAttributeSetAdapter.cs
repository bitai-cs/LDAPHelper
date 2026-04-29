// MockLdapAttributeSetAdapter.cs
using System.Collections.Generic;
using Bitai.LDAPHelper.Adapters;

namespace Bitai.LDAPHelper.Tests.Mocks
{
    public class MockLdapAttributeSetAdapter : ILdapAttributeSetAdapter
    {
        private Dictionary<string, MockLdapAttributeAdapter> _attributes = new();

        public void AddAttribute(string name, object value) {
            _attributes[name] = new MockLdapAttributeAdapter(value);
        }

        public bool ContainsKey(string attributeName) {
            return _attributes.ContainsKey(attributeName);
        }

        public ILdapAttributeAdapter GetAttribute(string attributeName) {
            return _attributes.TryGetValue(attributeName, out var attribute) ? attribute : null;
        }
    }
}