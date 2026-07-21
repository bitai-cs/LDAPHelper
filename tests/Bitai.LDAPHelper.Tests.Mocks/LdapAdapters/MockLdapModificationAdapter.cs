using Bitai.LDAPHelper.LdapAdapters;

namespace Bitai.LDAPHelper.Tests.Mocks.LdapAdapters;

/// <summary>
/// In-memory mock implementation of <see cref="ILdapModificationAdapter"/>.
/// </summary>
public class MockLdapModificationAdapter : ILdapModificationAdapter
{
    public MockLdapModificationAdapter(LdapModificationType type, string attributeName, object value) {
        ModificationType = type;
        AttributeName = attributeName;
        Value = value;
    }

    public LdapModificationType ModificationType { get; }
    public string AttributeName { get; }
    public object Value { get; }
    public ILdapAttributeAdapter Attribute => new MockLdapAttributeAdapter(AttributeName, Value);
}

/// <summary>
/// Simple DTO used to capture applied modifications in mock connection state.
/// </summary>
public class MockModification
{
    public string DistinguishedName { get; set; }
    public LdapModificationType ModificationType { get; set; }
    public string AttributeName { get; set; }
    public object Value { get; set; }
}
