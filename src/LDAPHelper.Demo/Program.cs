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
        internal const string SetupDemoFilePath = "C:\\LDAPHelperLib_DemoSetup.json";


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
        private static void loadSetup()
        {
            var _cb = new ConfigurationBuilder()
                .AddJsonFile(Program.SetupDemoFilePath, false, false);
            var _cr = _cb.Build();

            Program.DemoSetup = _cr.Get<DemoSetup>();

            Program.Selected_LdapServer = Program.DemoSetup.LdapServers[0].Address;
            Program.Selected_LdapServerPort = (int)LDAPHelper.LdapServerDefaultPorts.DefaultPort;
            Program.Selected_UseSsl = Program.DemoSetup.UseSSL;
            Program.Selected_ConnectionTimeout = Program.DemoSetup.ConnectionTimeout;
            Program.Selected_DomainAccountName = Program.DemoSetup.DomainAccountName;
            Program.Selected_BaseDN = Program.DemoSetup.BaseDNs[0].DN;
        }

        private static void configLog()
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();
        }

        private static void logTestTitle(string title)
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

        private static void requesAccounNamePassword()
        {
            Console.WriteLine();
        REQUEST_PASSWORD:
            Console.WriteLine($"ENTER PASSWORD FOR {Program.Selected_DomainAccountName}:");
            var _password = readPassword('*');
            if (string.IsNullOrEmpty(_password) || string.IsNullOrWhiteSpace(_password)) goto REQUEST_PASSWORD;
            Program.Selected_AccountNamePassword = _password;
            Log.Information($"Password entered. Thanks!");
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

        private static bool requestYESoNO(string question)
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
                configLog();

            EXECUTE_DEMO:

                Log.Information("LDAPHelper Test (.NET Core Console)");
                Console.WriteLine();

                Log.Information("Starting demo: " + DateTime.Now.ToString());

                loadSetup();

                requestLdapServer();

                requestLdapServerPort();

                requestBaseDN();

                requesAccounNamePassword();

                #region Authenticate
                //await Demo_AuthenticateUserAsync("DOMAIN\\usr_ext01", "565ttY89");
                //await Demo_AuthenticateUserAsync("DOMAIN\\wsmith", "Rt6Y77y");
                #endregion

                #region Search User & Groups 1
                await Demo_SearchUsersAndGroupsByAttributeAsync(LDAPHelper.DTO.EntryAttribute.sAMAccountName, "usr_ext01", LDAPHelper.DTO.RequiredEntryAttributes.AllWithMemberOf);

                //await Demo_SearchUsersAndGroupsByAttributeAsync(LDAPHelper.DTO.EntryAttribute.sAMAccountName, "3110494", LDAPHelper.DTO.RequiredEntryAttributes.AllWithMemberOf);

                //await Demo_SearchUsersAndGroupsByAttributeAsync(LDAPHelper.DTO.EntryAttribute.sAMAccountName, "4439690", LDAPHelper.DTO.RequiredEntryAttributes.AllWithMemberOf);

                //await Demo_SearchUsersAndGroupsByAttributeAsync(LDAPHelper.DTO.EntryAttribute.distinguishedName, "CN=USER_AQD_LP,OU=Aplicaciones,OU=Grupos,DC=cl,DC=company,DC=com", LDAPHelper.DTO.RequiredEntryAttributes.MinimunWithMemberOf);

                //await Demo_SearchUsersAndGroupsByAttributeAsync(LDAPHelper.DTO.EntryAttribute.cn, "*bastidas*", LDAPHelper.DTO.RequiredEntryAttributes.All);
                #endregion

                #region Search User & Groups 2
                await Demo_SearchUsersAndGroupsBy2AttributesAsync(LDAPHelper.DTO.EntryAttribute.sAMAccountName, "usr_*", LDAPHelper.DTO.EntryAttribute.cn, "*brien*", false, DTO.RequiredEntryAttributes.All);

                //await Demo_SearchUsersAndGroupsBy2AttributesAsync(LDAPHelper.DTO.EntryAttribute.sAMAccountName, "*administrator*", LDAPHelper.DTO.EntryAttribute.cn, "*administrator*", true, DTO.RequiredEntryAttributes.All);
                #endregion

                #region Search Any kind of Entries 
                //await Demo_GetEntriesByAttributeAsync(LDAPHelper.DTO.EntryAttribute.sAMAccountName, "usr*", DTO.RequiredEntryAttributes.All);

                //await Demo_GetEntriesByAttributeAsync(LDAPHelper.DTO.EntryAttribute.objectSid, "S-1-5-21-638406840-1180129177-883519231-179439", DTO.RequiredEntryAttributes.All);             
                #endregion

                #region Search parent entries
                await Demo_SearchParentEntriesAsync(DTO.EntryAttribute.sAMAccountName, "usr_ext01", DTO.RequiredEntryAttributes.All);

                //await Demo_SearchParentEntriesAsync(true, LDAPHelper.DTO.KeyEntryAttribute.sAMAccountName, "4303259", BaseDNEnum.com);

                //await Demo_SearchParentEntriesAsync(true, LDAPHelper.DTO.KeyEntryAttribute.distinguishedName, "CN=Administrator,CN=Users,DC=us,DC=company,DC=com", true, BaseDNEnum.cl_com);              
                #endregion

                #region Check Group membership 
                await Demo_CheckGroupMembershipAsync("usr_ext01", "SIMON_Worker");
                #endregion 

                var _yes = requestYESoNO("Dou you want to execute DEMO again?");
                if (_yes)
                    goto EXECUTE_DEMO;

                Console.WriteLine();

                Log.Warning("Demo complete: " + DateTime.Now.ToString());
            }
            catch (Exception ex)
            {
                Log.Error("***************************************************");
                Log.Error("Error in DEMO! See below details.");
                Log.Error(ex.Message);
                Log.Error(ex.StackTrace);
                Log.Error("***************************************************");
            }
            finally
            {
                Console.ReadLine();
            }
        }

        public static async Task Demo_AuthenticateUserAsync(string domainAccountName, string accountPassword)
        {
            try
            {
                logTestTitle("Demo_AuthenticateUserAsync");

                var authenticator = new LDAPHelper.LdhAuthenticator(getConnectionInfo());

                Log.Information("Authenticating account name...");
                var authenticated = await authenticator.Authenticate(new LDAPHelper.LdhCredentials(domainAccountName, accountPassword));
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

        public static async Task Demo_SearchUsersAndGroupsByAttributeAsync(LDAPHelper.DTO.EntryAttribute filterAttribute, string filterValue, LDAPHelper.DTO.RequiredEntryAttributes requiredResults)
        {
            try
            {
                logTestTitle("Demo_SearchUsersAndGroupsByAttributeAsync");

                var searcher = new LDAPHelper.LdhSearcher(getClientConfiguration());

                Log.Information($"Base DN: {searcher.SearchLimits.BaseDN}");
                Log.Information($"Searching by {filterAttribute}: {filterValue}");
                Console.WriteLine();

                var searchResult = await searcher.SearchUsersAndGroupsAsync(filterAttribute, filterValue, requiredResults, Program.Tag);

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

        public static async Task Demo_SearchUsersAndGroupsBy2AttributesAsync(LDAPHelper.DTO.EntryAttribute filterAttribute, string filterValue, LDAPHelper.DTO.EntryAttribute secondFilterAttribute, string secondFilterValue, bool conjunctiveFilters, LDAPHelper.DTO.RequiredEntryAttributes requiredResults)
        {
            try
            {
                logTestTitle("Demo_SearchUsersAndGroupsBy2AttributesAsync");

                var searcher = new LDAPHelper.LdhSearcher(getClientConfiguration());

                Log.Information($"Base DN: {searcher.SearchLimits.BaseDN}");
                Log.Information($"Searching by {filterAttribute}={filterValue} {(conjunctiveFilters ? " And " : " Or ")} { secondFilterAttribute}={secondFilterValue}");
                Console.WriteLine();

                var searchResult = await searcher.SearchUsersAndGroupsAsync(filterAttribute, filterValue, secondFilterAttribute, secondFilterValue, conjunctiveFilters, requiredResults, Program.Tag);
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

        public static async Task Demo_GetEntriesByAttributeAsync(LDAPHelper.DTO.EntryAttribute attribute, string attributeFilter, LDAPHelper.DTO.RequiredEntryAttributes requiredResults)
        {
            try
            {
                logTestTitle("Demo_GetEntriesByAttributeAsync");

                var searcher = new LDAPHelper.LdhSearcher(getClientConfiguration());

                Log.Information($"Base DN: {searcher.SearchLimits.BaseDN}");
                Log.Information($"Searching by {attribute}: {attributeFilter}");
                Console.WriteLine();

                var searchResult = await searcher.SearchEntriesAsync(attribute, attributeFilter, requiredResults, Program.Tag);
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

        public static async Task Demo_SearchParentEntriesAsync(LDAPHelper.DTO.EntryAttribute filterAttribute, string filterValue, LDAPHelper.DTO.RequiredEntryAttributes requiredEntryAttributes)
        {
            try
            {
                logTestTitle("Demo_SearchParentEntriesAsync");

                var searcher = new LDAPHelper.LdhSearcher(getClientConfiguration());

                Log.Information($"Base DN: {searcher.SearchLimits.BaseDN}");
                Log.Information($"Searching parent entris for {filterAttribute}: {filterValue}");
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

        public static async Task Demo_CheckGroupMembershipAsync(string sAMAccountName, string groupName)
        {
            try
            {
                logTestTitle("Demo_CheckGroupMembershipAsync");

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