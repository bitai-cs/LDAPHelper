namespace Bitai.LDAPHelper.LdapAdapters;

/// <summary>
/// Target interface for LDAP entry operations
/// </summary>
public interface ILdapEntryAdapter
{
    string DistinguishedName { get; }
    ILdapAttributeSetAdapter GetAttributeSet();
}