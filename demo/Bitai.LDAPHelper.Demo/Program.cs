using System;
using System.Threading.Tasks;
using Serilog;
using System.Linq;
using System.Collections.Generic;
using Bitai.LDAPHelper.DTO;
using Microsoft.Extensions.Configuration;
using Bitai.LDAPHelper.LdapAdapters.Novell;
using Bitai.LDAPHelper.Tests.Mocks.LdapAdapters;
using Bitai.LDAPHelper.Tests.Mocks.LdapData;
using Microsoft.Extensions.Logging;

namespace Bitai.LDAPHelper.Demo;

public partial class Program
{
    internal static string DemoSetup_FilePath = $"{Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory).Replace('\\', '/')}/ldaphelper_demosetup.json";
    internal const string Message_LdapEntriesNotFound = "LDAP entries not found with the provided filters.";
    internal static DemoSetup DemoSetup;
    internal static string RequestLabel = "My Demo";
    internal static string Selected_LdapServer;
    internal static int Selected_LdapServerPort;
    internal static bool Selected_UseSsl = false;
    internal static short Selected_ConnectionTimeout = 15;
    internal static string Selected_DomainAccountName;
    internal static string Selected_DomainAccountPassword;
    internal static string Selected_BaseDN;

    private static DemoContext _context;
    private static DemoSummary _summary;




    private static ImplementationType SelectImplementation(string[] args) {
        // Check command-line arguments
        if (args.Contains("--implementation") || args.Contains("-i")) {
            var index = Array.IndexOf(args, args.Contains("--implementation") ? "--implementation" : "-i");
            if (index + 1 < args.Length) {
                var impl = args[index + 1].ToLower();
                if (impl == "novell") return ImplementationType.Novell;
                if (impl == "mock") return ImplementationType.Mock;
            }
        }

        // Prompt user
        Console.WriteLine();
        Console.WriteLine("=== LDAP Helper Demo - Select Implementation ===");
        Console.WriteLine("1. Novell (Real LDAP Connection) - Requires actual LDAP server");
        Console.WriteLine("2. Mock (Simulated LDAP) - No server required, uses mock data");
        Console.WriteLine();

        while (true) {
            Console.Write("Enter your choice (1 or 2): ");
            var input = Console.ReadLine();
            if (input == "1") return ImplementationType.Novell;
            if (input == "2") return ImplementationType.Mock;
            Console.WriteLine("Invalid choice. Please enter 1 or 2.");
        }
    }

    private static async Task SetupNovellConnection() {
        requestLdapServer();
        requestLdapServerPort();
        requestBaseDN();
        requestPasswordToConnectLDAPServer();

        // Set context properties
        _context.SelectedLdapServer = Selected_LdapServer;
        _context.SelectedLdapServerPort = Selected_LdapServerPort;
        _context.SelectedUseSsl = Selected_UseSsl;
        _context.SelectedConnectionTimeout = Selected_ConnectionTimeout;
        _context.SelectedDomainAccountName = Selected_DomainAccountName;
        _context.SelectedDomainAccountPassword = Selected_DomainAccountPassword;
        _context.SelectedBaseDN = Selected_BaseDN;

        // Create Novell connection factory
        _context.ConnectionFactory = new NovellLdapConnectionFactoryAdapter();

        // Test connection
        Log.Information("Testing LDAP connection...");
        try {
            using (var connection = await _context.ConnectionFactory.CreateConnectionAsync(
                _context.GetConnectionInfo(),
                _context.GetDomainAccountCredential().DomainAccountName,
                _context.GetDomainAccountCredential().DomainAccountPassword)) {
                Log.Information("✅ Connection successful!");
            }
        }
        catch (Exception ex) {
            Log.Error($"❌ Connection failed: {ex.Message}");
            throw;
        }
    }

    public static async Task Main(string[] args)
    {
        try
        {
            configDemoLogger();
            
            // Select implementation
            var implementation = SelectImplementation(args);
            
            // Load configuration
            loadDemoSetup();
            
            // Initialize context
            _context = new DemoContext
            {
                Implementation = implementation,
                Configuration = DemoSetup,
                RequestLabel = "My Demo"
            };
            
            // Setup connection factory based on implementation
            if (implementation == ImplementationType.Mock)
            {
                Log.Information("Initializing Mock Implementation...");
                _context.ConnectionFactory = new MockLdapPersistentConnectionFactoryAdapter();

                // Create logger for MockLdapDataSeeder
                using var loggerFactory = LoggerFactory.Create(builder => {
                    builder.ClearProviders();
                    builder.AddSerilog(Log.Logger);
                });
                var logger = loggerFactory.CreateLogger<MockLdapDataSeeder>();

                // Seed mock data
                var seeder = new MockLdapDataSeeder(logger);
                seeder.SeedAllData();

                seeder.PrintAllData();

                // Set mock connection info (dummy values)
                _context.SelectedLdapServer = "mock-server";
                _context.SelectedLdapServerPort = 389;
                _context.SelectedUseSsl = false;
                _context.SelectedConnectionTimeout = DemoSetup.ConnectionTimeout;
                _context.SelectedDomainAccountName = DemoSetup.DomainUserAccountForRunTests;
                _context.SelectedDomainAccountPassword = "mock-password";
                _context.SelectedBaseDN = DemoSetup.BaseDNs[0].DN;
            }
            else
            {
                Log.Information("Initializing Novell (Real LDAP) Implementation...");
                await SetupNovellConnection();
            }
            
            // Initialize summary
            _summary = new DemoSummary();
            
            // Run demos
            await RunDemos();
            
            // Print summary
            _summary.PrintSummary();
            
            // Option to run again
            await AskToRunAgain();
        }
        catch (Exception ex)
        {
            Log.Error("***************************************************");
            Log.Error("Error in DEMO! See details below.");
            Log.Error(ex.Message);
            Log.Error(ex.StackTrace);
            Log.Error("***************************************************");
        }
        finally
        {
            Console.WriteLine();
            Log.Warning("Demo completed. Press Enter to exit.");
            Console.ReadLine();
        }
    }

    private static void loadDemoSetup() {
        Program.DemoSetup = new ConfigurationBuilder()
            .AddJsonFile(Program.DemoSetup_FilePath, false, false)
            .Build()
            .Get<DemoSetup>();

        Program.Selected_LdapServer = Program.DemoSetup.LdapServers[0].Address;
        Program.Selected_LdapServerPort = (int)LdapServerDefaultPorts.DefaultPort;
        Program.Selected_ConnectionTimeout = Program.DemoSetup.ConnectionTimeout;
        Program.Selected_DomainAccountName = Program.DemoSetup.DomainUserAccountForRunTests;
        Program.Selected_BaseDN = Program.DemoSetup.BaseDNs[0].DN;
    }

    private static void configDemoLogger() {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateLogger();
    }

    private static void printDemoTitle(string title) {
        Console.WriteLine();
        Console.WriteLine("**********************************************");
        Console.WriteLine("  " + title);
        Console.WriteLine("**********************************************");
    }

    private static void requestLdapServer() {
        Console.WriteLine();
        Console.WriteLine("SELECT OR ENTER LDAP SERVER DNS OR IP:");
        var _position = 0;
        foreach (var _s in Program.DemoSetup.LdapServers) {
            _position++;
            Console.WriteLine($"({_position}) {_s.Address}");
        }
        Console.WriteLine($"Leave empty to use {Program.Selected_LdapServer}");
        //Wait for answer...
        var _ldapServerEntered = Console.ReadLine();
        Program.Selected_LdapServer = string.IsNullOrEmpty(_ldapServerEntered) ?
            Program.Selected_LdapServer : (Program.DemoSetup.LdapServers[Convert.ToInt32(_ldapServerEntered) - 1].Address);
        Log.Information($"Will use {Program.Selected_LdapServer} LDAP Server.");
    }

    private static void requestLdapServerPort() {
        Console.WriteLine();
        Console.WriteLine("SELECT OR ENTER LDAP SERVER PORT:");
        Console.WriteLine($"(1) Default -> {(int)LDAPHelper.LdapServerDefaultPorts.DefaultPort}");
        Console.WriteLine($"(2) Default Global Catalog -> {(int)LDAPHelper.LdapServerDefaultPorts.DefaultGlobalCatalogPort}");
        Console.WriteLine($"(3) Default SSL -> {(int)LDAPHelper.LdapServerDefaultPorts.DefaultSslPort}");
        Console.WriteLine($"(4) Default SSL Global Catalog -> {(int)LDAPHelper.LdapServerDefaultPorts.DefaultGlobalCatalogSslPort}");

SELECT_AGAIN:

        Console.WriteLine($"Leave empty to use {Program.Selected_LdapServerPort}");
        var _portEntered = Console.ReadLine();
        if (!string.IsNullOrEmpty(_portEntered)) {
            var _portNumber = Convert.ToInt32(_portEntered);
            if (_portNumber <= 3) {
                switch (_portNumber) {
                    case 1:
                        Program.Selected_LdapServerPort = (int)LDAPHelper.LdapServerDefaultPorts.DefaultPort;
                        Program.Selected_UseSsl = false;
                        break;
                    case 2:
                        Program.Selected_LdapServerPort = (int)LDAPHelper.LdapServerDefaultPorts.DefaultGlobalCatalogPort;
                        Program.Selected_UseSsl = false;
                        break;
                    case 3:
                        Program.Selected_LdapServerPort = (int)LDAPHelper.LdapServerDefaultPorts.DefaultSslPort;
                        Program.Selected_UseSsl = true;
                        break;
                    case 4:
                        Program.Selected_LdapServerPort = (int)LDAPHelper.LdapServerDefaultPorts.DefaultGlobalCatalogSslPort;
                        Program.Selected_UseSsl = true;
                        break;
                    default:
                        goto SELECT_AGAIN;
                }
            }
            else {
                Program.Selected_LdapServerPort = _portNumber;
                Program.Selected_UseSsl = requestYESorNO($"Port {_portNumber} use SSL?");
            }
        }
        Log.Information($"Will use port:{Program.Selected_LdapServerPort} to connect LDAP Server.");
    }

    private static string requestAccountPassword(string domainAccountName) {
REQUEST_PASSWORD:

        Console.WriteLine($"ENTER PASSWORD FOR {domainAccountName}:");

        var password = readPassword('*');

        if (string.IsNullOrEmpty(password) || string.IsNullOrWhiteSpace(password)) goto REQUEST_PASSWORD;

        Log.Information($"Password entered.");

        return password;
    }

    private static void requestPasswordToConnectLDAPServer() {
        Console.WriteLine();

        Log.Information($"To connect to the LDAP server {Program.Selected_LdapServer} it is necessary to enter the password of the {Program.Selected_DomainAccountName}.");

        Program.Selected_DomainAccountPassword = requestAccountPassword(Program.Selected_DomainAccountName);
    }

    private static void requestBaseDN() {
        Console.WriteLine();
        Console.WriteLine("SELECT OR ENTER BASE DN:");
        var _position = 0;
        foreach (var _dn in Program.DemoSetup.BaseDNs) {
            _position++;
            Console.WriteLine($"({_position}) {_dn.DN}");
        }
        Console.WriteLine($"Leave empty to use {Program.Selected_BaseDN}");
        //Wait for answer...
        var _baseDNEntered = Console.ReadLine();
        Program.Selected_BaseDN = string.IsNullOrEmpty(_baseDNEntered) ?
            Program.Selected_BaseDN : (Program.DemoSetup.BaseDNs[Convert.ToInt32(_baseDNEntered) - 1].DN);
        Log.Information($"Will use {Program.Selected_BaseDN} LDAP Server.");
    }

    public static string readPassword(char passwordChar) {
        const int _ENTER = 13, _BACKSP = 8, _CTRLBACKSP = 127;
        int[] _FILTERED = { 0, 27, 9, 10 /*, 32 space, if you care */ }; // const

        var _pass = new Stack<char>();
        char _chr = (char)0;

        while ((_chr = Console.ReadKey(true).KeyChar) != _ENTER) {
            if (_chr == _BACKSP) {
                if (_pass.Count > 0) {
                    System.Console.Write("\b \b");
                    _pass.Pop();
                }
            }
            else if (_chr == _CTRLBACKSP) {
                while (_pass.Count > 0) {
                    System.Console.Write("\b \b");
                    _pass.Pop();
                }
            }
            else if (_FILTERED.Count(x => _chr == x) > 0) {
                //Nothing to do
            }
            else {
                _pass.Push((char)_chr);
                System.Console.Write(passwordChar);
            }
        }

        System.Console.WriteLine();

        return new string(_pass.Reverse().ToArray());
    }

    private static bool requestYESorNO(string question) {
        Console.WriteLine();
        Console.WriteLine(question);
YESORNO:
        Console.WriteLine("Waiting for your answer -> Yes = Y or No = N");
        //Wait for answer...
        var _yesOrNo = Console.ReadLine();
        if (_yesOrNo.Equals("y", StringComparison.OrdinalIgnoreCase)) {
            return true;
        }
        if (_yesOrNo.Equals("n", StringComparison.OrdinalIgnoreCase)) {
            return false;
        }
        else {
            Console.WriteLine("Please, answer Yes or No");
            goto YESORNO;
        }
    }

    private static void printMemberOf(DTO.LDAPEntry entry, int tabCount = 0) {
        if (entry.memberOfEntries == null)
            return;

        tabCount++;

        Log.Information($"{new string('\t', tabCount)}MemberOf entries:");

        foreach (var parentEntry in entry.memberOfEntries) {
            Log.Information($"{new string('\t', tabCount)}{parentEntry.distinguishedName}");

            printMemberOf(parentEntry, tabCount);
        }
    }

    private static async Task RunDemos()
    {
        #region Create User account
        if (DemoSetup.Demo_AccountManager_CreateUserAccount_RunTest)
        {
            await RunDemoWithErrorHandling("Create User Account", async () =>
            {
                string[] memberOf = null;
                if (!string.IsNullOrEmpty(DemoSetup.Demo_AccountManager_CreateUserAccount_MemberOf))
                {
                    memberOf = DemoSetup.Demo_AccountManager_CreateUserAccount_MemberOf.Split(',');
                }

                string[] objectClasses = null;
                if (!string.IsNullOrEmpty(DemoSetup.Demo_AccountManager_CreateUserAccount_ObjectClasses))
                {
                    objectClasses = DemoSetup.Demo_AccountManager_CreateUserAccount_ObjectClasses.Split(',');
                }

                await Demo_AccountManager_CreateUserAccount(
                    _context,
                    DemoSetup.Demo_AccountManager_CreateUserAccount_UserAccountName,
                    DemoSetup.Demo_AccountManager_CreateUserAccount_Password,
                    DemoSetup.Demo_AccountManager_CreateUserAccount_ContainerDN,
                    DemoSetup.Demo_AccountManager_CreateUserAccount_Name,
                    DemoSetup.Demo_AccountManager_CreateUserAccount_Surname,
                    DemoSetup.Demo_AccountManager_CreateUserAccount_DNSDomainName,
                    memberOf,
                    objectClasses,
                    DemoSetup.Demo_AccountManager_CreateUserAccount_UserAccountControlFlags);
            });
        }
        #endregion

        #region Password assignment
        if (DemoSetup.Demo_AccountManager_SetAccountPassword_RunTest)
        {
            await RunDemoWithErrorHandling("Set Account Password", async () =>
            {
                await Demo_AccountManager_SetPassword(_context, DemoSetup.Demo_AccountManager_SetAccountPassword_DistinguishedName);
            });
        }
        #endregion

        #region Authentication
        if (DemoSetup.Demo_Authenticator_Authenticate_RunTest)
        {
            await RunDemoWithErrorHandling("Authentication", async () =>
            {
                if (DemoSetup.Demo_Authenticator_Authenticate_RunTest_Simple)
                    await Demo_Authenticator_Authenticate_Simple(_context, DemoSetup.Demo_Authenticator_Authenticate_DomainAccountName);
                else
                    await Demo_Authenticator_Authenticate_WithAccountValidation(_context, DemoSetup.Demo_Authenticator_Authenticate_DomainAccountName);
            });
        }
        #endregion

        #region Disable user account
        if (DemoSetup.Demo_AccountManager_DisableUserAccount_RunTest)
        {
            await RunDemoWithErrorHandling("Disable User Account", async () =>
            {
                await Demo_AccountManager_DisableUserAccount(_context, DemoSetup.Demo_AccountManager_DisableUserAccount_UserAccountDistinguishedName);
            });
        }
        #endregion

        #region Remove user account
        if (DemoSetup.Demo_AccountManager_RemoveUserAccount_RunTest)
        {
            await RunDemoWithErrorHandling("Remove User Account", async () =>
            {
                await Demo_AccountManager_RemoveUserAccount(_context, DemoSetup.Demo_AccountManager_RemoveUserAccount_UserAccountDistinguishedName);
            });
        }
        #endregion

        #region Search Users
        if (DemoSetup.Demo_Searcher_SearchUsers_RunTest)
        {
            await RunDemoWithErrorHandling("Search Users by SAMAccountName", async () =>
            {
                await Demo_Searcher_SearchUsers(_context, EntryAttribute.sAMAccountName, DemoSetup.Demo_Searcher_SearchUsers_Filter_sAMAccountName, RequiredEntryAttributes.All);
            });
            
            await RunDemoWithErrorHandling("Search Users by CN", async () =>
            {
                await Demo_Searcher_SearchUsers(_context, EntryAttribute.cn, DemoSetup.Demo_Searcher_SearchUsers_Filter_cn, RequiredEntryAttributes.All);
            });
        }
        #endregion

        #region Search Users by 2 filters
        if (DemoSetup.Demo_Searcher_SearchUsers_RunTest)
        {
            await RunDemoWithErrorHandling("Search Users by Two Filters", async () =>
            {
                await Demo_Searcher_SearchUsersByTwoFilters(_context,
                    EntryAttribute.sAMAccountName, DemoSetup.Demo_Searcher_SearchUsersByTwoFilters_Filter1_sAMAccountName,
                    EntryAttribute.cn, DemoSetup.Demo_Searcher_SearchUsersByTwoFilters_Filter2_cn,
                    false, RequiredEntryAttributes.All);
            });
        }
        #endregion

        #region Search any kind of entries 
        if (DemoSetup.Demo_Searcher_SearchEntries_RunTest)
        {
            await RunDemoWithErrorHandling("Search Entries by CN", async () =>
            {
                await Demo_Searcher_SearchEntries(_context, EntryAttribute.cn, DemoSetup.Demo_Searcher_SearchEntries_Filter_cn, false, RequiredEntryAttributes.All);
            });
            
            await RunDemoWithErrorHandling("Search Entries by ObjectSid", async () =>
            {
                await Demo_Searcher_SearchEntries(_context, EntryAttribute.objectSid, DemoSetup.Demo_Searcher_SearchEntries_Filter_objectSid, false, RequiredEntryAttributes.All);
            });
        }
        #endregion

        #region Search any kind of entries by 2 filters
        if (DemoSetup.Demo_Searcher_SearchEntries_RunTest)
        {
            await RunDemoWithErrorHandling("Search Entries by Two Filters", async () =>
            {
                await Demo_Searcher_SearchEntriesByTwoFilters(_context,
                    EntryAttribute.cn, DemoSetup.Demo_Searcher_SearchEntriesByTwoFilters_Filter1_cn,
                    EntryAttribute.cn, DemoSetup.Demo_Searcher_SearchEntriesByTwoFilters_Filter2_cn,
                    false, RequiredEntryAttributes.All);
            });
        }
        #endregion

        #region Search parent entries of an entry
        if (DemoSetup.Demo_Searcher_SearchParentEntries_RunTest)
        {
            await RunDemoWithErrorHandling("Search Parent Entries", async () =>
            {
                await Demo_Searcher_SearchParentEntries(_context, EntryAttribute.sAMAccountName, DemoSetup.Demo_Searcher_SearchParentEntries_Filter_sAMAccountName, RequiredEntryAttributes.All);
            });
        }
        #endregion

        #region Check group membership 
        if (DemoSetup.Demo_GroupMembershipValidator_RunTest)
        {
            await RunDemoWithErrorHandling("Group Membership Validation", async () =>
            {
                await Demo_GroupMembershipValidator_CheckGroupMembership(_context,
                    DemoSetup.Demo_GroupMembershipValidator_CheckGroupmembership_sAMAccountName,
                    DemoSetup.Demo_GroupMembershipValidator_CheckGroupmembership_Check_GroupName);
            });
        }
        #endregion
    }

    private static async Task RunDemoWithErrorHandling(string demoName, Func<Task> demoAction)
    {
        try
        {
            await demoAction();
            _summary.RecordDemoResult(demoName, true);
        }
        catch (Exception ex)
        {
            Log.Error($"Demo '{demoName}' failed: {ex.Message}");
            _summary.RecordDemoResult(demoName, false, ex);
        }
    }

    private static async Task AskToRunAgain()
    {
        Console.WriteLine();
        Console.Write("Do you want to run the demo again? (Y/N): ");
        var response = Console.ReadLine();
        if (response?.Trim().Equals("Y", StringComparison.OrdinalIgnoreCase) == true)
        {
            // Clear and run again
            Console.Clear();
            await Main(new string[0]);
        }
    }
}
