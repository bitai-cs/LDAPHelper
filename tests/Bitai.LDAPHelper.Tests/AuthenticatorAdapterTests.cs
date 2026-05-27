using Bitai.LDAPHelper.DTO;
using Bitai.LDAPHelper.Tests.Mocks.LdapAdapters;

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
            var userMockEntry = CreateMockUserEntry("Victor", "Bastidas", searchLimits, out var userSearchFilter, out var _);

            //Mock connection
            var mockConnection = new MockLdapConnectionAdapter();
            //Mock the search for the user
            mockConnection.AddSearchResult(userSearchFilter.ToString(), new List<MockLdapEntryAdapter> { userMockEntry });

            //Mock connection factory
            var mockConnectionFactory = new MockLdapConnectionFactoryAdapter(mockConnection);

            //Connection information
            var connectionInfo = CreateValidConnectionInfo(true);

            var authenticator = new Authenticator(connectionInfo, mockConnectionFactory);

            var credential = new LDAPDomainAccountCredential("domain", userSearchFilter.FilterValue.Value, "p@55w0rd");
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
            var userMockEntry = CreateMockUserEntry("Victor", "Bastidas", null, out var _, out var _);

            //Mock connection
            var mockConnection = new MockLdapConnectionAdapter();

            //Mock connection factory
            var mockConnectionFactory = new MockLdapConnectionFactoryAdapter(mockConnection);

            //Connection information
            var connectionInfo = CreateValidConnectionInfo(true);

            var authenticator = new Authenticator(connectionInfo, mockConnectionFactory);

            var credential = new LDAPDistinguishedNameCredential(userMockEntry.DistinguishedName, "p@55w0rd");

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
            var userMockEntry = CreateMockUserEntry("Victor", "Bastidas", searchLimits, out var _, out var userSearchFilter);

            //Mock connection
            var mockConnection = new MockLdapConnectionAdapter();
            //Mock the search for the user
            mockConnection.AddSearchResult(userSearchFilter.ToString(), new List<MockLdapEntryAdapter> { userMockEntry });

            //Mock connection factory
            var mockConnectionFactory = new MockLdapConnectionFactoryAdapter(mockConnection);

            //Connection information
            var connectionInfo = CreateValidConnectionInfo(true);

            var authenticator = new Authenticator(connectionInfo, mockConnectionFactory);

            var credential = new LDAPDistinguishedNameCredential(userMockEntry.DistinguishedName, "p@55w0rd");
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
            var userMockEntry = CreateMockUserEntry("Victor", "Bastidas", searchLimits, out var userSearchFilter, out var _);

            //Mock connection
            var mockConnection = new MockLdapConnectionAdapter();
            //Mock the search for the user
            mockConnection.AddSearchResult(userSearchFilter.ToString(), new List<MockLdapEntryAdapter> { userMockEntry });

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
            var userMockEntry = CreateMockUserEntry("Victor", "Bastidas", null, out var _, out var userSearchFilter);

            //Mock connection
            var mockConnection = new MockLdapConnectionAdapter();

            //Mock connection factory
            var mockConnectionFactory = new MockLdapConnectionFactoryAdapter(mockConnection);

            //Connection information
            var connectionInfo = CreateValidConnectionInfo(true);

            var authenticator = new Authenticator(connectionInfo, mockConnectionFactory);

            var credential = new LDAPDistinguishedNameCredential(userMockEntry.DistinguishedName, "123456");

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
            var userMockEntry = CreateMockUserEntry("Victor", "Bastidas", searchLimits, out var _, out var userSearchFilter);

            //Mock connection
            var mockConnection = new MockLdapConnectionAdapter();
            //Mock the search for the user
            mockConnection.AddSearchResult(userSearchFilter.ToString(), new List<MockLdapEntryAdapter> { userMockEntry });

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