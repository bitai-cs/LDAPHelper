namespace Bitai.LDAPHelper.LdapAdapters;

/// <summary>
/// LDAP modification operation types
/// </summary>
public enum LdapModificationType
{
    Add = 0,
    Delete = 1,
    Replace = 2
}

/// <summary>
/// LDAP search scope values
/// </summary>
public enum LdapSearchScope
{
    ScopeBase = 0,
    ScopeOne = 1,
    ScopeSub = 2
}
