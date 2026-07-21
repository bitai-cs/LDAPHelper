namespace Bitai.LDAPHelper.LdapAdapters;

/// <summary>
/// LDAP modification operation types
/// </summary>
public enum LdapModificationType
{
    /// <summary>
    /// Adds an attribute value.
    /// </summary>
    Add = 0,

    /// <summary>
    /// Deletes an attribute value.
    /// </summary>
    Delete = 1,

    /// <summary>
    /// Replaces an attribute value.
    /// </summary>
    Replace = 2
}

/// <summary>
/// LDAP search scope values
/// </summary>
public enum LdapSearchScope
{
    /// <summary>
    /// Search only the base object.
    /// </summary>
    ScopeBase = 0,

    /// <summary>
    /// Search one level below the base object.
    /// </summary>
    ScopeOne = 1,

    /// <summary>
    /// Search the entire subtree below the base object.
    /// </summary>
    ScopeSub = 2
}
