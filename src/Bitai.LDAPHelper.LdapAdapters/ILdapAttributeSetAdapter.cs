namespace Bitai.LDAPHelper.LdapAdapters;

/// <summary>
/// Defines operations for building and reading LDAP attribute sets.
/// </summary>
public interface ILdapAttributeSetAdapter
{
    /// <summary>
    /// Adds a single string value to an attribute.
    /// </summary>
    /// <param name="name">Attribute name.</param>
    /// <param name="value">Attribute value.</param>
    void AddAttribute(string name, string value);

    /// <summary>
    /// Adds multiple string values to an attribute.
    /// </summary>
    /// <param name="name">Attribute name.</param>
    /// <param name="values">Attribute values.</param>
    void AddAttribute(string name, string[] values);

    /// <summary>
    /// Adds a binary value to an attribute.
    /// </summary>
    /// <param name="name">Attribute name.</param>
    /// <param name="value">Binary attribute value.</param>
    void AddAttribute(string name, byte[] value);

    /// <summary>
    /// Determines whether an attribute exists in the set.
    /// </summary>
    /// <param name="attributeName">Attribute name to look up.</param>
    /// <returns><see langword="true"/> if found; otherwise <see langword="false"/>.</returns>
    bool ContainsKey(string attributeName);

    /// <summary>
    /// Gets an attribute by name.
    /// </summary>
    /// <param name="attributeName">Attribute name.</param>
    /// <returns>The attribute adapter when found; otherwise <see langword="null"/>.</returns>
    ILdapAttributeAdapter GetAttribute(string attributeName);        
}
