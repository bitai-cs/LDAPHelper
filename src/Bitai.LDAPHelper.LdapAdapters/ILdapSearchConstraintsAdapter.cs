namespace Bitai.LDAPHelper.LdapAdapters;

/// <summary>
/// Defines LDAP server-side search constraints.
/// </summary>
public interface ILdapSearchConstraintsAdapter
{
    /// <summary>
    /// Gets or sets the server-side time limit in seconds.
    /// </summary>
    int ServerTimeLimit { get; set; }

    /// <summary>
    /// Gets or sets the maximum number of entries returned by the server.
    /// </summary>
    int MaxResults { get; set; }
}
