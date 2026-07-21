namespace Bitai.LDAPHelper.LdapAdapters;

/// <summary>
/// Defines access to queued LDAP search responses.
/// </summary>
public interface ILdapSearchQueueAdapter
{
    /// <summary>
    /// Gets the next response from the queue.
    /// </summary>
    /// <returns>The next message adapter; or <see langword="null"/> when no more responses are available.</returns>
    ILdapMessageAdapter GetResponse();
}
