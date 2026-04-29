// SearcherAdapterTests.cs
using Bitai.LDAPHelper.Adapters;
using Bitai.LDAPHelper.DTO;
using Bitai.LDAPHelper.Tests.Mocks;
using System;
using System.Buffers.Text;
using System.Threading.Tasks;
using Xunit;

namespace Bitai.LDAPHelper.Tests
{
    public class SearcherAdapterTests
    {
        [Fact]
        public async Task SearchEntriesAsync_WithMockAdapter_ReturnsExpectedEntries() {
            // Arrange
            var mockConnection = new MockLdapConnectionAdapter();
            var mockEntry = new MockLdapEntryAdapter("CN=Test User,DC=domain,DC=com");

            mockEntry.AddAttribute("objectSid", new byte[] { 1, 2, 3, 4, 5 });
            mockEntry.AddAttribute("sAMAccountName", "testuser");
            mockEntry.AddAttribute("cn", "Test User");
            mockEntry.AddAttribute("displayName", "Test User Display");
            mockEntry.AddAttribute("objectClass", new[] { "top", "person", "organizationalPerson", "user" });
            mockEntry.AddAttribute("userAccountControl", "512");

            mockConnection.AddSearchResult("(sAMAccountName=testuser)", new List<MockLdapEntryAdapter> { mockEntry });

            var mockFactory = new MockLdapConnectionFactoryAdapter(mockConnection);

            var connectionInfo = new ConnectionInfo(
                server: "localhost",
                port: 389,
                useSSL: false,
                connectionTimeout: 30);

            var searchLimits = new SearchLimits(baseDN: "DC=domain,DC=com") {
                MaxSearchResults = 100,
                MaxSearchTimeout = 60
            };

            var credential = new LDAPDomainAccountCredential("domain", "testuser", "password");
            var searcher = new Searcher(connectionInfo, searchLimits, credential, mockFactory);

            var filter = new QueryFilters.AttributeFilter(
                DTO.EntryAttribute.sAMAccountName,
                new QueryFilters.FilterValue("testuser"));

            // Act
            var result = await searcher.SearchEntriesAsync(
                filter,
                RequiredEntryAttributes.Minimun,
                "TestRequest");

            // Assert
            Assert.True(result.IsSuccessfulOperation);
            Assert.Single(result.Entries);
            Assert.Equal("testuser", result.Entries.First().samAccountName);
            Assert.Equal("CN=Test User,DC=domain,DC=com", result.Entries.First().distinguishedName);
        }

        [Fact]
        public async Task SearchEntriesAsync_WithNoResults_ReturnsEmptyList() {
            // Arrange
            var mockConnection = new MockLdapConnectionAdapter();
            var mockFactory = new MockLdapConnectionFactoryAdapter(mockConnection);

            var connectionInfo = new ConnectionInfo(
                server: "localhost",
                port: 389,
                useSSL: false,
                connectionTimeout: 30);

            var searchLimits = new SearchLimits(baseDN: "DC=domain,DC=com") {
                MaxSearchResults = 100,
                MaxSearchTimeout = 60
            };

            var credential = new LDAPDomainAccountCredential("domain", "testuser", "password");
            var searcher = new Searcher(connectionInfo, searchLimits, credential, mockFactory);

            var filter = new QueryFilters.AttributeFilter(
                DTO.EntryAttribute.sAMAccountName,
                new QueryFilters.FilterValue("nonexistent"));

            // Act
            var result = await searcher.SearchEntriesAsync(
                filter,
                RequiredEntryAttributes.Minimun,
                "TestRequest");

            // Assert
            Assert.True(result.IsSuccessfulOperation);
            Assert.Empty(result.Entries);
        }
    }
}