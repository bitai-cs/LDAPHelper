namespace Bitai.LDAPHelper.LdapAdapters;

/// <summary>
/// Defines read access to an LDAP entry and its attribute set.
/// </summary>
public interface ILdapEntryAdapter
{
    /// <summary>
    /// Gets the distinguished name of the entry.
    /// </summary>
    string DistinguishedName { get; }

    /// <summary>
    /// Gets the attribute set associated with the entry.
    /// </summary>
    /// <returns>An attribute set adapter.</returns>
    ILdapAttributeSetAdapter GetAttributeSet();
}
