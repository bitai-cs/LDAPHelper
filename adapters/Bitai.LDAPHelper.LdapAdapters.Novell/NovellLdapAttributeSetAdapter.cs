using Novell.Directory.Ldap;

namespace Bitai.LDAPHelper.LdapAdapters.Novell;

/// <summary>
/// Adapter that wraps a <see cref="LdapAttributeSet"/> from the Novell LDAP library and exposes it
/// through the <see cref="ILdapAttributeSetAdapter"/> interface expected by the application.
/// </summary>
public class NovellLdapAttributeSetAdapter : ILdapAttributeSetAdapter
{
    /// <summary>
    /// The underlying Novell LDAP attribute set being adapted.
    /// </summary>
    private readonly LdapAttributeSet _attributeSet;

    /// <summary>
    /// Initializes a new instance of the <see cref="NovellLdapAttributeSetAdapter"/> class
    /// that wraps the specified <see cref="LdapAttributeSet"/>.
    /// </summary>
    /// <param name="attributeSet">
    /// The <see cref="LdapAttributeSet"/> instance to wrap. Must not be <c>null</c>.
    /// </param>
    public NovellLdapAttributeSetAdapter(LdapAttributeSet attributeSet) {
        _attributeSet = attributeSet;
    }

    /// <summary>
    /// Determines whether the attribute set contains an attribute with the specified name.
    /// </summary>
    /// <param name="attributeName">The name of the attribute to locate.</param>
    /// <returns><c>true</c> if the attribute set contains an attribute with the specified name; otherwise, <c>false</c>.</returns>
    public bool ContainsKey(string attributeName) {
        return _attributeSet.ContainsKey(attributeName);
    }

    /// <summary>
    /// Retrieves an attribute by name from the underlying <see cref="LdapAttributeSet"/>.
    /// </summary>
    /// <param name="attributeName">The name of the attribute to retrieve.</param>
    /// <returns>
    /// An <see cref="ILdapAttributeAdapter"/> that wraps the found <see cref="LdapAttribute"/>,
    /// or <c>null</c> if no attribute with the specified name exists.
    /// </returns>
    public ILdapAttributeAdapter GetAttribute(string attributeName) {
        LdapAttribute attribute = _attributeSet.GetAttribute(attributeName);
        return attribute != null ? new NovellLdapAttributeAdapter(attribute) : null;
    }

    /// <summary>
    /// Adds a single-valued string attribute to the underlying <see cref="LdapAttributeSet"/>.
    /// </summary>
    /// <param name="name">The attribute name to add.</param>
    /// <param name="value">The string value for the attribute.</param>
    public void AddAttribute(string name, string value) {
        _attributeSet.Add(new LdapAttribute(name, value));
    }

    /// <summary>
    /// Adds a multi-valued string attribute to the underlying <see cref="LdapAttributeSet"/>.
    /// </summary>
    /// <param name="name">The attribute name to add.</param>
    /// <param name="values">An array of string values for the attribute.</param>
    public void AddAttribute(string name, string[] values) {
        _attributeSet.Add(new LdapAttribute(name, values));
    }

    /// <summary>
    /// Adds a binary (byte array) attribute to the underlying <see cref="LdapAttributeSet"/>.
    /// </summary>
    /// <param name="name">The attribute name to add.</param>
    /// <param name="value">The byte array value for the attribute.</param>
    public void AddAttribute(string name, byte[] value) {
        _attributeSet.Add(new LdapAttribute(name, value));
    }

    /// <summary>
    /// Returns the wrapped <see cref="LdapAttributeSet"/> instance.
    /// </summary>
    /// <returns>The underlying <see cref="LdapAttributeSet"/> that this adapter wraps.</returns>
    public LdapAttributeSet GetWrappedAttributeSet() {
        return _attributeSet;
    }
}