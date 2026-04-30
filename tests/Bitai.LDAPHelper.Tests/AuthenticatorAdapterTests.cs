// AuthenticatorAdapterTests.cs
using System.Threading.Tasks;
using Bitai.LDAPHelper.DTO;
using Bitai.LDAPHelper.Tests.Mocks;
using Xunit;

namespace Bitai.LDAPHelper.Tests
{
    public class AuthenticatorAdapterTests : BaseTests
    {
        [Fact]
        public async Task AuthenticateUser_ReturnsSuccess() {
            //Mock connection
            var mockConnection = new MockLdapConnectionAdapter();

            //Mock connection factory
            var mockConnectionFactory = new MockLdapConnectionFactoryAdapter(mockConnection);

            //Connection information
            var connectionInfo = CreateValidConnectionInfo(true);

            var authenticator = new Authenticator(connectionInfo, mockConnectionFactory);

            var credential = new LDAPDomainAccountCredential("domain", "dummy", "p@55w0rd");

            //Execute authentication
            var result = await authenticator.AuthenticateAsync(credential, "TestAuth");

            //Assert results
            Assert.True(result.IsAuthenticated);
            Assert.True(result.IsSuccessfulOperation);
        }

        [Fact]
        public async Task AuthenticateUser_WithVerification_ReturnsSuccess() {
            //Search limits
            var searchLimits = CreateValidSearchLimits();

            //Mock LDAP entry for the user
            var mockEntry = CreateMockUserEntry("Victor", "Bastidas", searchLimits, out var expectedFilter, out var dummy);

            //Mock connection
            var mockConnection = new MockLdapConnectionAdapter();
            //Mock the search for the user
            mockConnection.AddSearchResult(expectedFilter.ToString(), new List<MockLdapEntryAdapter> { mockEntry });

            //Mock connection factory
            var mockConnectionFactory = new MockLdapConnectionFactoryAdapter(mockConnection);

            //Connection information
            var connectionInfo = CreateValidConnectionInfo(true);

            var authenticator = new Authenticator(connectionInfo, mockConnectionFactory);

            var credential = new LDAPDomainAccountCredential("domain", expectedFilter.FilterValue.Value, "p@55w0rd");
            var credentialForSearching = new LDAPDomainAccountCredential("domain", "admin", "p@55w0rd");

            //Execute authentication
            var result = await authenticator.AuthenticateAsync(credential, searchLimits, credentialForSearching, "TestAuth");

            //Assert results
            Assert.True(result.IsAuthenticated);
            Assert.True(result.IsSuccessfulOperation);
        }

        [Fact]
        public async Task AuthenticateDN_ReturnsSuccess() {
            //Mock LDAP entry for the user
            var mockEntry = CreateMockUserEntry("Victor", "Bastidas", null, out var dummy, out var expectedFilter);

            //Mock connection
            var mockConnection = new MockLdapConnectionAdapter();

            //Mock connection factory
            var mockConnectionFactory = new MockLdapConnectionFactoryAdapter(mockConnection);

            //Connection information
            var connectionInfo = CreateValidConnectionInfo(true);

            var authenticator = new Authenticator(connectionInfo, mockConnectionFactory);

            var credential = new LDAPDistinguishedNameCredential(mockEntry.DistinguishedName, "p@55w0rd");

            //Execute authentication
            var result = await authenticator.AuthenticateAsync(credential, "TestAuth");

            //Assert results
            Assert.True(result.IsAuthenticated);
            Assert.True(result.IsSuccessfulOperation);
        }

        [Fact]
        public async Task AuthenticateDN_WithVerification_ReturnsSuccess() {
            //Search limits
            var searchLimits = CreateValidSearchLimits();

            //Mock LDAP entry for the user
            var mockEntry = CreateMockUserEntry("Victor", "Bastidas", searchLimits, out var dummy, out var expectedFilter);

            //Mock connection
            var mockConnection = new MockLdapConnectionAdapter();
            //Mock the search for the user
            mockConnection.AddSearchResult(expectedFilter.ToString(), new List<MockLdapEntryAdapter> { mockEntry });

            //Mock connection factory
            var mockConnectionFactory = new MockLdapConnectionFactoryAdapter(mockConnection);

            //Connection information
            var connectionInfo = CreateValidConnectionInfo(true);

            var authenticator = new Authenticator(connectionInfo, mockConnectionFactory);

            var credential = new LDAPDistinguishedNameCredential(mockEntry.DistinguishedName, "p@55w0rd");
            var credentialForSearching = new LDAPDomainAccountCredential("domain", "admin", "p@55w0rd");

            //Execute authentication
            var result = await authenticator.AuthenticateAsync(credential, searchLimits, credentialForSearching, "TestAuth");

            //Assert results
            Assert.True(result.IsAuthenticated);
            Assert.True(result.IsSuccessfulOperation);
        }




        [Fact]
        public async Task AuthenticateUser_ReturnsFailed() {
            //Mock connection
            var mockConnection = new MockLdapConnectionAdapter();

            //Mock connection factory
            var mockConnectionFactory = new MockLdapConnectionFactoryAdapter(mockConnection);

            //Connection information
            var connectionInfo = CreateValidConnectionInfo(true);

            var authenticator = new Authenticator(connectionInfo, mockConnectionFactory);

            var credential = new LDAPDomainAccountCredential("domain", "dummy", "123456");

            //Execute authentication
            var result = await authenticator.AuthenticateAsync(credential, "TestAuth");

            //Assert results
            Assert.False(result.IsAuthenticated);
            Assert.True(result.IsSuccessfulOperation);
            Assert.True(string.IsNullOrEmpty(result.ErrorType));
            //Assert.Contains("could not be found", result.OperationMessage);
        }

        [Fact]
        public async Task AuthenticateUser_WithVerification_ReturnsFailed() {
            //Search limits
            var searchLimits = CreateValidSearchLimits();

            //Mock LDAP entry for the user
            var mockEntry = CreateMockUserEntry("Victor", "Bastidas", searchLimits, out var expectedFilter, out var dummy);

            //Mock connection
            var mockConnection = new MockLdapConnectionAdapter();
            //Mock the search for the user
            mockConnection.AddSearchResult(expectedFilter.ToString(), new List<MockLdapEntryAdapter> { mockEntry });

            //Mock connection factory
            var mockConnectionFactory = new MockLdapConnectionFactoryAdapter(mockConnection);

            //Connection information
            var connectionInfo = CreateValidConnectionInfo(true);

            var authenticator = new Authenticator(connectionInfo, mockConnectionFactory);

            var credential = new LDAPDomainAccountCredential("domain", "unknownUser", "p@55w0rd");
            var credentialForSearching = new LDAPDomainAccountCredential("domain", "admin", "p@55w0rd");

            //Execute authentication
            var result = await authenticator.AuthenticateAsync(credential, searchLimits, credentialForSearching, "TestAuth");

            //Assert results
            Assert.False(result.IsAuthenticated);
            Assert.True(result.IsSuccessfulOperation);
            Assert.True(string.IsNullOrEmpty(result.ErrorType));
        }

        [Fact]
        public async Task AuthenticateDN_ReturnsFailed() {
            //Mock LDAP entry for the user
            var mockEntry = CreateMockUserEntry("Victor", "Bastidas", null, out var dummy, out var expectedFilter);

            //Mock connection
            var mockConnection = new MockLdapConnectionAdapter();

            //Mock connection factory
            var mockConnectionFactory = new MockLdapConnectionFactoryAdapter(mockConnection);

            //Connection information
            var connectionInfo = CreateValidConnectionInfo(true);

            var authenticator = new Authenticator(connectionInfo, mockConnectionFactory);

            var credential = new LDAPDistinguishedNameCredential(mockEntry.DistinguishedName, "123456");

            //Execute authentication
            var result = await authenticator.AuthenticateAsync(credential, "TestAuth");

            //Assert results
            Assert.False(result.IsAuthenticated);
            Assert.True(result.IsSuccessfulOperation);
            Assert.True(string.IsNullOrEmpty(result.ErrorType));
        }

        [Fact]
        public async Task AuthenticateDN_WithVerification_ReturnsFailed() {
            //Search limits
            var searchLimits = CreateValidSearchLimits();

            //Mock LDAP entry for the user
            var mockEntry = CreateMockUserEntry("Victor", "Bastidas", searchLimits, out var dummy, out var expectedFilter);

            //Mock connection
            var mockConnection = new MockLdapConnectionAdapter();
            //Mock the search for the user
            mockConnection.AddSearchResult(expectedFilter.ToString(), new List<MockLdapEntryAdapter> { mockEntry });

            //Mock connection factory
            var mockConnectionFactory = new MockLdapConnectionFactoryAdapter(mockConnection);

            //Connection information
            var connectionInfo = CreateValidConnectionInfo(true);

            var authenticator = new Authenticator(connectionInfo, mockConnectionFactory);

            var credential = new LDAPDistinguishedNameCredential("CN=Unknown User,DC=domain,DC=com", "p@55w0rd");
            var credentialForSearching = new LDAPDomainAccountCredential("domain", "admin", "p@55w0rd");

            //Execute authentication
            var result = await authenticator.AuthenticateAsync(credential, searchLimits, credentialForSearching, "TestAuth");

            //Assert results
            Assert.False(result.IsAuthenticated);
            Assert.True(result.IsSuccessfulOperation);
            Assert.True(string.IsNullOrEmpty(result.ErrorType));
        }
    }
}