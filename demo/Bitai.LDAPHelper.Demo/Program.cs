using System;
using System.Threading.Tasks;
using Serilog;
using Serilog.Sinks.SystemConsole;
using System.Linq;
using System.Security;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using Bitai.LDAPHelper.Extensions;
using Bitai.LDAPHelper.DTO;

namespace Bitai.LDAPHelper.Demo
{
	/// <summary>
	/// Demo Program
	/// </summary>
	public partial class Program
	{
		internal static string DemoSetup_FilePath = $"{Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory).Replace('\\', '/')}/ldap-helper-demo-setup.json";

		internal const string Message_LdapEntriesNotFound = "LDAP entries not found with the provided filters.";

		/// <summary>
		/// DemoSetup object
		/// </summary>
		internal static DemoSetup DemoSetup;
		/// <summary>
		/// Custom (optional) tag value to label request. Can be null. 
		/// </summary>
		internal static string RequestLabel = "My Demo";
		/// <summary>
		/// LDAP Server IP or DNS name
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
		/// Domain account name to connect LDAP Server
		/// </summary>
		internal static string Selected_DomainAccountName;
		/// <summary>
		/// Domain account passsword
		/// </summary>
		internal static string Selected_DomainAccountPassword;
		/// <summary>
		/// Selected BaseDN to limit search in DEMO
		/// </summary>
		internal static string Selected_BaseDN;


		#region Private Static Methods
		private static void loadDemoSetup()
		{
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

		private static LDAPHelper.ConnectionInfo getConnectionInfo()
		{
			return new LDAPHelper.ConnectionInfo(Program.Selected_LdapServer, Program.Selected_LdapServerPort, Program.Selected_UseSsl, Program.Selected_ConnectionTimeout);
		}

		private static DTO.LDAPDomainAccountCredential getDomainAccountCredential()
		{
			var domainAccountCredentialParts = Program.Selected_DomainAccountName.Split(new char[] { '\\' });

			return new DTO.LDAPDomainAccountCredential(domainAccountCredentialParts[0], domainAccountCredentialParts[1], Program.Selected_DomainAccountPassword);
		}

		public static LDAPHelper.SearchLimits getSearchLimits()
		{
			return new LDAPHelper.SearchLimits(Program.Selected_BaseDN);
		}

		private static LDAPHelper.ClientConfiguration getClientConfiguration()
		{
			return new LDAPHelper.ClientConfiguration(getConnectionInfo(), getDomainAccountCredential(), getSearchLimits());
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
					Program.Selected_UseSsl = requestYESorNO($"Port {_portNumber} use SSL?");
				}
			}
			Log.Information($"Will use port:{Program.Selected_LdapServerPort} to connect LDAP Server.");
		}

		private static string requestAccountPassword(string domainAccountName)
		{
		REQUEST_PASSWORD:

			Console.WriteLine($"ENTER PASSWORD FOR {domainAccountName}:");

			var password = readPassword('*');

			if (string.IsNullOrEmpty(password) || string.IsNullOrWhiteSpace(password)) goto REQUEST_PASSWORD;

			Log.Information($"Password entered.");

			return password;
		}

		private static void requestPasswordToConnectLDAPServer()
		{
			Console.WriteLine();

			Log.Information($"To connect to the LDAP server {Program.Selected_LdapServer} it is necessary to enter the password of the {Program.Selected_DomainAccountName}.");

			Program.Selected_DomainAccountPassword = requestAccountPassword(Program.Selected_DomainAccountName);
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




		/// <summary>
		/// Program entry point
		/// </summary>
		/// <returns></returns>
		public static async Task Main()
		{
			try
			{
				configDemoLogger();

			EXECUTE_DEMO:

				Log.Information($"{nameof(Bitai.LDAPHelper)} Test (.NET Core Console)");
				Console.WriteLine();

				Log.Information("Starting demo: " + DateTime.Now.ToString());
				Console.WriteLine();

				Log.Warning($"Demo configuration file: {Program.DemoSetup_FilePath}");
				Log.Warning("Verify that the demo configuration file exists. If you want to use a different one, assign the file path in the Program.DemoSetup_FilePath variable.");

				loadDemoSetup();

				requestLdapServer();

				requestLdapServerPort();

				requestBaseDN();

				requestPasswordToConnectLDAPServer();

				#region Create User account
				if (DemoSetup.Demo_AccountManager_CreateUserAccount_RunTest)
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

					await Demo_AccountManager_CreateUserAccount(DemoSetup.Demo_AccountManager_CreateUserAccount_UserAccountName, DemoSetup.Demo_AccountManager_CreateUserAccount_Password, DemoSetup.Demo_AccountManager_CreateUserAccount_ContainerDN, DemoSetup.Demo_AccountManager_CreateUserAccount_Name, DemoSetup.Demo_AccountManager_CreateUserAccount_Surname, DemoSetup.Demo_AccountManager_CreateUserAccount_DNSDomainName, memberOf, objectClasses, DemoSetup.Demo_AccountManager_CreateUserAccount_UserAccountControlFlags);
				}
				#endregion

				#region Password assignment
				if (DemoSetup.Demo_AccountManager_SetAccountPassword_RunTest)
				{
					await Demo_AccountManager_SetPassword(DemoSetup.Demo_AccountManager_SetAccountPassword_DistinguishedName);
				}
				#endregion

				#region Authentication
				if (DemoSetup.Demo_Authenticator_Authenticate_RunTest)
				{
					if (DemoSetup.Demo_Authenticator_Authenticate_RunTest_Simple)
						await Demo_Authenticator_Authenticate_Simple(DemoSetup.Demo_Authenticator_Authenticate_DomainAccountName);
					else
						await Demo_Authenticator_Authenticate_WithAccountValidation(DemoSetup.Demo_Authenticator_Authenticate_DomainAccountName);
				}
				#endregion

				#region Disable user account
				if (DemoSetup.Demo_AccountManager_DisableUserAccount_RunTest)
				{
					await Demo_AccountManager_DisableUserAccount(DemoSetup.Demo_AccountManager_DisableUserAccount_UserAccountDistinguishedName);
				}
				#endregion

				#region Remove user account
				if (DemoSetup.Demo_AccountManager_RemoveUserAccount_RunTest)
				{
					await Demo_AccountManager_RemoveUserAccount(DemoSetup.Demo_AccountManager_RemoveUserAccount_UserAccountDistinguishedName);
				}
				#endregion

				#region Search Users
				if (DemoSetup.Demo_Searcher_SearchUsers_RunTest)
				{
					await Demo_Searcher_SearchUsers(EntryAttribute.sAMAccountName, DemoSetup.Demo_Searcher_SearchUsers_Filter_sAMAccountName, RequiredEntryAttributes.All);

					await Demo_Searcher_SearchUsers(EntryAttribute.cn, DemoSetup.Demo_Searcher_SearchUsers_Filter_cn, RequiredEntryAttributes.All);
				}
				#endregion

				#region Search Users by 2 filters
				if (DemoSetup.Demo_Searcher_SearchUsers_RunTest)
				{
					await Demo_Searcher_SearchUsersByTwoFilters(EntryAttribute.sAMAccountName, DemoSetup.Demo_Searcher_SearchUsersByTwoFilters_Filter1_sAMAccountName, EntryAttribute.cn, DemoSetup.Demo_Searcher_SearchUsersByTwoFilters_Filter2_cn, false, RequiredEntryAttributes.All);
				}
				#endregion

				#region Search any kind of entries 
				if (DemoSetup.Demo_Searcher_SearchEntries_RunTest)
				{
					await Demo_Searcher_SearchEntries(EntryAttribute.cn, DemoSetup.Demo_Searcher_SearchEntries_Filter_cn, false, RequiredEntryAttributes.All);

					await Demo_Searcher_SearchEntries(EntryAttribute.objectSid, DemoSetup.Demo_Searcher_SearchEntries_Filter_objectSid, false, RequiredEntryAttributes.All);
				}
				#endregion

				#region Search any kind of entries by 2 filters
				if (DemoSetup.Demo_Searcher_SearchEntries_RunTest)
				{
					await Demo_Searcher_SearchEntriesByTwoFilters(EntryAttribute.cn, DemoSetup.Demo_Searcher_SearchEntriesByTwoFilters_Filter1_cn, EntryAttribute.cn, DemoSetup.Demo_Searcher_SearchEntriesByTwoFilters_Filter2_cn, false, RequiredEntryAttributes.All);
				}
				#endregion

				#region Search parent entries of an entry
				if (DemoSetup.Demo_Searcher_SearchParentEntries_RunTest)
				{
					await Demo_Searcher_SearchParentEntries(EntryAttribute.sAMAccountName, DemoSetup.Demo_Searcher_SearchParentEntries_Filter_sAMAccountName, RequiredEntryAttributes.All);
				}
				#endregion

				#region Check group membership 
				if (DemoSetup.Demo_GroupMembershipValidator_RunTest)
				{
					await Demo_GroupMembershipValidator_CheckGroupMembership(DemoSetup.Demo_GroupMembershipValidator_CheckGroupmembership_sAMAccountName, DemoSetup.Demo_GroupMembershipValidator_CheckGroupmembership_Check_GroupName);
				}
				#endregion

				var executeAgain = requestYESorNO("Dou you want to execute DEMO again?");
				if (executeAgain)
				{
					Console.WriteLine("You can take advantage, before running the demo again, to change the parameters in the demo's configuration file if you need it. To continue press a key.");

					Console.ReadLine();

					goto EXECUTE_DEMO;
				}

				Console.WriteLine();

				Log.Warning("Demo completed: " + DateTime.Now.ToString());
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

		public static async Task Demo_AccountManager_CreateUserAccount(string userAccountName, string password, string distinguishedNameOfContainer, string name, string surName, string dnsDomainName, string[] memberOf, string[] objectClasses, string userAccountControlFlags)
		{
			try
			{
				printDemoTitle(nameof(Demo_AccountManager_CreateUserAccount));

				string fullName = $"{name} {surName}";
				var newUserAccount = new LDAPMsADUserAccount(distinguishedNameOfContainer)
				{
					GivenName = name,
					Sn = surName,
					Cn = fullName,
					Name = fullName,
					DisplayName = fullName,
					MemberOf = memberOf,
					ObjectClass = objectClasses,
					Password = password,
					SAMAccountName = userAccountName,
					UserAccountControl = userAccountControlFlags,
					UserPrincipalName = $"{userAccountName}@{dnsDomainName}"
				};
				Log.Information($"New user account data:");
				Log.Information("{@newUserAccount}", newUserAccount);

				var accountManager = new LDAPHelper.AccountManager(getClientConfiguration());

				Log.Information("Creating new user account...");
				var result = await accountManager.CreateUserAccountForMsAD(newUserAccount, RequestLabel);

				if (result.IsSuccessfulOperation)
				{
					Log.Information("Operation completed.");
					Log.Information("{@result}", result);
				}
				else
				{
					Log.Error("Unsuccessful operation!");
					Log.Error("{@result}", result);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine();

				Log.Error(ex.Message);
				Log.Error(ex.StackTrace);
			}
		}

		public static async Task Demo_AccountManager_SetPassword(string distinguishedName)
		{
			try
			{
				printDemoTitle(nameof(Demo_AccountManager_SetPassword));

				Log.Information($"Enter new password for {distinguishedName}");

				var password = requestAccountPassword(distinguishedName);
				var credential = new DTO.LDAPDistinguishedNameCredential(distinguishedName, password);
				var accountManager = new LDAPHelper.AccountManager(getClientConfiguration());

				Log.Information("Setting account password...");
				var result = await accountManager.SetUserAccountPasswordForMsAD(credential);

				if (result.IsSuccessfulOperation)
				{
					Log.Information(result.OperationMessage);
				}
				else
				{
					Log.Error(result.OperationMessage);
					if (result.HasErrorObject)
					{
						Log.Error(result.ErrorObject.Message);
						Log.Error(result.ErrorObject.StackTrace);
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine();

				Log.Error(ex.Message);
				Log.Error(ex.StackTrace);
			}
		}

		public static async Task Demo_AccountManager_DisableUserAccount(string distinguishedName)
		{
			try
			{
				printDemoTitle(nameof(Demo_AccountManager_DisableUserAccount));

				var accountManager = new LDAPHelper.AccountManager(getClientConfiguration());

				Log.Information("Disabling user account {dn}", distinguishedName);
				var result = await accountManager.DisableUserAccountForMsAD(distinguishedName, Program.RequestLabel);

				if (result.IsSuccessfulOperation)
				{
					Log.Information(result.OperationMessage);
					Log.Information("{@r}", result);
				}
				else
				{
					Log.Error(result.OperationMessage);
					Log.Error("{@r}", result);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine();

				Log.Error(ex.Message);
				Log.Error(ex.StackTrace);
			}
		}

		public static async Task Demo_AccountManager_RemoveUserAccount(string distinguishedName)
		{
			try
			{
				printDemoTitle(nameof(Demo_AccountManager_RemoveUserAccount));

				var accountManager = new LDAPHelper.AccountManager(getClientConfiguration());

				Log.Information("Remove user account {dn}", distinguishedName);
				var result = await accountManager.RemoveUserAccountForMsAD(distinguishedName, Program.RequestLabel);

				if (result.IsSuccessfulOperation)
				{
					Log.Information(result.OperationMessage);
					Log.Information("{@r}", result);
				}
				else
				{
					Log.Error(result.OperationMessage);
					Log.Error("{@r}", result);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine();

				Log.Error(ex.Message);
				Log.Error(ex.StackTrace);
			}
		}

		public static async Task Demo_Authenticator_Authenticate_WithAccountValidation(string domainAccountCredential)
		{
			try
			{
				printDemoTitle(nameof(Demo_Authenticator_Authenticate_WithAccountValidation));

				Log.Information($"To authenticate {domainAccountCredential} you need to enter a password (it's not necessary the real password).");

				var accountPassword = requestAccountPassword(domainAccountCredential);

				var authenticator = new LDAPHelper.Authenticator(getConnectionInfo());

				var domainAccountCredentialParts = domainAccountCredential.Split(new char[] { '\\' });

				Log.Information("Authenticating account name...");
				var authenticationResult = await authenticator.AuthenticateAsync(new LDAPDomainAccountCredential(domainAccountCredentialParts[0], domainAccountCredentialParts[1], accountPassword), getSearchLimits(), getDomainAccountCredential(), RequestLabel);
				if (authenticationResult.IsSuccessfulOperation)
				{
					Log.Information("{@model}", authenticationResult);

					if (authenticationResult.IsAuthenticated)
						Log.Information("Account name authenticated.");
					else
						Log.Warning("Account name NOt authenticated.");
				}
				else
				{
					if (authenticationResult.HasErrorObject)
						throw new Exception(authenticationResult.OperationMessage, authenticationResult.ErrorObject);
					else
						throw new Exception(authenticationResult.OperationMessage);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine();

				Log.Error($"{domainAccountCredential} failed to authenticate!");
				Log.Error(ex.Message);
				Log.Error(ex.StackTrace);
			}
		}

		public static async Task Demo_Authenticator_Authenticate_Simple(string domainAccountCredential)
		{
			try
			{
				printDemoTitle(nameof(Demo_Authenticator_Authenticate_Simple));

				Log.Information($"To authenticate {domainAccountCredential} you need to enter a password (it's not necessary the real password).");

				var accountPassword = requestAccountPassword(domainAccountCredential);

				var authenticator = new Authenticator(getConnectionInfo());

				var domainAccountCredentialParts = domainAccountCredential.Split(new char[] { '\\' });

				Log.Information("Authenticating account name...");
				var authenticationResult = await authenticator.AuthenticateAsync(new LDAPDomainAccountCredential(domainAccountCredentialParts[0], domainAccountCredentialParts[1], accountPassword), RequestLabel);
				if (authenticationResult.IsSuccessfulOperation)
				{
					Log.Information("{@model}", authenticationResult);

					if (authenticationResult.IsAuthenticated)
						Log.Information("Account name authenticated.");
					else
						Log.Warning("Account name NOt authenticated.");
				}
				else
				{
					if (authenticationResult.HasErrorObject)
						throw new Exception(authenticationResult.OperationMessage, authenticationResult.ErrorObject);
					else
						throw new Exception(authenticationResult.OperationMessage);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine();

				Log.Error($"{domainAccountCredential} failed to authenticate!");
				Log.Error(ex.Message);
				Log.Error(ex.StackTrace);
			}
		}

		public static async Task Demo_Searcher_SearchUsers(EntryAttribute filterAttribute, string filterValue, RequiredEntryAttributes requiredEntryAttributes)
		{
			try
			{
				printDemoTitle("Demo_Searcher_SearchUsers");

				//Create search filter
				var onlyUsersFilterCombiner = QueryFilters.AttributeFilterCombiner.CreateOnlyUsersFilterCombiner();
				var attributeFilter = new QueryFilters.AttributeFilter(filterAttribute, new QueryFilters.FilterValue(filterValue));
				var searchFilterCombiner = new QueryFilters.AttributeFilterCombiner(false, true, new List<QueryFilters.ICombinableFilter> { onlyUsersFilterCombiner, attributeFilter });

				var searcher = new LDAPHelper.Searcher(getClientConfiguration());

				Log.Information($"Base DN: {searcher.SearchLimits.BaseDN}");
				Log.Information($"Searching by {searchFilterCombiner}");
				Console.WriteLine();

				var searchResult = await searcher.SearchEntriesAsync(searchFilterCombiner, requiredEntryAttributes, Program.RequestLabel);		
				if (searchResult.IsSuccessfulOperation)
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
						Log.Information(entry.userAccountControl);

						if (entry.memberOfEntries == null || entry.memberOfEntries.Count() > 0)
							Console.WriteLine();

						printMemberOf(entry);

						Console.WriteLine();
					}

					Log.Information($"{searchResult.Entries.Count()} entrie(s) found.");
				}
                else 
				{
                    if (searchResult.HasErrorObject)
                        throw new Exception(searchResult.OperationMessage, searchResult.ErrorObject);
                    else
                        throw new Exception(searchResult.OperationMessage);
                }
            }
			catch (Exception ex)
			{
                Console.WriteLine();
                
                Log.Error(ex.Message);
                Log.Error(ex.StackTrace);
            }
		}

		public static async Task Demo_Searcher_SearchUsersByTwoFilters(EntryAttribute filterAttribute, string filterValue, EntryAttribute secondFilterAttribute, string secondFilterValue, bool conjunctiveFilters, RequiredEntryAttributes requiredEntryAttributes)
		{
			try
			{
				printDemoTitle("Demo_Searcher_SearchUsersByTwoFilters");

				//Create search filter
				var onlyUsersFilterCombiner = QueryFilters.AttributeFilterCombiner.CreateOnlyUsersFilterCombiner();
				var attributeFilter1 = new QueryFilters.AttributeFilter(filterAttribute, new QueryFilters.FilterValue(filterValue));
				var attributeFilter2 = new QueryFilters.AttributeFilter(secondFilterAttribute, new QueryFilters.FilterValue(secondFilterValue));
				var filter1Filter2Combiner = new QueryFilters.AttributeFilterCombiner(false, conjunctiveFilters, new List<QueryFilters.ICombinableFilter> { attributeFilter1, attributeFilter2 });
				var searchFilterCombiner = new QueryFilters.AttributeFilterCombiner(false, true, new List<QueryFilters.ICombinableFilter> { onlyUsersFilterCombiner, filter1Filter2Combiner });

				var searcher = new LDAPHelper.Searcher(getClientConfiguration());

				Log.Information($"Base DN: {searcher.SearchLimits.BaseDN}");
				Log.Information($"Searching by {searchFilterCombiner}");
				Console.WriteLine();

				var searchResult = await searcher.SearchEntriesAsync(searchFilterCombiner, requiredEntryAttributes, Program.RequestLabel);		
				if (searchResult.IsSuccessfulOperation)
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
                        Log.Information(entry.userAccountControl);

                        Console.WriteLine();
					}

					Log.Information($"{searchResult.Entries.Count()} entrie(s) found.");
				}
				else 
				{
                    if (searchResult.HasErrorObject)
                        throw new Exception(searchResult.OperationMessage, searchResult.ErrorObject);
                    else
                        throw new Exception(searchResult.OperationMessage);
                }
			}
			catch (Exception ex)
			{
				Console.WriteLine();

				Log.Error(ex.Message);
				Log.Error(ex.StackTrace);
			}
		}

		public static async Task Demo_Searcher_SearchEntries(EntryAttribute filterAttribute, string filterValue, bool filterValueNegated, RequiredEntryAttributes requiredEntryAttributes)
		{
			try
			{
				printDemoTitle("Demo_Searcher_SearchEntries");

				//Create filter
				var filter = new QueryFilters.AttributeFilter(filterValueNegated, filterAttribute, new QueryFilters.FilterValue(filterValue));

				var searcher = new LDAPHelper.Searcher(getClientConfiguration());

				Log.Information($"Base DN: {searcher.SearchLimits.BaseDN}");
				Log.Information($"Searching by {filter}");
				Console.WriteLine();

				var searchResult = await searcher.SearchEntriesAsync(filter, requiredEntryAttributes, Program.RequestLabel);
                if (searchResult.IsSuccessfulOperation) {
                    foreach (var entry in searchResult.Entries) {
                        Log.Information(entry.company);
                        Log.Information(entry.co);
                        Log.Information(entry.samAccountName);
                        Log.Information(entry.cn);
                        Log.Information(entry.displayName);
                        Log.Information(entry.distinguishedName);
                        Log.Information(entry.objectSid);
                        Log.Information(entry.userAccountControl);

                        Console.WriteLine();
                    }

                    Log.Information($"{searchResult.Entries.Count()} entrie(s) found.");
                }
                else {
                    if (searchResult.HasErrorObject)
                        throw new Exception(searchResult.OperationMessage, searchResult.ErrorObject);
                    else
                        throw new Exception(searchResult.OperationMessage);
                }
            }
			catch (Exception ex)
			{
				Console.WriteLine();

				Log.Error(ex.Message);
				Log.Error(ex.StackTrace);
			}
		}

		public static async Task Demo_Searcher_SearchEntriesByTwoFilters(EntryAttribute filterAttribute, string filterValue, EntryAttribute secondFilterAttribute, string secondFilterValue, bool conjunctiveFilters, RequiredEntryAttributes requiredEntryAttributes)
		{
			try
			{
				printDemoTitle("Demo_Searcher_SearchEntriesByTwoFilters");

				//Create filters
				var filter1 = new QueryFilters.AttributeFilter(false, filterAttribute, new QueryFilters.FilterValue(filterValue));
				var filter2 = new QueryFilters.AttributeFilter(false, secondFilterAttribute, new QueryFilters.FilterValue(secondFilterValue));
				var filterCombiner = new QueryFilters.AttributeFilterCombiner(false, conjunctiveFilters, new List<QueryFilters.ICombinableFilter> { filter1, filter2 });

				var searcher = new LDAPHelper.Searcher(getClientConfiguration());

				Log.Information($"Base DN: {searcher.SearchLimits.BaseDN}");
				Log.Information($"Searching by {filterCombiner}");
				Console.WriteLine();

				var searchResult = await searcher.SearchEntriesAsync(filterCombiner, requiredEntryAttributes, Program.RequestLabel);
                if (searchResult.IsSuccessfulOperation) {
                    foreach (var entry in searchResult.Entries) {
                        Log.Information(entry.company);
                        Log.Information(entry.co);
                        Log.Information(entry.samAccountName);
                        Log.Information(entry.cn);
                        Log.Information(entry.displayName);
                        Log.Information(entry.distinguishedName);
                        Log.Information(entry.objectSid);
                        Log.Information(entry.userAccountControl);

                        Console.WriteLine();
                    }

                    Log.Information($"{searchResult.Entries.Count()} entrie(s) found.");
                }
                else {
                    if (searchResult.HasErrorObject)
                        throw new Exception(searchResult.OperationMessage, searchResult.ErrorObject);
                    else
                        throw new Exception(searchResult.OperationMessage);
                }
            }
			catch (Exception ex)
			{
				Console.WriteLine();

				Log.Error(ex.Message);
				Log.Error(ex.StackTrace);
			}
		}

		public static async Task Demo_Searcher_SearchParentEntries(EntryAttribute filterAttribute, string filterValue, RequiredEntryAttributes requiredEntryAttributes)
		{
			try
			{
				printDemoTitle("Demo_Searcher_SearchParentEntries");

				//Create search filter
				var attributeFilter = new QueryFilters.AttributeFilter(filterAttribute, new QueryFilters.FilterValue(filterValue));

				var searcher = new LDAPHelper.Searcher(getClientConfiguration());

				Log.Information($"Base DN: {searcher.SearchLimits.BaseDN}");
				Log.Information($"Searching parent entries for {filterAttribute}: {filterValue}");
				Console.WriteLine();

				var searchResult = await searcher.SearchParentEntriesAsync(attributeFilter, requiredEntryAttributes, Program.RequestLabel);
                if (searchResult.IsSuccessfulOperation) {
                    foreach (var entry in searchResult.Entries) {
                        Log.Information(entry.company);
                        Log.Information(entry.co);
                        Log.Information(entry.samAccountName);
                        Log.Information(entry.cn);
                        Log.Information(entry.displayName);
                        Log.Information(entry.distinguishedName);
                        Log.Information(entry.objectSid);
                        Log.Information(entry.userAccountControl);

                        Console.WriteLine();
                    }

                    Log.Information($"{searchResult.Entries.Count()} entrie(s) found.");
                }
                else {
                    if (searchResult.HasErrorObject)
                        throw new Exception(searchResult.OperationMessage, searchResult.ErrorObject);
                    else
                        throw new Exception(searchResult.OperationMessage);
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

				var validator = new LDAPHelper.GroupMembershipValidator(getClientConfiguration());

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
}