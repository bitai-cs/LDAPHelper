using Bitai.LDAPHelper.LdapAdapters.LdapHelperMock;
using Bitai.LDAPHelper.LdapAdapters.LdapHelperMock.LdapData;
using Microsoft.Extensions.Logging.Abstractions;

namespace Bitai.LDAPHelper.Tests
{
    /// <summary>
    /// Provides a single <see cref="MockLdapPersistentConnectionFactoryAdapter"/> shared by every test in the assembly,
    /// so the mock data store is seeded exactly once.
    /// </summary>
    public static class LdapMockFixture
    {
        private static readonly Lazy<MockLdapPersistentConnectionFactoryAdapter> _factory = new(
            () => new MockLdapPersistentConnectionFactoryAdapter(
                NullLogger<MockLdapPersistentConnectionFactoryAdapter>.Instance,
                NullLogger<MockLdapDataSeeder>.Instance),
            LazyThreadSafetyMode.ExecutionAndPublication);

        public static MockLdapPersistentConnectionFactoryAdapter Factory => _factory.Value;
    }
}
