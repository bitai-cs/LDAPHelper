using Bitai.LDAPHelper.Adapters;
using Bitai.LDAPHelper.DTO;
using Bitai.LDAPHelper.Tests.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Bitai.LDAPHelper.Tests
{
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
            // Arrange
            var mockConnection = new MockLdapConnectionAdapter();

            var userEntry = CreateMockUserEntry(
                firstName: "John",
                lastName: "Doe",
                searchLimits: _validSearchLimits,
                searchFilterSAMAccountName: out var userFilter,
                searchFilterDistinguishedName: out _,
                memberOfDistinguishedNames: new[] { "CN=Devs,OU=IT,DC=domain,DC=com" }
            );

            var groupEntry = CreateMockGroupEntry(
                groupName: "Devs",
                organitationalUnitName: "IT",
                searchLimits: _validSearchLimits,
                searchFilterDistinguishedName: out var groupFilter
            );

            mockConnection.AddSearchResult(userFilter.ToString(), new List<MockLdapEntryAdapter> { userEntry });
            mockConnection.AddSearchResult(groupFilter.ToString(), new List<MockLdapEntryAdapter> { groupEntry });

            var mockConnectionFactory = new MockLdapConnectionFactoryAdapter(mockConnection);
            var validator = new GroupMembershipValidator(_validConnectionInfo, _validSearchLimits, _validCredential, mockConnectionFactory);

            var result = await validator.CheckGroupMembershipAsync("john.doe", "Devs");

            Assert.True(result);
        }

        [Fact]
        public async Task CheckGroupMembershipAsync_UserIsIndirectMember_ReturnsTrue() {
            // Arrange
            var mockConnection = new MockLdapConnectionAdapter();

            // User -> DevTeam (direct) -> Devs (indirect)
            var userEntry = CreateMockUserEntry(
                firstName: "Jane",
                lastName: "Smith",
                searchLimits: _validSearchLimits,
                searchFilterSAMAccountName: out var userFilter,
                searchFilterDistinguishedName: out _,
                memberOfDistinguishedNames: new[] { "CN=DevTeam,OU=IT,DC=domain,DC=com" }
            );

            var devTeamGroup = CreateMockGroupEntry(
                groupName: "DevTeam",
                organitationalUnitName: "IT",
                searchLimits: _validSearchLimits,
                searchFilterDistinguishedName: out var devTeamFilter
            );
            devTeamGroup.AddAttribute("memberOf", new[] { "CN=Devs,OU=IT,DC=domain,DC=com" });

            var devsGroup = CreateMockGroupEntry(
                groupName: "Devs",
                organitationalUnitName: "IT",
                searchLimits: _validSearchLimits,
                searchFilterDistinguishedName: out var devsFilter
            );

            mockConnection.AddSearchResult(userFilter.ToString(), new List<MockLdapEntryAdapter> { userEntry });
            mockConnection.AddSearchResult(devTeamFilter.ToString(), new List<MockLdapEntryAdapter> { devTeamGroup });
            mockConnection.AddSearchResult(devsFilter.ToString(), new List<MockLdapEntryAdapter> { devsGroup });

            var mockConnectionFactory = new MockLdapConnectionFactoryAdapter(mockConnection);
            var validator = new GroupMembershipValidator(_validConnectionInfo, _validSearchLimits, _validCredential, mockConnectionFactory);

            // Act
            var result = await validator.CheckGroupMembershipAsync("jane.smith", "Devs");

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task CheckGroupMembershipAsync_UserIsMemberThroughMultipleLevels_ReturnsTrue() {
            // Arrange
            var mockConnection = new MockLdapConnectionAdapter();

            // User -> Level1 -> Level2 -> Level3 -> TargetGroup
            var userEntry = CreateMockUserEntry(
                firstName: "Deep",
                lastName: "Nested",
                searchLimits: _validSearchLimits,
                searchFilterSAMAccountName: out var userFilter,
                searchFilterDistinguishedName: out _,
                memberOfDistinguishedNames: new[] { "CN=Level1,OU=IT,DC=domain,DC=com" }
            );

            var level1 = CreateMockGroupEntry("Level1", "IT", _validSearchLimits, out var level1Filter);
            level1.AddAttribute("memberOf", new[] { "CN=Level2,OU=IT,DC=domain,DC=com" });

            var level2 = CreateMockGroupEntry("Level2", "IT", _validSearchLimits, out var level2Filter);
            level2.AddAttribute("memberOf", new[] { "CN=Level3,OU=IT,DC=domain,DC=com" });

            var level3 = CreateMockGroupEntry("Level3", "IT", _validSearchLimits, out var level3Filter);
            level3.AddAttribute("memberOf", new[] { "CN=TargetGroup,OU=IT,DC=domain,DC=com" });

            var targetGroup = CreateMockGroupEntry("TargetGroup", "IT", _validSearchLimits, out var targetFilter);

            mockConnection.AddSearchResult(userFilter.ToString(), new List<MockLdapEntryAdapter> { userEntry });
            mockConnection.AddSearchResult(level1Filter.ToString(), new List<MockLdapEntryAdapter> { level1 });
            mockConnection.AddSearchResult(level2Filter.ToString(), new List<MockLdapEntryAdapter> { level2 });
            mockConnection.AddSearchResult(level3Filter.ToString(), new List<MockLdapEntryAdapter> { level3 });
            mockConnection.AddSearchResult(targetFilter.ToString(), new List<MockLdapEntryAdapter> { targetGroup });

            var mockConnectionFactory = new MockLdapConnectionFactoryAdapter(mockConnection);
            var validator = new GroupMembershipValidator(_validConnectionInfo, _validSearchLimits, _validCredential, mockConnectionFactory);

            // Act
            var result = await validator.CheckGroupMembershipAsync("deep.nested", "TargetGroup");

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task CheckGroupMembershipAsync_UserIsNotMember_ReturnsFalse() {
            // Arrange
            var mockConnection = new MockLdapConnectionAdapter();

            var userEntry = CreateMockUserEntry(
                firstName: "Bob",
                lastName: "Johnson",
                searchLimits: _validSearchLimits,
                searchFilterSAMAccountName: out var userFilter,
                searchFilterDistinguishedName: out _,
                memberOfDistinguishedNames: new[] { "CN=DevTeam,OU=IT,DC=domain,DC=com" }
            );

            var devTeamGroup = CreateMockGroupEntry("DevTeam", "IT", _validSearchLimits, out var devTeamFilter);

            mockConnection.AddSearchResult(userFilter.ToString(), new List<MockLdapEntryAdapter> { userEntry });
            mockConnection.AddSearchResult(devTeamFilter.ToString(), new List<MockLdapEntryAdapter> { devTeamGroup });

            var mockConnectionFactory = new MockLdapConnectionFactoryAdapter(mockConnection);
            var validator = new GroupMembershipValidator(_validConnectionInfo, _validSearchLimits, _validCredential, mockConnectionFactory);

            // Act
            var result = await validator.CheckGroupMembershipAsync("bob.johnson", "Devs");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task CheckGroupMembershipAsync_UserNotFound_ThrowsException() {
            var mockConnection = new MockLdapConnectionAdapter();
            // No user entry added - will not be found

            var mockConnectionFactory = new MockLdapConnectionFactoryAdapter(mockConnection);
            var validator = new GroupMembershipValidator(_validConnectionInfo, _validSearchLimits, _validCredential, mockConnectionFactory);

            await Assert.ThrowsAsync<EntryNotFoundException>(() => validator.CheckGroupMembershipAsync("nonexistent.user", "Devs"));
        }

        //[Fact]
        //public async Task CheckGroupMembershipAsync_WithCircularReference_HandlesGracefullyAndReturnsTrue() {
        //    // Arrange
        //    var mockConnection = new MockLdapConnectionAdapter();

        //    var userEntry = CreateMockUserEntry(
        //        firstName: "Alice",
        //        lastName: "Brown",
        //        searchLimits: _validSearchLimits,
        //        searchFilterSAMAccountName: out var userFilter,
        //        searchFilterDistinguishedName: out _,
        //        memberOfDistinguishedNames: new[] { "CN=GroupA,OU=IT,DC=domain,DC=com" }
        //    );

        //    var groupA = CreateMockGroupEntry("GroupA", "IT", _validSearchLimits, out var groupAFilter);
        //    groupA.AddAttribute("memberOf", new[] { "CN=GroupB,OU=IT,DC=domain,DC=com" });

        //    var groupB = CreateMockGroupEntry("GroupB", "IT", _validSearchLimits, out var groupBFilter);
        //    groupB.AddAttribute("memberOf", new[] { "CN=GroupA,OU=IT,DC=domain,DC=com" }); // Circular reference

        //    mockConnection.AddSearchResult(userFilter.ToString(), new List<MockLdapEntryAdapter> { userEntry });
        //    mockConnection.AddSearchResult(groupAFilter.ToString(), new List<MockLdapEntryAdapter> { groupA });
        //    mockConnection.AddSearchResult(groupBFilter.ToString(), new List<MockLdapEntryAdapter> { groupB });

        //    var mockConnectionFactory = new MockLdapConnectionFactoryAdapter(mockConnection);

        //    var validator = new GroupMembershipValidator(_validConnectionInfo, _validSearchLimits, _validCredential, mockConnectionFactory);

        //    // Act - Should not throw StackOverflowException
        //    var result = await validator.CheckGroupMembershipAsync("alice.brown", "GroupA");

        //    // Assert
        //    Assert.True(result);
        //}

        [Fact]
        public async Task CheckGroupMembershipAsync_CaseInsensitiveComparison_ReturnsTrue() {
            // Arrange
            var mockConnection = new MockLdapConnectionAdapter();

            var groupEntry = CreateMockGroupEntry("Devs", "IT", _validSearchLimits, out var groupFilter);

            var userEntry = CreateMockUserEntry(
                firstName: "John",
                lastName: "Cena",
                searchLimits: _validSearchLimits,
                searchFilterSAMAccountName: out var userFilter,
                searchFilterDistinguishedName: out _,
                memberOfDistinguishedNames: new[] { groupEntry.DistinguishedName }
            );

            mockConnection.AddSearchResult(groupFilter.ToString(), new List<MockLdapEntryAdapter> { groupEntry });
            mockConnection.AddSearchResult(userFilter.ToString(), new List<MockLdapEntryAdapter> { userEntry });

            var mockConnectionFactory = new MockLdapConnectionFactoryAdapter(mockConnection);

            var validator = new GroupMembershipValidator(_validConnectionInfo, _validSearchLimits, _validCredential, mockConnectionFactory);

            var result = await validator.CheckGroupMembershipAsync("JOHN.CENA", "DEVS");

            Assert.True(result);
        }

        [Fact]
        public async Task CheckGroupMembershipAsync_NullSAMAccountName_ThrowsArgumentNullException() {
            // Arrange
            var mockConnectionFactory = new MockLdapConnectionFactoryAdapter(new MockLdapConnectionAdapter());
            var validator = new GroupMembershipValidator(_validConnectionInfo, _validSearchLimits, _validCredential, mockConnectionFactory);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(
                () => validator.CheckGroupMembershipAsync(null, "Devs"));
        }

        [Fact]
        public async Task CheckGroupMembershipAsync_EmptySAMAccountName_ThrowsArgumentNullException() {
            // Arrange
            var mockConnectionFactory = new MockLdapConnectionFactoryAdapter(new MockLdapConnectionAdapter());
            var validator = new GroupMembershipValidator(_validConnectionInfo, _validSearchLimits, _validCredential, mockConnectionFactory);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(
                () => validator.CheckGroupMembershipAsync(string.Empty, "Devs"));
        }

        [Fact]
        public async Task CheckGroupMembershipAsync_SAMAccountNameContainsWildcard_ThrowsArgumentException() {
            // Arrange
            var mockConnectionFactory = new MockLdapConnectionFactoryAdapter(new MockLdapConnectionAdapter());
            var validator = new GroupMembershipValidator(_validConnectionInfo, _validSearchLimits, _validCredential, mockConnectionFactory);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(
                () => validator.CheckGroupMembershipAsync("john.*", "Devs"));
        }

        [Fact]
        public async Task CheckGroupMembershipAsync_NullParentGroupCN_ThrowsArgumentNullException() {
            // Arrange
            var mockConnectionFactory = new MockLdapConnectionFactoryAdapter(new MockLdapConnectionAdapter());
            var validator = new GroupMembershipValidator(_validConnectionInfo, _validSearchLimits, _validCredential, mockConnectionFactory);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(
                () => validator.CheckGroupMembershipAsync("john.doe", null));
        }

        [Fact]
        public async Task CheckGroupMembershipAsync_EmptyParentGroupCN_ThrowsArgumentNullException() {
            // Arrange
            var mockConnectionFactory = new MockLdapConnectionFactoryAdapter(new MockLdapConnectionAdapter());
            var validator = new GroupMembershipValidator(_validConnectionInfo, _validSearchLimits, _validCredential, mockConnectionFactory);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(
                () => validator.CheckGroupMembershipAsync("john.doe", string.Empty));
        }

        [Fact]
        public async Task CheckGroupMembershipAsync_ParentGroupCNContainsWildcard_ThrowsArgumentException() {
            // Arrange
            var mockConnectionFactory = new MockLdapConnectionFactoryAdapter(new MockLdapConnectionAdapter());
            var validator = new GroupMembershipValidator(_validConnectionInfo, _validSearchLimits, _validCredential, mockConnectionFactory);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(
                () => validator.CheckGroupMembershipAsync("john.doe", "Dev*"));
        }

        [Fact]
        public async Task CheckGroupMembershipAsync_UserInMultipleGroups_FindsCorrectGroup() {
            var mockConnection = new MockLdapConnectionAdapter();

            var userEntry = CreateMockUserEntry(
                firstName: "Tom",
                lastName: "Anderson",
                searchLimits: _validSearchLimits,
                searchFilterSAMAccountName: out var userFilter,
                searchFilterDistinguishedName: out _,
                memberOfDistinguishedNames: new[]
                {
                    "CN=Users,CN=Builtin,DC=domain,DC=com",
                    "CN=Devs,OU=IT,DC=domain,DC=com",
                    "CN=PowerUsers,CN=Builtin,DC=domain,DC=com"
                }
            );

            var devsGroup = CreateMockGroupEntry("Devs", "IT", _validSearchLimits, out var devsGroupFilter);

            mockConnection.AddSearchResult(userFilter.ToString(), new List<MockLdapEntryAdapter> { userEntry });
            mockConnection.AddSearchResult(devsGroupFilter.ToString(), new List<MockLdapEntryAdapter> { devsGroup });

            var mockConnectionFactory = new MockLdapConnectionFactoryAdapter(mockConnection);

            var validator = new GroupMembershipValidator(_validConnectionInfo, _validSearchLimits, _validCredential, mockConnectionFactory);

            var result = await validator.CheckGroupMembershipAsync("tom.anderson", "Devs");

            Assert.True(result);
        }

        #endregion

        #region GetAllGroupMembershipsAsync Tests

        [Fact]
        public async Task GetAllGroupMembershipsAsync_UserWithDirectMemberships_ReturnsAllDirectGroups() {
            // Arrange
            var mockConnection = new MockLdapConnectionAdapter();

            var groupEntry1 = CreateMockGroupEntry("Devs", "IT", _validSearchLimits, out var groupSearchFilter1);
            var groupEntry2 = CreateMockGroupEntry("QA", "IT", _validSearchLimits, out var groupSearchFilter2);
            var groupEntry3 = CreateMockGroupEntry("Security", "IT", _validSearchLimits, out var groupSearchFilter3);

            var userEntry = CreateMockUserEntry(
                firstName: "David",
                lastName: "Clark",
                searchLimits: _validSearchLimits,
                searchFilterSAMAccountName: out var userSearchFilter,
                searchFilterDistinguishedName: out _,
                memberOfDistinguishedNames: new[]
                {
                    groupEntry1.DistinguishedName,
                    groupEntry2.DistinguishedName,
                    groupEntry3.DistinguishedName
                }
            );

            mockConnection.AddSearchResult(groupSearchFilter1.ToString(), new List<MockLdapEntryAdapter> { groupEntry1 });
            mockConnection.AddSearchResult(groupSearchFilter2.ToString(), new List<MockLdapEntryAdapter> { groupEntry2 });
            mockConnection.AddSearchResult(groupSearchFilter3.ToString(), new List<MockLdapEntryAdapter> { groupEntry3 });
            mockConnection.AddSearchResult(userSearchFilter.ToString(), new List<MockLdapEntryAdapter> { userEntry });

            var mockConnectionFactory = new MockLdapConnectionFactoryAdapter(mockConnection);

            var validator = new GroupMembershipValidator(_validConnectionInfo, _validSearchLimits, _validCredential, mockConnectionFactory);

            var groups = await validator.GetAllGroupMembershipsAsync("david.clark");

            Assert.NotNull(groups);
            Assert.Contains("Devs", groups);
            Assert.Contains("QA", groups);
            Assert.Contains("Security", groups);
            Assert.Equal(3, groups.Length);
        }

        [Fact]
        public async Task GetAllGroupMembershipsAsync_UserWithNestedMemberships_ReturnsAllGroupsIncludingIndirect() {
            // Arrange
            var mockConnection = new MockLdapConnectionAdapter();

            var itDeptGroup = CreateMockGroupEntry("ITDepartment", "IT", _validSearchLimits, out var itDeptFilter);
            
            var devTeamGroup = CreateMockGroupEntry("DevTeam", "IT", _validSearchLimits, out var devTeamFilter);
            devTeamGroup.AddAttribute("memberOf", new[] { itDeptGroup.DistinguishedName });

            // User -> DevTeam (direct) -> ITDepartment (indirect)
            var userEntry = CreateMockUserEntry(
                firstName: "David",
                lastName: "Miller",
                searchLimits: _validSearchLimits,
                searchFilterSAMAccountName: out var userFilter,
                searchFilterDistinguishedName: out _,
                memberOfDistinguishedNames: new[] { devTeamGroup.DistinguishedName }
            );

            mockConnection.AddSearchResult(userFilter.ToString(), new List<MockLdapEntryAdapter> { userEntry });
            mockConnection.AddSearchResult(devTeamFilter.ToString(), new List<MockLdapEntryAdapter> { devTeamGroup });
            mockConnection.AddSearchResult(itDeptFilter.ToString(), new List<MockLdapEntryAdapter> { itDeptGroup });

            var mockConnectionFactory = new MockLdapConnectionFactoryAdapter(mockConnection);

            var validator = new GroupMembershipValidator(_validConnectionInfo, _validSearchLimits, _validCredential, mockConnectionFactory);

            var groups = await validator.GetAllGroupMembershipsAsync("david.miller");

            // Assert - Should include both direct (DevTeam) and indirect (ITDepartment) groups
            Assert.NotNull(groups);
            Assert.Contains("DevTeam", groups);
            Assert.Contains("ITDepartment", groups);
        }

        [Fact]
        public async Task GetAllGroupMembershipsAsync_UserWithDeepNesting_ReturnsAllGroupsInHierarchy() {
            // Arrange
            var mockConnection = new MockLdapConnectionAdapter();

            var userEntry = CreateMockUserEntry(
                firstName: "Deep",
                lastName: "Hierarchy",
                searchLimits: _validSearchLimits,
                searchFilterSAMAccountName: out var userFilter,
                searchFilterDistinguishedName: out _,
                memberOfDistinguishedNames: new[] { "CN=Level1,OU=IT,DC=domain,DC=com" }
            );

            var level1 = CreateMockGroupEntry("Level1", "IT", _validSearchLimits, out var level1Filter);
            level1.AddAttribute("memberOf", new[] { "CN=Level2,OU=IT,DC=domain,DC=com" });

            var level2 = CreateMockGroupEntry("Level2", "IT", _validSearchLimits, out var level2Filter);
            level2.AddAttribute("memberOf", new[] { "CN=Level3,OU=IT,DC=domain,DC=com" });

            var level3 = CreateMockGroupEntry("Level3", "IT", _validSearchLimits, out var level3Filter);

            mockConnection.AddSearchResult(userFilter.ToString(), new List<MockLdapEntryAdapter> { userEntry });
            mockConnection.AddSearchResult(level1Filter.ToString(), new List<MockLdapEntryAdapter> { level1 });
            mockConnection.AddSearchResult(level2Filter.ToString(), new List<MockLdapEntryAdapter> { level2 });
            mockConnection.AddSearchResult(level3Filter.ToString(), new List<MockLdapEntryAdapter> { level3 });

            var mockConnectionFactory = new MockLdapConnectionFactoryAdapter(mockConnection);
            var validator = new GroupMembershipValidator(_validConnectionInfo, _validSearchLimits, _validCredential, mockConnectionFactory);

            // Act
            var groups = await validator.GetAllGroupMembershipsAsync("deep.hierarchy");

            // Assert
            Assert.NotNull(groups);
            Assert.Contains("Level1", groups);
            Assert.Contains("Level2", groups);
            Assert.Contains("Level3", groups);
            Assert.Equal(3, groups.Length);
        }

        [Fact]
        public async Task GetAllGroupMembershipsAsync_UserWithNoGroups_ReturnsEmptyArray() {
            var mockConnection = new MockLdapConnectionAdapter();

            var userEntry = CreateMockUserEntry(
                firstName: "Emily",
                lastName: "White",
                searchLimits: _validSearchLimits,
                searchFilterSAMAccountName: out var userFilter,
                searchFilterDistinguishedName: out _,
                memberOfDistinguishedNames: null
            );

            mockConnection.AddSearchResult(userFilter.ToString(), new List<MockLdapEntryAdapter> { userEntry });

            var mockConnectionFactory = new MockLdapConnectionFactoryAdapter(mockConnection);
            var validator = new GroupMembershipValidator(_validConnectionInfo, _validSearchLimits, _validCredential, mockConnectionFactory);

            var groups = await validator.GetAllGroupMembershipsAsync("emily.white");

            // Assert
            Assert.NotNull(groups);
            Assert.Empty(groups);
        }

        [Fact]
        public async Task GetAllGroupMembershipsAsync_UserNotFound_ThrowsException() {
            // Arrange
            var mockConnection = new MockLdapConnectionAdapter();
            // No user entry added

            var mockConnectionFactory = new MockLdapConnectionFactoryAdapter(mockConnection);
            var validator = new GroupMembershipValidator(_validConnectionInfo, _validSearchLimits, _validCredential, mockConnectionFactory);

            // Act
            var ex = await Assert.ThrowsAnyAsync<EntryNotFoundException>(() => validator.GetAllGroupMembershipsAsync("nonexistent.user"));
        }

        [Fact]
        public async Task GetAllGroupMembershipsAsync_RemovesDuplicateGroups() {
            // Arrange
            var mockConnection = new MockLdapConnectionAdapter();

            var groupEntry = CreateMockGroupEntry("Devs", "IT", _validSearchLimits, out var groupFilter);

            var userEntry = CreateMockUserEntry(
                firstName: "Chris",
                lastName: "Brown",
                searchLimits: _validSearchLimits,
                searchFilterSAMAccountName: out var userFilter,
                searchFilterDistinguishedName: out _,
                memberOfDistinguishedNames: new[]
                {
                    groupEntry.DistinguishedName,
                    groupEntry.DistinguishedName  // Duplicate
                }
            );

            mockConnection.AddSearchResult(groupFilter.ToString(), new List<MockLdapEntryAdapter> { groupEntry });
            mockConnection.AddSearchResult(userFilter.ToString(), new List<MockLdapEntryAdapter> { userEntry });

            var mockConnectionFactory = new MockLdapConnectionFactoryAdapter(mockConnection);

            var validator = new GroupMembershipValidator(_validConnectionInfo, _validSearchLimits, _validCredential, mockConnectionFactory);

            // Act
            var groups = await validator.GetAllGroupMembershipsAsync("chris.brown");

            // Assert - Should only have one instance of Devs
            Assert.NotNull(groups);
            Assert.Single(groups);
            Assert.Equal("Devs", groups[0]);
        }

        [Fact]
        public async Task GetAllGroupMembershipsAsync_CaseInsensitiveDistinct_ReturnsUniqueGroups() {
            // Arrange
            var mockConnection = new MockLdapConnectionAdapter();

            var groupEntry = CreateMockGroupEntry("Devs", "IT", _validSearchLimits, out var groupFilter);
            var groupEntryDuplicateCase = CreateMockGroupEntry("DEVS", "IT", _validSearchLimits, out var groupFilterDuplicateCase);

            var userEntry = CreateMockUserEntry(
                firstName: "Case",
                lastName: "Sensitive",
                searchLimits: _validSearchLimits,
                searchFilterSAMAccountName: out var userFilter,
                searchFilterDistinguishedName: out _,
                memberOfDistinguishedNames: new[]
                {
                    groupEntry.DistinguishedName,
                    groupEntryDuplicateCase.DistinguishedName  // Same group, different case
                }
            );

            mockConnection.AddSearchResult(groupFilter.ToString(), new List<MockLdapEntryAdapter> { groupEntry });
            mockConnection.AddSearchResult(groupFilterDuplicateCase.ToString(), new List<MockLdapEntryAdapter> { groupEntryDuplicateCase });
            mockConnection.AddSearchResult(userFilter.ToString(), new List<MockLdapEntryAdapter> { userEntry });

            var mockConnectionFactory = new MockLdapConnectionFactoryAdapter(mockConnection);
            var validator = new GroupMembershipValidator(_validConnectionInfo, _validSearchLimits, _validCredential, mockConnectionFactory);

            // Act
            var groups = await validator.GetAllGroupMembershipsAsync("case.sensitive");

            // Assert - Should treat as same group (case-insensitive)
            Assert.NotNull(groups);
            Assert.Single(groups);
            Assert.Equal("Devs", groups[0], StringComparer.OrdinalIgnoreCase);
        }

        //[Fact]
        //public async Task GetAllGroupMembershipsAsync_WithCircularReference_HandlesGracefullyAndReturnsUniqueGroups() {
        //    // Arrange
        //    var mockConnection = new MockLdapConnectionAdapter();

        //    var userEntry = CreateMockUserEntry(
        //        firstName: "Circular",
        //        lastName: "Reference",
        //        searchLimits: _validSearchLimits,
        //        searchFilterSAMAccountName: out var userFilter,
        //        searchFilterDistinguishedName: out _,
        //        memberOfDistinguishedNames: new[] { "CN=GroupA,OU=IT,DC=domain,DC=com" }
        //    );

        //    var groupA = CreateMockGroupEntry("GroupA", "IT", _validSearchLimits, out var groupAFilter);
        //    groupA.AddAttribute("memberOf", new[] { "CN=GroupB,OU=IT,DC=domain,DC=com" });

        //    var groupB = CreateMockGroupEntry("GroupB", "IT", _validSearchLimits, out var groupBFilter);
        //    groupB.AddAttribute("memberOf", new[] { "CN=GroupA,OU=IT,DC=domain,DC=com" }); // Circular

        //    mockConnection.AddSearchResult(userFilter.ToString(), new List<MockLdapEntryAdapter> { userEntry });
        //    mockConnection.AddSearchResult(groupAFilter.ToString(), new List<MockLdapEntryAdapter> { groupA });
        //    mockConnection.AddSearchResult(groupBFilter.ToString(), new List<MockLdapEntryAdapter> { groupB });

        //    var mockConnectionFactory = new MockLdapConnectionFactoryAdapter(mockConnection);
        //    var validator = new GroupMembershipValidator(_validConnectionInfo, _validSearchLimits, _validCredential, mockConnectionFactory);

        //    // Act - Should not throw StackOverflowException
        //    var groups = await validator.GetAllGroupMembershipsAsync("circular.reference");

        //    // Assert - Should have both groups but no duplicates from circular reference
        //    Assert.NotNull(groups);
        //    Assert.Contains("GroupA", groups);
        //    Assert.Contains("GroupB", groups);
        //    Assert.Equal(2, groups.Length);
        //}

        [Fact]
        public async Task GetAllGroupMembershipsAsync_NullSAMAccountName_ThrowsArgumentNullException() {
            // Arrange
            var mockConnectionFactory = new MockLdapConnectionFactoryAdapter(new MockLdapConnectionAdapter());
            var validator = new GroupMembershipValidator(_validConnectionInfo, _validSearchLimits, _validCredential, mockConnectionFactory);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(
                () => validator.GetAllGroupMembershipsAsync(null));
        }

        [Fact]
        public async Task GetAllGroupMembershipsAsync_EmptySAMAccountName_ThrowsArgumentNullException() {
            // Arrange
            var mockConnectionFactory = new MockLdapConnectionFactoryAdapter(new MockLdapConnectionAdapter());
            var validator = new GroupMembershipValidator(_validConnectionInfo, _validSearchLimits, _validCredential, mockConnectionFactory);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(
                () => validator.GetAllGroupMembershipsAsync(string.Empty));
        }

        [Fact]
        public async Task GetAllGroupMembershipsAsync_SAMAccountNameContainsWildcard_ThrowsArgumentException() {
            // Arrange
            var mockConnectionFactory = new MockLdapConnectionFactoryAdapter(new MockLdapConnectionAdapter());
            var validator = new GroupMembershipValidator(_validConnectionInfo, _validSearchLimits, _validCredential, mockConnectionFactory);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(
                () => validator.GetAllGroupMembershipsAsync("john.*"));
        }

        #endregion

        #region Integration and Edge Cases

        [Fact]
        public async Task CheckGroupMembershipAsync_WithMultipleNestedPaths_ReturnsTrueIfAnyPathLeadsToTarget() {
            // Arrange
            var mockConnection = new MockLdapConnectionAdapter();

            // User has two paths: one leads to TargetGroup, one doesn't
            var userEntry = CreateMockUserEntry(
                firstName: "Multi",
                lastName: "Path",
                searchLimits: _validSearchLimits,
                searchFilterSAMAccountName: out var userFilter,
                searchFilterDistinguishedName: out _,
                memberOfDistinguishedNames: new[]
                {
                    "CN=TeamA,OU=IT,DC=domain,DC=com",
                    "CN=TeamB,OU=IT,DC=domain,DC=com"
                }
            );

            var teamA = CreateMockGroupEntry("TeamA", "IT", _validSearchLimits, out var teamAFilter);
            teamA.AddAttribute("memberOf", new[] { "CN=OtherGroup,OU=IT,DC=domain,DC=com" });

            var teamB = CreateMockGroupEntry("TeamB", "IT", _validSearchLimits, out var teamBFilter);
            teamB.AddAttribute("memberOf", new[] { "CN=TargetGroup,OU=IT,DC=domain,DC=com" });

            var otherGroup = CreateMockGroupEntry("OtherGroup", "IT", _validSearchLimits, out _);
            var targetGroup = CreateMockGroupEntry("TargetGroup", "IT", _validSearchLimits, out var targetFilter);

            mockConnection.AddSearchResult(userFilter.ToString(), new List<MockLdapEntryAdapter> { userEntry });
            mockConnection.AddSearchResult(teamAFilter.ToString(), new List<MockLdapEntryAdapter> { teamA });
            mockConnection.AddSearchResult(teamBFilter.ToString(), new List<MockLdapEntryAdapter> { teamB });
            mockConnection.AddSearchResult(targetFilter.ToString(), new List<MockLdapEntryAdapter> { targetGroup });

            var mockConnectionFactory = new MockLdapConnectionFactoryAdapter(mockConnection);
            var validator = new GroupMembershipValidator(_validConnectionInfo, _validSearchLimits, _validCredential, mockConnectionFactory);

            // Act
            var result = await validator.CheckGroupMembershipAsync("multi.path", "TargetGroup");

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task GetAllGroupMembershipsAsync_WhenSearchFails_ThrowsException() {
            var mockConnection = new MockLdapConnectionAdapter();

            // Don't add any search results - this will cause search to fail

            var mockConnectionFactory = new MockLdapConnectionFactoryAdapter(mockConnection);
            
            var validator = new GroupMembershipValidator(_validConnectionInfo, _validSearchLimits, _validCredential, mockConnectionFactory);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<EntryNotFoundException>(
                () => validator.GetAllGroupMembershipsAsync("john.doe"));

            Assert.Contains("no one entry was found", exception.Message.ToLower());
        }

        [Fact]
        public async Task CheckGroupMembershipAsync_WithWhitespaceInCN_HandlesCorrectly() {
            // Arrange
            var mockConnection = new MockLdapConnectionAdapter();

            var userEntry = CreateMockUserEntry(
                firstName: "White",
                lastName: "Space",
                searchLimits: _validSearchLimits,
                searchFilterSAMAccountName: out var userFilter,
                searchFilterDistinguishedName: out _,
                memberOfDistinguishedNames: new[] { "CN=Dev Team,OU=IT,DC=domain,DC=com" }
            );

            var groupEntry = CreateMockGroupEntry("Dev Team", "IT", _validSearchLimits, out var groupFilter);

            mockConnection.AddSearchResult(userFilter.ToString(), new List<MockLdapEntryAdapter> { userEntry });
            mockConnection.AddSearchResult(groupFilter.ToString(), new List<MockLdapEntryAdapter> { groupEntry });

            var mockConnectionFactory = new MockLdapConnectionFactoryAdapter(mockConnection);
            var validator = new GroupMembershipValidator(_validConnectionInfo, _validSearchLimits, _validCredential, mockConnectionFactory);

            // Act
            var result = await validator.CheckGroupMembershipAsync("white.space", "Dev Team");

            // Assert
            Assert.True(result);
        }

        #endregion
    }
}