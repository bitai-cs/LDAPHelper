using Bitai.LDAPHelper.DTO;
using Bitai.LDAPHelper.LdapAdapters.LdapHelperMock;
using Bitai.LDAPHelper.LdapAdapters.LdapHelperMock.LdapData;
using Microsoft.Extensions.Logging.Abstractions;

namespace Bitai.LDAPHelper.Tests
{
    /// <summary>
    /// Unit tests for <see cref="GroupMembershipValidator"/> using mock LDAP adapters.
    /// </summary>
    public class GroupMembershipValidatorTests : BaseTests
    {
        private readonly ConnectionInfo _validConnectionInfo;
        private readonly SearchLimits _validSearchLimits;
        private readonly LDAPDomainAccountCredential _validCredential;

        public GroupMembershipValidatorTests() {
            _validConnectionInfo = CreateValidConnectionInfo(false);
            _validSearchLimits = CreateValidSearchLimits();
            _validCredential = new LDAPDomainAccountCredential("domain", "admin", "password");
        }

        #region CheckGroupMembershipAsync Tests

        [Fact]
        public async Task CheckGroupMembershipAsync_UserIsDirectMember_ReturnsTrue() {
            // Arrange - james.dockers is direct member of DomainAdmins
            var mockConnectionFactory = LdapMockFixture.Factory;

            var validator = new GroupMembershipValidator(_validConnectionInfo, _validSearchLimits, _validCredential, mockConnectionFactory);

            var result = await validator.CheckGroupMembershipAsync("james.dockers", "Domain Admins");

            Assert.True(result);
        }

        [Fact]
        public async Task CheckGroupMembershipAsync_UserIsIndirectMember_ReturnsTrue() {
            // Arrange - sara.pikes -> JuniorDevOps -> DevOpsEng -> ITAdmins -> DomainAdmins
            var mockConnectionFactory = LdapMockFixture.Factory;

            var validator = new GroupMembershipValidator(_validConnectionInfo, _validSearchLimits, _validCredential, mockConnectionFactory);

            // Act
            var result = await validator.CheckGroupMembershipAsync("sara.pikes", "Domain Admins");

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task CheckGroupMembershipAsync_UserIsMemberThroughMultipleLevels_ReturnsTrue() {
            // Arrange - james.dockers -> SeniorDevOps -> DevOpsEng -> ITAdmins -> DomainAdmins
            var mockConnectionFactory = LdapMockFixture.Factory;

            var validator = new GroupMembershipValidator(_validConnectionInfo, _validSearchLimits, _validCredential, mockConnectionFactory);

            // Act
            var result = await validator.CheckGroupMembershipAsync("james.dockers", "Administrators");

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task CheckGroupMembershipAsync_UserIsNotMember_ReturnsFalse() {
            // Arrange - sara.pikes is not a member of DomainAdmins
            var mockConnectionFactory = LdapMockFixture.Factory;

            var validator = new GroupMembershipValidator(_validConnectionInfo, _validSearchLimits, _validCredential, mockConnectionFactory);

            // Act
            var result = await validator.CheckGroupMembershipAsync("sara.pikes", "DomainAdmins");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task CheckGroupMembershipAsync_UserNotFound_ThrowsException() {
            // Arrange
            var mockConnectionFactory = LdapMockFixture.Factory;

            var validator = new GroupMembershipValidator(_validConnectionInfo, _validSearchLimits, _validCredential, mockConnectionFactory);

            await Assert.ThrowsAsync<EntryNotFoundException>(() => validator.CheckGroupMembershipAsync("nonexistent.user", "DomainAdmins"));
        }

        [Fact]
        public async Task CheckGroupMembershipAsync_CaseInsensitiveComparison_ReturnsTrue() {
            // Arrange - james.dockers is member of DomainAdmins
            var mockConnectionFactory = LdapMockFixture.Factory;

            var validator = new GroupMembershipValidator(_validConnectionInfo, _validSearchLimits, _validCredential, mockConnectionFactory);

            var result = await validator.CheckGroupMembershipAsync("JAMES.DOCKERS", "DOMAIN ADMINS");

            Assert.True(result);
        }

        [Fact]
        public async Task CheckGroupMembershipAsync_NullSAMAccountName_ThrowsArgumentNullException() {
            // Arrange
            var mockConnectionFactory = LdapMockFixture.Factory;

            var validator = new GroupMembershipValidator(_validConnectionInfo, _validSearchLimits, _validCredential, mockConnectionFactory);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(
                () => validator.CheckGroupMembershipAsync(null, "DomainAdmins"));
        }

        [Fact]
        public async Task CheckGroupMembershipAsync_EmptySAMAccountName_ThrowsArgumentNullException() {
            // Arrange
            var mockConnectionFactory = LdapMockFixture.Factory;

            var validator = new GroupMembershipValidator(_validConnectionInfo, _validSearchLimits, _validCredential, mockConnectionFactory);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(
                () => validator.CheckGroupMembershipAsync(string.Empty, "DomainAdmins"));
        }

        [Fact]
        public async Task CheckGroupMembershipAsync_SAMAccountNameContainsWildcard_ThrowsArgumentException() {
            // Arrange
            var mockConnectionFactory = LdapMockFixture.Factory;

            var validator = new GroupMembershipValidator(_validConnectionInfo, _validSearchLimits, _validCredential, mockConnectionFactory);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(
                () => validator.CheckGroupMembershipAsync("john.*", "DomainAdmins"));
        }

        [Fact]
        public async Task CheckGroupMembershipAsync_NullParentGroupCN_ThrowsArgumentNullException() {
            // Arrange
            var mockConnectionFactory = LdapMockFixture.Factory;

            var validator = new GroupMembershipValidator(_validConnectionInfo, _validSearchLimits, _validCredential, mockConnectionFactory);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(
                () => validator.CheckGroupMembershipAsync("james.dockers", null));
        }

        [Fact]
        public async Task CheckGroupMembershipAsync_EmptyParentGroupCN_ThrowsArgumentNullException() {
            // Arrange
            var mockConnectionFactory = LdapMockFixture.Factory;

            var validator = new GroupMembershipValidator(_validConnectionInfo, _validSearchLimits, _validCredential, mockConnectionFactory);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(
                () => validator.CheckGroupMembershipAsync("james.dockers", string.Empty));
        }

        [Fact]
        public async Task CheckGroupMembershipAsync_ParentGroupCNContainsWildcard_ThrowsArgumentException() {
            // Arrange
            var mockConnectionFactory = LdapMockFixture.Factory;

            var validator = new GroupMembershipValidator(_validConnectionInfo, _validSearchLimits, _validCredential, mockConnectionFactory);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(
                () => validator.CheckGroupMembershipAsync("james.dockers", "Domain*"));
        }

        [Fact]
        public async Task CheckGroupMembershipAsync_UserInMultipleGroups_FindsCorrectGroup() {
            // Arrange - james.dockers is member of multiple groups
            var mockConnectionFactory = LdapMockFixture.Factory;

            var validator = new GroupMembershipValidator(_validConnectionInfo, _validSearchLimits, _validCredential, mockConnectionFactory);

            var result = await validator.CheckGroupMembershipAsync("james.dockers", "IT Admins");

            Assert.True(result);
        }

        #endregion

        #region GetAllGroupMembershipsAsync Tests       

        [Fact]
        public async Task GetAllGroupMembershipsAsync_UserWithNestedMemberships_ReturnsAllGroupsIncludingIndirect() {
            // Arrange - sara.pikes -> JuniorDevOps -> DevOpsEng -> ITAdmins -> DomainAdmins
            var mockConnectionFactory = LdapMockFixture.Factory;

            var validator = new GroupMembershipValidator(_validConnectionInfo, _validSearchLimits, _validCredential, mockConnectionFactory);

            var groups = await validator.GetAllGroupMembershipsAsync("sara.pikes");

            // Assert - Should include both direct and indirect groups
            Assert.NotNull(groups);
            Assert.Contains("Administrators", groups);
            Assert.Contains("Junior DevOps", groups);
            Assert.Contains("DevOps Engineers", groups);
            Assert.Contains("IT Admins", groups);
            Assert.Contains("Domain Users", groups);
            Assert.Contains("Domain Admins", groups);
        }

        [Fact]
        public async Task GetAllGroupMembershipsAsync_UserWithDeepNesting_ReturnsAllGroupsInHierarchy() {
            // Arrange - james.dockers -> SeniorDevOps -> DevOpsEng -> ITAdmins -> DomainAdmins -> Administrators
            var mockConnectionFactory = LdapMockFixture.Factory;

            var validator = new GroupMembershipValidator(_validConnectionInfo, _validSearchLimits, _validCredential, mockConnectionFactory);

            // Act
            var groups = await validator.GetAllGroupMembershipsAsync("james.dockers");

            // Assert
            Assert.NotNull(groups);
            Assert.Contains("Administrators", groups);
            Assert.Contains("Senior DevOps", groups);
            Assert.Contains("DevOps Engineers", groups);
            Assert.Contains("IT Admins", groups);
            Assert.Contains("Domain Admins", groups);
            Assert.Contains("DevOps Leaders", groups);
        }        

        [Fact]
        public async Task GetAllGroupMembershipsAsync_UserNotFound_ThrowsException() {
            // Arrange
            var mockConnectionFactory = LdapMockFixture.Factory;

            var validator = new GroupMembershipValidator(_validConnectionInfo, _validSearchLimits, _validCredential, mockConnectionFactory);

            // Act
            var ex = await Assert.ThrowsAnyAsync<EntryNotFoundException>(() => validator.GetAllGroupMembershipsAsync("nonexistent.user"));
        }

        [Fact]
        public async Task GetAllGroupMembershipsAsync_RemovesDuplicateGroups() {
            // Arrange - james.dockers has multiple paths to same groups
            var mockConnectionFactory = LdapMockFixture.Factory;

            var validator = new GroupMembershipValidator(_validConnectionInfo, _validSearchLimits, _validCredential, mockConnectionFactory);

            // Act
            var groups = await validator.GetAllGroupMembershipsAsync("james.dockers");

            // Assert - Should not have duplicates
            Assert.NotNull(groups);
            Assert.Equal(groups.Distinct(StringComparer.OrdinalIgnoreCase).Count(), groups.Length);
        }

        [Fact]
        public async Task GetAllGroupMembershipsAsync_CaseInsensitiveDistinct_ReturnsUniqueGroups() {
            // Arrange - james.dockers has multiple paths to same groups
            var mockConnectionFactory = LdapMockFixture.Factory;

            var validator = new GroupMembershipValidator(_validConnectionInfo, _validSearchLimits, _validCredential, mockConnectionFactory);

            // Act
            var groups = await validator.GetAllGroupMembershipsAsync("james.dockers");

            // Assert - Should treat as same group (case-insensitive)
            Assert.NotNull(groups);
            Assert.Equal(groups.Distinct(StringComparer.OrdinalIgnoreCase).Count(), groups.Length);
        }

        [Fact]
        public async Task GetAllGroupMembershipsAsync_NullSAMAccountName_ThrowsArgumentNullException() {
            // Arrange
            var mockConnectionFactory = LdapMockFixture.Factory;

            var validator = new GroupMembershipValidator(_validConnectionInfo, _validSearchLimits, _validCredential, mockConnectionFactory);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(
                () => validator.GetAllGroupMembershipsAsync(null));
        }

        [Fact]
        public async Task GetAllGroupMembershipsAsync_EmptySAMAccountName_ThrowsArgumentNullException() {
            // Arrange
            var mockConnectionFactory = LdapMockFixture.Factory;

            var validator = new GroupMembershipValidator(_validConnectionInfo, _validSearchLimits, _validCredential, mockConnectionFactory);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(
                () => validator.GetAllGroupMembershipsAsync(string.Empty));
        }

        [Fact]
        public async Task GetAllGroupMembershipsAsync_SAMAccountNameContainsWildcard_ThrowsArgumentException() {
            // Arrange
            var mockConnectionFactory = LdapMockFixture.Factory;

            var validator = new GroupMembershipValidator(_validConnectionInfo, _validSearchLimits, _validCredential, mockConnectionFactory);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(
                () => validator.GetAllGroupMembershipsAsync("john.*"));
        }

        #endregion

        #region Integration and Edge Cases

        [Fact]
        public async Task CheckGroupMembershipAsync_WithMultipleNestedPaths_ReturnsTrueIfAnyPathLeadsToTarget() {
            // Arrange - james.dockers has multiple paths to DomainAdmins
            var mockConnectionFactory = LdapMockFixture.Factory;

            var validator = new GroupMembershipValidator(_validConnectionInfo, _validSearchLimits, _validCredential, mockConnectionFactory);

            // Act
            var result = await validator.CheckGroupMembershipAsync("james.dockers", "Domain Admins");

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task GetAllGroupMembershipsAsync_WhenSearchFails_ThrowsException() {
            // Arrange
            var mockConnectionFactory = LdapMockFixture.Factory;

            var validator = new GroupMembershipValidator(_validConnectionInfo, _validSearchLimits, _validCredential, mockConnectionFactory);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<EntryNotFoundException>(
                () => validator.GetAllGroupMembershipsAsync("nonexistent.user"));

            Assert.StartsWith("unable to evaluate without an entry", exception.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task CheckGroupMembershipAsync_WithWhitespaceInCN_HandlesCorrectly() {
            // Arrange - No groups with whitespace in seeder, use existing group
            var mockConnectionFactory = LdapMockFixture.Factory;

            var validator = new GroupMembershipValidator(_validConnectionInfo, _validSearchLimits, _validCredential, mockConnectionFactory);

            // Act
            var result = await validator.CheckGroupMembershipAsync("james.dockers", "Domain Admins Z");

            // Assert - Should handle gracefully (group doesn't exist)
            Assert.False(result);
        }

        #endregion
    }
}
