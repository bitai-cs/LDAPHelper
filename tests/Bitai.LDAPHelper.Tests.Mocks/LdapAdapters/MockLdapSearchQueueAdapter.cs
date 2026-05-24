using Bitai.LDAPHelper.LdapAdapters;

namespace Bitai.LDAPHelper.Tests.Mocks.LdapAdapters;

public class MockLdapSearchQueueAdapter : ILdapSearchQueueAdapter
{
    private Queue<ILdapMessageAdapter> _messages = new();

    public void AddSearchResult(ILdapMessageAdapter message) {
        _messages.Enqueue(message);
    }

    public ILdapMessageAdapter GetResponse() {
        return _messages.Count > 0 ? _messages.Dequeue() : null;
    }
}