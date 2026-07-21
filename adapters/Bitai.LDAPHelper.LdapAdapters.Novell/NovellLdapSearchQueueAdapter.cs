using Novell.Directory.Ldap;

namespace Bitai.LDAPHelper.LdapAdapters.Novell;

/// <summary>
/// Adapter for <see cref="LdapSearchQueue"/>.
/// </summary>
public class NovellLdapSearchQueueAdapter : ILdapSearchQueueAdapter
{
    private readonly LdapSearchQueue _searchQueue;

    /// <summary>
    /// Initializes a new instance of the <see cref="NovellLdapSearchQueueAdapter"/> class.
    /// </summary>
    /// <param name="searchQueue">Wrapped search queue.</param>
    public NovellLdapSearchQueueAdapter(LdapSearchQueue searchQueue) {
        _searchQueue = searchQueue;
    }

    /// <inheritdoc/>
    public ILdapMessageAdapter GetResponse() {
        LdapMessage message = _searchQueue.GetResponse();
        return message != null ? new NovellLdapMessageAdapter(message) : null;
    }
}
