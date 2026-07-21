using Bitai.LDAPHelper.LdapAdapters;

namespace Bitai.LDAPHelper.LdapAdapters.LdapHelperMock;

/// <summary>
/// In-memory mock LDAP entry used by test scenarios.
/// </summary>
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

    public void AddAttribute(string name, object value) {
        if (value is string stringValue)
            AddAttribute(name, stringValue);
        else if (value is string[] stringArrayValue)
            AddAttribute(name, stringArrayValue);
        else if (value is byte[] byteValue)
            AddAttribute(name, byteValue);
        else
            throw new ArgumentException($"Unsupported attribute value type: {value.GetType()}");
    }

    public ILdapAttributeSetAdapter GetAttributeSet() {
        return _attributeSet;
    }
}
