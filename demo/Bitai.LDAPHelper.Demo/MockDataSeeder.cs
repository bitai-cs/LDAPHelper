using System;
using System.Collections.Generic;
using System.Linq;
using Bitai.LDAPHelper.DTO;
using Bitai.LDAPHelper.Tests.Mocks;
using Serilog;

namespace Bitai.LDAPHelper.Demo
{
    public class MockDataSeeder
    {
        private readonly DemoContext _context;
        private readonly MockDataStore _dataStore;

        public MockDataSeeder(DemoContext context)
        {
            _context = context;
            _dataStore = MockDataStore.Instance;
        }

        public void SeedAllData()
        {
            _dataStore.Clear();
            
            SeedContainerOUs();
            SeedStandardUsers();
            SeedStandardGroups();
            SeedDemoSpecificData();
            SeedGroupMemberships();
            
            Log.Information($"Mock data seeded successfully. Total entries: {_dataStore.Count}");
        }

        private void SeedContainerOUs()
        {
            // Create container OUs that might be referenced in config
            var containers = new[] 
            { 
                "OU=TEST_RPA_MDA,OU=ADM,OU=CERTUS,DC=certus,DC=edu,DC=pe",
                "OU=Users,DC=certus,DC=edu,DC=pe",
                "OU=Groups,DC=certus,DC=edu,DC=pe",
                "OU=IT,DC=certus,DC=edu,DC=pe"
            };

            foreach (var containerDN in containers)
            {
                var container = new MockLdapEntryAdapter(containerDN);
                container.AddAttribute("objectClass", new[] { "top", "organizationalUnit" });
                container.AddAttribute("ou", containerDN.Split(',')[0].Replace("OU=", ""));
                _dataStore.AddOrUpdateEntry(container);
            }
        }

        private void SeedStandardUsers()
        {
            var users = new List<(string FirstName, string LastName, string SAMAccountName, string Container)>
            {
                ("John", "Doe", "jdoe", "OU=Users,DC=certus,DC=edu,DC=pe"),
                ("Jane", "Smith", "jsmith", "OU=Users,DC=certus,DC=edu,DC=pe"),
                ("Bob", "Johnson", "bjohnson", "OU=Users,DC=certus,DC=edu,DC=pe"),
                ("Alice", "Brown", "abrown", "OU=Users,DC=certus,DC=edu,DC=pe"),
                ("Administrator", "Admin", "Administrator", "CN=Users,DC=certus,DC=edu,DC=pe")
            };

            foreach (var user in users)
            {
                CreateMockUser(
                    user.FirstName,
                    user.LastName,
                    user.SAMAccountName,
                    user.Container
                );
            }
        }

        private void SeedStandardGroups()
        {
            var groups = new[]
            {
                ("Devs", "IT", "Devs"),
                ("Admins", "IT", "Admins"),
                ("Administradores", "IT", "Administradores"),
                ("Users", "IT", "Users"),
                ("Developers", "IT", "Developers")
            };

            foreach (var group in groups)
            {
                CreateMockGroup(group.Item1, group.Item2, group.Item3);
            }
        }

        private void SeedDemoSpecificData()
        {
            // Create user account from config if demo requires it
            if (_context.Configuration.Demo_AccountManager_CreateUserAccount_RunTest ||
                _context.Configuration.Demo_Authenticator_Authenticate_RunTest)
            {
                var testUser = _context.Configuration.Demo_AccountManager_CreateUserAccount_UserAccountName;
                if (!string.IsNullOrEmpty(testUser) && testUser != "vbastidas01")
                {
                    var container = _context.Configuration.Demo_AccountManager_CreateUserAccount_ContainerDN;
                    CreateMockUser(
                        _context.Configuration.Demo_AccountManager_CreateUserAccount_Name ?? "Test",
                        _context.Configuration.Demo_AccountManager_CreateUserAccount_Surname ?? "User",
                        testUser,
                        container
                    );
                }
            }

            // Create group for membership validation
            if (_context.Configuration.Demo_GroupMembershipValidator_RunTest)
            {
                var groupName = _context.Configuration.Demo_GroupMembershipValidator_CheckGroupmembership_Check_GroupName;
                if (!string.IsNullOrEmpty(groupName))
                {
                    CreateMockGroup(groupName, "IT", groupName);
                }
            }
        }

        private void SeedGroupMemberships()
        {
            // Add users to groups
            var user = _dataStore.GetEntry($"CN=John Doe,OU=Users,DC=certus,DC=edu,DC=pe");
            var devs = _dataStore.GetEntry($"CN=Devs,OU=IT,DC=certus,DC=edu,DC=pe");
            
            if (user != null && devs != null)
            {
                var currentMemberOf = user.GetAttributeSet().GetAttribute("memberOf")?.StringValueArray ?? new string[0];
                var newMemberOf = currentMemberOf.Concat(new[] { devs.DistinguishedName }).ToArray();
                user.AddAttribute("memberOf", newMemberOf);
                _dataStore.AddOrUpdateEntry(user);
            }
        }

        private void CreateMockUser(string firstName, string lastName, string samAccountName, string containerDN)
        {
            var distinguishedName = $"CN={firstName} {lastName},{containerDN}";
            var user = new MockLdapEntryAdapter(distinguishedName);
            
            // Generate realistic SID (S-1-5-21-domain-rid)
            var random = new Random();
            var sidBytes = new byte[28];
            random.NextBytes(sidBytes);
            sidBytes[0] = 1; // Revision
            sidBytes[1] = 5; // Sub authority count
            
            user.AddAttribute("objectSid", sidBytes);
            user.AddAttribute("objectGuid", Guid.NewGuid().ToByteArray());
            user.AddAttribute("sAMAccountName", samAccountName);
            user.AddAttribute("cn", $"{firstName} {lastName}");
            user.AddAttribute("sn", lastName);
            user.AddAttribute("givenName", firstName);
            user.AddAttribute("displayName", $"{firstName} {lastName}");
            user.AddAttribute("mail", $"{samAccountName}@certus.edu.pe");
            user.AddAttribute("userPrincipalName", $"{samAccountName}@certus.edu.pe");
            user.AddAttribute("userAccountControl", "512"); // Normal account
            user.AddAttribute("objectClass", new[] { "top", "person", "organizationalPerson", "user" });
            user.AddAttribute("sAMAccountType", "805306368"); // Normal user account
            user.AddAttribute("whenCreated", DateTime.UtcNow.AddDays(-30).ToString("yyyyMMddHHmmss.0Z"));
            user.AddAttribute("distinguishedName", distinguishedName);
            
            _dataStore.AddOrUpdateEntry(user);
        }

        private void CreateMockGroup(string groupName, string ou, string samAccountName)
        {
            var distinguishedName = $"CN={groupName},OU={ou},DC=certus,DC=edu,DC=pe";
            var group = new MockLdapEntryAdapter(distinguishedName);
            
            var random = new Random();
            var sidBytes = new byte[28];
            random.NextBytes(sidBytes);
            sidBytes[0] = 1;
            sidBytes[1] = 5;
            
            group.AddAttribute("objectSid", sidBytes);
            group.AddAttribute("objectGuid", Guid.NewGuid().ToByteArray());
            group.AddAttribute("sAMAccountName", samAccountName);
            group.AddAttribute("cn", groupName);
            group.AddAttribute("objectClass", new[] { "top", "group" });
            group.AddAttribute("sAMAccountType", "268435456"); // Security group
            group.AddAttribute("groupType", "-2147483640"); // Universal security group
            group.AddAttribute("whenCreated", DateTime.UtcNow.AddDays(-30).ToString("yyyyMMddHHmmss.0Z"));
            group.AddAttribute("distinguishedName", distinguishedName);
            
            _dataStore.AddOrUpdateEntry(group);
        }
    }
}