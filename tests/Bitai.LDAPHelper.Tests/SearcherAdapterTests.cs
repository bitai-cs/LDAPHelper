// SearcherAdapterTests.cs
using Bitai.LDAPHelper.Adapters;
using Bitai.LDAPHelper.DTO;
using Bitai.LDAPHelper.Tests.Mocks;
using NuGet.Frameworks;
using System;
using System.Buffers.Text;
using System.Threading.Tasks;
using Xunit;

namespace Bitai.LDAPHelper.Tests
{
    public class SearcherAdapterTests : BaseTests
    {
        [Fact]
        public async Task SearchEntries_ReturnsExpectedEntries() {
            var connectionInfo = CreateValidConnectionInfo(ssl: true);

            var mockConnection = new MockLdapConnectionAdapter();
            
            var mockConnectionFactory = new MockLdapConnectionFactoryAdapter(mockConnection);

            var searchLimits = CreateValidSearchLimits();

            var mockUserEntry = CreateMockUserEntry("Test", "User", searchLimits, out var userSearchFilter, out var _); 

            mockConnection.AddSearchResult(userSearchFilter.ToString(), new List<MockLdapEntryAdapter> { mockUserEntry });
           
            var credential = new LDAPDomainAccountCredential("domain", "admin", "p@55w0rd");

            var searcher = new Searcher(connectionInfo, searchLimits, credential, mockConnectionFactory);

            var result = await searcher.SearchEntriesAsync(userSearchFilter, RequiredEntryAttributes.Minimun, "TestRequest");

            Assert.True(result.IsSuccessfulOperation);
            Assert.Single(result.Entries);
            Assert.Equal(userSearchFilter.FilterValue.Value, result.Entries.First().samAccountName);
            Assert.Equal(mockUserEntry.DistinguishedName, result.Entries.First().distinguishedName);
        }

        [Fact]
        public async Task SearchEntries_ReturnsEmptyList() {
            var connectionInfo = CreateValidConnectionInfo(ssl: true);

            var mockConnection = new MockLdapConnectionAdapter();

            var mockConnectionFactory = new MockLdapConnectionFactoryAdapter(mockConnection);

            var searchLimits = CreateValidSearchLimits();

            var mockUserEntry = CreateMockUserEntry("Test", "User", searchLimits, out var userSearchFilter, out var _);

            mockConnection.AddSearchResult(userSearchFilter.ToString(), new List<MockLdapEntryAdapter> { mockUserEntry });

            var credential = new LDAPDomainAccountCredential("domain", "admin", "p@55w0rd");

            var searcher = new Searcher(connectionInfo, searchLimits, credential, mockConnectionFactory);

            GenerateCommonUserSearchFilter("Hacker", "User", searchLimits, out var unknownUserSearchFilter, out var _);

            var result = await searcher.SearchEntriesAsync(unknownUserSearchFilter, RequiredEntryAttributes.Minimun, "TestRequest");

            Assert.True(result.IsSuccessfulOperation);
            Assert.Empty(result.Entries);
        }

        [Fact]
        public async Task SearchParentEntries_ReturnsExpectedEntries() {
            var connectionInfo = CreateValidConnectionInfo(ssl: true);

            var mockConnection = new MockLdapConnectionAdapter();

            var mockConnectionFactory = new MockLdapConnectionFactoryAdapter(mockConnection);

            var searchLimits = CreateValidSearchLimits();

            var mockGroupEntry1 = CreateMockGroupEntry("Developers", "IT", searchLimits, out var groupSearchFilter1);
            var mockGroupEntry2 = CreateMockGroupEntry("Administrators", "Trusted", searchLimits, out var groupSearchFilter2);
            var mockUserEntry = CreateMockUserEntry("John", "Doe", searchLimits, out var userSearchFilter, out var _, new string[] { mockGroupEntry1.DistinguishedName, mockGroupEntry2.DistinguishedName });

            mockConnection.AddSearchResult(groupSearchFilter1.ToString(), new List<MockLdapEntryAdapter> { mockGroupEntry1 });
            mockConnection.AddSearchResult(groupSearchFilter2.ToString(), new List<MockLdapEntryAdapter> { mockGroupEntry2 });
            mockConnection.AddSearchResult(userSearchFilter.ToString(), new List<MockLdapEntryAdapter> { mockUserEntry });

            var credential = new LDAPDomainAccountCredential("domain", "admin", "p@55w0rd");

            var searcher = new Searcher(connectionInfo, searchLimits, credential, mockConnectionFactory);

            var result = await searcher.SearchParentEntriesAsync(userSearchFilter, RequiredEntryAttributes.Minimun, "TestRequest");

            Assert.True(result.IsSuccessfulOperation);
            Assert.NotEmpty(result.Entries);
        }

        [Fact]
        public async Task SearchParentEntries_ReturnsEmptyList() {
            var connectionInfo = CreateValidConnectionInfo(ssl: true);

            var mockConnection = new MockLdapConnectionAdapter();

            var mockConnectionFactory = new MockLdapConnectionFactoryAdapter(mockConnection);

            var searchLimits = CreateValidSearchLimits();

            var mockGroupEntry1 = CreateMockGroupEntry("Developers", "IT", searchLimits, out var groupSearchFilter1);
            var mockGroupEntry2 = CreateMockGroupEntry("Administrators", "Trusted", searchLimits, out var groupSearchFilter2);
            var mockUserEntry = CreateMockUserEntry("John", "Doe", searchLimits, out var userSearchFilter, out var _, new string[] { mockGroupEntry1.DistinguishedName, mockGroupEntry2.DistinguishedName });

            mockConnection.AddSearchResult(groupSearchFilter1.ToString(), new List<MockLdapEntryAdapter> { mockGroupEntry1 });
            mockConnection.AddSearchResult(groupSearchFilter2.ToString(), new List<MockLdapEntryAdapter> { mockGroupEntry2 });
            mockConnection.AddSearchResult(userSearchFilter.ToString(), new List<MockLdapEntryAdapter> { mockUserEntry });

            var credential = new LDAPDomainAccountCredential("domain", "admin", "p@55w0rd");

            var searcher = new Searcher(connectionInfo, searchLimits, credential, mockConnectionFactory);

            GenerateCommonUserSearchFilter("Dummiest", "User", searchLimits, out var expectedDummiestUserSearchFilter, out var _);
            var result = await searcher.SearchParentEntriesAsync(expectedDummiestUserSearchFilter, RequiredEntryAttributes.Minimun, "TestRequest");

            Assert.False(result.IsSuccessfulOperation);
            Assert.Empty(result.Entries);
            Assert.Contains("no one entry was found", result.OperationMessage.ToLower());
        }
    }
}