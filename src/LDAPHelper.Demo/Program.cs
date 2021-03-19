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
	public class Program
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
			Program.Selected_LdapServerPort = (int)LDAPHelper.LdapServerCommonPorts.DefaultPort;
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

		private static LDAPHelper.LdhUserCredentials getCredentials()
		{
			return new LDAPHelper.LdhUserCredentials(Program.Selected_DomainAccountName, Program.Selected_AccountNamePassword);
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
			Console.WriteLine($"(1) Default -> {(int)LDAPHelper.LdapServerCommonPorts.DefaultPort}");
			Console.WriteLine($"(2) Default Global Catalog -> {(int)LDAPHelper.LdapServerCommonPorts.GlobalCatalogPort}");
			Console.WriteLine($"(3) Default SSL -> {(int)LDAPHelper.LdapServerCommonPorts.DefaultSslPort}");
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
							Program.Selected_LdapServerPort = (int)LDAPHelper.LdapServerCommonPorts.DefaultPort;
							break;
						case 2:
							Program.Selected_LdapServerPort = (int)LDAPHelper.LdapServerCommonPorts.GlobalCatalogPort;
							break;
						case 3:
							Program.Selected_LdapServerPort = (int)LDAPHelper.LdapServerCommonPorts.DefaultSslPort;
							break;
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
		#endregion


		public static void Main(string[] args)
		{
			Task.Run(() => MainAsync(args)).GetAwaiter().GetResult();
		}

		public static async Task MainAsync(string[] args)
		{
			string _filterValue;

			try
			{
				configLog();

			EXECUTE_DEMO:

				Log.Information("LDAPHelper Test (.NET Core Console)");
				Console.WriteLine();

				Log.Warning("Inicio: " + DateTime.Now.ToString());


				loadSetup();

				requestLdapServer();

				requestLdapServerPort();

				requesAccounNamePassword();

				requestBaseDN();

				#region Authenticate
				//await Test_AuthenticateUser("LANPERU\\usr_ext01", "L4t4m2018");
				//await Test_AuthenticateUser("LANPERU\\4439690", "810117V|k0");
				#endregion

				#region Search User & Groups 1
				//await Demo_SearchUsersAndGroupsByAttributeAsync(LDAPHelper.DTO.EntryAttribute.sAMAccountName, "usr_ext01", LDAPHelper.DTO.RequiredEntryAttributes.All);

				await Demo_SearchUsersAndGroupsByAttributeAsync(LDAPHelper.DTO.EntryAttribute.sAMAccountName, "4439690", LDAPHelper.DTO.RequiredEntryAttributes.All);

				//await Demo_SearchUsersAndGroupsByAttributeAsync(LDAPHelper.DTO.EntryAttribute.cn, "*bastidas*", LDAPHelper.DTO.RequiredEntryAttributes.All);

				//await Demo_SearchUsersAndGroupsByAttributeAsync(LDAPHelper.DTO.EntryAttribute.cn, "Cristian Hernán*", LDAPHelper.DTO.RequiredEntryAttributes.All);

				//_filterValue = "CN=Grupo Seguridad Lanperu (LANPERU),OU=Security,OU=Groups,OU=LAN Peru,OU=Equipos,DC=pe,DC=lan,DC=com";
				//_filterValue = _filterValue.ReplaceSpecialCharsToScapedChars();
				//await Demo_SearchUsersAndGroupsByAttributeAsync(LDAPHelper.DTO.EntryAttribute.distinguishedName, _filterValue, LDAPHelper.DTO.RequiredEntryAttributes.All);
				#endregion

				#region Search User & Groups 2
				//_filterValue = "*simon_*";
				//await Demo_SearchUsersAndGroupsBy2AttributesAsync(LDAPHelper.DTO.EntryAttribute.sAMAccountName, _filterValue, LDAPHelper.DTO.EntryAttribute.cn, _filterValue, false, DTO.RequiredEntryAttributes.All);

				await Demo_SearchUsersAndGroupsBy2AttributesAsync(LDAPHelper.DTO.EntryAttribute.sAMAccountName, "*administrator*", LDAPHelper.DTO.EntryAttribute.cn, "*administrator*", true, DTO.RequiredEntryAttributes.All);
				#endregion

				#region Seaarch Any kind of Entries 
				await Demo_GetEntriesByAttributeAsync(LDAPHelper.DTO.EntryAttribute.sAMAccountName, "usr*", DTO.RequiredEntryAttributes.All);

				await Demo_GetEntriesByAttributeAsync(LDAPHelper.DTO.EntryAttribute.objectSid, "S-1-5-21-638406840-1180129177-883519231-179439", DTO.RequiredEntryAttributes.All);

				//await Demo_GetEntriesByAttributeAsync(true, LDAPHelper.DTO.EntryAttribute.sAMAccountName, "usr*", BaseDNEnum.br_com);

				//await Demo_GetEntriesByAttributeAsync(true, LDAPHelper.DTO.EntryAttribute.sAMAccountName, "usr*", BaseDNEnum.com);

				//await Demo_GetEntriesByAttributeAsync(false, LDAPHelper.DTO.EntryAttribute.sAMAccountName, "victor.bastidas.g");

				//await Demo_GetEntriesByAttributeAsync(true, LDAPHelper.DTO.EntryAttribute.objectSid, "S-1-5-21-1913140505-990805927-1540833222-54805", BaseDNEnum.br_com);

				//await Demo_GetEntriesByAttributeAsync(true, LDAPHelper.DTO.EntryAttribute.objectSid, "S-1-5-21-1913140505-990805927-1540833222-54805", BaseDNEnum.com);

				//await Demo_GetEntriesByAttributeAsync(false, LDAPHelper.DTO.EntryAttribute.objectSid, "S-1-5-21-1913140505-990805927-1540833222-54805");

				//await Demo_GetEntriesByAttributeAsync(true, LDAPHelper.DTO.EntryAttribute.distinguishedName, "CN=SIMON_Administrador,OU=Aplicaciones,OU=Grupos,DC=cl,DC=lan,DC=com", BaseDNEnum.cl_com);

				//await Demo_GetEntriesByAttributeAsync(true, LDAPHelper.DTO.EntryAttribute.distinguishedName, "CN=SIMON_Administrador,OU=Aplicaciones,OU=Grupos,DC=cl,DC=lan,DC=com", BaseDNEnum.br_com);

				//await Demo_GetEntriesByAttributeAsync(true, LDAPHelper.DTO.EntryAttribute.distinguishedName, "CN=SIMON_Administrador,OU=Aplicaciones,OU=Grupos,DC=cl,DC=lan,DC=com", BaseDNEnum.com);

				//await Demo_GetEntriesByAttributeAsync(false, LDAPHelper.DTO.EntryAttribute.distinguishedName, "CN=SIMON_Administrador,OU=Aplicaciones,OU=Grupos,DC=cl,DC=lan,DC=com");
				#endregion


				//await Demo_GetGroupMembershipEntriesAsync(true, LDAPHelper.DTO.KeyEntryAttribute.sAMAccountName, "usr_ext01", BaseDNEnum.com);

				//await Demo_GetGroupMembershipEntriesAsync(true, LDAPHelper.DTO.KeyEntryAttribute.sAMAccountName, "4303259", BaseDNEnum.com);

				//await Demo_GetGroupMembershipEntriesAsync(true, LDAPHelper.DTO.KeyEntryAttribute.distinguishedName, "CN=Administrator,CN=Users,DC=pe,DC=lan,DC=com", true, BaseDNEnum.cl_com);

				//await Demo_GetGroupMembershipEntriesAsync(true, LDAPHelper.DTO.KeyEntryAttribute.distinguishedName, "CN=Administrator,CN=Users,DC=pe,DC=lan,DC=com", true, BaseDNEnum.com);

				//await Demo_GetGroupMembershipEntriesAsync(false, LDAPHelper.DTO.KeyEntryAttribute.distinguishedName, "CN=Administrator,CN=Users,DC=pe,DC=lan,DC=com", true);

				//await Demo_GetGroupMembershipEntriesAsync(true, LDAPHelper.DTO.KeyEntryAttribute.sAMAccountName, "victor.bastidas.g", BaseDNEnum.br_com);

				//await Demo_GetGroupMembershipEntriesAsync(true, LDAPHelper.DTO.KeyEntryAttribute.sAMAccountName, "victor.bastidas.g", BaseDNEnum.cl_com);

				//await Demo_GetGroupMembershipEntriesAsync(true, LDAPHelper.DTO.KeyEntryAttribute.sAMAccountName, "victor.bastidas.g", BaseDNEnum.com);

				//await Demo_GetGroupMembershipEntriesAsync(false, LDAPHelper.DTO.KeyEntryAttribute.sAMAccountName, "victor.bastidas.g");

				//await Demo_GetGroupMembershipEntriesAsync(true, LDAPHelper.DTO.KeyEntryAttribute.objectSid, "S-1-5-21-1913140505-990805927-1540833222-54805", true, BaseDNEnum.br_com);

				//await Demo_GetGroupMembershipEntriesAsync(true, LDAPHelper.DTO.KeyEntryAttribute.objectSid, "S-1-5-21-1913140505-990805927-1540833222-54805", true, BaseDNEnum.cl_com);

				//await Demo_GetGroupMembershipEntriesAsync(true, LDAPHelper.DTO.KeyEntryAttribute.objectSid, "S-1-5-21-1913140505-990805927-1540833222-54805", true, BaseDNEnum.com);

				//await Demo_GetGroupMembershipEntriesAsync(false, LDAPHelper.DTO.KeyEntryAttribute.objectSid, "S-1-5-21-1913140505-990805927-1540833222-54805", true);



				//await Demo_GetGroupMembershipEntriesAsync(true, LDAPHelper.DTO.EntryAttribute.sAMAccountName, "4303259", BaseDNEnum.com);

				var _yes = requestYESoNO("Dou you want to execute DEMPO again?");
				if (_yes)
					goto EXECUTE_DEMO;

				Console.WriteLine();

				Log.Warning("Fin: " + DateTime.Now.ToString());
			}
			catch (Exception ex)
			{
				Log.Error("***************************************************");
				Log.Error("Error en la ejecución.");
				Log.Error(ex.Message);
				Log.Error("***************************************************");
			}
			finally
			{
				Console.ReadLine();
			}
		}

		public static async Task<bool> Test_AuthenticateUser(string domainUsername, string password)
		{
			try
			{
				logTestTitle("Test00_Authenticate");

				var _c = new LDAPHelper.LdhAuthenticator(getConnectionInfo());

				Log.Information("Autenticando usuario...");
				var _auth = await _c.AuthenticateUser(new LDAPHelper.LdhUserCredentials(domainUsername, password));
				if (_auth)
					Log.Information("Usuario autententicado.");
				else
					Log.Warning("Usuario NO autententicado.");

				return true;
			}
			catch (Exception ex)
			{
				Console.WriteLine();
				Log.Error(ex.Message);
				Log.Error(ex.StackTrace);

				return false;
			}
		}

		public static async Task<bool> Demo_SearchUsersAndGroupsByAttributeAsync(LDAPHelper.DTO.EntryAttribute filterAttribute, string filterValue, LDAPHelper.DTO.RequiredEntryAttributes requiredResults)
		{
			try
			{
				logTestTitle("Demo_SearchUsersAndGroupsByAttributeAsync");

				var _s = new LDAPHelper.LdhSearcher(getClientConfiguration());

				Log.Information(string.Format("Base DN: {0}", _s.SearchLimits.BaseDN));
				Log.Information(string.Format("Búscando con el atributo {0}: {1}...", filterAttribute.ToString(), filterValue));
				Console.WriteLine();

				var _result = await _s.SearchUsersAndGroupsAsync(filterAttribute, filterValue, requiredResults, Program.Tag);

				if (_result.Entries.Count() == 0)
				{
					if (_result.HasErrorInfo)
					{
						Log.Error(_result.ErrorType);
						Log.Error(_result.ErrorMessage);
					}
					else
					{
						Log.Warning("No se encontraron registros con el criterio de búsqueda proporcionado.");
					}
				}
				else
				{
					foreach (var _entry in _result.Entries)
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

					Log.Information(string.Format("{0} entries.", _result.Entries.Count().ToString()));
				}

				return true;
			}
			catch (Exception ex)
			{
				Console.WriteLine();
				Log.Error(ex.Message);
				Log.Error(ex.StackTrace);

				return false;
			}
		}

		public static async Task<bool> Demo_SearchUsersAndGroupsBy2AttributesAsync(LDAPHelper.DTO.EntryAttribute filterAttribute, string filterValue, LDAPHelper.DTO.EntryAttribute secondFilterAttribute, string secondFilterValue, bool conjunctiveFilters, LDAPHelper.DTO.RequiredEntryAttributes requiredResults)
		{
			if (string.IsNullOrEmpty(filterValue))
			{
				throw new ArgumentException($"'{nameof(filterValue)}' cannot be null or empty.", nameof(filterValue));
			}

			if (string.IsNullOrEmpty(secondFilterValue))
			{
				throw new ArgumentException($"'{nameof(secondFilterValue)}' cannot be null or empty.", nameof(secondFilterValue));
			}

			try
			{
				logTestTitle("Demo_SearchUsersAndGroupsBy2AttributesAsync");

				var _s = new LDAPHelper.LdhSearcher(getClientConfiguration());

				Log.Information(string.Format("Base DN: {0}", _s.SearchLimits.BaseDN));
				Log.Information(string.Format("Búscando con el atributo {1}: {2} {0} {3}: {4}", (conjunctiveFilters ? " Y " : " O "), filterAttribute.ToString(), filterValue, secondFilterAttribute.ToString(), secondFilterValue));
				Console.WriteLine();

				var _result = await _s.SearchUsersAndGroupsAsync(filterAttribute, filterValue, secondFilterAttribute, secondFilterValue, conjunctiveFilters, requiredResults, Program.Tag);
				if (_result.Entries.Count().Equals(0))
					Log.Warning("No se encontraron registros con el criterio de búsqueda proporcionado.");
				else
				{
					foreach (var _entry in _result.Entries)
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

					Log.Information(string.Format("{0} entries.", _result.Entries.Count()));
				}

				return true;
			}
			catch (Exception ex)
			{
				Console.WriteLine();
				Log.Error(ex.Message);

				return false;
			}
		}

		public static async Task<bool> Demo_GetEntriesByAttributeAsync(LDAPHelper.DTO.EntryAttribute attribute, string attributeFilter, LDAPHelper.DTO.RequiredEntryAttributes requiredResults)
		{
			try
			{
				logTestTitle("Demo_GetEntriesByAttributeAsync");

				var _s = new LDAPHelper.LdhSearcher(getClientConfiguration());

				Log.Information(string.Format("Base DN: {0}", _s.SearchLimits.BaseDN));
				Log.Information(string.Format("Búscando en por {0}: {1}...", attribute.ToString(), attributeFilter));
				Console.WriteLine();

				var _result = await _s.SearchEntriesAsync(attribute, attributeFilter, requiredResults, Program.Tag);
				if (_result.Entries.Count().Equals(0))
					Log.Warning(string.Format("No se encontraron datos para {0}: {1}", attribute.ToString(), attributeFilter));
				else
				{
					Log.Information(string.Format("Se encontraron datos para {0}: {1}", attribute.ToString(), attributeFilter));
					Console.WriteLine();
					foreach (var _entry in _result.Entries)
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
				}

				return true;
			}
			catch (Exception ex)
			{
				Console.WriteLine();
				Log.Error(ex.Message);

				return false;
			}
		}

		public static async Task<bool> Demo_GetGroupMembershipEntriesAsync(LDAPHelper.DTO.EntryKeyAttribute filterAttribute, string filterValue, LDAPHelper.DTO.RequiredEntryAttributes requiredResults, bool recursive)
		{
			try
			{
				logTestTitle("Demo_GetGroupMembershipEntriesAsync");

				var _s = new LDAPHelper.LdhSearcher(getClientConfiguration());

				Log.Information(string.Format("Base DN: {0}", _s.SearchLimits.BaseDN));
				Log.Information(string.Format("Obteniendo grupos a los que pertenece el {0}: {1}...", filterAttribute.ToString(), filterValue));
				Console.WriteLine();

				var _result = await _s.SearchGroupMembershipEntriesAsync(filterAttribute, filterValue, requiredResults, Program.Tag, recursive);

				foreach (var _entry in _result.Entries.OrderBy(f => f.distinguishedName))
					Log.Information(_entry.distinguishedName);

				Log.Warning(string.Format("{0} entries.", _result.Entries.Count()));

				return true;
			}
			catch (Exception ex)
			{
				Console.WriteLine();
				Log.Error(ex.Message);

				return false;
			}
		}

		public static async Task<bool> Demo_GetGroupMembershipCNs(LDAPHelper.DTO.EntryKeyAttribute keyAttribute, string keyAttributeFilter, LDAPHelper.DTO.RequiredEntryAttributes requiredResults, bool recursive)
		{
			try
			{
				logTestTitle("Demo_GetGroupMembershipCNs");

				var _s = new LDAPHelper.LdhSearcher(getClientConfiguration());

				Log.Information(string.Format("Base DN: {0}", _s.SearchLimits.BaseDN));
				Log.Information(string.Format("Obteniendo grupos a los que pertenece el {0}: {1}...", keyAttribute, keyAttributeFilter));
				Console.WriteLine();

				var _results = await _s.SearchGroupMembershipCNsAsync(keyAttribute, keyAttributeFilter, Program.Tag, recursive);

				foreach (var _entry in _results)
					Log.Information(_entry);

				Log.Warning(string.Format("{0} entries.", _results.Count()));

				return true;
			}
			catch (Exception ex)
			{
				Console.WriteLine();
				Log.Error(ex.Message);

				return false;
			}
		}

		public static async Task<bool> Demo_GetGroupMembershipEntriesAsync(string ldapServer, int port, string baseDN, LDAPHelper.DTO.EntryAttribute attribute, string attributeFilter, LDAPHelper.DTO.RequiredEntryAttributes requiredResults, bool recursive)
		{
			try
			{
				logTestTitle("Demo_GetGroupMembershipEntriesAsync");

				var _s = new LDAPHelper.LdhSearcher(getClientConfiguration());

				Log.Information(string.Format("Base DN: {0}", _s.SearchLimits.BaseDN));
				Log.Information(string.Format("Obteniendo grupos a los que pertenece el {0}: {1}...", attribute.ToString(), attributeFilter));
				Console.WriteLine();

				var _results = await _s.SearchGroupMembershipEntriesAsync(attribute, attributeFilter, LDAPHelper.DTO.RequiredEntryAttributes.MinimunWithMemberAndMemberOf, Program.Tag, recursive);

				foreach (var _entry in _results.Entries.OrderBy(f => f.distinguishedName))
					Log.Information(_entry.distinguishedName);

				Log.Warning(string.Format("{0} entries.", _results.Entries.Count()));

				return true;
			}
			catch (Exception ex)
			{
				Console.WriteLine();
				Log.Error(ex.Message);

				return false;
			}
		}
	}
}