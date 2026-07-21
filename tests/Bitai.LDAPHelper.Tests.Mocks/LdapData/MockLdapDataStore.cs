using Bitai.LDAPHelper.Tests.Mocks.LdapAdapters;

namespace Bitai.LDAPHelper.Tests.Mocks.LdapData;

/// <summary>
/// Thread-safe in-memory LDAP entry store used by persistent mock adapters.
/// </summary>
public class MockLdapDataStore
{
    private static readonly Lazy<MockLdapDataStore> _instance = new Lazy<MockLdapDataStore>(() => new MockLdapDataStore());
    public static MockLdapDataStore Instance => _instance.Value;

    private readonly Dictionary<string, MockLdapEntryAdapter> _entries;
    private readonly ReaderWriterLockSlim _lock;

    private MockLdapDataStore() {
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
