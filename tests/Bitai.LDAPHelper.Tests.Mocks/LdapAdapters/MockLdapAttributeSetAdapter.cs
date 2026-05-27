using Bitai.LDAPHelper.LdapAdapters;

namespace Bitai.LDAPHelper.Tests.Mocks.LdapAdapters;

public class MockLdapAttributeSetAdapter : ILdapAttributeSetAdapter
{
    private Dictionary<string, MockLdapAttributeAdapter> _attributeDictionary = new();

    //public void AddAttribute(string name, object value) {
    //    _attributes[name] = new MockLdapAttributeAdapter(name, value);
    //}

    public void AddAttribute(string name, string value) {
        _attributeDictionary[name] = new MockLdapAttributeAdapter(name, value);
    }

    public void AddAttribute(string name, string[] values) {
        _attributeDictionary[name] = new MockLdapAttributeAdapter(name, values);
    }

    public void AddAttribute(string name, byte[] value) {
        _attributeDictionary[name] = new MockLdapAttributeAdapter(name, value);
    }

    public void AddAttribute(MockLdapAttributeAdapter attribute) {
        if (attribute != null) {
            _attributeDictionary[attribute.Name] = attribute;
        }
    }

    public bool ContainsKey(string attributeName) {
        return _attributeDictionary.ContainsKey(attributeName);
    }

    public ILdapAttributeAdapter GetAttribute(string attributeName) {
        _attributeDictionary.TryGetValue(attributeName, out var attribute);
        return attribute;
    }

    public bool RemoveAttribute(string attributeName) {
        return _attributeDictionary.Remove(attributeName);
    }

    public void Clear() {
        _attributeDictionary.Clear();
    }

    public int Count => _attributeDictionary.Count;

    public IEnumerable<string> GetAttributeNames() {
        return _attributeDictionary.Keys;
    }

    public Dictionary<string, MockLdapAttributeAdapter> GetAllAttributes() {
        return new Dictionary<string, MockLdapAttributeAdapter>(_attributeDictionary);
    }

    public bool VerifyAttributeValue(string attributeName, string expectedValue) {
        if (!_attributeDictionary.ContainsKey(attributeName))
            return false;

        var attribute = _attributeDictionary[attributeName];
        return attribute.StringValue == expectedValue;
    }

    public bool VerifyAttributeArrayValues(string attributeName, string[] expectedValues) {
        if (!_attributeDictionary.ContainsKey(attributeName))
            return false;

        var attribute = _attributeDictionary[attributeName];
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

    public bool VerifyAttributeByteValue(string attributeName, byte[] expectedValue) {
        if (!_attributeDictionary.ContainsKey(attributeName))
            return false;

        var attribute = _attributeDictionary[attributeName];
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