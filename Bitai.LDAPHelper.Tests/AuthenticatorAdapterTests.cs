// AuthenticatorAdapterTests.cs
using System.Threading.Tasks;
using Bitai.LDAPHelper.DTO;
using Bitai.LDAPHelper.Tests.Mocks;
using Xunit;

namespace Bitai.LDAPHelper.Tests
{
    public class AuthenticatorAdapterTests
    {
        [Fact]
        public async Task AuthenticateAsync_WithValidCredentials_ReturnsSuccess() {
            // Arrange
            var mockConnection = new MockLdapConnectionAdapter();
            var mockEntry = new MockLdapEntryAdapter("CN=Test User,DC=domain,DC=com");
            mockEntry.AddAttribute("objectSid", new byte[] { 1, 2, 3, 4, 5 });
            mockEntry.AddAttribute("sAMAccountName", "testuser");

            // Mock the search for the user
            mockConnection.AddSearchResult("(sAMAccountName=testuser)", new List<MockLdapEntryAdapter> { mockEntry });

            var mockFactory = new MockLdapConnectionFactoryAdapter(mockConnection);

            var connectionInfo = new ConnectionInfo(
                server: "localhost",
                port: 389,
                useSSL: false,
                connectionTimeout: 30);

            var authenticator = new Authenticator(connectionInfo, mockFactory);
            var credential = new LDAPDomainAccountCredential("domain", "testuser", "correctpassword");

            // Act
            var result = await authenticator.AuthenticateAsync(credential, "TestAuth");

            // Assert
            Assert.True(result.IsAuthenticated);
            Assert.True(result.IsSuccessfulOperation);
        }

        [Fact]
        public async Task AuthenticateAsync_WithInvalidCredentials_ReturnsFalse() {
            // Arrange
            var mockConnection = new MockLdapConnectionAdapter();

            // Mock the search for the user (will be found but password fails)
            var mockEntry = new MockLdapEntryAdapter("CN=Test User,DC=domain,DC=com");
            mockEntry.AddAttribute("objectSid", new byte[] { 1, 2, 3, 4, 5 });
            mockEntry.AddAttribute("sAMAccountName", "testuser");
            mockConnection.AddSearchResult("(sAMAccountName=testuser)", new List<MockLdapEntryAdapter> { mockEntry });

            var mockFactory = new MockLdapConnectionFactoryAdapter(mockConnection);

            var connectionInfo = new ConnectionInfo(
                server: "localhost",
                port: 389,
                useSSL: false,
                connectionTimeout: 30);

            var authenticator = new Authenticator(connectionInfo, mockFactory);
            var credential = new LDAPDomainAccountCredential("domain", "testuser", "wrongpassword");

            // Act
            var result = await authenticator.AuthenticateAsync(credential, "TestAuth");

            // Assert
            Assert.False(result.IsAuthenticated);
            Assert.True(result.IsSuccessfulOperation);
            Assert.Contains("password", result.OperationMessage, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task AuthenticateAsync_WithUserNotFound_ReturnsFalseWithAppropriateMessage() {
            // Arrange
            var mockConnection = new MockLdapConnectionAdapter();

            // No entries added - user won't be found
            var mockFactory = new MockLdapConnectionFactoryAdapter(mockConnection);

            var connectionInfo = new ConnectionInfo(
                server: "localhost",
                port: 389,
                useSSL: false,
                connectionTimeout: 30);

            var authenticator = new Authenticator(connectionInfo, mockFactory);
            var credential = new LDAPDomainAccountCredential("domain", "nonexistent", "password");

            // Act
            var result = await authenticator.AuthenticateAsync(credential, "TestAuth");

            // Assert
            Assert.False(result.IsAuthenticated);
            Assert.True(result.IsSuccessfulOperation);
            Assert.Contains("could not be found", result.OperationMessage);
        }
    }
}