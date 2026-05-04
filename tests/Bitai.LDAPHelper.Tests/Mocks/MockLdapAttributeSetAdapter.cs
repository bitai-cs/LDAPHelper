using System.Collections.Generic;
using Bitai.LDAPHelper.Adapters;

namespace Bitai.LDAPHelper.Tests.Mocks
{
    public class MockLdapAttributeSetAdapter : ILdapAttributeSetAdapter
    {
        private Dictionary<string, MockLdapAttributeAdapter> _attributes = new();

        public void AddAttribute(string name, string value) {
            _attributes[name] = new MockLdapAttributeAdapter(value);
        }

        public void AddAttribute(string name, string[] value) {
            _attributes[name] = new MockLdapAttributeAdapter(value);
        }

        public void AddAttribute(string name, byte[] value) {
            _attributes[name] = new MockLdapAttributeAdapter(value);
        }

        public bool ContainsKey(string attributeName) {
            return _attributes.ContainsKey(attributeName);
        }

        public ILdapAttributeAdapter GetAttribute(string attributeName) {
            return _attributes.TryGetValue(attributeName, out var attribute) ? attribute : null;
        }

        /// <summary>
        /// Gets all attributes for verification in tests
        /// </summary>
        public Dictionary<string, MockLdapAttributeAdapter> GetAllAttributes() {
            return new Dictionary<string, MockLdapAttributeAdapter>(_attributes);
        }

        /// <summary>
        /// Verifies that an attribute exists with the expected string value
        /// </summary>
        public bool VerifyAttributeValue(string attributeName, string expectedValue) {
            if (!_attributes.ContainsKey(attributeName))
                return false;

            var attribute = _attributes[attributeName];
            return attribute.StringValue == expectedValue;
        }

        /// <summary>
        /// Verifies that an attribute exists with the expected string array values
        /// </summary>
        public bool VerifyAttributeArrayValues(string attributeName, string[] expectedValues) {
            if (!_attributes.ContainsKey(attributeName))
                return false;

            var attribute = _attributes[attributeName];
            var actualValues = attribute.StringValueArray;

            if (actualValues == null && expectedValues == null)
                return true;

            if (actualValues == null || expectedValues == null)
                return false;

            if (actualValues.Length != expectedValues.Length)
                return false;

            for (int i = 0; i < actualValues.Length; i++) {
                if (actualValues[i] != expectedValues[i])
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Verifies that an attribute exists with the expected byte array value
        /// </summary>
        public bool VerifyAttributeByteValue(string attributeName, byte[] expectedValue) {
            if (!_attributes.ContainsKey(attributeName))
                return false;

            var attribute = _attributes[attributeName];
            var actualValue = attribute.ByteValue as byte[];

            if (actualValue == null && expectedValue == null)
                return true;

            if (actualValue == null || expectedValue == null)
                return false;

            if (actualValue.Length != expectedValue.Length)
                return false;

            for (int i = 0; i < actualValue.Length; i++) {
                if (actualValue[i] != expectedValue[i])
                    return false;
            }

            return true;
        }
    }
}