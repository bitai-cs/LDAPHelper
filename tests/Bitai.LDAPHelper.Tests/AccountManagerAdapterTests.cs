using Bitai.LDAPHelper.DTO;
using Bitai.LDAPHelper.Tests.Mocks.LdapAdapters;

namespace Bitai.LDAPHelper.Tests
{
    /// <summary>
    /// Integration-style unit tests for <see cref="AccountManager"/> using mock LDAP adapters.
    /// </summary>
    public class AccountManagerAdapterTests: BaseTests
    {
        [Fact]
        public async Task CreateUserAccountForMsAD_ReturnsSuccess() {
            var mockConnection = new MockLdapConnectionAdapter();

            var mockConnectionFactory = new MockLdapConnectionFactoryAdapter(mockConnection);

            var connectionInfo = CreateValidConnectionInfo(ssl: true);

            var searchLimits = CreateValidSearchLimits();

            var credential = new LDAPDomainAccountCredential("domain", "admin", "p@55w0rd");

            var accountManager = new AccountManager(connectionInfo, searchLimits, credential, mockConnectionFactory);

            var newUser = new LDAPMsADUserAccount {
                DistinguishedNameOfContainer = $"CN=Software Developers;OU=IT,{searchLimits.BaseDN}",
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
            var mockConnection = new MockLdapConnectionAdapter();

            var mockConnectionFactory = new MockLdapConnectionFactoryAdapter(mockConnection);

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
            var mockConnection = new MockLdapConnectionAdapter();
            
            var mockConnectionFactory = new MockLdapConnectionFactoryAdapter(mockConnection);

            var connectionInfo = CreateValidConnectionInfo(ssl: true);

            var searchLimits = CreateValidSearchLimits();

            var credential = new LDAPDomainAccountCredential("domain", "admin", "p@55w0rd");

            var accountManager = new AccountManager(connectionInfo, searchLimits, credential, mockConnectionFactory);

            var groupName = "Accountants";
            var groupContainerName = "Finance";
            var mockGroupEntry1 = CreateMockGroupEntry(groupName, groupContainerName, searchLimits, out var groupSearchFilter1);
            var mockGroupEntry2 = CreateMockGroupEntry("Auditors", "Trusted", searchLimits, out var groupSearchFilter2);

            var mockUserEntry = CreateMockUserEntry("John", "Doe", searchLimits, out var _, out var userSearchFilter, new string[] { mockGroupEntry2.DistinguishedName }, groupName, groupContainerName);

            mockConnection.AddSearchResult(groupSearchFilter1.ToString(), new List<MockLdapEntryAdapter> { mockGroupEntry1 });
            mockConnection.AddSearchResult(groupSearchFilter2.ToString(), new List<MockLdapEntryAdapter> { mockGroupEntry2 });
            mockConnection.AddSearchResult(userSearchFilter.ToString(), new List<MockLdapEntryAdapter> { mockUserEntry });

            var result = await accountManager.SetMsADUserAccountPassword(EntryAttribute.distinguishedName, mockUserEntry.DistinguishedName, "TestPassword", postUpdateTestAuthentication: true);

            // Assert
            Assert.True(result.IsSuccessfulOperation);
            Assert.Contains("password set successfully", result.OperationMessage.ToLower());
        }

        [Fact]
        public async Task SetUserAccountPasswordForMsAD_AccountNotFound_ReturnsFailed() {
            var mockConnection = new MockLdapConnectionAdapter();

            var mockConnectionFactory = new MockLdapConnectionFactoryAdapter(mockConnection);

            var connectionInfo = CreateValidConnectionInfo(ssl: true);

            var searchLimits = CreateValidSearchLimits();

            var credential = new LDAPDomainAccountCredential("domain", "admin", "p@55w0rd");

            var accountManager = new AccountManager(connectionInfo, searchLimits, credential, mockConnectionFactory);

            var groupName = "Accountants";
            var groupContainerName = "Finance";
            var mockGroupEntry1 = CreateMockGroupEntry(groupName, groupContainerName, searchLimits, out var groupSearchFilter1);
            var mockGroupEntry2 = CreateMockGroupEntry("Auditors", "Trusted", searchLimits, out var groupSearchFilter2);

            var mockUserEntry = CreateMockUserEntry("John", "Doe", searchLimits, out var _, out var userSearchFilter, new string[] { mockGroupEntry2.DistinguishedName }, groupName, groupContainerName);

            mockConnection.AddSearchResult(groupSearchFilter1.ToString(), new List<MockLdapEntryAdapter> { mockGroupEntry1 });
            mockConnection.AddSearchResult(groupSearchFilter2.ToString(), new List<MockLdapEntryAdapter> { mockGroupEntry2 });
            //Do not add to trigger account verification error!
            //mockConnection.AddSearchResult(userSearchFilter.ToString(), new List<MockLdapEntryAdapter> { mockUserEntry });

            var userCredential = new LDAPDistinguishedNameCredential(mockUserEntry.DistinguishedName, "NewP@ssw0rd");

            var result = await accountManager.SetMsADUserAccountPassword(EntryAttribute.distinguishedName, mockUserEntry.DistinguishedName, "TestPassword", postUpdateTestAuthentication: true);

            Assert.False(result.IsSuccessfulOperation);
            Assert.StartsWith("user account not found", result.OperationMessage, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task DisableUserAccountForMsAD_ValidAccount_ReturnsSuccess() {
            var mockConnection = new MockLdapConnectionAdapter();

            var mockConnectionFactory = new MockLdapConnectionFactoryAdapter(mockConnection);

            var connectionInfo = CreateValidConnectionInfo(ssl: true);

            var searchLimits = CreateValidSearchLimits();

            var credential = new LDAPDomainAccountCredential("test", "admin", "password");

            var accountManager = new AccountManager(connectionInfo, searchLimits, credential, mockConnectionFactory);

            var groupName = "Interviewers";
            var groupContainerName = "Human Resources";
            var mockGroupEntry1 = CreateMockGroupEntry(groupName, groupContainerName, searchLimits, out var groupSearchFilter1);
            var mockGroupEntry2 = CreateMockGroupEntry("Auditors", "Trusted", searchLimits, out var groupSearchFilter2);
            var mockUserEntry = CreateMockUserEntry("Francis", "Peralta", searchLimits, out var _, out var userSearchFilter, new string[] { mockGroupEntry2.DistinguishedName }, groupName, groupContainerName);

            mockConnection.AddSearchResult(groupSearchFilter1.ToString(), new List<MockLdapEntryAdapter> { mockGroupEntry1 });
            mockConnection.AddSearchResult(groupSearchFilter2.ToString(), new List<MockLdapEntryAdapter> { mockGroupEntry2 });
            mockConnection.AddSearchResult(userSearchFilter.ToString(), new List<MockLdapEntryAdapter> { mockUserEntry });

            var result = await accountManager.DisableMsADUserAccount(EntryAttribute.distinguishedName, mockUserEntry.DistinguishedName, "TestDisable");

            // Assert
            Assert.True(result.IsSuccessfulOperation);          
            Assert.Contains("has been disabled", result.OperationMessage.ToLower());
        }

        [Fact]
        public async Task DisableUserAccountForMsAD_AccountNotFound_ReturnsSuccess() {
            var mockConnection = new MockLdapConnectionAdapter();

            var mockConnectionFactory = new MockLdapConnectionFactoryAdapter(mockConnection);

            var connectionInfo = CreateValidConnectionInfo(ssl: true);

            var searchLimits = CreateValidSearchLimits();

            var credential = new LDAPDomainAccountCredential("test", "admin", "password");

            var accountManager = new AccountManager(connectionInfo, searchLimits, credential, mockConnectionFactory);

            var groupName = "Interviewers";
            var groupContainerName = "Human Resources";
            var mockGroupEntry1 = CreateMockGroupEntry(groupName, groupContainerName, searchLimits, out var groupSearchFilter1);
            var mockGroupEntry2 = CreateMockGroupEntry("Auditors", "Trusted", searchLimits, out var groupSearchFilter2);
            var mockUserEntry = CreateMockUserEntry("Francis", "Peralta", searchLimits, out var _, out var userSearchFilter, new string[] { mockGroupEntry2.DistinguishedName }, groupName, groupContainerName);

            mockConnection.AddSearchResult(groupSearchFilter1.ToString(), new List<MockLdapEntryAdapter> { mockGroupEntry1 });
            mockConnection.AddSearchResult(groupSearchFilter2.ToString(), new List<MockLdapEntryAdapter> { mockGroupEntry2 });
            //Do not add user account in order to trigger user not found validation.
            //mockConnection.AddSearchResult(userSearchFilter.ToString(), new List<MockLdapEntryAdapter> { mockUserEntry });

            var result = await accountManager.DisableMsADUserAccount(EntryAttribute.distinguishedName, mockUserEntry.DistinguishedName, "TestDisable");

            // Assert
            Assert.False(result.IsSuccessfulOperation);
            Assert.StartsWith("user account not found", result.OperationMessage, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task RemoveUserAccountForMsAD_ValidAccount_ReturnsSuccess() {
            var mockConnection = new MockLdapConnectionAdapter();

            var mockConnectionFactory = new MockLdapConnectionFactoryAdapter(mockConnection);

            var connectionInfo = CreateValidConnectionInfo(ssl: true);

            var searchLimits = CreateValidSearchLimits();

            var credential = new LDAPDomainAccountCredential("domain", "admin", "p@55w0rd");

            var groupName = "Accountants";
            var groupContainerName = "Finance";
            var mockGroupEntry1 = CreateMockGroupEntry(groupName, groupContainerName, searchLimits, out var groupSearchFilter1);
            var mockGroupEntry2 = CreateMockGroupEntry("Auditors", "Trusted", searchLimits, out var groupSearchFilter2);
            var mockUserEntry = CreateMockUserEntry("John", "Doe", searchLimits, out var _, out var userSearchFilter, new string[] { mockGroupEntry2.DistinguishedName }, groupName, groupContainerName);

            mockConnection.AddSearchResult(groupSearchFilter1.ToString(), new List<MockLdapEntryAdapter> { mockGroupEntry1 });
            mockConnection.AddSearchResult(groupSearchFilter2.ToString(), new List<MockLdapEntryAdapter> { mockGroupEntry2 });
            mockConnection.AddSearchResult(userSearchFilter.ToString(), new List<MockLdapEntryAdapter> { mockUserEntry });

            var accountManager = new AccountManager(connectionInfo, searchLimits, credential, mockConnectionFactory);
            
            var result = await accountManager.RemoveMsADUserAccount(EntryAttribute.distinguishedName, mockUserEntry.DistinguishedName, "TestDelete");

            Assert.True(result.IsSuccessfulOperation);
            Assert.Contains("successfully removed", result.OperationMessage.ToLower());
        }

        [Fact]
        public async Task RemoveUserAccountForMsAD_AccountNotFound_ReturnsSuccess() {
            var mockConnection = new MockLdapConnectionAdapter();

            var mockConnectionFactory = new MockLdapConnectionFactoryAdapter(mockConnection);

            var connectionInfo = CreateValidConnectionInfo(ssl: true);

            var searchLimits = CreateValidSearchLimits();

            var credential = new LDAPDomainAccountCredential("domain", "admin", "p@55w0rd");

            var groupName = "Accountants";
            var groupContainerName = "Finance";
            var mockGroupEntry1 = CreateMockGroupEntry(groupName, groupContainerName, searchLimits, out var groupSearchFilter1);
            var mockGroupEntry2 = CreateMockGroupEntry("Auditors", "Trusted", searchLimits, out var groupSearchFilter2);
            var mockUserEntry = CreateMockUserEntry("John", "Doe", searchLimits, out var _, out var userSearchFilter, new string[] { mockGroupEntry2.DistinguishedName }, groupName, groupContainerName);

            mockConnection.AddSearchResult(groupSearchFilter1.ToString(), new List<MockLdapEntryAdapter> { mockGroupEntry1 });
            mockConnection.AddSearchResult(groupSearchFilter2.ToString(), new List<MockLdapEntryAdapter> { mockGroupEntry2 });
            //Do not add user entry to trigger account not found validation.
            //mockConnection.AddSearchResult(userSearchFilter.ToString(), new List<MockLdapEntryAdapter> { mockUserEntry });

            var accountManager = new AccountManager(connectionInfo, searchLimits, credential, mockConnectionFactory);

            var result = await accountManager.RemoveMsADUserAccount(EntryAttribute.distinguishedName, mockUserEntry.DistinguishedName, "TestDelete");

            Assert.False(result.IsSuccessfulOperation);
            Assert.Contains("user account not found", result.OperationMessage, StringComparison.OrdinalIgnoreCase);
        }
    }
}
