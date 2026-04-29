// TestHelper.cs
using Bitai.LDAPHelper.Adapters;
using Bitai.LDAPHelper.Tests.Mocks;

namespace Bitai.LDAPHelper.Tests
{
    public static class TestHelper
    {
        public static MockLdapConnectionAdapter CreateMockConnectionWithTestData() {
            var mockConnection = new MockLdapConnectionAdapter();

            // Add a test user
            var testUser = new MockLdapEntryAdapter("CN=John Doe,OU=Users,DC=test,DC=com");
            testUser.AddAttribute("objectSid", new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 });
            testUser.AddAttribute("objectGuid", System.Guid.NewGuid().ToByteArray());
            testUser.AddAttribute("sAMAccountName", "jdoe");
            testUser.AddAttribute("cn", "John Doe");
            testUser.AddAttribute("displayName", "John Doe");
            testUser.AddAttribute("givenName", "John");
            testUser.AddAttribute("sn", "Doe");
            testUser.AddAttribute("mail", "john.doe@test.com");
            testUser.AddAttribute("userPrincipalName", "jdoe@test.com");
            testUser.AddAttribute("objectClass", new[] { "top", "person", "organizationalPerson", "user" });
            testUser.AddAttribute("userAccountControl", "512");

            mockConnection.AddSearchResult("(sAMAccountName=jdoe)", new List<MockLdapEntryAdapter> { testUser });
            mockConnection.AddSearchResult("(mail=john.doe@test.com)", new List<MockLdapEntryAdapter> { testUser });

            // Add a test group
            var testGroup = new MockLdapEntryAdapter("CN=Developers,OU=Groups,DC=test,DC=com");
            testGroup.AddAttribute("objectSid", new byte[] { 9, 10, 11, 12, 13, 14, 15, 16 });
            testGroup.AddAttribute("sAMAccountName", "Developers");
            testGroup.AddAttribute("cn", "Developers");
            testGroup.AddAttribute("objectClass", new[] { "top", "group" });
            testGroup.AddAttribute("member", new[] { "CN=John Doe,OU=Users,DC=test,DC=com" });

            mockConnection.AddSearchResult("(sAMAccountName=Developers)", new List<MockLdapEntryAdapter> { testGroup });

            return mockConnection;
        }

        public static ILdapConnectionFactoryAdapter CreateMockFactoryWithTestData() {
            var mockConnection = CreateMockConnectionWithTestData();
            return new MockLdapConnectionFactoryAdapter(mockConnection);
        }
    }
}