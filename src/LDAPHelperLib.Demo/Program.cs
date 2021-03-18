using System;
using System.Threading.Tasks;
using Serilog;
using Serilog.Sinks.SystemConsole;
using System.Linq;
using System.Security;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace LDAPHelperLib.Demo
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
			Program.Selected_LdapServerPort = (int)LdapHelperLib.LdapServerCommonPorts.DefaultPort;
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

		private static LdapHelperLib.LdhConnectionInfo getConnectionInfo()
		{
			return new LdapHelperLib.LdhConnectionInfo(Program.Selected_LdapServer, Program.Selected_LdapServerPort, Program.Selected_UseSsl, Program.Selected_ConnectionTimeout);
		}

		private static LdapHelperLib.LdhUserCredentials getCredentials()
		{
			return new LdapHelperLib.LdhUserCredentials(Program.Selected_DomainAccountName, Program.Selected_AccountNamePassword);
		}

		public static LdapHelperLib.LdhSearchLimits getSearchLimits()
		{
			return new LdapHelperLib.LdhSearchLimits(Program.Selected_BaseDN);
		}

		private static LdapHelperLib.LdhClientConfiguration getClientConfiguration()
		{
			return new LdapHelperLib.LdhClientConfiguration(getConnectionInfo(), getCredentials(), getSearchLimits());
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
			Console.WriteLine($"(1) Default -> {(int)LdapHelperLib.LdapServerCommonPorts.DefaultPort}");
			Console.WriteLine($"(2) Default Global Catalog -> {(int)LdapHelperLib.LdapServerCommonPorts.GlobalCatalogPort}");
			Console.WriteLine($"(3) Default SSL -> {(int)LdapHelperLib.LdapServerCommonPorts.DefaultSslPort}");
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
							Program.Selected_LdapServerPort = (int)LdapHelperLib.LdapServerCommonPorts.DefaultPort;
							break;
						case 2:
							Program.Selected_LdapServerPort = (int)LdapHelperLib.LdapServerCommonPorts.GlobalCatalogPort;
							break;
						case 3:
							Program.Selected_LdapServerPort = (int)LdapHelperLib.LdapServerCommonPorts.DefaultSslPort;
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
				Log.Information("LdapHelperLib Test (.NET Core Console)");
				Console.WriteLine();

				Log.Warning("Inicio: " + DateTime.Now.ToString());

				loadSetup();

				requestLdapServer();

				requestLdapServerPort();

				requesAccounNamePassword();

				requestBaseDN();

				//await Test_AuthenticateUser("LANPERU\\usr_ext01", "L4t4m2018");
				//await Test_AuthenticateUser("LANPERU\\4439690", "810117V|k0");

				await Demo_SearchUsersAndGroupsByAttributeAsync(LdapHelperDTO.EntryAttribute.sAMAccountName, "usr_ext01", LdapHelperDTO.RequiredEntryAttributes.All);

				await Demo_SearchUsersAndGroupsByAttributeAsync(LdapHelperDTO.EntryAttribute.sAMAccountName, "4270826", LdapHelperDTO.RequiredEntryAttributes.All);

				await Demo_SearchUsersAndGroupsByAttributeAsync(LdapHelperDTO.EntryAttribute.cn, "*bastidas*", LdapHelperDTO.RequiredEntryAttributes.All);

				await Demo_SearchUsersAndGroupsByAttributeAsync(LdapHelperDTO.EntryAttribute.cn, "Cristian Hernán*", LdapHelperDTO.RequiredEntryAttributes.All);

				await Demo_SearchUsersAndGroupsByAttributeAsync(LdapHelperDTO.EntryAttribute.distinguishedName, "CN=Grupo Seguridad Lanperu (LANPERU),OU=Security,OU=Groups,OU=LAN Peru,OU=Equipos,DC=pe,DC=lan,DC=com", LdapHelperDTO.RequiredEntryAttributes.All);




				//_attributeFiler = "*" + LdapHelperLib.Utils.ConvertspecialCharsToScapedChars("simon_") + "*";
				//await Demo_SearchUsersAndGroupsBy2AttributesAsync(true, LdapHelperDTO.EntryAttribute.sAMAccountName, _attributeFiler, LdapHelperDTO.EntryAttribute.cn, _attributeFiler, false, BaseDNEnum.cl_com);

				//await Demo_SearchUsersAndGroupsBy2AttributesAsync(false, LdapHelperDTO.EntryAttribute.sAMAccountName, "*administrator*", LdapHelperDTO.EntryAttribute.cn, "*administrator*", true);

				//await Demo_SearchUsersAndGroupsBy2AttributesAsync(true, LdapHelperDTO.EntryAttribute.sAMAccountName, "4250889", LdapHelperDTO.EntryAttribute.cn, "4250889", false);

				//await Demo_SearchUsersAndGroupsBy2AttributesAsync(true, LdapHelperDTO.EntryAttribute.sAMAccountName, "4238977", LdapHelperDTO.EntryAttribute.cn, "4238977", false);

				//await Demo_SearchUsersAndGroupsBy2AttributesAsync(true, LdapHelperDTO.EntryAttribute.sAMAccountName, "atordeur", LdapHelperDTO.EntryAttribute.cn, "atordeur", false, BaseDNEnum.cl_com);

				//await Demo_SearchUsersAndGroupsBy2AttributesAsync(true, LdapHelperDTO.EntryAttribute.sAMAccountName, "atordeur", LdapHelperDTO.EntryAttribute.cn, "*tordeur*", false, BaseDNEnum.cl_com);



				//await Demo_GetEntriesByAttributeAsync(true, LdapHelperDTO.EntryAttribute.sAMAccountName, "usr*", BaseDNEnum.cl_com);

				//await Demo_GetEntriesByAttributeAsync(true, LdapHelperDTO.EntryAttribute.sAMAccountName, "usr*", BaseDNEnum.br_com);

				//await Demo_GetEntriesByAttributeAsync(true, LdapHelperDTO.EntryAttribute.sAMAccountName, "usr*", BaseDNEnum.com);

				//await Demo_GetEntriesByAttributeAsync(false, LdapHelperDTO.EntryAttribute.sAMAccountName, "victor.bastidas.g");

				//await Demo_GetEntriesByAttributeAsync(true, LdapHelperDTO.EntryAttribute.objectSid, "S-1-5-21-1913140505-990805927-1540833222-54805", BaseDNEnum.cl_com);

				//await Demo_GetEntriesByAttributeAsync(true, LdapHelperDTO.EntryAttribute.objectSid, "S-1-5-21-1913140505-990805927-1540833222-54805", BaseDNEnum.br_com);

				//await Demo_GetEntriesByAttributeAsync(true, LdapHelperDTO.EntryAttribute.objectSid, "S-1-5-21-1913140505-990805927-1540833222-54805", BaseDNEnum.com);

				//await Demo_GetEntriesByAttributeAsync(false, LdapHelperDTO.EntryAttribute.objectSid, "S-1-5-21-1913140505-990805927-1540833222-54805");

				//await Demo_GetEntriesByAttributeAsync(true, LdapHelperDTO.EntryAttribute.distinguishedName, "CN=SIMON_Administrador,OU=Aplicaciones,OU=Grupos,DC=cl,DC=lan,DC=com", BaseDNEnum.cl_com);

				//await Demo_GetEntriesByAttributeAsync(true, LdapHelperDTO.EntryAttribute.distinguishedName, "CN=SIMON_Administrador,OU=Aplicaciones,OU=Grupos,DC=cl,DC=lan,DC=com", BaseDNEnum.br_com);

				//await Demo_GetEntriesByAttributeAsync(true, LdapHelperDTO.EntryAttribute.distinguishedName, "CN=SIMON_Administrador,OU=Aplicaciones,OU=Grupos,DC=cl,DC=lan,DC=com", BaseDNEnum.com);

				//await Demo_GetEntriesByAttributeAsync(false, LdapHelperDTO.EntryAttribute.distinguishedName, "CN=SIMON_Administrador,OU=Aplicaciones,OU=Grupos,DC=cl,DC=lan,DC=com");



				//await Demo_GetGroupMembershipEntriesAsync(true, LdapHelperDTO.KeyEntryAttribute.sAMAccountName, "usr_ext01", BaseDNEnum.com);

				//await Demo_GetGroupMembershipEntriesAsync(true, LdapHelperDTO.KeyEntryAttribute.sAMAccountName, "4303259", BaseDNEnum.com);

				//await Demo_GetGroupMembershipEntriesAsync(true, LdapHelperDTO.KeyEntryAttribute.distinguishedName, "CN=Administrator,CN=Users,DC=pe,DC=lan,DC=com", true, BaseDNEnum.cl_com);

				//await Demo_GetGroupMembershipEntriesAsync(true, LdapHelperDTO.KeyEntryAttribute.distinguishedName, "CN=Administrator,CN=Users,DC=pe,DC=lan,DC=com", true, BaseDNEnum.com);

				//await Demo_GetGroupMembershipEntriesAsync(false, LdapHelperDTO.KeyEntryAttribute.distinguishedName, "CN=Administrator,CN=Users,DC=pe,DC=lan,DC=com", true);

				//await Demo_GetGroupMembershipEntriesAsync(true, LdapHelperDTO.KeyEntryAttribute.sAMAccountName, "victor.bastidas.g", BaseDNEnum.br_com);

				//await Demo_GetGroupMembershipEntriesAsync(true, LdapHelperDTO.KeyEntryAttribute.sAMAccountName, "victor.bastidas.g", BaseDNEnum.cl_com);

				//await Demo_GetGroupMembershipEntriesAsync(true, LdapHelperDTO.KeyEntryAttribute.sAMAccountName, "victor.bastidas.g", BaseDNEnum.com);

				//await Demo_GetGroupMembershipEntriesAsync(false, LdapHelperDTO.KeyEntryAttribute.sAMAccountName, "victor.bastidas.g");

				//await Demo_GetGroupMembershipEntriesAsync(true, LdapHelperDTO.KeyEntryAttribute.objectSid, "S-1-5-21-1913140505-990805927-1540833222-54805", true, BaseDNEnum.br_com);

				//await Demo_GetGroupMembershipEntriesAsync(true, LdapHelperDTO.KeyEntryAttribute.objectSid, "S-1-5-21-1913140505-990805927-1540833222-54805", true, BaseDNEnum.cl_com);

				//await Demo_GetGroupMembershipEntriesAsync(true, LdapHelperDTO.KeyEntryAttribute.objectSid, "S-1-5-21-1913140505-990805927-1540833222-54805", true, BaseDNEnum.com);

				//await Demo_GetGroupMembershipEntriesAsync(false, LdapHelperDTO.KeyEntryAttribute.objectSid, "S-1-5-21-1913140505-990805927-1540833222-54805", true);



				//await Demo_GetGroupMembershipEntriesAsync(true, LdapHelperDTO.EntryAttribute.sAMAccountName, "4303259", BaseDNEnum.com);


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

				var _c = new LdapHelperLib.LdhAuthenticator(getConnectionInfo());

				Log.Information("Autenticando usuario...");
				var _auth = await _c.AuthenticateUser(new LdapHelperLib.LdhUserCredentials(domainUsername, password));
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

		public static async Task<bool> Demo_SearchUsersAndGroupsByAttributeAsync(LdapHelperDTO.EntryAttribute filterAttribute, string filterValue, LdapHelperDTO.RequiredEntryAttributes requiredResults)
		{
			try
			{
				logTestTitle("Demo_SearchUsersAndGroupsByAttributeAsync");

				var _s = new LdapHelperLib.LdhSearcher(getClientConfiguration());

				Log.Information(string.Format("Base DN: {0}", _s.SearchLimits.BaseDN));
				Log.Information(string.Format("Búscando con el atributo {0}: {1}...", filterAttribute.ToString(), filterValue));
				Console.WriteLine();

				var _result = await _s.SearchUsersAndGroupsByAttributeAsync(filterAttribute, filterValue, requiredResults, Program.Tag);

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
					foreach (var _i in _result.Entries)
					{
						Log.Information(_i.cn);
						Log.Information(_i.objectSid);
						Log.Information(_i.samAccountName);
						Log.Information(_i.distinguishedName);
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
		
		public static async Task<bool> Demo_SearchUsersAndGroupsBy2AttributesAsync(LdapHelperDTO.EntryAttribute filterAttribute, string filterValue, LdapHelperDTO.EntryAttribute secondFilterAttribute, string secondFilterValue, bool conjunctiveFilters, LdapHelperDTO.RequiredEntryAttributes requiredResults)
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

				var _s = new LdapHelperLib.LdhSearcher(getClientConfiguration());

				Log.Information(string.Format("Base DN: {0}", _s.SearchLimits.BaseDN));
				Log.Information(string.Format("Búscando con el atributo {1}: {2} {0} {3}: {4}", (conjunctiveFilters ? " Y " : " O "), filterAttribute.ToString(), filterValue, secondFilterAttribute.ToString(), secondFilterValue));
				Console.WriteLine();

				var _result = await _s.SearchUsersAndGroupsBy2AttributesAsync(filterAttribute, filterValue, secondFilterAttribute, secondFilterValue, conjunctiveFilters, requiredResults, Program.Tag);
				if (_result.Entries.Count().Equals(0))
					Log.Warning("No se encontraron registros con el criterio de búsqueda proporcionado.");
				else
				{
					foreach (var _i in _result.Entries)
					{
						Log.Information(_i.cn);
						Log.Information(_i.objectSid);
						Log.Information(_i.samAccountName);
						Log.Information(_i.distinguishedName);
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

		public static async Task<bool> Demo_GetEntriesByAttributeAsync(LdapHelperDTO.EntryAttribute attribute, string attributeFilter, LdapHelperDTO.RequiredEntryAttributes requiredResults)
		{
			try
			{
				logTestTitle("Demo_GetEntriesByAttributeAsync");

				var _s = new LdapHelperLib.LdhSearcher(getClientConfiguration());

				Log.Information(string.Format("Base DN: {0}", _s.SearchLimits.BaseDN));
				Log.Information(string.Format("Búscando en por {0}: {1}...", attribute.ToString(), attributeFilter));
				Console.WriteLine();

				var _result = await _s.SearchEntriesByAttributeAsync(attribute, attributeFilter, requiredResults, Program.Tag);
				if (_result.Entries.Count().Equals(0))
					Log.Warning(string.Format("No se encontraron datos para {0}: {1}", attribute.ToString(), attributeFilter));
				else
				{
					Log.Information(string.Format("Se encontraron datos para {0}: {1}", attribute.ToString(), attributeFilter));
					Console.WriteLine();
					foreach (var _entry in _result.Entries)
					{
						Log.Information(_entry.cn);
						Log.Information(_entry.objectSid);
						Log.Information(_entry.samAccountName);
						Log.Information(_entry.distinguishedName);
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

		public static async Task<bool> Demo_GetGroupMembershipEntriesAsync(LdapHelperDTO.EntryKeyAttribute filterAttribute, string filterValue, LdapHelperDTO.RequiredEntryAttributes requiredResults, bool recursive)
		{
			try
			{
				logTestTitle("Demo_GetGroupMembershipEntriesAsync");

				var _s = new LdapHelperLib.LdhSearcher(getClientConfiguration());

				Log.Information(string.Format("Base DN: {0}", _s.SearchLimits.BaseDN));
				Log.Information(string.Format("Obteniendo grupos a los que pertenece el {0}: {1}...", filterAttribute.ToString(), filterValue));
				Console.WriteLine();

				var _result = await _s.SearchGroupMembershipEntries(filterAttribute, filterValue, requiredResults, Program.Tag, recursive);

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

		public static async Task<bool> Demo_GetGroupMembershipCNs(LdapHelperDTO.EntryKeyAttribute keyAttribute, string keyAttributeFilter, LdapHelperDTO.RequiredEntryAttributes requiredResults, bool recursive)
		{
			try
			{
				logTestTitle("Demo_GetGroupMembershipCNs");

				var _s = new LdapHelperLib.LdhSearcher(getClientConfiguration());

				Log.Information(string.Format("Base DN: {0}", _s.SearchLimits.BaseDN));
				Log.Information(string.Format("Obteniendo grupos a los que pertenece el {0}: {1}...", keyAttribute, keyAttributeFilter));
				Console.WriteLine();

				var _results = await _s.SearchGroupMembershipCNs(keyAttribute, keyAttributeFilter, Program.Tag, recursive);

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

		public static async Task<bool> Demo_GetGroupMembershipEntriesAsync(string ldapServer, int port, string baseDN, LdapHelperDTO.EntryAttribute attribute, string attributeFilter, LdapHelperDTO.RequiredEntryAttributes requiredResults, bool recursive)
		{
			try
			{
				logTestTitle("Demo_GetGroupMembershipEntriesAsync");

				var _s = new LdapHelperLib.LdhSearcher(getClientConfiguration());

				Log.Information(string.Format("Base DN: {0}", _s.SearchLimits.BaseDN));
				Log.Information(string.Format("Obteniendo grupos a los que pertenece el {0}: {1}...", attribute.ToString(), attributeFilter));
				Console.WriteLine();

				var _results = await _s.SearchGroupMembershipEntries(attribute, attributeFilter, LdapHelperDTO.RequiredEntryAttributes.MinimunWithMemberAndMemberOf, Program.Tag, recursive);

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