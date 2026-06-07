namespace Bitai.LDAPHelper.LdapAdapters;

/// <summary>
/// Target interface for LDAP message operations
/// </summary>
public interface ILdapMessageAdapter
{
    ILdapEntryAdapter Entry { get; }
    bool IsSearchResult { get; }
    bool IsSearchDone { get; }
}