// MockLdapSearchQueueAdapter.cs
using System.Collections.Generic;
using Bitai.LDAPHelper.Adapters;

namespace Bitai.LDAPHelper.Tests.Mocks
{
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
}