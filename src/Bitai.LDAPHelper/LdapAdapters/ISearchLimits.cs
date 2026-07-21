namespace Bitai.LDAPHelper.LdapAdapters;

/// <summary>
/// Defines LDAP search boundaries and limits.
/// </summary>
public interface ISearchLimits
{
    /// <summary>
    /// Gets or sets the base distinguished name from which the search starts.
    /// </summary>
    string BaseDN { get; set; }

    /// <summary>
    /// Gets or sets the LDAP search scope.
    /// </summary>
    LdapSearchScope LdapSearchScope { get; set; }

    /// <summary>
    /// Gets or sets the maximum number of entries to return.
    /// </summary>
    int MaxSearchResults { get; set; }

    /// <summary>
    /// Gets or sets the maximum server processing time in seconds.
    /// </summary>
    int MaxSearchTimeout { get; set; }
}
