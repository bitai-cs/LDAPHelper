using Bitai.LDAPHelper.DTO;
using Bitai.LDAPHelper.LdapAdapters.LdapHelperMock;
using Bitai.LDAPHelper.LdapAdapters.LdapHelperMock.LdapData;
using Microsoft.Extensions.Logging.Abstractions;

namespace Bitai.LDAPHelper.Tests
{
    /// <summary>
    /// Integration-style unit tests for <see cref="Searcher"/> using mock LDAP adapters.
    /// </summary>
    public class SearcherAdapterTests : BaseTests
    {
        [Fact]
        public async Task SearchEntries_ReturnsExpectedEntries() {
            var connectionInfo = CreateValidConnectionInfo(ssl: true);

            var mockConnectionFactory = LdapMockFixture.Factory;

            var searchLimits = CreateValidSearchLimits();
           
            var credential = new LDAPDomainAccountCredential("domain", "admin", "p@55w0rd");

            var searcher = new Searcher(connectionInfo, searchLimits, credential, mockConnectionFactory);

            var userSearchFilter = CreateSearchFilter("sAMAccountName", "james.dockers");

            var result = await searcher.SearchEntriesAsync(userSearchFilter, RequiredEntryAttributes.Minimun, "TestRequest");

            Assert.True(result.IsSuccessfulOperation);
            Assert.Single(result.Entries);
            Assert.Equal("james.dockers", result.Entries.First().samAccountName);
        }

        [Fact]
        public async Task SearchEntries_ReturnsEmptyList() {
            var connectionInfo = CreateValidConnectionInfo(ssl: true);

            var mockConnectionFactory = LdapMockFixture.Factory;

            var searchLimits = CreateValidSearchLimits();

            var credential = new LDAPDomainAccountCredential("domain", "admin", "p@55w0rd");

            var searcher = new Searcher(connectionInfo, searchLimits, credential, mockConnectionFactory);

            // Search for a user that doesn't exist in the seeder
            var unknownUserSearchFilter = CreateSearchFilter("sAMAccountName", "nonexistent.user");

            var result = await searcher.SearchEntriesAsync(unknownUserSearchFilter, RequiredEntryAttributes.Minimun, "TestRequest");

            Assert.True(result.IsSuccessfulOperation);
            Assert.Empty(result.Entries);
        }

        [Fact]
        public async Task SearchParentEntries_ReturnsExpectedEntries() {
            var connectionInfo = CreateValidConnectionInfo(ssl: true);

            var mockConnectionFactory = LdapMockFixture.Factory;

            var searchLimits = CreateValidSearchLimits();

            // Use data from the seeder - james.dockers is member of DomainAdmins, ITAdmins, DevOpsEng, SeniorDevOps, DevOpsLeaders
            var userDistinguishedName = "CN=James Dockers,OU=Seniors,OU=DevOps,OU=IT,DC=va,DC=bitai,DC=com";
            var mockUserEntry = FindEntryInStore(userDistinguishedName);
            var userSearchFilter = CreateSearchFilter("sAMAccountName", "james.dockers");

            var credential = new LDAPDomainAccountCredential("domain", "admin", "p@55w0rd");

            var searcher = new Searcher(connectionInfo, searchLimits, credential, mockConnectionFactory);

            var result = await searcher.SearchParentEntriesAsync(userSearchFilter, RequiredEntryAttributes.Minimun, "TestRequest");

            Assert.True(result.IsSuccessfulOperation);
            Assert.NotEmpty(result.Entries);
        }

        [Fact]
        public async Task SearchParentEntries_ReturnsEmptyList() {
            var connectionInfo = CreateValidConnectionInfo(ssl: true);

            var mockConnectionFactory = LdapMockFixture.Factory;

            var searchLimits = CreateValidSearchLimits();

            var credential = new LDAPDomainAccountCredential("domain", "admin", "p@55w0rd");

            var searcher = new Searcher(connectionInfo, searchLimits, credential, mockConnectionFactory);

            // Search for a user that doesn't exist in the seeder
            var unknownUserSearchFilter = CreateSearchFilter("sAMAccountName", "nonexistent.user");

            var result = await searcher.SearchParentEntriesAsync(unknownUserSearchFilter, RequiredEntryAttributes.Minimun, "TestRequest");

            Assert.False(result.IsSuccessfulOperation);
            Assert.Null(result.Entries);
            Assert.StartsWith("nonexistent entry", result.OperationMessage, StringComparison.OrdinalIgnoreCase);
        }
    }
}
