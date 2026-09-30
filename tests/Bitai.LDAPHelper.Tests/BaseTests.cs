using Bitai.LDAPHelper.LdapAdapters.LdapHelperMock;
using Bitai.LDAPHelper.LdapAdapters.LdapHelperMock.LdapData;

namespace Bitai.LDAPHelper.Tests
{
    /// <summary>
    /// Shared test-fixture helpers for LDAP helper test suites.
    /// </summary>
    public class BaseTests
    {
        /// <summary>
        /// Finds an entry in the persistent mock data store by its distinguished name.
        /// </summary>
        protected MockLdapEntryAdapter FindEntryInStore(string distinguishedName)
        {
            var entry = MockLdapDataStore.Instance.GetEntry(distinguishedName);
            if (entry == null)
                throw new InvalidOperationException($"Entry '{distinguishedName}' not found in the persistent mock data store. Make sure the seeder has been run.");
            return entry;
        }

        /// <summary>
        /// Creates a search filter for a given attribute and value.
        /// </summary>
        protected QueryFilters.AttributeFilter CreateSearchFilter(string attributeName, string value)
        {
            var attribute = (DTO.EntryAttribute)Enum.Parse(typeof(DTO.EntryAttribute), attributeName);
            return new QueryFilters.AttributeFilter(attribute, new QueryFilters.FilterValue(value));
        }

        public ConnectionInfo CreateValidConnectionInfo(bool ssl) {
            return new ConnectionInfo(server: "localhost", port: 389, useSSL: ssl, connectionTimeout: 30);
        }

        public ConnectionInfo CreateInvalidConnectionInfo(bool ssl) {
            return new ConnectionInfo(server: "0.0.0.0", port: 0, useSSL: ssl, connectionTimeout: 30);
        }

        public SearchLimits CreateValidSearchLimits() {
            return new SearchLimits("DC=va,DC=bitai,DC=com") {
                MaxSearchResults = 1000,
                MaxSearchTimeout = 60
            };
        }
    }
}
