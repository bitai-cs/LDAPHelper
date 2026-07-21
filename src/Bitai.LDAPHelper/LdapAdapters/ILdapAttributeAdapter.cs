namespace Bitai.LDAPHelper.LdapAdapters; 

/// <summary>
/// Defines read access to LDAP attribute values in multiple representations.
/// </summary>
public interface ILdapAttributeAdapter
{
    /// <summary>
    /// Gets the first binary value of the attribute.
    /// </summary>
    byte[] ByteValue { get; }

    /// <summary>
    /// Gets the first string value of the attribute.
    /// </summary>
    string StringValue { get; }

    /// <summary>
    /// Gets all string values of the attribute.
    /// </summary>
    string[] StringValueArray { get; }

    /// <summary>
    /// Gets all binary values of the attribute.
    /// </summary>
    byte[][] ByteValueArray { get; }
}
