namespace Bitai.LDAPHelper.LdapAdapters;

/// <summary>
/// Adapter for LDAP modification operations
/// </summary>
public interface ILdapModificationAdapter
{
    LdapModificationType ModificationType { get; }
    ILdapAttributeAdapter Attribute { get; }
}