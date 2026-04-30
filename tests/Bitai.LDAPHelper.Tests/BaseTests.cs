using Bitai.LDAPHelper.Tests.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitai.LDAPHelper.Tests
{
    public class BaseTests
    {
        protected const string LDAP_SERVER = "localhost";
        protected const int LDAP_PORT = 389;
        protected const string LDAP_USER_DN = "cn=read-admin,dc=domain,dc=com";
        protected const string LDAP_PASSWORD = "hiddnepwd";

        /// <summary>
        /// Creates a mock LDAP entry representing a user populated with common Active Directory attributes.
        /// </summary>
        /// <param name="firstName">The user's first name. Must not be null or empty.</param>
        /// <param name="lastName">The user's last name. Must not be null or empty.</param>
        /// <returns>
        /// A <see cref="MockLdapEntryAdapter"/> containing a distinguished name and a collection of attributes
        /// typically present on an AD user object (for example: objectSid, objectGUID, sAMAccountName, cn, sn,
        /// givenName, mail, title, department, userPrincipalName, userAccountControl, objectClass and various
        /// timestamp/account-related attributes).
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="firstName"/> or <paramref name="lastName"/> is null or an empty string.
        /// </exception>
        /// <remarks>
        /// This helper is intended for unit tests. Attribute values are deterministic based on the supplied
        /// names (e.g. sAMAccountName and mail) or fixed example values for timestamps and identifiers.
        /// </remarks>
        public MockLdapEntryAdapter CreateMockUserEntry(string firstName, string lastName, SearchLimits? searchLimits, out QueryFilters.AttributeFilter searchFilterSAMAccountName, out QueryFilters.AttributeFilter searchFilterDistinguishedName) {
            if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName))
                throw new ArgumentException("First name and last name cannot be null or empty.");

            //Mock LDAP entry for the user
            var mockLdapEntry = new MockLdapEntryAdapter($"CN={firstName} {lastName},{(searchLimits != null ? searchLimits.BaseDN : "DC=domain,DC=com")}");
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
            //mockLdapEntry.AddAttribute("whenChanged", "20220102000000.0Z");
            //mockLdapEntry.AddAttribute("lastLogon", "20220103000000.0Z");
            //mockLdapEntry.AddAttribute("lastLogonTimestamp", "20220104000000.0Z");
            //mockLdapEntry.AddAttribute("lastLogoff", "20220105000000.0Z");
            //mockLdapEntry.AddAttribute("pwdLastSet", "20220106000000.0Z");
            //mockLdapEntry.AddAttribute("accountExpires", "20220107000000.0Z");
            mockLdapEntry.AddAttribute("logonCount", "10");
            //mockLdapEntry.AddAttribute("badPasswordTime", "20220108000000.0Z");
            mockLdapEntry.AddAttribute("badPwdCount", "2");
            //mockLdapEntry.AddAttribute("lastBadPasswordAttempt", "20220109000000.0Z");
            //mockLdapEntry.AddAttribute("lastBadPasswordAttemptTimestamp", "20220110000000.0Z");

            searchFilterSAMAccountName = new QueryFilters.AttributeFilter(DTO.EntryAttribute.sAMAccountName, new QueryFilters.FilterValue($"{firstName.ToLower()}.{lastName.ToLower()}"));

            searchFilterDistinguishedName = new QueryFilters.AttributeFilter(DTO.EntryAttribute.distinguishedName, new QueryFilters.FilterValue(mockLdapEntry.DistinguishedName));

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
    }
}
