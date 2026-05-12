using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Bitai.LDAPHelper.Tests.Mocks;

namespace Bitai.LDAPHelper.Demo
{
    public class MockDataStore
    {
        private static readonly Lazy<MockDataStore> _instance = new Lazy<MockDataStore>(() => new MockDataStore());
        public static MockDataStore Instance => _instance.Value;

        private readonly Dictionary<string, MockLdapEntryAdapter> _entries;
        private readonly ReaderWriterLockSlim _lock;

        private MockDataStore() {
            _entries = new Dictionary<string, MockLdapEntryAdapter>(StringComparer.OrdinalIgnoreCase);
            _lock = new ReaderWriterLockSlim();
        }

        #region Basic CRUD Operations

        public void AddOrUpdateEntry(MockLdapEntryAdapter entry) {
            _lock.EnterWriteLock();
            try {
                _entries[entry.DistinguishedName] = entry;
            }
            finally {
                _lock.ExitWriteLock();
            }
        }

        public MockLdapEntryAdapter GetEntry(string distinguishedName) {
            _lock.EnterReadLock();
            try {
                return _entries.TryGetValue(distinguishedName, out var entry) ? entry : null;
            }
            finally {
                _lock.ExitReadLock();
            }
        }

        public List<MockLdapEntryAdapter> SearchEntries(Func<MockLdapEntryAdapter, bool> predicate) {
            _lock.EnterReadLock();
            try {
                return _entries.Values.Where(predicate).ToList();
            }
            finally {
                _lock.ExitReadLock();
            }
        }

        public bool RemoveEntry(string distinguishedName) {
            _lock.EnterWriteLock();
            try {
                return _entries.Remove(distinguishedName);
            }
            finally {
                _lock.ExitWriteLock();
            }
        }

        public void Clear() {
            _lock.EnterWriteLock();
            try {
                _entries.Clear();
            }
            finally {
                _lock.ExitWriteLock();
            }
        }

        public List<MockLdapEntryAdapter> GetAllEntries() {
            _lock.EnterReadLock();
            try {
                return _entries.Values.ToList();
            }
            finally {
                _lock.ExitReadLock();
            }
        }

        public int Count {
            get {
                _lock.EnterReadLock();
                try {
                    return _entries.Count;
                }
                finally {
                    _lock.ExitReadLock();
                }
            }
        }

        #endregion
    }
}