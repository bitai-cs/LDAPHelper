using System;
using System.Threading.Tasks;
using Serilog;
using Serilog.Sinks.SystemConsole;
using System.Linq;
using System.Security;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using LDAPHelper.Extensions;

namespace LDAPHelper.Demo
{
    /// <summary>
    /// Demo Program
    /// </summary>
    public partial class Program
    {
        internal const string DemoSetup_FilePath = "C:\\LDAPHelperLib_DemoSetup.json";


        /// <summary>
        /// DemoSetup object
        /// </summary>
        internal static DemoSetup DemoSetup;
        /// <summary>
        /// Custom (optional) tag value to label request. Can be null. 
        /// </summary>
        internal static string Tag = "My Demo";
        /// <summary>
        /// LDAP Server ip or DNS name
        /// </summary>
        internal static string Selected_LdapServer;
        /// <summary>
        /// LDAP Server port 
        /// </summary>
        internal static int Selected_LdapServerPort;
        /// <summary>
        /// connection to LDAP Server will use SSL encryption
        /// </summary>
        internal static bool Selected_UseSsl = false;
        /// <summary>
        /// Max time in seconds to connect a LDAP Server 		
        /// </summary>
        internal static short Selected_ConnectionTimeout = 15;
        /// <summary>
        /// Domain Username to connect LDAP Server
        /// </summary>
        internal static string Selected_DomainAccountName;
        /// <summary>
        /// Domain Username passsword
        /// </summary>
        internal static string Selected_AccountNamePassword;
        /// <summary>
        /// Selected BaseDN for DEMO
        /// </summary>
        internal static string Selected_BaseDN;


        #region Private Static Methods
        private static void loadDemoSetup()
        {
            var _cb = new ConfigurationBuilder()
                .AddJsonFile(Program.DemoSetup_FilePath, false, false);
            var _cr = _cb.Build();

            Program.DemoSetup = _cr.Get<DemoSetup>();

            Program.Selected_LdapServer = Program.DemoSetup.LdapServers[0].Address;
            Program.Selected_LdapServerPort = (int)LDAPHelper.LdapServerDefaultPorts.DefaultPort;
            Program.Selected_UseSsl = Program.DemoSetup.UseSSL;
            Program.Selected_ConnectionTimeout = Program.DemoSetup.ConnectionTimeout;
            Program.Selected_DomainAccountName = Program.DemoSetup.DomainAccountName;
            Program.Selected_BaseDN = Program.DemoSetup.BaseDNs[0].DN;
        }

        private static void configDemoLogger()
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();
        }

        private static void printDemoTitle(string title)
        {
            Console.WriteLine();
            Console.WriteLine("**********************************************");
            Console.WriteLine("  " + title);
            Console.WriteLine("**********************************************");
        }

        private static LDAPHelper.LdhConnectionInfo getConnectionInfo()
        {
            return new LDAPHelper.LdhConnectionInfo(Program.Selected_LdapServer, Program.Selected_LdapServerPort, Program.Selected_UseSsl, Program.Selected_ConnectionTimeout);
        }

        private static LDAPHelper.LdhCredentials getCredentials()
        {
            return new LDAPHelper.LdhCredentials(Program.Selected_DomainAccountName, Program.Selected_AccountNamePassword);
        }

        public static LDAPHelper.LdhSearchLimits getSearchLimits()
        {
            return new LDAPHelper.LdhSearchLimits(Program.Selected_BaseDN);
        }

        private static LDAPHelper.LdhClientConfiguration getClientConfiguration()
        {
            return new LDAPHelper.LdhClientConfiguration(getConnectionInfo(), getCredentials(), getSearchLimits());
        }

        private static void requestLdapServer()
        {
            Console.WriteLine();
            Console.WriteLine("SELECT OR ENTER LDAP SERVER DNS OR IP:");
            var _position = 0;
            foreach (var _s in Program.DemoSetup.LdapServers)
            {
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

        private static void requestLdapServerPort()
        {
            Console.WriteLine();
            Console.WriteLine("SELECT OR ENTER LDAP SERVER PORT:");
            Console.WriteLine($"(1) Default -> {(int)LDAPHelper.LdapServerDefaultPorts.DefaultPort}");
            Console.WriteLine($"(2) Default Global Catalog -> {(int)LDAPHelper.LdapServerDefaultPorts.DefaultGlobalCatalogPort}");
            Console.WriteLine($"(3) Default SSL -> {(int)LDAPHelper.LdapServerDefaultPorts.DefaultSslPort}");
            Console.WriteLine($"(4) Default SSL Global Catalog -> {(int)LDAPHelper.LdapServerDefaultPorts.DefaultGlobalCatalogSslPort}");

        SELECT_AGAIN:

            Console.WriteLine($"Leave empty to use {Program.Selected_LdapServerPort}");
            var _portEntered = Console.ReadLine();
            if (!string.IsNullOrEmpty(_portEntered))
            {
                var _portNumber = Convert.ToInt32(_portEntered);
                if (_portNumber <= 3)
                {
                    switch (_portNumber)
                    {
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
                else
                {
                    Program.Selected_LdapServerPort = _portNumber;
                }
            }
            Log.Information($"Will use port:{Program.Selected_LdapServerPort} to connect LDAP Server.");
        }

        private static string requesAccounPassword(string domainAccountName)
        {
            Console.WriteLine();

        REQUEST_PASSWORD:

            Console.WriteLine($"ENTER PASSWORD FOR {domainAccountName}:");

            var password = readPassword('*');

            if (string.IsNullOrEmpty(password) || string.IsNullOrWhiteSpace(password)) goto REQUEST_PASSWORD;

            Log.Information($"Password entered.");

            return password;
        }

        private static void requestBaseDN()
        {
            Console.WriteLine();
            Console.WriteLine("SELECT OR ENTER BASE DN:");
            var _position = 0;
            foreach (var _dn in Program.DemoSetup.BaseDNs)
            {
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

        public static string readPassword(char passwordChar)
        {
            const int _ENTER = 13, _BACKSP = 8, _CTRLBACKSP = 127;
            int[] _FILTERED = { 0, 27, 9, 10 /*, 32 space, if you care */ }; // const

            var _pass = new Stack<char>();
            char _chr = (char)0;

            while ((_chr = Console.ReadKey(true).KeyChar) != _ENTER)
            {
                if (_chr == _BACKSP)
                {
                    if (_pass.Count > 0)
                    {
                        System.Console.Write("\b \b");
                        _pass.Pop();
                    }
                }
                else if (_chr == _CTRLBACKSP)
                {
                    while (_pass.Count > 0)
                    {
                        System.Console.Write("\b \b");
                        _pass.Pop();
                    }
                }
                else if (_FILTERED.Count(x => _chr == x) > 0)
                {
                    //Nothing to do
                }
                else
                {
                    _pass.Push((char)_chr);
                    System.Console.Write(passwordChar);
                }
            }

            System.Console.WriteLine();

            return new string(_pass.Reverse().ToArray());
        }

        private static bool requestYESorNO(string question)
        {
            Console.WriteLine();
            Console.WriteLine(question);
        YESORNO:
            Console.WriteLine("Waiting for your answer -> Yes = Y or No = N");
            //Wait for answer...
            var _yesOrNo = Console.ReadLine();
            if (_yesOrNo.Equals("y", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            if (_yesOrNo.Equals("n", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
            else
            {
                Console.WriteLine("Please, answer Yes or No");
                goto YESORNO;
            }
        }

        private static void printMemberOf(DTO.LDAPEntry entry, int tabCount = 0)
        {
            if (entry.memberOfEntries == null)
                return;

            tabCount++;

            Log.Information($"{new string('\t', tabCount)}MemberOf entries:");

            foreach (var parentEntry in entry.memberOfEntries)
            {
                Log.Information($"{new string('\t', tabCount)}{parentEntry.distinguishedName}");

                printMemberOf(parentEntry, tabCount);
            }
        }
        #endregion


        public static void Main(string[] args)
        {
            Task.Run(() => MainAsync(args)).GetAwaiter().GetResult();
        }

        public static async Task MainAsync(string[] args)
        {
            try
            {
                configDemoLogger();

            EXECUTE_DEMO:

                Log.Information("LDAPHelper Test (.NET Core Console)");
                Console.WriteLine();

                Log.Information("Starting demo: " + DateTime.Now.ToString());
                Console.WriteLine();

                Log.Warning($"Demo configuration file: {Program.DemoSetup_FilePath}");
                Log.Warning("Verify that the demo configuration file exists. If you want to use a different one, assign the file path in the Program.DemoSetup_FilePath variable.");

                loadDemoSetup();

                requestLdapServer();

                requestLdapServerPort();

                requestBaseDN();

                Program.Selected_AccountNamePassword = requesAccounPassword(Program.Selected_DomainAccountName);

                #region Authenticatio
                await Demo_Authenticator_Authenticate(DemoSetup.Demo_Authenticator_Authenticate_DomainAccountName);
                #endregion

                #region Search User & Groups
                await Demo_Searcher_SearchUsersAndGroups(LDAPHelper.DTO.EntryAttribute.sAMAccountName, DemoSetup.Demo_Searcher_SearchUsersAndGroups_Filter_sAMAccountName, LDAPHelper.DTO.RequiredEntryAttributes.AllWithMemberOf);

                await Demo_Searcher_SearchUsersAndGroups(LDAPHelper.DTO.EntryAttribute.cn, DemoSetup.Demo_Searcher_SearchUsersAndGroups_Filter_cn, LDAPHelper.DTO.RequiredEntryAttributes.AllWithMemberOf);
                #endregion

                #region Search User & Groups by 2 filters
                await Demo_Searcher_SearchUsersAndGroupsByTwoFilters(LDAPHelper.DTO.EntryAttribute.sAMAccountName, DemoSetup.Demo_Searcher_SearchUsersAndGroupsByTwoFilters_Filter1_sAMAccountName, LDAPHelper.DTO.EntryAttribute.cn, DemoSetup.Demo_Searcher_SearchUsersAndGroupsByTwoFilters_Filter2_cn, false, DTO.RequiredEntryAttributes.All);
                #endregion

                #region Search any kind of entries 
                await Demo_Searcher_SearchEntries(LDAPHelper.DTO.EntryAttribute.cn, DemoSetup.Demo_Searcher_SearchEntries_Filter_cn, DTO.RequiredEntryAttributes.All);

                await Demo_Searcher_SearchEntries(LDAPHelper.DTO.EntryAttribute.objectSid, DemoSetup.Demo_Searcher_SearchEntries_Filter_objectSid, DTO.RequiredEntryAttributes.All);
                #endregion

                #region Search any kind of entries by 2 filters
                await Demo_Searcher_SearchEntriesByTwoFilters(LDAPHelper.DTO.EntryAttribute.cn, DemoSetup.Demo_Searcher_SearchEntriesByTwoFilters_Filter1_cn, LDAPHelper.DTO.EntryAttribute.cn, DemoSetup.Demo_Searcher_SearchEntriesByTwoFilters_Filter2_cn, false, DTO.RequiredEntryAttributes.All);
                #endregion

                #region Search parent entries of an entry
                await Demo_Searcher_SearchParentEntries(DTO.EntryAttribute.sAMAccountName, DemoSetup.Demo_Searcher_SearchParentEntries_Filter_sAMAccountName, DTO.RequiredEntryAttributes.All);
                #endregion

                #region Check group membership 
                await Demo_GroupMembershipValidator_CheckGroupMembership(DemoSetup.Demo_GroupMembershipValidator_CheckGroupmembership_sAMAccountName, DemoSetup.Demo_GroupMembershipValidator_CheckGroupmembership_Check_GroupName);
                #endregion 

                var executeAgain = requestYESorNO("Dou you want to execute DEMO again?");
                if (executeAgain)
                    goto EXECUTE_DEMO;

                Console.WriteLine();

                Log.Warning("Demo complete: " + DateTime.Now.ToString());
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
                Console.ReadLine();
            }
        }

        public static async Task Demo_Authenticator_Authenticate(string domainAccountName)
        {
            try
            {
                printDemoTitle("Demo_Authenticator_Authenticate");

                Log.Information($"To authenticate {domainAccountName} you need to enter a password (it's not necessary the real password).");

                var accountPassword = requesAccounPassword(domainAccountName);

                var authenticator = new LDAPHelper.LdhAuthenticator(getConnectionInfo());

                Log.Information("Authenticating account name...");
                var authenticated = await authenticator.AuthenticateAsync(new LDAPHelper.LdhCredentials(domainAccountName, accountPassword));
                if (authenticated)
                    Log.Information("Account name authenticated.");
                else
                    Log.Warning("Account name NOt authenticated.");
            }
            catch (Exception ex)
            {
                Console.WriteLine();

                Log.Error(ex.Message);
                Log.Error(ex.StackTrace);
            }
        }

        public static async Task Demo_Searcher_SearchUsersAndGroups(LDAPHelper.DTO.EntryAttribute filterAttribute, string filterValue, LDAPHelper.DTO.RequiredEntryAttributes requiredEntryAttributes)
        {
            try
            {
                printDemoTitle("Demo_Searcher_SearchUsersAndGroups");

                var searcher = new LDAPHelper.LdhSearcher(getClientConfiguration());

                Log.Information($"Base DN: {searcher.SearchLimits.BaseDN}");
                Log.Information($"Searching by {filterAttribute}: {filterValue}");
                Console.WriteLine();

                var searchResult = await searcher.SearchUsersAndGroupsAsync(filterAttribute, filterValue, requiredEntryAttributes, Program.Tag);

                if (searchResult.Entries.Count() == 0)
                {
                    if (searchResult.HasErrorInfo)
                        throw searchResult.ErrorObject;
                    else
                        Log.Warning(Program.Message_LdapEntriesNotFound);
                }
                else
                {
                    foreach (var entry in searchResult.Entries)
                    {
                        Log.Information(entry.company);
                        Log.Information(entry.co);
                        Log.Information(entry.samAccountName);
                        Log.Information(entry.cn);
                        Log.Information(entry.displayName);
                        Log.Information(entry.distinguishedName);
                        Log.Information(entry.objectSid);

                        if (entry.memberOfEntries == null || entry.memberOfEntries.Count() > 0)
                            Console.WriteLine();

                        printMemberOf(entry);

                        Console.WriteLine();
                    }

                    Log.Information($"{searchResult.Entries.Count()} entrie(s) found.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine();

                Log.Error(ex.Message);
                Log.Error(ex.StackTrace);
            }
        }

        public static async Task Demo_Searcher_SearchUsersAndGroupsByTwoFilters(LDAPHelper.DTO.EntryAttribute filterAttribute, string filterValue, LDAPHelper.DTO.EntryAttribute secondFilterAttribute, string secondFilterValue, bool conjunctiveFilters, LDAPHelper.DTO.RequiredEntryAttributes requiredEntryAttributes)
        {
            try
            {
                printDemoTitle("Demo_Searcher_SearchUsersAndGroupsByTwoFilters");

                var searcher = new LDAPHelper.LdhSearcher(getClientConfiguration());

                Log.Information($"Base DN: {searcher.SearchLimits.BaseDN}");
                Log.Information($"Searching by {filterAttribute}={filterValue} {(conjunctiveFilters ? " And " : " Or ")} { secondFilterAttribute}={secondFilterValue}");
                Console.WriteLine();

                var searchResult = await searcher.SearchUsersAndGroupsAsync(filterAttribute, filterValue, secondFilterAttribute, secondFilterValue, conjunctiveFilters, requiredEntryAttributes, Program.Tag);
                if (searchResult.Entries.Count().Equals(0))
                {
                    if (searchResult.HasErrorInfo)
                        throw searchResult.ErrorObject;
                    else
                        Log.Warning(Program.Message_LdapEntriesNotFound);
                }
                else
                {
                    foreach (var entry in searchResult.Entries)
                    {
                        Log.Information(entry.company);
                        Log.Information(entry.co);
                        Log.Information(entry.samAccountName);
                        Log.Information(entry.cn);
                        Log.Information(entry.displayName);
                        Log.Information(entry.distinguishedName);
                        Log.Information(entry.objectSid);

                        Console.WriteLine();
                    }

                    Log.Information($"{searchResult.Entries.Count()} entrie(s) found.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine();

                Log.Error(ex.Message);
                Log.Error(ex.StackTrace);
            }
        }

        public static async Task Demo_Searcher_SearchEntries(LDAPHelper.DTO.EntryAttribute filterAttribute, string filterValue, LDAPHelper.DTO.RequiredEntryAttributes requiredEntryAttributes)
        {
            try
            {
                printDemoTitle("Demo_Searcher_SearchEntries");

                var searcher = new LDAPHelper.LdhSearcher(getClientConfiguration());

                Log.Information($"Base DN: {searcher.SearchLimits.BaseDN}");
                Log.Information($"Searching by {filterAttribute}: {filterValue}");
                Console.WriteLine();

                var searchResult = await searcher.SearchEntriesAsync(filterAttribute, filterValue, requiredEntryAttributes, Program.Tag);
                if (searchResult.Entries.Count().Equals(0))
                {
                    if (searchResult.HasErrorInfo)
                        throw searchResult.ErrorObject;
                    else
                        Log.Warning(Program.Message_LdapEntriesNotFound);
                }
                else
                {
                    foreach (var _entry in searchResult.Entries)
                    {
                        Log.Information(_entry.company);
                        Log.Information(_entry.co);
                        Log.Information(_entry.samAccountName);
                        Log.Information(_entry.cn);
                        Log.Information(_entry.displayName);
                        Log.Information(_entry.distinguishedName);
                        Log.Information(_entry.objectSid);

                        Console.WriteLine();
                    }

                    Log.Information($"{searchResult.Entries.Count()} entrie(s) found.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine();

                Log.Error(ex.Message);
                Log.Error(ex.StackTrace);
            }
        }

        public static async Task Demo_Searcher_SearchEntriesByTwoFilters(LDAPHelper.DTO.EntryAttribute filterAttribute, string filterValue, LDAPHelper.DTO.EntryAttribute secondFilterAttribute, string secondFilterValue, bool conjunctiveFilters, LDAPHelper.DTO.RequiredEntryAttributes requiredEntryAttributes)
        {
            try
            {
                printDemoTitle("Demo_Searcher_SearchEntriesByTwoFilters");

                var searcher = new LDAPHelper.LdhSearcher(getClientConfiguration());

                Log.Information($"Base DN: {searcher.SearchLimits.BaseDN}");
                Log.Information($"Searching by {filterAttribute}={filterValue} {(conjunctiveFilters ? " And " : " Or ")} { secondFilterAttribute}={secondFilterValue}");
                Console.WriteLine();

                var searchResult = await searcher.SearchEntriesAsync(filterAttribute, filterValue, secondFilterAttribute, secondFilterValue, conjunctiveFilters, requiredEntryAttributes, Program.Tag);
                if (searchResult.Entries.Count().Equals(0))
                {
                    if (searchResult.HasErrorInfo)
                        throw searchResult.ErrorObject;
                    else
                        Log.Warning(Program.Message_LdapEntriesNotFound);
                }
                else
                {
                    foreach (var entry in searchResult.Entries)
                    {
                        Log.Information(entry.company);
                        Log.Information(entry.co);
                        Log.Information(entry.samAccountName);
                        Log.Information(entry.cn);
                        Log.Information(entry.displayName);
                        Log.Information(entry.distinguishedName);
                        Log.Information(entry.objectSid);

                        Console.WriteLine();
                    }

                    Log.Information($"{searchResult.Entries.Count()} entrie(s) found.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine();

                Log.Error(ex.Message);
                Log.Error(ex.StackTrace);
            }
        }

        public static async Task Demo_Searcher_SearchParentEntries(LDAPHelper.DTO.EntryAttribute filterAttribute, string filterValue, LDAPHelper.DTO.RequiredEntryAttributes requiredEntryAttributes)
        {
            try
            {
                printDemoTitle("Demo_Searcher_SearchParentEntries");

                var searcher = new LDAPHelper.LdhSearcher(getClientConfiguration());

                Log.Information($"Base DN: {searcher.SearchLimits.BaseDN}");
                Log.Information($"Searching parent entries for {filterAttribute}: {filterValue}");
                Console.WriteLine();

                var searchResult = await searcher.SearchParentEntriesAsync(filterAttribute, filterValue, requiredEntryAttributes, Program.Tag);
                if (searchResult.Entries.Count().Equals(0))
                {
                    if (searchResult.HasErrorInfo)
                        throw searchResult.ErrorObject;
                    else
                        Log.Warning(Program.Message_LdapEntriesNotFound);
                }
                else
                {
                    foreach (var entry in searchResult.Entries)
                    {
                        Log.Information(entry.company);
                        Log.Information(entry.co);
                        Log.Information(entry.samAccountName);
                        Log.Information(entry.cn);
                        Log.Information(entry.displayName);
                        Log.Information(entry.distinguishedName);
                        Log.Information(entry.objectSid);

                        Console.WriteLine();
                    }

                    Log.Information($"{searchResult.Entries.Count()} entrie(s) found.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine();

                Log.Error(ex.Message);
                Log.Error(ex.StackTrace);
            }
        }

        public static async Task Demo_GroupMembershipValidator_CheckGroupMembership(string sAMAccountName, string groupName)
        {
            try
            {
                printDemoTitle("Demo_GroupMembershipValidator_CheckGroupMembership");

                var validator = new LDAPHelper.LdhGroupMembershipValidator(getClientConfiguration());

                Log.Information($"Base DN: {validator.SearchLimits.BaseDN}");
                Log.Information($"Checking {groupName} membership for {sAMAccountName}");
                Console.WriteLine();

                var result = await validator.CheckGroupMembershipAsync(sAMAccountName, groupName);

                if (result)
                    Log.Information($"{sAMAccountName} BELONGS to the group {groupName}.");
                else
                    Log.Information($"{sAMAccountName} DOES NOT BELONG to the group {groupName}.");
            }
            catch (Exception ex)
            {
                Console.WriteLine();

                Log.Error(ex.Message);
                Log.Error(ex.StackTrace);
            }
        }
    }

    public partial class Program
    {
        internal const string Message_LdapEntriesNotFound = "LDAP entries not found with the provided filters.";
    }
}