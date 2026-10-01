using Bitai.LDAPHelper.DTO;
using Bitai.LDAPHelper.LdapAdapters.LdapHelperMock;
using Bitai.LDAPHelper.LdapAdapters.LdapHelperMock.LdapData;
using Microsoft.Extensions.Logging.Abstractions;

namespace Bitai.LDAPHelper.Tests
{
    /// <summary>
    /// Integration-style unit tests for <see cref="AccountManager"/> using mock LDAP adapters.
    /// </summary>
    public class AccountManagerAdapterTests: BaseTests
    {
        [Fact]
        public async Task CreateUserAccountForMsAD_ReturnsSuccess() {
            var mockConnectionFactory = LdapMockFixture.Factory;

            var connectionInfo = CreateValidConnectionInfo(ssl: true);

            var searchLimits = CreateValidSearchLimits();

            var credential = new LDAPDomainAccountCredential("domain", "admin", "p@55w0rd");

            var accountManager = new AccountManager(connectionInfo, searchLimits, credential, mockConnectionFactory);

            var newUser = new LDAPMsADUserAccount {
                DistinguishedNameOfContainer = $"CN=Software Developers;OU=IT,{searchLimits.BaseDN}",
                DistinguishedName = $"CN=John Doe,CN=Software Developers;OU=IT,{searchLimits.BaseDN}",
                Cn = "John Doe",
                DisplayName = "John Doe (Fullstack)",
                SAMAccountName = "john.doe",
                GivenName = "John",
                Sn = "Doe",
                UserPrincipalName = "jdoe@domain",
                ObjectClass = new[] { "top", "person", "organizationalPerson", "user" },
                Password = "P@ssw0rd"
            };

            var result = await accountManager.CreateUserAccountForMsAD(newUser, "TestCreate");

            // Assert
            Assert.True(result.IsSuccessfulOperation);
            Assert.Contains("ms ad user account created at", result.OperationMessage.ToLower());
        }

        [Fact]
        public async Task CreateUserAccountForMsAD_MissingRequiredAttr_ReturnsError() {
            var mockConnectionFactory = LdapMockFixture.Factory;

            var connectionInfo = CreateValidConnectionInfo(ssl: true);

            var searchLimits = CreateValidSearchLimits();

            var credential = new LDAPDomainAccountCredential("domain", "admin", "p@55w0rd");

            var accountManager = new AccountManager(connectionInfo, searchLimits, credential, mockConnectionFactory);

            var newUser = new LDAPMsADUserAccount {
                DistinguishedNameOfContainer = $"CN=Software Developers;OU=IT,{searchLimits.BaseDN}",
                Cn = "John Doe",
                DisplayName = "John Doe (Fullstack)",
                //SAMAccountName = "john.doe", /* Without this ttr the process will throw an error*/
                GivenName = "John",
                Sn = "Doe",
                UserPrincipalName = "jdoe@domain",
                ObjectClass = new[] { "top", "person", "organizationalPerson", "user" },
                Password = "P@ssw0rd"
            };

            var result = await accountManager.CreateUserAccountForMsAD(newUser, "TestCreate");

            Assert.False(result.IsSuccessfulOperation);
            Assert.StartsWith("unable to create", result.OperationMessage, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task SetUserAccountPasswordForMsAD_ValidAccount_ReturnsSuccess() {
            // Arrange
            var mockConnectionFactory = LdapMockFixture.Factory;

            var connectionInfo = CreateValidConnectionInfo(ssl: true);

            var searchLimits = CreateValidSearchLimits();

            var credential = new LDAPDomainAccountCredential("domain", "admin", "p@55w0rd");

            var accountManager = new AccountManager(connectionInfo, searchLimits, credential, mockConnectionFactory);

            // Disposable user, so the seeded data shared by all tests is never modified
            var userDistinguishedName = CreateDisposableUser("setpassword");

            var result = await accountManager.SetMsADUserAccountPassword(EntryAttribute.distinguishedName, userDistinguishedName, "TestPassword", postUpdateTestAuthentication: true);

            // Assert
            Assert.True(result.IsSuccessfulOperation);
            Assert.Contains("password set successfully", result.OperationMessage.ToLower());
        }

        [Fact]
        public async Task SetUserAccountPasswordForMsAD_AccountNotFound_ReturnsFailed() {
            var mockConnectionFactory = LdapMockFixture.Factory;

            var connectionInfo = CreateValidConnectionInfo(ssl: true);

            var searchLimits = CreateValidSearchLimits();

            var credential = new LDAPDomainAccountCredential("domain", "admin", "p@55w0rd");

            var accountManager = new AccountManager(connectionInfo, searchLimits, credential, mockConnectionFactory);

            // Use a DN that doesn't exist in the seeder
            var nonExistentUserDN = "CN=Non Existent User,OU=IT,DC=va,DC=bitai,DC=com";

            var result = await accountManager.SetMsADUserAccountPassword(EntryAttribute.distinguishedName, nonExistentUserDN, "TestPassword", postUpdateTestAuthentication: true);

            Assert.False(result.IsSuccessfulOperation);
            Assert.StartsWith("user account not found", result.OperationMessage, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task DisableUserAccountForMsAD_ValidAccount_ReturnsSuccess() {
            var mockConnectionFactory = LdapMockFixture.Factory;

            var connectionInfo = CreateValidConnectionInfo(ssl: true);

            var searchLimits = CreateValidSearchLimits();

            var credential = new LDAPDomainAccountCredential("test", "admin", "password");

            var accountManager = new AccountManager(connectionInfo, searchLimits, credential, mockConnectionFactory);

            // Disposable user, so the seeded data shared by all tests is never modified
            var userDistinguishedName = CreateDisposableUser("disable");

            var result = await accountManager.DisableMsADUserAccount(EntryAttribute.distinguishedName, userDistinguishedName, "TestDisable");

            // Assert
            Assert.True(result.IsSuccessfulOperation);
            Assert.Contains("has been disabled", result.OperationMessage.ToLower());
        }

        [Fact]
        public async Task DisableUserAccountForMsAD_AccountNotFound_ReturnsSuccess() {
            var mockConnectionFactory = LdapMockFixture.Factory;

            var connectionInfo = CreateValidConnectionInfo(ssl: true);

            var searchLimits = CreateValidSearchLimits();

            var credential = new LDAPDomainAccountCredential("test", "admin", "password");

            var accountManager = new AccountManager(connectionInfo, searchLimits, credential, mockConnectionFactory);

            // Use a DN that doesn't exist in the seeder
            var nonExistentUserDN = "CN=Non Existent User,OU=IT,DC=va,DC=bitai,DC=com";

            var result = await accountManager.DisableMsADUserAccount(EntryAttribute.distinguishedName, nonExistentUserDN, "TestDisable");

            // Assert
            Assert.False(result.IsSuccessfulOperation);
            Assert.StartsWith("user account not found", result.OperationMessage, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task RemoveUserAccountForMsAD_ValidAccount_ReturnsSuccess() {
            var mockConnectionFactory = LdapMockFixture.Factory;

            var connectionInfo = CreateValidConnectionInfo(ssl: true);

            var searchLimits = CreateValidSearchLimits();

            var credential = new LDAPDomainAccountCredential("domain", "admin", "p@55w0rd");

            // Disposable user, so the seeded data shared by all tests is never modified
            var userDistinguishedName = CreateDisposableUser("remove");

            var accountManager = new AccountManager(connectionInfo, searchLimits, credential, mockConnectionFactory);

            var result = await accountManager.RemoveMsADUserAccount(EntryAttribute.distinguishedName, userDistinguishedName, "TestDelete");

            Assert.True(result.IsSuccessfulOperation);
            Assert.Contains("successfully removed", result.OperationMessage.ToLower());
        }

        [Fact]
        public async Task RemoveUserAccountForMsAD_AccountNotFound_ReturnsSuccess() {
            var mockConnectionFactory = LdapMockFixture.Factory;

            var connectionInfo = CreateValidConnectionInfo(ssl: true);

            var searchLimits = CreateValidSearchLimits();

            var credential = new LDAPDomainAccountCredential("domain", "admin", "p@55w0rd");

            // Use a DN that doesn't exist in the seeder
            var nonExistentUserDN = "CN=Non Existent User,OU=IT,DC=va,DC=bitai,DC=com";

            var accountManager = new AccountManager(connectionInfo, searchLimits, credential, mockConnectionFactory);

            var result = await accountManager.RemoveMsADUserAccount(EntryAttribute.distinguishedName, nonExistentUserDN, "TestDelete");

            Assert.False(result.IsSuccessfulOperation);
            Assert.Contains("user account not found", result.OperationMessage, StringComparison.OrdinalIgnoreCase);
        }
    }
}
