using Bitai.LDAPHelper.Adapters;
using Bitai.LDAPHelper.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bitai.LDAPHelper.Tests.Mocks
{
    public class MockLdapConnectionAdapter : ILdapConnectionAdapter
    {
        private bool _disposed = false;
        private Dictionary<string, List<MockLdapEntryAdapter>> _searchResults = new();
        private bool _isBound = false;

        public int ConnectionTimeout { get; set; }
        public bool SecureSocketLayer { get; set; }
        public bool IsBound => _isBound;
        public List<MockLdapEntryAdapter> CreatedEntries { get; } = new List<MockLdapEntryAdapter>();
        public List<MockModification> Modifications { get; } = new List<MockModification>();
        public List<string> DeletedEntries { get; } = new List<string>();

        public void AddSearchResult(string filterPattern, List<MockLdapEntryAdapter> entries) {
            _searchResults[filterPattern] = entries;
        }

        public void ServerCertificateValidationByPass() {
            throw new InvalidOperationException($"{nameof(ServerCertificateValidationByPass)}: Not allowed in mocked classes!");
        }

        public Task ConnectAsync(string host, int port) {
            if (string.IsNullOrEmpty(host) || host == "unknown" || host == "0.0.0.0" || port <= 0)
                throw new Exception($"{nameof(MockLdapConnectionAdapter)}.{nameof(MockLdapConnectionAdapter.ConnectAsync)}: Invalid server connection!");

            return Task.CompletedTask;
        }

        public Task BindAsync(string userDN, string password) {
            if (string.IsNullOrEmpty(userDN) || string.IsNullOrEmpty(password) || userDN.Contains("hacker") || password.Contains("123456"))
                throw new Novell.Directory.Ldap.LdapException($"{nameof(MockLdapConnectionAdapter)}.{nameof(MockLdapConnectionAdapter.BindAsync)}: Invalid credentials!");

            _isBound = !string.IsNullOrEmpty(userDN) && !string.IsNullOrEmpty(password);

            return Task.CompletedTask;
        }        

        public Task<ILdapSearchQueueAdapter> SearchAsync(string searchBase, int searchScope, string searchFilter, string[] attributeNames, bool typesOnly, ILdapSearchConstraintsAdapter constraints) {
            var matchingEntries = new List<MockLdapEntryAdapter>();

            // Find matching results based on filter
            foreach (var kvp in _searchResults) {
                if (searchFilter.Contains(kvp.Key) || kvp.Key == searchFilter) {
                    matchingEntries.AddRange(kvp.Value);
                }
            }

            var mockQueue = new MockLdapSearchQueueAdapter();
            foreach (var entry in matchingEntries) {
                mockQueue.AddSearchResult(new MockLdapMessageAdapter(entry));
            }

            return Task.FromResult<ILdapSearchQueueAdapter>(mockQueue);
        }

        public ILdapAttributeSetAdapter CreateAttributeSet() {
            return new MockLdapAttributeSetAdapter();
        }

        public Task AddEntryAsync(string distinguishedName, ILdapAttributeSetAdapter attributes) {
            var mockAttributes = (MockLdapAttributeSetAdapter)attributes;
            if (!mockAttributes.ContainsKey(EntryAttribute.distinguishedName.ToString()))
                mockAttributes.AddAttribute(EntryAttribute.distinguishedName.ToString(), distinguishedName);

            var entry = new MockLdapEntryAdapter(distinguishedName, mockAttributes);

            CreatedEntries.Add(entry);

            return Task.CompletedTask;
        }

        public ILdapModificationAdapter CreateModification(LdapModificationType type, string attributeName, object value) {
            return new MockLdapModificationAdapter(type, attributeName, value);
        }

        public Task ModifyEntryAsync(string distinguishedName, IEnumerable<ILdapModificationAdapter> modifications) {
            foreach (var mod in modifications) {
                var mockMod = (MockLdapModificationAdapter)mod;

                Modifications.Add(new MockModification {
                    DistinguishedName = distinguishedName,
                    ModificationType = mockMod.ModificationType,
                    AttributeName = mockMod.AttributeName,
                    Value = mockMod.Value
                });
            }
            return Task.CompletedTask;
        }

        public Task DeleteEntryAsync(string distinguishedName) {
            DeletedEntries.Add(distinguishedName);
            return Task.CompletedTask;
        }                

        public void Disconnect() {
            _isBound = false;
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) {
            if (!_disposed) {
                if (disposing) {
                    // Cleanup
                }
                _disposed = true;
            }
        }        
    }
}