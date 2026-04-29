// MockLdapAttributeAdapter.cs
using Bitai.LDAPHelper.Adapters;

namespace Bitai.LDAPHelper.Tests.Mocks
{
    public class MockLdapAttributeAdapter : ILdapAttributeAdapter
    {
        private readonly object _value;
        private readonly string[] _arrayValue;

        public MockLdapAttributeAdapter(object value) {
            _value = value;

            if (value is string[] array)
                _arrayValue = array;
            else if (value is string str)
                _arrayValue = new[] { str };
            else
                _arrayValue = new[] { value?.ToString() };
        }

        public object ByteValue => _value is byte[]? _value : null;

        public string StringValue => _value?.ToString();

        public string[] StringValueArray => _arrayValue;
    }
}