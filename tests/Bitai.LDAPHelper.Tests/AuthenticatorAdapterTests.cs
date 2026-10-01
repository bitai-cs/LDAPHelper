using Bitai.LDAPHelper.DTO;
using Bitai.LDAPHelper.LdapAdapters.LdapHelperMock;
using Bitai.LDAPHelper.LdapAdapters.LdapHelperMock.LdapData;
using Microsoft.Extensions.Logging.Abstractions;

namespace Bitai.LDAPHelper.Tests
{
    /// <summary>
    /// Integration-style unit tests for <see cref="Authenticator"/> using mock LDAP adapters.
    /// </summary>
    public class AuthenticatorAdapterTests : BaseTests
    {
        [Fact]
        public async Task AuthenticateUser_ReturnsSuccess() {
            //Mock connection factory
            var mockConnectionFactory = LdapMockFixture.Factory;

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

            //Mock connection factory
            var mockConnectionFactory = LdapMockFixture.Factory;

            //Connection information
            var connectionInfo = CreateValidConnectionInfo(true);

            var authenticator = new Authenticator(connectionInfo, mockConnectionFactory);

            // Use data from the seeder - james.dockers user
            var userSearchFilter = CreateSearchFilter("sAMAccountName", "james.dockers");

            var credential = new LDAPDomainAccountCredential("domain", "james.dockers", "p@55w0rd");
            var credentialForSearching = new LDAPDomainAccountCredential("domain", "admin", "p@55w0rd");

            //Execute authentication
            var result = await authenticator.AuthenticateAsync(credential, searchLimits, credentialForSearching, "TestAuth");

            //Assert results
            Assert.True(result.IsAuthenticated);
            Assert.True(result.IsSuccessfulOperation);
        }

        [Fact]
        public async Task AuthenticateDN_ReturnsSuccess() {
            //Mock connection factory
            var mockConnectionFactory = LdapMockFixture.Factory;

            //Connection information
            var connectionInfo = CreateValidConnectionInfo(true);

            var authenticator = new Authenticator(connectionInfo, mockConnectionFactory);

            // Use data from the seeder - james.dockers user
            var userDistinguishedName = "CN=James Dockers,OU=Seniors,OU=DevOps,OU=IT,DC=va,DC=bitai,DC=com";

            var credential = new LDAPDistinguishedNameCredential(userDistinguishedName, "p@55w0rd");

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

            //Mock connection factory
            var mockConnectionFactory = LdapMockFixture.Factory;

            //Connection information
            var connectionInfo = CreateValidConnectionInfo(true);

            var authenticator = new Authenticator(connectionInfo, mockConnectionFactory);

            // Use data from the seeder - james.dockers user
            var userDistinguishedName = "CN=James Dockers,OU=Seniors,OU=DevOps,OU=IT,DC=va,DC=bitai,DC=com";

            var credential = new LDAPDistinguishedNameCredential(userDistinguishedName, "p@55w0rd");
            var credentialForSearching = new LDAPDomainAccountCredential("domain", "admin", "p@55w0rd");

            //Execute authentication
            var result = await authenticator.AuthenticateAsync(credential, searchLimits, credentialForSearching, "TestAuth");

            //Assert results
            Assert.True(result.IsAuthenticated);
            Assert.True(result.IsSuccessfulOperation);
        }

        [Fact]
        public async Task AuthenticateUser_ReturnsFailed() {
            //Mock connection factory
            var mockConnectionFactory = LdapMockFixture.Factory;

            //Connection information
            var connectionInfo = CreateValidConnectionInfo(true);

            var authenticator = new Authenticator(connectionInfo, mockConnectionFactory);

            var credential = new LDAPDomainAccountCredential("domain", "dummy", "wrongpassword");

            //Execute authentication
            var result = await authenticator.AuthenticateAsync(credential, "TestAuth");

            //Assert results
            Assert.False(result.IsAuthenticated);
            Assert.True(result.IsSuccessfulOperation);
            Assert.True(string.IsNullOrEmpty(result.ErrorType));
        }

        [Fact]
        public async Task AuthenticateUser_WithVerification_ReturnsFailed() {
            //Search limits
            var searchLimits = CreateValidSearchLimits();

            //Mock connection factory
            var mockConnectionFactory = LdapMockFixture.Factory;

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
            //Mock connection factory
            var mockConnectionFactory = LdapMockFixture.Factory;

            //Connection information
            var connectionInfo = CreateValidConnectionInfo(true);

            var authenticator = new Authenticator(connectionInfo, mockConnectionFactory);

            // Use data from the seeder - james.dockers user
            var userDistinguishedName = "CN=James Dockers,OU=Seniors,OU=DevOps,OU=IT,DC=va,DC=bitai,DC=com";

            var credential = new LDAPDistinguishedNameCredential(userDistinguishedName, "wrongpassword");

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

            //Mock connection factory
            var mockConnectionFactory = LdapMockFixture.Factory;

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
