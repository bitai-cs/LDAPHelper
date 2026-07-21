namespace Bitai.LDAPHelper.LdapAdapters;

/// <summary>
/// Defines access to a single LDAP modification operation.
/// </summary>
public interface ILdapModificationAdapter
{
    /// <summary>
    /// Gets the modification operation type.
    /// </summary>
    LdapModificationType ModificationType { get; }

    /// <summary>
    /// Gets the attribute associated with the modification.
    /// </summary>
    ILdapAttributeAdapter Attribute { get; }
}
