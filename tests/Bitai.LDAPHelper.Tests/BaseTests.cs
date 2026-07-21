using Bitai.LDAPHelper.LdapAdapters.LdapHelperMock;

namespace Bitai.LDAPHelper.Tests
{
    /// <summary>
    /// Shared test-fixture helpers for LDAP helper test suites.
    /// </summary>
    public class BaseTests
    {
        public MockLdapEntryAdapter CreateMockUserEntry(string firstName, string lastName, SearchLimits? searchLimits, out QueryFilters.AttributeFilter searchFilterSAMAccountName, out QueryFilters.AttributeFilter searchFilterDistinguishedName, string[] memberOfDistinguishedNames = null, string groupName = null, string groupContainerName = null) {
            if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName))
                throw new ArgumentException("First name and last name cannot be null or empty.");

            //Mock LDAP entry for the user
            var mockLdapEntry = new MockLdapEntryAdapter(
                $"CN={firstName} {lastName}" + 
                (string.IsNullOrEmpty(groupName) ? string.Empty : $",CN={groupName}" ) + 
                (string.IsNullOrEmpty(groupContainerName) ? string.Empty : $",OU={groupContainerName}") + 
                (searchLimits != null ? string.Concat(",", searchLimits.BaseDN) : ",DC=domain,DC=com"));
            //Add more attributes
            mockLdapEntry.AddAttribute("objectSid", new byte[] { 1, 5, 0, 0, 0, 0, 0, 5, 21, 0, 0, 0, 134, 161, 247, 215, 208, 13, 248, 19, 35, 76, 31, 226, 79, 4, 0, 0 });
            mockLdapEntry.AddAttribute("objectGUID", new byte[] { 0x12, 0x34, 0x56, 0x78, 0x9A, 0xBC, 0xDE, 0xF0, 0x12, 0x34, 0x56, 0x78, 0x9A, 0xBC, 0xDE, 0xF0 });
            mockLdapEntry.AddAttribute("sAMAccountName", $"{firstName.ToLower()}.{lastName.ToLower()}");
            mockLdapEntry.AddAttribute("cn", $"{firstName} {lastName}");
            mockLdapEntry.AddAttribute("sn", $"{lastName}");
            mockLdapEntry.AddAttribute("givenName", $"{firstName}");
            mockLdapEntry.AddAttribute("mail", $"{firstName.ToLower()}.{lastName.ToLower()}@bitaitec.com");
            mockLdapEntry.AddAttribute("title", "Software Engineer");
            mockLdapEntry.AddAttribute("department", "IT");
            mockLdapEntry.AddAttribute("mail", $"{firstName.ToLower()}.{lastName.ToLower()}@bitaitec.com");
            mockLdapEntry.AddAttribute("userPrincipalName", $"{firstName.ToLower()}.{lastName.ToLower()}@domain");
            mockLdapEntry.AddAttribute("userAccountControl", "512");
            mockLdapEntry.AddAttribute("objectClass", new string[] { "top", "person", "organizationalPerson", "user" });
            mockLdapEntry.AddAttribute("sAMAccountType", "805306368");
            mockLdapEntry.AddAttribute("whenCreated", "20220101000000.0Z");
            if (memberOfDistinguishedNames != null && memberOfDistinguishedNames.Length > 0) {
                mockLdapEntry.AddAttribute("memberOf", memberOfDistinguishedNames);
            }
            else {
                mockLdapEntry.AddAttribute("memberOf", new string[] { "CN=Devs,OU=IT,DC=domain,DC=com", "CN=Admins,OU=Trusted,DC=domain,DC=com" });
            }
            mockLdapEntry.AddAttribute("logonCount", "10");
            mockLdapEntry.AddAttribute("badPwdCount", "2");
            //mockLdapEntry.AddAttribute("whenChanged", "20220102000000.0Z");
            //mockLdapEntry.AddAttribute("lastLogon", "20220103000000.0Z");
            //mockLdapEntry.AddAttribute("lastLogonTimestamp", "20220104000000.0Z");
            //mockLdapEntry.AddAttribute("lastLogoff", "20220105000000.0Z");
            //mockLdapEntry.AddAttribute("pwdLastSet", "20220106000000.0Z");
            //mockLdapEntry.AddAttribute("accountExpires", "20220107000000.0Z");
            //mockLdapEntry.AddAttribute("badPasswordTime", "20220108000000.0Z");            
            //mockLdapEntry.AddAttribute("lastBadPasswordAttempt", "20220109000000.0Z");
            //mockLdapEntry.AddAttribute("lastBadPasswordAttemptTimestamp", "20220110000000.0Z");

            GenerateSearchFilter(mockLdapEntry.GetAttributeSet().GetAttribute(DTO.EntryAttribute.sAMAccountName.ToString()).StringValue, DTO.EntryAttribute.sAMAccountName, out searchFilterSAMAccountName);
            GenerateSearchFilter(mockLdapEntry.GetAttributeSet().GetAttribute(DTO.EntryAttribute.distinguishedName.ToString()).StringValue, DTO.EntryAttribute.distinguishedName, out searchFilterDistinguishedName);

            return mockLdapEntry;
        }

        public MockLdapEntryAdapter CreateMockGroupEntry(string groupName, string organitationalUnitName, SearchLimits? searchLimits, out QueryFilters.AttributeFilter searchFilterDistinguishedName) {
            if (string.IsNullOrEmpty(groupName) || string.IsNullOrEmpty(organitationalUnitName))
                throw new ArgumentException("First name and last name cannot be null or empty.");

            //Mock LDAP entry for the user
            var mockLdapEntry = new MockLdapEntryAdapter($"CN={groupName},OU={organitationalUnitName},{(searchLimits != null ? searchLimits.BaseDN : "DC=domain,DC=com")}");
            // SID for a typical Security Group (RID 1105)
            // S-1-5-21-3623811974-335183920-3791444003-1105
            mockLdapEntry.AddAttribute("objectSid", new byte[] { 1, 5, 0, 0, 0, 0, 0, 5, 21, 0, 0, 0, 134, 161, 247, 215, 208, 13, 248, 19, 35, 76, 31, 226, 79, 5, 0, 0 });
            // A unique 16-byte array for the Group's GUID
            // GUID: f8e9d7c6-b5a4-4321-8765-43210fedcba9
            mockLdapEntry.AddAttribute("objectGUID", new byte[] { 0xC6, 0xD7, 0xE9, 0xF8, 0xA4, 0xB5, 0x21, 0x43, 0x87, 0x65, 0x43, 0x21, 0x0F, 0xED, 0xCB, 0xA9 });
            mockLdapEntry.AddAttribute("sAMAccountName", $"{groupName}");
            mockLdapEntry.AddAttribute("cn", $"{groupName}");
            mockLdapEntry.AddAttribute("objectClass", new string[] { "top", "group" });
            mockLdapEntry.AddAttribute("sAMAccountType", "268435456");
            mockLdapEntry.AddAttribute("groupType", "-2147483640"); // Represents a Universal Security Group
            mockLdapEntry.AddAttribute("whenCreated", "20220101000000.0Z");
            //mockLdapEntry.AddAttribute("memberOf", new string[] { "CN=IT,DC=domain,DC=com", "CN=Trusted,DC=domain,DC=com" });
            //mockLdapEntry.AddAttribute("whenChanged", "20220102000000.0Z");
            //mockLdapEntry.AddAttribute("lastLogon", "20220103000000.0Z");
            //mockLdapEntry.AddAttribute("lastLogonTimestamp", "20220104000000.0Z");
            //mockLdapEntry.AddAttribute("lastLogoff", "20220105000000.0Z");
            //mockLdapEntry.AddAttribute("pwdLastSet", "20220106000000.0Z");
            //mockLdapEntry.AddAttribute("accountExpires", "20220107000000.0Z");

            GenerateCommonGroupSearchFilter(groupName, organitationalUnitName, out searchFilterDistinguishedName);

            return mockLdapEntry;
        }

        public ConnectionInfo CreateValidConnectionInfo(bool ssl) {
            return new ConnectionInfo(server: "localhost", port: 389, useSSL: ssl, connectionTimeout: 30);
        }

        public ConnectionInfo CreateInvalidConnectionInfo(bool ssl) {
            return new ConnectionInfo(server: "0.0.0.0", port: 0, useSSL: ssl, connectionTimeout: 30);
        }

        public SearchLimits CreateValidSearchLimits() {
            return new SearchLimits("DC=domain,DC=com") {
                MaxSearchResults = 1000,
                MaxSearchTimeout = 60
            };
        }

        public void GenerateCommonUserSearchFilter(string firstName, string lastName, SearchLimits searchLimits, out QueryFilters.AttributeFilter searchFilterSAMAccountName, out QueryFilters.AttributeFilter searchFilterDistinguishedName) {
            searchFilterSAMAccountName = new QueryFilters.AttributeFilter(DTO.EntryAttribute.sAMAccountName, new QueryFilters.FilterValue($"{firstName.ToLower()}.{lastName.ToLower()}"));

            searchFilterDistinguishedName = new QueryFilters.AttributeFilter(DTO.EntryAttribute.distinguishedName, new QueryFilters.FilterValue($"CN={firstName} {lastName},{searchLimits.BaseDN}"));
        }

        public void GenerateSearchFilter(string filterValue, DTO.EntryAttribute attribute, out QueryFilters.AttributeFilter searchFilter) {
            searchFilter = new QueryFilters.AttributeFilter(attribute, new QueryFilters.FilterValue(filterValue));
        }

        public void GenerateCommonGroupSearchFilter(string groupName, string organitationalUnitName, out QueryFilters.AttributeFilter searchFilterDistinguishedName) {
            searchFilterDistinguishedName = new QueryFilters.AttributeFilter(DTO.EntryAttribute.distinguishedName, new QueryFilters.FilterValue($"CN={groupName},OU={organitationalUnitName},DC=domain,DC=com"));
        }
    }
}
