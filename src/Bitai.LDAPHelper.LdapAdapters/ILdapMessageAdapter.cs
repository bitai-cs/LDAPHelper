namespace Bitai.LDAPHelper.LdapAdapters;

/// <summary>
/// Defines accessors for LDAP protocol messages returned by search operations.
/// </summary>
public interface ILdapMessageAdapter
{
    /// <summary>
    /// Gets the LDAP entry when the message represents a search result; otherwise <see langword="null"/>.
    /// </summary>
    ILdapEntryAdapter Entry { get; }

    /// <summary>
    /// Gets a value indicating whether this message is a search-result message.
    /// </summary>
    bool IsSearchResult { get; }

    /// <summary>
    /// Gets a value indicating whether this message signals end-of-search.
    /// </summary>
    bool IsSearchDone { get; }
}
