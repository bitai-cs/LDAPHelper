using Bitai.LDAPHelper.Tests.Mocks.LdapAdapters;
using Microsoft.Extensions.Logging;

namespace Bitai.LDAPHelper.Tests.Mocks.LdapData;

/// <summary>
/// Seeds deterministic mock LDAP data for integration-style tests and demos.
/// </summary>
public class MockLdapDataSeeder
{
    private readonly MockLdapDataStore _dataStore;
    private readonly Random _random;
    private readonly ILogger<MockLdapDataSeeder> _logger;

    // Progressive RID counters (thread-safe)
    private static int _nextUserRid = 1000;     // Users start from 1000
    private static int _nextGroupRid = 2000;    // Groups start from 2000
    private static int _nextComputerRid = 3000;  // Computers start from 3000
    private static int _nextOtherRid = 4000;     // Other objects start from 4000

    public MockLdapDataSeeder(ILogger<MockLdapDataSeeder> logger) {
        _dataStore = MockLdapDataStore.Instance;
        _random = new Random();
        _logger = logger;
    }

    public void SeedAllData() {
        _dataStore.Clear();

        _logger.LogInformation("=".PadRight(60, '='));
        _logger.LogInformation("MOCK DATA SEEDING STARTED");
        _logger.LogInformation("=".PadRight(60, '='));
        _logger.LogInformation($"RID counters initialized - Users: {_nextUserRid}, Groups: {_nextGroupRid}, Computers: {_nextComputerRid}");
        _logger.LogInformation("=".PadRight(60, '='));

        // Create domain structure
        SeedDomainStructure();

        // Create Organizational Units (OUs)
        SeedOrganizationalUnits();

        // Create standard users
        SeedStandardUsers();

        // Create groups
        SeedGroups();

        // Create computer entries (for search demos)
        SeedComputerEntries();

        // Create demo-specific users
        SeedDemoSpecificUsers();

        // Create demo-specific groups
        SeedDemoSpecificGroups();

        // Establish group memberships
        SeedGroupMemberships();

        // Create additional relationships
        SeedAdditionalRelationships();

        _logger.LogInformation("=".PadRight(60, '='));
        _logger.LogInformation($"MOCK DATA SEEDING COMPLETED");
        _logger.LogInformation($"Total Entries Created: {_dataStore.Count}");
        _logger.LogInformation($"Users: {CountEntriesByObjectClass("user")}");
        _logger.LogInformation($"Groups: {CountEntriesByObjectClass("group")}");
        _logger.LogInformation($"Computers: {CountEntriesByObjectClass("computer")}");
        _logger.LogInformation($"OUs: {CountEntriesByObjectClass("organizationalUnit")}");
        _logger.LogInformation($"Final RID counters - Users: {_nextUserRid}, Groups: {_nextGroupRid}, Computers: {_nextComputerRid}");
        _logger.LogInformation("=".PadRight(60, '='));
    }

    public void PrintAllData() {
        _logger.LogInformation("=".PadRight(60, '='));
        _logger.LogInformation($"LISTING ALL ENTRIES: {_dataStore.Count}");
        foreach (var entry in _dataStore.GetAllEntries())
        {
            _logger.LogInformation(entry.DistinguishedName);
        }
        _logger.LogInformation("=".PadRight(60, '='));
    }

    #region Domain Structure

    private void SeedDomainStructure() {
        // Create domain roots (no RID needed for domains)
        var domains = new[]
        {
            "DC=holding,DC=latam,DC=com",
            "DC=us,DC=latam,DC=com",
            "DC=pe,DC=latam,DC=com",
            "DC=br,DC=latam,DC=com",
            "DC=mx,DC=latam,DC=com",
            "DC=cl,DC=latam,DC=com"
        };

        foreach (var domainDN in domains) {
            var domain = new MockLdapEntryAdapter(domainDN);
            domain.AddAttribute("objectClass", new[] { "top", "domain", "domainDNS" });
            domain.AddAttribute("dc", domainDN.Split(',')[0].Replace("DC=", ""));
            domain.AddAttribute("distinguishedName", domainDN);
            _dataStore.AddOrUpdateEntry(domain);
        }
    }

    private void SeedOrganizationalUnits() {
        var ous = new[]
        {
            // IT Department structure
            "OU=IT,DC=holding,DC=latam,DC=com",
            "OU=DevOps,OU=IT,DC=holding,DC=latam,DC=com",
            "OU=Seniors,OU=DevOps,OU=IT,DC=holding,DC=latam,DC=com",
            "OU=Juniors,OU=DevOps,OU=IT,DC=holding,DC=latam,DC=com",
            "OU=Interships,OU=DevOps,OU=IT,DC=holding,DC=latam,DC=com",
            "OU=Support,OU=IT,DC=holding,DC=latam,DC=com",
            "OU=Interships,OU=Support,OU=IT,DC=us,DC=latam,DC=com",
            "OU=Interships,OU=Support,OU=IT,DC=pe,DC=latam,DC=com",
            
            // US Region
            "OU=IT,DC=us,DC=latam,DC=com",
            "OU=Sales,DC=us,DC=latam,DC=com",
            "OU=Marketing,DC=us,DC=latam,DC=com",
            "OU=HR,DC=us,DC=latam,DC=com",
            
            // PE Region
            "OU=IT,DC=pe,DC=latam,DC=com",
            "OU=Sales,DC=pe,DC=latam,DC=com",
            "OU=Marketing,DC=pe,DC=latam,DC=com",
            "OU=HR,DC=pe,DC=latam,DC=com",
            
            // BR Region
            "OU=IT,DC=br,DC=latam,DC=com",
            "OU=Sales,DC=br,DC=latam,DC=com",
            
            // MX Region
            "OU=IT,DC=mx,DC=latam,DC=com",
            "OU=Sales,DC=mx,DC=latam,DC=com",
            
            // CL Region
            "OU=IT,DC=cl,DC=latam,DC=com",
            "OU=Sales,DC=cl,DC=latam,DC=com",
            
            // Built-in containers
            "CN=Users,DC=holding,DC=latam,DC=com",
            "CN=Computers,DC=holding,DC=latam,DC=com",
            "CN=Builtin,DC=holding,DC=latam,DC=com"
        };

        foreach (var ouDN in ous) {
            var ou = new MockLdapEntryAdapter(ouDN);
            ou.AddAttribute("objectClass", new[] { "top", "organizationalUnit" });
            var ouName = ouDN.Split(',')[0].Replace("OU=", "").Replace("CN=", "");
            ou.AddAttribute("ou", ouName);
            ou.AddAttribute("name", ouName);
            ou.AddAttribute("distinguishedName", ouDN);

            _dataStore.AddOrUpdateEntry(ou);
        }
    }

    #endregion

    #region Users

    private void SeedStandardUsers() {
        var users = new List<UserData>
        {
            // IT DevOps - Holding
            new UserData("James", "Dockers", "james.dockers", "HOLDING", "OU=Seniors,OU=DevOps,OU=IT,DC=holding,DC=latam,DC=com", "Senior DevOps Engineer"),
            new UserData("Sara", "Pikes", "sara.pikes", "HOLDING", "OU=Juniors,OU=DevOps,OU=IT,DC=holding,DC=latam,DC=com", "Junior DevOps Engineer"),
            new UserData("Robert", "Miller", "robert.miller", "HOLDING", "OU=Interships,OU=DevOps,OU=IT,DC=holding,DC=latam,DC=com", "Intern"),
            
            // IT Support - US
            new UserData("Maria", "Gonzalez", "maria.gonzalez", "US", "OU=Support,OU=IT,DC=us,DC=latam,DC=com", "Support Lead"),
            new UserData("John", "Smith", "john.smith", "US", "OU=Interships,OU=Support,OU=IT,DC=us,DC=latam,DC=com", "Support Intern"),
            
            // IT Support - PE
            new UserData("Carlos", "Rodriguez", "carlos.rodriguez", "PE", "OU=Support,OU=IT,DC=pe,DC=latam,DC=com", "Support Analyst"),
            new UserData("Ana", "Martinez", "ana.martinez", "PE", "OU=Interships,OU=Support,OU=IT,DC=pe,DC=latam,DC=com", "Support Intern"),
            
            // Regional IT
            new UserData("Paulo", "Silva", "paulo.silva", "BR", "OU=IT,DC=br,DC=latam,DC=com", "IT Administrator"),
            new UserData("Miguel", "Sanchez", "miguel.sanchez", "MX", "OU=IT,DC=mx,DC=latam,DC=com", "IT Administrator"),
            new UserData("Fernando", "Lopez", "fernando.lopez", "CL", "OU=IT,DC=cl,DC=latam,DC=com", "IT Administrator"),
            
            // Additional users for search demos
            new UserData("Isaac", "Newton", "isaac.newton", "HOLDING", "OU=Seniors,OU=DevOps,OU=IT,DC=holding,DC=latam,DC=com", "Principal Engineer"),
            new UserData("Manuel", "Cordoba", "manuel.cordoba", "HOLDING", "OU=Seniors,OU=DevOps,OU=IT,DC=holding,DC=latam,DC=com", "Tech Lead"),
            new UserData("Saint", "Seiya", "saint.seiya", "HOLDING", "OU=Juniors,OU=DevOps,OU=IT,DC=holding,DC=latam,DC=com", "Junior Developer", memberOf: new string[] { "CN=Users,DC=holding,DC=latam,DC=com" }),
            new UserData("Ken", "Master", "ken.master", "HOLDING", "OU=Seniors,OU=DevOps,OU=IT,DC=holding,DC=latam,DC=com", "Security Specialist", memberOf: new string[] { "CN=Administrators,CN=Builtin,DC=holding,DC=latam,DC=com" }),
            
            // Additional users for realistic environment
            new UserData("Alice", "Wonderland", "alice.wonder", "HOLDING", "OU=Support,OU=IT,DC=holding,DC=latam,DC=com", "Support Analyst"),
            new UserData("Bruce", "Wayne", "bruce.wayne", "US", "OU=IT,DC=us,DC=latam,DC=com", "Security Architect"),
            new UserData("Clark", "Kent", "clark.kent", "US", "OU=IT,DC=us,DC=latam,DC=com", "Journalist"),
            new UserData("Diana", "Prince", "diana.prince", "PE", "OU=IT,DC=pe,DC=latam,DC=com", "Security Consultant"),
            new UserData("Barry", "Allen", "barry.allen", "BR", "OU=IT,DC=br,DC=latam,DC=com", "Network Engineer"),
            new UserData("Arthur", "Curry", "arthur.curry", "MX", "OU=IT,DC=mx,DC=latam,DC=com", "Infrastructure Engineer"),
        };

        foreach (var user in users) {
            CreateMockUser(user);
        }
    }

    private void SeedDemoSpecificUsers() {
        // Create user for CreateUserAccount demo
        var newUser = new UserData(
            "Victor",
            "Bastidas",
            "victor.bastidas",
            "HOLDING",
            "OU=Seniors,OU=DevOps,OU=IT,DC=holding,DC=latam,DC=com",
            "Senior DevOps Engineer"
        );
        CreateMockUser(newUser);

        // Create user for password reset demo
        var passwordResetUser = new UserData(
            "Red",
            "Robbin",
            "red.robbin",
            "HOLDING",
            "OU=Interships,OU=DevOps,OU=IT,DC=holding,DC=latam,DC=com",
            "Intern"
        );
        CreateMockUser(passwordResetUser);

        // Create user for disable user demo
        var disableUser = new UserData(
            "Sara",
            "Pikes",
            "sara.pikes",
            "US",
            "OU=Interships,OU=Support,OU=IT,DC=us,DC=latam,DC=com",
            "Support Intern",
            userAccountControl: "514" // Already disabled
        );
        CreateMockUser(disableUser);

        // Create user for remove user demo
        var removeUser = new UserData(
            "Magic",
            "Cuy",
            "magic.cuy",
            "PE",
            "OU=Interships,OU=Support,OU=IT,DC=pe,DC=latam,DC=com",
            "Support Intern"
        );
        CreateMockUser(removeUser);

        // Create user for group membership validation
        var groupMemberUser = new UserData(
            "Ken",
            "Master",
            "ken.master",
            "HOLDING",
            "OU=Seniors,OU=DevOps,OU=IT,DC=holding,DC=latam,DC=com",
            "Security Specialist"
        );
        CreateMockUser(groupMemberUser);
    }

    private void CreateMockUser(UserData user) {
        var distinguishedName = $"CN={user.FirstName} {user.LastName},{user.ContainerDN}";
        var existingEntry = _dataStore.GetEntry(distinguishedName);
        if (existingEntry != null)
            return;

        var userEntry = new MockLdapEntryAdapter(distinguishedName);

        // Get next progressive RID for user (thread-safe)
        var rid = Interlocked.Increment(ref _nextUserRid);
        var sidBytes = GenerateSidBytes(rid);

        userEntry.AddAttribute("objectSid", sidBytes);
        userEntry.AddAttribute("objectGuid", Guid.NewGuid().ToByteArray());
        userEntry.AddAttribute("sAMAccountName", user.SAMAccountName);
        userEntry.AddAttribute("sAMAccountType", "805306368"); // Normal user account
        userEntry.AddAttribute("cn", $"{user.FirstName} {user.LastName}");
        userEntry.AddAttribute("sn", user.LastName);
        userEntry.AddAttribute("givenName", user.FirstName);
        userEntry.AddAttribute("displayName", $"{user.FirstName} {user.LastName}");
        userEntry.AddAttribute("name", $"{user.FirstName} {user.LastName}");
        userEntry.AddAttribute("mail", $"{user.SAMAccountName}@{user.Domain.ToLower()}.latam.com");
        userEntry.AddAttribute("userPrincipalName", $"{user.SAMAccountName}@{user.Domain.ToLower()}.latam.com");
        userEntry.AddAttribute("userAccountControl", user.UserAccountControl);
        userEntry.AddAttribute("objectClass", new[] { "top", "person", "organizationalPerson", "user" });
        userEntry.AddAttribute("whenCreated", DateTime.UtcNow.AddDays(-_random.Next(1, 365)).ToString("yyyyMMddHHmmss.0Z"));
        userEntry.AddAttribute("whenChanged", DateTime.UtcNow.AddDays(-_random.Next(1, 30)).ToString("yyyyMMddHHmmss.0Z"));
        userEntry.AddAttribute("distinguishedName", distinguishedName);
        userEntry.AddAttribute("title", user.Title);
        userEntry.AddAttribute("department", user.Department ?? "IT");
        userEntry.AddAttribute("company", $"{user.Domain} LATAM");
        userEntry.AddAttribute("telephoneNumber", $"+1-555-{_random.Next(100, 999)}-{_random.Next(1000, 9999)}");

        if (!string.IsNullOrEmpty(user.Manager)) {
            userEntry.AddAttribute("manager", user.Manager);
        }

        if (user.MemberOf != null && user.MemberOf.Any()) {
            userEntry.AddAttribute("memberOf", user.MemberOf);
        }

        _dataStore.AddOrUpdateEntry(userEntry);
        _logger.LogDebug($"Created user: {user.SAMAccountName} ({distinguishedName}) with RID: {rid}");
    }

    #endregion

    #region Groups

    private void SeedGroups() {
        var groups = new List<GroupData>
        {
            // Global groups
            new GroupData("Domain Admins", "CN=Users,DC=holding,DC=latam,DC=com", "DomainAdmins", "Domain Administrators Group"),
            new GroupData("Domain Users", "CN=Users,DC=holding,DC=latam,DC=com", "DomainUsers", "All domain users"),
            new GroupData("Domain Computers", "CN=Users,DC=holding,DC=latam,DC=com", "DomainComputers", "All domain computers"),
            
            // IT Department groups
            new GroupData("IT Admins", "OU=IT,DC=holding,DC=latam,DC=com", "ITAdmins", "IT Administrators"),
            new GroupData("DevOps Engineers", "OU=DevOps,OU=IT,DC=holding,DC=latam,DC=com", "DevOpsEng", "DevOps Engineering Team"),
            new GroupData("Senior DevOps", "OU=Seniors,OU=DevOps,OU=IT,DC=holding,DC=latam,DC=com", "SeniorDevOps", "Senior DevOps Engineers"),
            new GroupData("Junior DevOps", "OU=Juniors,OU=DevOps,OU=IT,DC=holding,DC=latam,DC=com", "JuniorDevOps", "Junior DevOps Engineers"),
            new GroupData("Interns", "OU=Interships,OU=DevOps,OU=IT,DC=holding,DC=latam,DC=com", "Interns", "Intern Program Participants"),
            
            // Support groups
            new GroupData("Support Team", "OU=Support,OU=IT,DC=holding,DC=latam,DC=com", "SupportTeam", "Support Department"),
            new GroupData("US Support", "OU=Support,OU=IT,DC=us,DC=latam,DC=com", "USSupport", "US Support Team"),
            new GroupData("PE Support", "OU=Support,OU=IT,DC=pe,DC=latam,DC=com", "PESupport", "Peru Support Team"),
            
            // Regional groups
            new GroupData("US IT Team", "OU=IT,DC=us,DC=latam,DC=com", "USITTeam", "US IT Department"),
            new GroupData("PE IT Team", "OU=IT,DC=pe,DC=latam,DC=com", "PEITTeam", "Peru IT Department"),
            new GroupData("BR IT Team", "OU=IT,DC=br,DC=latam,DC=com", "BRITTeam", "Brazil IT Department"),
            new GroupData("MX IT Team", "OU=IT,DC=mx,DC=latam,DC=com", "MXITTeam", "Mexico IT Department"),
            new GroupData("CL IT Team", "OU=IT,DC=cl,DC=latam,DC=com", "CLITTeam", "Chile IT Department"),
            
            // Security groups
            new GroupData("Administrators", "CN=Builtin,DC=holding,DC=latam,DC=com", "Administrators", "Built-in Administrators Group"),
            new GroupData("Security Analysts", "OU=IT,DC=holding,DC=latam,DC=com", "SecurityAnalysts", "Security Team"),
        };

        foreach (var group in groups) {
            CreateMockGroup(group);
        }
    }

    private void SeedDemoSpecificGroups() {
        // Groups referenced in JSON config
        var demoSpecificGroups = new[]
        {
            new GroupData("Administrators", "CN=Builtin,DC=holding,DC=latam,DC=com", "Administrators", "Built-in Administrators"),
            new GroupData("DevOps Leaders", "OU=Seniors,OU=DevOps,OU=IT,DC=holding,DC=latam,DC=com", "DevOpsLeaders", "DevOps Leadership Team")
        };

        foreach (var group in demoSpecificGroups) {
            CreateMockGroup(group);
        }
    }

    private void CreateMockGroup(GroupData group) {
        var groupDistinguishedName = group.GeneratedDistinguishedName;
        var existingEntry = _dataStore.GetEntry(groupDistinguishedName);
        if (existingEntry != null)
            return;

        var groupEntry = new MockLdapEntryAdapter(groupDistinguishedName);

        // Get next progressive RID for group (thread-safe)
        var rid = Interlocked.Increment(ref _nextGroupRid);
        var sidBytes = GenerateSidBytes(rid);

        groupEntry.AddAttribute("objectSid", sidBytes);
        groupEntry.AddAttribute("objectGuid", Guid.NewGuid().ToByteArray());
        groupEntry.AddAttribute("sAMAccountName", group.SAMAccountName);
        groupEntry.AddAttribute("sAMAccountType", "268435456"); // Security group
        groupEntry.AddAttribute("cn", group.Name);
        groupEntry.AddAttribute("name", group.Name);
        groupEntry.AddAttribute("displayName", group.Name);
        groupEntry.AddAttribute("description", group.Description);
        groupEntry.AddAttribute("objectClass", new[] { "top", "group" });
        groupEntry.AddAttribute("groupType", "-2147483640"); // Universal security group
        groupEntry.AddAttribute("whenCreated", DateTime.UtcNow.AddDays(-_random.Next(1, 365)).ToString("yyyyMMddHHmmss.0Z"));
        groupEntry.AddAttribute("distinguishedName", groupDistinguishedName);

        _dataStore.AddOrUpdateEntry(groupEntry);
        _logger.LogDebug($"Created group: {group.SAMAccountName} ({groupDistinguishedName}) with RID: {rid}");
    }

    #endregion

    #region Computers

    private void SeedComputerEntries() {
        var computers = new[]
        {
            // Servers
            new ComputerData("DEVSERVER01", "OU=DevOps,OU=IT,DC=holding,DC=latam,DC=com", "Development Server", "server"),
            new ComputerData("DBSERVER01", "OU=IT,DC=holding,DC=latam,DC=com", "Database Server", "server"),
            new ComputerData("WEBSERVER01", "OU=IT,DC=us,DC=latam,DC=com", "Web Server US", "server"),
            new ComputerData("APPSERVER01", "OU=IT,DC=pe,DC=latam,DC=com", "Application Server PE", "server"),
            new ComputerData("FILESERVER01", "OU=IT,DC=br,DC=latam,DC=com", "File Server BR", "server"),
            new ComputerData("MAILSERVER01", "OU=IT,DC=mx,DC=latam,DC=com", "Mail Server MX", "server"),
            new ComputerData("MONITOR01", "OU=IT,DC=cl,DC=latam,DC=com", "Monitoring Server", "server"),
            
            // Workstations
            new ComputerData("WS-USA-001", "CN=Computers,DC=us,DC=latam,DC=com", "USA Workstation 001", "workstation"),
            new ComputerData("WS-PE-001", "CN=Computers,DC=pe,DC=latam,DC=com", "Peru Workstation 001", "workstation"),
            new ComputerData("WS-BR-001", "CN=Computers,DC=br,DC=latam,DC=com", "Brazil Workstation 001", "workstation"),
            new ComputerData("DEV-LAPTOP-001", "OU=DevOps,OU=IT,DC=holding,DC=latam,DC=com", "Developer Laptop", "workstation"),
        };

        foreach (var computer in computers) {
            CreateMockComputer(computer);
        }
    }

    private void CreateMockComputer(ComputerData computer) {
        var distinguishedName = $"CN={computer.Name},{computer.ContainerDN}";
        var existingEntry = _dataStore.GetEntry(distinguishedName);
        if (existingEntry != null)
            return;

        var computerEntry = new MockLdapEntryAdapter(distinguishedName);

        // Get next progressive RID for computer (thread-safe)
        var rid = Interlocked.Increment(ref _nextComputerRid);
        var sidBytes = GenerateSidBytes(rid);

        computerEntry.AddAttribute("objectSid", sidBytes);
        computerEntry.AddAttribute("objectGuid", Guid.NewGuid().ToByteArray());
        computerEntry.AddAttribute("sAMAccountName", $"{computer.Name}$");
        computerEntry.AddAttribute("sAMAccountType", "805306369"); // Computer account
        computerEntry.AddAttribute("cn", computer.Name);
        computerEntry.AddAttribute("name", computer.Name);
        computerEntry.AddAttribute("displayName", computer.Description);
        computerEntry.AddAttribute("description", computer.Description);
        computerEntry.AddAttribute("objectClass", new[] { "top", "person", "organizationalPerson", "user", "computer" });
        computerEntry.AddAttribute("userAccountControl", "4096"); // Workstation/server account
        computerEntry.AddAttribute("operatingSystem", computer.Type == "server" ? "Windows Server 2022" : "Windows 11 Pro");
        computerEntry.AddAttribute("operatingSystemVersion", "10.0 (20348)");
        computerEntry.AddAttribute("dNSHostName", $"{computer.Name}.latam.com");
        computerEntry.AddAttribute("whenCreated", DateTime.UtcNow.AddDays(-_random.Next(1, 365)).ToString("yyyyMMddHHmmss.0Z"));
        computerEntry.AddAttribute("distinguishedName", distinguishedName);

        _dataStore.AddOrUpdateEntry(computerEntry);
        _logger.LogDebug($"Created computer: {computer.Name} ({distinguishedName}) with RID: {rid}");
    }

    #endregion

    #region Group Memberships

    private void SeedGroupMemberships() {
        // MemberOf mappings: User -> Groups
        var memberships = new Dictionary<string, List<string>>
        {
            // James Dockers - Senior DevOps
            { "james.dockers", new List<string> { "DomainAdmins", "ITAdmins", "DevOpsEng", "SeniorDevOps", "DevOpsLeaders" } },
            
            // Sara Pikes - Junior DevOps
            { "sara.pikes", new List<string> { "DomainUsers", "DevOpsEng", "JuniorDevOps" } },
            
            // Robert Miller - Intern
            { "robert.miller", new List<string> { "DomainUsers", "Interns" } },
            
            // Maria Gonzalez - Support Lead
            { "maria.gonzalez", new List<string> { "DomainAdmins", "ITAdmins", "SupportTeam", "USSupport" } },
            
            // John Smith - Support Intern
            { "john.smith", new List<string> { "DomainUsers", "Interns", "USSupport" } },
            
            // Carlos Rodriguez - Support Analyst
            { "carlos.rodriguez", new List<string> { "DomainUsers", "SupportTeam", "PESupport" } },
            
            // Ana Martinez - Support Intern
            { "ana.martinez", new List<string> { "DomainUsers", "Interns", "PESupport" } },
            
            // Paulo Silva - BR IT
            { "paulo.silva", new List<string> { "DomainAdmins", "ITAdmins", "BRITTeam" } },
            
            // Miguel Sanchez - MX IT
            { "miguel.sanchez", new List<string> { "DomainAdmins", "ITAdmins", "MXITTeam" } },
            
            // Fernando Lopez - CL IT
            { "fernando.lopez", new List<string> { "DomainAdmins", "ITAdmins", "CLITTeam" } },
            
            // Isaac Newton - Principal Engineer
            { "isaac.newton", new List<string> { "DomainAdmins", "ITAdmins", "DevOpsEng", "SeniorDevOps" } },
            
            // Manuel Cordoba - Tech Lead
            { "manuel.cordoba", new List<string> { "DomainAdmins", "ITAdmins", "DevOpsEng", "SeniorDevOps", "DevOpsLeaders" } },
            
            // Saint Seiya - Junior Developer
            { "saint.seiya", new List<string> { "DomainUsers", "DevOpsEng", "JuniorDevOps" } },
            
            // Ken Master - Security Specialist
            { "ken.master", new List<string> { "DomainAdmins", "ITAdmins", "SecurityAnalysts", "Administrators", "SeniorDevOps" } },
            
            // Victor Bastidas - New user (will be added after creation)
            { "victor.bastidas", new List<string> { "DomainUsers", "DevOpsEng", "SeniorDevOps" } },
            
            // Red Robbin - Intern
            { "red.robbin", new List<string> { "DomainUsers", "Interns" } },
            
            // Magic Cuy - Intern
            { "magic.cuy", new List<string> { "DomainUsers", "Interns", "PESupport" } },
            
            // Additional users
            { "alice.wonder", new List<string> { "DomainUsers", "SupportTeam" } },
            { "bruce.wayne", new List<string> { "DomainAdmins", "ITAdmins", "SecurityAnalysts", "Administrators" } },
            { "clark.kent", new List<string> { "DomainUsers", "USITTeam" } },
            { "diana.prince", new List<string> { "DomainAdmins", "SecurityAnalysts" } },
            { "barry.allen", new List<string> { "DomainAdmins", "BRITTeam" } },
            { "arthur.curry", new List<string> { "DomainUsers", "MXITTeam" } },
        };

        foreach (var membership in memberships) {
            var user = FindUserBySAMAccountName(membership.Key);
            if (user != null) {
                var groupDNs = new List<string>();
                foreach (var groupName in membership.Value) {
                    var group = FindGroupBySAMAccountName(groupName);
                    if (group != null) {
                        groupDNs.Add(group.DistinguishedName);
                    }
                }

                if (groupDNs.Any()) {
                    var currentMemberOf = user.GetAttributeSet().GetAttribute("memberOf")?.StringValueArray ?? new string[0];
                    var newMemberOf = currentMemberOf.Union(groupDNs).ToArray();
                    user.AddAttribute("memberOf", newMemberOf);
                    _dataStore.AddOrUpdateEntry(user);
                }
            }
        }
    }

    private void SeedAdditionalRelationships() {
        // Add manager relationships
        var managers = new Dictionary<string, string>
        {
            { "james.dockers", "CN=Isaac Newton,OU=Seniors,OU=DevOps,OU=IT,DC=holding,DC=latam,DC=com" },
            { "sara.pikes", "CN=James Dockers,OU=Seniors,OU=DevOps,OU=IT,DC=holding,DC=latam,DC=com" },
            { "robert.miller", "CN=James Dockers,OU=Seniors,OU=DevOps,OU=IT,DC=holding,DC=latam,DC=com" },
            { "maria.gonzalez", "CN=Bruce Wayne,OU=IT,DC=us,DC=latam,DC=com" },
            { "carlos.rodriguez", "CN=Diana Prince,OU=IT,DC=pe,DC=latam,DC=com" },
            { "saint.seiya", "CN=Manuel Cordoba,OU=Seniors,OU=DevOps,OU=IT,DC=holding,DC=latam,DC=com" },
            { "ken.master", "CN=Isaac Newton,OU=Seniors,OU=DevOps,OU=IT,DC=holding,DC=latam,DC=com" },
        };

        foreach (var relation in managers) {
            var user = FindUserBySAMAccountName(relation.Key);
            if (user != null) {
                user.AddAttribute("manager", relation.Value);
                _dataStore.AddOrUpdateEntry(user);
            }
        }

        // Add nested group memberships (groups within groups)
        var nestedMemberships = new Dictionary<string, List<string>>
        {
            { "DomainAdmins", new List<string> { "Administrators" } },
            { "ITAdmins", new List<string> { "DomainAdmins" } },
            { "DevOpsEng", new List<string> { "ITAdmins" } },
            { "SeniorDevOps", new List<string> { "DevOpsEng" } },
            { "JuniorDevOps", new List<string> { "DevOpsEng" } },
            { "SecurityAnalysts", new List<string> { "ITAdmins" } },
            { "USSupport", new List<string> { "SupportTeam" } },
            { "PESupport", new List<string> { "SupportTeam" } },
        };

        foreach (var nested in nestedMemberships) {
            var group = FindGroupBySAMAccountName(nested.Key);
            if (group != null) {
                var parentDNs = new List<string>();
                foreach (var parentName in nested.Value) {
                    var parentGroup = FindGroupBySAMAccountName(parentName);
                    if (parentGroup != null) {
                        parentDNs.Add(parentGroup.DistinguishedName);
                    }
                }

                if (parentDNs.Any()) {
                    var currentMemberOf = group.GetAttributeSet().GetAttribute("memberOf")?.StringValueArray ?? new string[0];
                    var newMemberOf = currentMemberOf.Union(parentDNs).ToArray();
                    group.AddAttribute("memberOf", newMemberOf);
                    _dataStore.AddOrUpdateEntry(group);
                }
            }
        }
    }

    #endregion

    #region Helper Methods

    private byte[] GenerateSidBytes(int rid) {
        // S-1-5-21-domain-rid format
        var sidBytes = new byte[28];
        sidBytes[0] = 1; // Revision
        sidBytes[1] = 5; // Subauthority count

        // Identifier authority (NT Authority)
        sidBytes[2] = 0;
        sidBytes[3] = 0;
        sidBytes[4] = 0;
        sidBytes[5] = 0;
        sidBytes[6] = 0;
        sidBytes[7] = 5;

        // Domain identifier (48-bit random value)
        var domainId = _random.NextInt64(1000000, 9999999);
        var domainBytes = BitConverter.GetBytes(domainId);
        Array.Copy(domainBytes, 0, sidBytes, 8, Math.Min(domainBytes.Length, 16));

        // RID (progressive value passed as parameter)
        var ridBytes = BitConverter.GetBytes(rid);
        Array.Copy(ridBytes, 0, sidBytes, 24, 4);

        return sidBytes;
    }

    private MockLdapEntryAdapter FindUserBySAMAccountName(string samAccountName) {
        var allEntries = _dataStore.GetAllEntries();
        return allEntries.FirstOrDefault(e =>
            e.GetAttributeSet().GetAttribute("sAMAccountName")?.StringValue == samAccountName);
    }

    private MockLdapEntryAdapter FindGroupBySAMAccountName(string samAccountName) {
        var allEntries = _dataStore.GetAllEntries();
        return allEntries.FirstOrDefault(e =>
            e.GetAttributeSet().GetAttribute("objectClass")?.StringValueArray?.Contains("group") == true &&
            e.GetAttributeSet().GetAttribute("sAMAccountName")?.StringValue == samAccountName);
    }

    private int CountEntriesByObjectClass(string objectClass) {
        var allEntries = _dataStore.GetAllEntries();
        return allEntries.Count(e =>
            e.GetAttributeSet().GetAttribute("objectClass")?.StringValueArray?.Contains(objectClass) == true);
    }

    #endregion

    #region Helper Classes

    private class UserData
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string SAMAccountName { get; set; }
        public string Domain { get; set; }
        public string ContainerDN { get; set; }
        public string Title { get; set; }
        public string Department { get; set; }
        public string UserAccountControl { get; set; }
        public string Manager { get; set; }
        public string[] MemberOf { get; set; }

        public UserData(string firstName, string lastName, string samAccountName,
            string domain, string containerDN, string title, string department = "IT",
            string userAccountControl = "512", string manager = null, string[] memberOf = null) {
            FirstName = firstName;
            LastName = lastName;
            SAMAccountName = samAccountName;
            Domain = domain;
            ContainerDN = containerDN;
            Title = title;
            Department = department;
            UserAccountControl = userAccountControl;
            Manager = manager;
            MemberOf = memberOf;
        }
    }

    private class GroupData
    {
        public string Name { get; set; }
        public string ContainerDistinguishedName { get; set; }
        public string SAMAccountName { get; set; }
        public string Description { get; set; }
        public string GeneratedDistinguishedName { get;  private set; }

        public GroupData(string name, string distinguishedName, string samAccountName, string description) {
            Name = name;
            ContainerDistinguishedName = distinguishedName;
            SAMAccountName = samAccountName;
            Description = description;

            GeneratedDistinguishedName = $"CN={name},{distinguishedName}";
        }
    }

    private class ComputerData
    {
        public string Name { get; set; }
        public string ContainerDN { get; set; }
        public string Description { get; set; }
        public string Type { get; set; } // "server" or "workstation"

        public ComputerData(string name, string containerDN, string description, string type) {
            Name = name;
            ContainerDN = containerDN;
            Description = description;
            Type = type;
        }
    }
 
    #endregion
}
