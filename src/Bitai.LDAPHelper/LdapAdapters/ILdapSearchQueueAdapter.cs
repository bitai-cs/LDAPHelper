namespace Bitai.LDAPHelper.LdapAdapters;

/// <summary>
/// Target interface for LDAP search queue operations
/// </summary>
public interface ILdapSearchQueueAdapter
{
    ILdapMessageAdapter GetResponse();
}