using System;
using System.Threading.Tasks;
using Serilog;
using Serilog.Sinks.SystemConsole;
using System.Linq;
using System.Security;

namespace LDAPHelperLib.Demo
{
	//public enum BaseDNEnum
	//{
	//	com,
	//	cl_com,
	//	br_com,
	//	pe_com,
	//	ar_com,
	//}

	public class Program
	{
		/// <summary>
		/// Custom (optional) tag value to label request. Can be null. 
		/// </summary>
		internal static string Tag = "My Demo";
		/// <summary>
		/// LDAP Server ip or DNS name
		/// </summary>
		internal static string LdapServer = "10.11.58.13";
		/// <summary>
		/// LDAP Server port 
		/// </summary>
		internal static int LdapServerPort = (int)LdapHelperLib.LdapServerCommonPorts.DefaultPort;
		internal static bool UseSsl = false;
		internal static short ConnectionTimeout = 15;
		/// <summary>
		/// Domain Username to connect LDAP Server
		/// </summary>
		internal static string DomainUserName = "LANPERU\\4439690";
		/// <summary>
		/// Domain Username passsword
		/// </summary>
		internal static string AccountPassword = "810117V|k0";
		/// <summary>
		/// Example of Base DN (Distinguished Name) that defines the minimum scope of directory searches 
		/// </summary>
		internal static string BaseDN_1 = "DC=lan, DC=com";
		/// <summary>
		/// Example of Base DN (Distinguished Name) that defines the minimum scope of directory searches 
		/// </summary>
		internal static string BaseDN_2 = "DC=pe, DC=lan, DC=com";
		/// <summary>
		/// Example of Base DN (Distinguished Name) that defines the minimum scope of directory searches 
		/// </summary>
		internal static string BaseDN_3 = "DC=cl, DC=lan, DC=com";
		/// <summary>
		/// Example of Base DN (Distinguished Name) that defines the minimum scope of directory searches 
		/// </summary>
		internal static string BaseDN_4 = "DC=ar, DC=lan, DC=com";
		/// <summary>
		/// Example of Base DN (Distinguished Name) that defines the minimum scope of directory searches 
		/// </summary>
		internal static string BaseDN_5 = "DC=br, DC=lan, DC=com";
		/// <summary>
		/// Example of Base DN (Distinguished Name) that defines the minimum scope of directory searches 
		/// </summary>
		internal static string BaseDN_6 = "DC=us, DC=lan, DC=com";
		/// <summary>
		/// Example of Base DN (Distinguished Name) that defines the minimum scope of directory searches 
		/// </summary>
		internal static string BaseDN_7 = "DC=ca, DC=lan, DC=com";
		internal static string SelectedBaseDN = BaseDN_1;


		#region Private Static Methods
		private static void configLog()
		{
			Log.Logger = new LoggerConfiguration()
				.WriteTo.Console()
				.CreateLogger();
		}

		private static void log_TestTitle(string title)
		{
			Console.WriteLine();
			Console.WriteLine("**********************************************");
			Console.WriteLine("  " + title);
			Console.WriteLine("**********************************************");
		}

		private static LdapHelperLib.LdapConnectionInfo getConnectionInfo()
		{
			return new LdapHelperLib.LdapConnectionInfo(Program.LdapServer, Program.LdapServerPort, Program.UseSsl, Program.ConnectionTimeout, getCredentials());
		}

		private static LdapHelperLib.LdapUserCredentials getCredentials()
		{
			return new LdapHelperLib.LdapUserCredentials(Program.DomainUserName, Program.AccountPassword);
		}

		private static LdapHelperLib.LdapClientConfiguration getClientConfiguration()
		{
			return new LdapHelperLib.LdapClientConfiguration(getConnectionInfo(), getCredentials(), Program.SelectedBaseDN);
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

				string _attributeFiler = null;

				Console.WriteLine("Enter LDAP server IP address or DNS name:");
				Console.WriteLine($"Leave empty to use {Program.LdapServer}");
				var _ldapServerEntered = Console.ReadLine();
				if (string.IsNullOrEmpty(_ldapServerEntered))
					_ldapServerEntered = Program.LdapServer;
				Log.Information($"Will use {_ldapServerEntered} LDAP Server.");

				Console.WriteLine("Enter LDAP server port:");
				Console.WriteLine($"Leave empty to use {Program.LdapServerPort}");
				var _portEntered = Console.ReadLine();
				var _portNumber = string.IsNullOrEmpty(_portEntered) ? Program.LdapServerPort : Convert.ToInt32(_portEntered);
				Log.Information($"Will use port:{_portNumber} to connect LDAP Server.");

				await Test_AuthenticateUser("LANPERU\\usr_ext01", "L4t4m2018");
				//await Test_AuthenticateUser(true, BaseDNEnum.br_com);



				//await Test_GetUsersAndGroupsByAttributeEnumerable(true, LdapHelperDTO.EntryAttribute.sAMAccountName, "usr_ext01", BaseDNEnum.com);

				//await Test_GetUsersAndGroupsByAttributeQueuedResult(true, LdapHelperDTO.EntryAttribute.sAMAccountName, "usr_ext01", BaseDNEnum.com);

				//await Test_GetUsersAndGroupsByAttributeQueuedResult(true, LdapHelperDTO.EntryAttribute.cn, "*caba*", BaseDNEnum.ar_com);

				//await Test_GetUsersAndGroupsByAttributeQueuedResult(true, LdapHelperDTO.EntryAttribute.cn, "*caba*", BaseDNEnum.br_com);

				//await Test_GetUsersAndGroupsByAttributeQueuedResult(true, LdapHelperDTO.EntryAttribute.cn, "*caba*", BaseDNEnum.cl_com);

				//await Test_GetUsersAndGroupsByAttributeQueuedResult(true, LdapHelperDTO.EntryAttribute.cn, "Incandela", BaseDNEnum.pe_com, WildcardTypeEnum.atBeginningAtEnd);

				//_attributeFiler = "*" + LdapHelperLib.Utils.ConvertspecialCharsToScapedChars("(LIMCAR)") + "*";
				//await Test_GetUsersAndGroupsByAttributeQueuedResult(true, LdapHelperDTO.EntryAttribute.cn, _attributeFiler, BaseDNEnum.pe_com);

				//_attributeFiler = LdapHelperLib.Utils.ConvertspecialCharsToScapedChars("4303259");
				//await Test_GetUsersAndGroupsByAttributeQueuedResult(true, LdapHelperDTO.EntryAttribute.sAMAccountName, _attributeFiler, BaseDNEnum.com);

				//_attributeFiler = LdapHelperLib.Utils.ConvertspecialCharsToScapedChars("usr_ext01");
				//await Test_GetUsersAndGroupsByAttributeQueuedResult(true, LdapHelperDTO.EntryAttribute.sAMAccountName, _attributeFiler, BaseDNEnum.pe_com);

				//_attributeFiler = "*" + LdapHelperLib.Utils.ConvertspecialCharsToScapedChars("ramirez arce") + "*";
				//await Test_GetUsersAndGroupsByAttributeQueuedResult(true, LdapHelperDTO.EntryAttribute.cn, _attributeFiler, BaseDNEnum.pe_com);

				//await Test_GetUsersAndGroupsByAttributeQueuedResult(true, LdapHelperDTO.EntryAttribute.distinguishedName, "CN=Grupo Seguridad Lanperu (LANPERU),OU=Security,OU=Groups,OU=LAN Peru,OU=Equipos,DC=pe,DC=lan,DC=com", BaseDNEnum.pe_com, WildcardTypeEnum.none);

				//await Test_GetUsersAndGroupsByAttributeQueuedResult(true, LdapHelperDTO.EntryAttribute.cn, "*usr*", BaseDNEnum.pe_com);

				//await Test_GetUsersAndGroupsByAttributeQueuedResult(true, LdapHelperDTO.EntryAttribute.cn, "*simon*", BaseDNEnum.cl_com);

				//await Test_GetUsersAndGroupsByAttributeQueuedResult(true, LdapHelperDTO.EntryAttribute.cn, "*simon*", BaseDNEnum.br_com);

				//await Test_GetUsersAndGroupsByAttributeQueuedResult(true, LdapHelperDTO.EntryAttribute.cn, "*tordeur*", BaseDNEnum.com);

				//await Test_GetUsersAndGroupsByAttributeQueuedResult(true, LdapHelperDTO.EntryAttribute.sAMAccountName, "*atordeur*", BaseDNEnum.cl_com);

				//sAMAccountName: 4270826
				//distinguishedName: CN=Joanna Pamela Martinez Bazalar,CN=Users,DC=cl,DC=lan,DC=com
				//distinguishedName: CN=Tordeur Coeurnelle\, Alienor Claude Catherine,OU=Expiran,OU=TemporalOU,DC=cl,DC=lan,DC=com
				//name: Tordeur Coeurnelle, Alienor Claude Catherine
				//await Test_GetUsersAndGroupsByAttributeQueuedResult(true, LdapHelperDTO.EntryAttribute.name, "Tordeur Coeurnelle, Alienor Claude Catherine", BaseDNEnum.cl_com);

				//await Test_GetUsersAndGroupsByAttributeQueuedResult(true, LdapHelperDTO.EntryAttribute.cn, "*silva lucidato*", BaseDNEnum.br_com);

				//await Test_GetUsersAndGroupsByAttributeQueuedResult(true, LdapHelperDTO.EntryAttribute.objectSid, "S-1-5-21-1913140505-990805927-1540833222-54805", BaseDNEnum.com);



				//_attributeFiler = "*" + LdapHelperLib.Utils.ConvertspecialCharsToScapedChars("simon_") + "*";
				//await Test_GetUsersAndGroupsBy2Attributes(true, LdapHelperDTO.EntryAttribute.sAMAccountName, _attributeFiler, LdapHelperDTO.EntryAttribute.cn, _attributeFiler, false, BaseDNEnum.cl_com);

				//await Test_GetUsersAndGroupsBy2Attributes(false, LdapHelperDTO.EntryAttribute.sAMAccountName, "*administrator*", LdapHelperDTO.EntryAttribute.cn, "*administrator*", true);

				//await Test_GetUsersAndGroupsBy2Attributes(true, LdapHelperDTO.EntryAttribute.sAMAccountName, "4250889", LdapHelperDTO.EntryAttribute.cn, "4250889", false);

				//await Test_GetUsersAndGroupsBy2Attributes(true, LdapHelperDTO.EntryAttribute.sAMAccountName, "4238977", LdapHelperDTO.EntryAttribute.cn, "4238977", false);

				//await Test_GetUsersAndGroupsBy2Attributes(true, LdapHelperDTO.EntryAttribute.sAMAccountName, "atordeur", LdapHelperDTO.EntryAttribute.cn, "atordeur", false, BaseDNEnum.cl_com);

				//await Test_GetUsersAndGroupsBy2Attributes(true, LdapHelperDTO.EntryAttribute.sAMAccountName, "atordeur", LdapHelperDTO.EntryAttribute.cn, "*tordeur*", false, BaseDNEnum.cl_com);



				//await Test_GetEntriesByAttribute(true, LdapHelperDTO.EntryAttribute.sAMAccountName, "usr*", BaseDNEnum.cl_com);

				//await Test_GetEntriesByAttribute(true, LdapHelperDTO.EntryAttribute.sAMAccountName, "usr*", BaseDNEnum.br_com);

				//await Test_GetEntriesByAttribute(true, LdapHelperDTO.EntryAttribute.sAMAccountName, "usr*", BaseDNEnum.com);

				//await Test_GetEntriesByAttribute(false, LdapHelperDTO.EntryAttribute.sAMAccountName, "victor.bastidas.g");

				//await Test_GetEntriesByAttribute(true, LdapHelperDTO.EntryAttribute.objectSid, "S-1-5-21-1913140505-990805927-1540833222-54805", BaseDNEnum.cl_com);

				//await Test_GetEntriesByAttribute(true, LdapHelperDTO.EntryAttribute.objectSid, "S-1-5-21-1913140505-990805927-1540833222-54805", BaseDNEnum.br_com);

				//await Test_GetEntriesByAttribute(true, LdapHelperDTO.EntryAttribute.objectSid, "S-1-5-21-1913140505-990805927-1540833222-54805", BaseDNEnum.com);

				//await Test_GetEntriesByAttribute(false, LdapHelperDTO.EntryAttribute.objectSid, "S-1-5-21-1913140505-990805927-1540833222-54805");

				//await Test_GetEntriesByAttribute(true, LdapHelperDTO.EntryAttribute.distinguishedName, "CN=SIMON_Administrador,OU=Aplicaciones,OU=Grupos,DC=cl,DC=lan,DC=com", BaseDNEnum.cl_com);

				//await Test_GetEntriesByAttribute(true, LdapHelperDTO.EntryAttribute.distinguishedName, "CN=SIMON_Administrador,OU=Aplicaciones,OU=Grupos,DC=cl,DC=lan,DC=com", BaseDNEnum.br_com);

				//await Test_GetEntriesByAttribute(true, LdapHelperDTO.EntryAttribute.distinguishedName, "CN=SIMON_Administrador,OU=Aplicaciones,OU=Grupos,DC=cl,DC=lan,DC=com", BaseDNEnum.com);

				//await Test_GetEntriesByAttribute(false, LdapHelperDTO.EntryAttribute.distinguishedName, "CN=SIMON_Administrador,OU=Aplicaciones,OU=Grupos,DC=cl,DC=lan,DC=com");



				//await Test_GetGroupMembershipEntriesForEntry(true, LdapHelperDTO.KeyEntryAttribute.sAMAccountName, "usr_ext01", BaseDNEnum.com);

				//await Test_GetGroupMembershipEntriesForEntry(true, LdapHelperDTO.KeyEntryAttribute.sAMAccountName, "4303259", BaseDNEnum.com);

				//await Test_GetGroupMembershipEntriesForEntry(true, LdapHelperDTO.KeyEntryAttribute.distinguishedName, "CN=Administrator,CN=Users,DC=pe,DC=lan,DC=com", true, BaseDNEnum.cl_com);

				//await Test_GetGroupMembershipEntriesForEntry(true, LdapHelperDTO.KeyEntryAttribute.distinguishedName, "CN=Administrator,CN=Users,DC=pe,DC=lan,DC=com", true, BaseDNEnum.com);

				//await Test_GetGroupMembershipEntriesForEntry(false, LdapHelperDTO.KeyEntryAttribute.distinguishedName, "CN=Administrator,CN=Users,DC=pe,DC=lan,DC=com", true);

				//await Test_GetGroupMembershipEntriesForEntry(true, LdapHelperDTO.KeyEntryAttribute.sAMAccountName, "victor.bastidas.g", BaseDNEnum.br_com);

				//await Test_GetGroupMembershipEntriesForEntry(true, LdapHelperDTO.KeyEntryAttribute.sAMAccountName, "victor.bastidas.g", BaseDNEnum.cl_com);

				//await Test_GetGroupMembershipEntriesForEntry(true, LdapHelperDTO.KeyEntryAttribute.sAMAccountName, "victor.bastidas.g", BaseDNEnum.com);

				//await Test_GetGroupMembershipEntriesForEntry(false, LdapHelperDTO.KeyEntryAttribute.sAMAccountName, "victor.bastidas.g");

				//await Test_GetGroupMembershipEntriesForEntry(true, LdapHelperDTO.KeyEntryAttribute.objectSid, "S-1-5-21-1913140505-990805927-1540833222-54805", true, BaseDNEnum.br_com);

				//await Test_GetGroupMembershipEntriesForEntry(true, LdapHelperDTO.KeyEntryAttribute.objectSid, "S-1-5-21-1913140505-990805927-1540833222-54805", true, BaseDNEnum.cl_com);

				//await Test_GetGroupMembershipEntriesForEntry(true, LdapHelperDTO.KeyEntryAttribute.objectSid, "S-1-5-21-1913140505-990805927-1540833222-54805", true, BaseDNEnum.com);

				//await Test_GetGroupMembershipEntriesForEntry(false, LdapHelperDTO.KeyEntryAttribute.objectSid, "S-1-5-21-1913140505-990805927-1540833222-54805", true);



				//await Test_GetGroupMembershipEntries(true, LdapHelperDTO.EntryAttribute.sAMAccountName, "4303259", BaseDNEnum.com);


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
				log_TestTitle("Test00_Authenticate");

				var _c = new LdapHelperLib.Authenticator(getClientConfiguration());

				Log.Information("Autenticando usuario...");
				var _auth = await _c.AuthenticateUser(new LdapHelperLib.LdapUserCredentials(domainUsername, password));
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

		public static async Task<bool> Test_GetUsersAndGroupsByAttributeEnumerable(LdapHelperDTO.EntryAttribute filterAttribute, string filterValue, LdapHelperDTO.RequiredEntryAttributes requiredResults)
		{
			try
			{
				log_TestTitle("Test_GetUsersAndGroupsByAttributeEnumerable");

				var _s = new LdapHelperLib.Searcher(getClientConfiguration());

				Log.Information(string.Format("Base DN: {0}", _s.BaseDN));
				Log.Information(string.Format("Búscando con el atributo {0}: {1}...", filterAttribute.ToString(), filterValue));
				Console.WriteLine();

				var _result = await _s.SearchUsersAndGroupsByAttributeAsync(filterAttribute, filterValue, requiredResults);

				if (_result.Count() == 0)
					Log.Warning("No se encontraron registros con el criterio de búsqueda proporcionado.");
				else
				{
					foreach (var _i in _result)
					{
						Log.Information(_i.cn);
						Log.Information(_i.objectSid);
						Log.Information(_i.samAccountName);
						Log.Information(_i.distinguishedName);
						Console.WriteLine();
					}

					Log.Information(string.Format("{0} entries.", _result.Count().ToString()));
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

		public static async Task<bool> Test_GetUsersAndGroupsByAttributeQueuedResult(LdapHelperDTO.EntryAttribute filterAttribute, string filterValue, LdapHelperDTO.RequiredEntryAttributes requiredResults)
		{
			try
			{
				log_TestTitle("Test_GetUsersAndGroupsByAttributeQueuedResult");

				var _s = new LdapHelperLib.Searcher(getClientConfiguration());

				Log.Information(string.Format("Base DN: {0}", _s.BaseDN));
				Log.Information(string.Format("Búscando con el atributo {0}: {1}...", filterAttribute.ToString(), filterValue));
				Console.WriteLine();

				var _result = await _s.SearchUsersAndGroupsByAttributeQueuedModeAsync(filterAttribute, filterValue, requiredResults, Program.Tag);
				if (string.IsNullOrEmpty(_result.ErrorType))
				{
					Log.Error(_result.ErrorType);
					Log.Error(_result.ErrorMessage);
					//Log.Error(_result.Error.StackTrace);
					Console.WriteLine();
				}

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

		public static async Task<bool> Test_GetUsersAndGroupsBy2Attributes(LdapHelperDTO.EntryAttribute filterAttribute, string filterValue, LdapHelperDTO.EntryAttribute secondFilterAttribute, string secondFilterValue, bool conjunctiveFilters, LdapHelperDTO.RequiredEntryAttributes requiredResults)
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
				log_TestTitle("Test_GetUsersAndGroupsBy2Attributes");

				var _s = new LdapHelperLib.Searcher(getClientConfiguration());

				Log.Information(string.Format("Base DN: {0}", _s.BaseDN));
				Log.Information(string.Format("Búscando con el atributo {1}: {2} {0} {3}: {4}", (conjunctiveFilters ? " Y " : " O "), filterAttribute.ToString(), filterValue, secondFilterAttribute.ToString(), secondFilterValue));
				Console.WriteLine();

				var _results = await _s.SearchUsersAndGroupsBy2AttributesAsync(filterAttribute, filterValue, secondFilterAttribute, secondFilterValue, conjunctiveFilters, requiredResults, Program.Tag);
				if (_results.Count().Equals(0))
					Log.Warning("No se encontraron registros con el criterio de búsqueda proporcionado.");
				else
				{
					foreach (var _i in _results)
					{
						Log.Information(_i.cn);
						Log.Information(_i.objectSid);
						Log.Information(_i.samAccountName);
						Log.Information(_i.distinguishedName);
						Console.WriteLine();
					}

					Log.Information(string.Format("{0} entries.", _results.Count()));
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

		public static async Task<bool> Test_GetEntriesByAttribute(LdapHelperDTO.EntryAttribute attribute, string attributeFilter, LdapHelperDTO.RequiredEntryAttributes requiredResults)
		{
			try
			{
				log_TestTitle("Test_GetEntriesByAttribute");

				var _s = new LdapHelperLib.Searcher(getClientConfiguration());

				Log.Information(string.Format("Base DN: {0}", _s.BaseDN));
				Log.Information(string.Format("Búscando en por {0}: {1}...", attribute.ToString(), attributeFilter));
				Console.WriteLine();

				var _result = await _s.SearchEntriesByAttributeAsync(attribute, attributeFilter, requiredResults, Program.Tag);
				if (_result.Count().Equals(0))
					Log.Warning(string.Format("No se encontraron datos para {0}: {1}", attribute.ToString(), attributeFilter));
				else
				{
					Log.Information(string.Format("Se encontraron datos para {0}: {1}", attribute.ToString(), attributeFilter));
					Console.WriteLine();
					foreach (var _entry in _result)
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

		public static async Task<bool> Test_GetGroupMembershipEntriesForEntry(LdapHelperDTO.KeyEntryAttribute filterAttribute, string filterValue, LdapHelperDTO.RequiredEntryAttributes requiredResults)
		{
			try
			{
				log_TestTitle("Test_GetGroupMembershipEntriesForEntry");

				var _s = new LdapHelperLib.Searcher(getClientConfiguration());

				Log.Information(string.Format("Base DN: {0}", _s.BaseDN));
				Log.Information(string.Format("Obteniendo grupos a los que pertenece el {0}: {1}...", filterAttribute.ToString(), filterValue));
				Console.WriteLine();

				var _results = await _s.SearchGroupMembershipEntriesForEntry(filterAttribute, filterValue, requiredResults);

				foreach (var _entry in _results.OrderBy(f => f.distinguishedName))
					Log.Information(_entry.distinguishedName);

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

		public static async Task<bool> Test_GetGroupMembershipCNsForEntry(LdapHelperDTO.KeyEntryAttribute keyAttribute, string keyAttributeFilter, LdapHelperDTO.RequiredEntryAttributes requiredResults)
		{
			try
			{
				log_TestTitle("Test_GetGroupMembershipCNsForEntry");

				var _s = new LdapHelperLib.Searcher(getClientConfiguration());

				Log.Information(string.Format("Base DN: {0}", _s.BaseDN));
				Log.Information(string.Format("Obteniendo grupos a los que pertenece el {0}: {1}...", keyAttribute, keyAttributeFilter));
				Console.WriteLine();

				var _results = await _s.SearchGroupMembershipCNsForEntry(keyAttribute, keyAttributeFilter);

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

		public static async Task<bool> Test_GetGroupMembershipEntries(string ldapServer, int port, string baseDN, LdapHelperDTO.EntryAttribute attribute, string attributeFilter, LdapHelperDTO.RequiredEntryAttributes requiredResults)
		{
			try
			{
				log_TestTitle("Test_GetGroupMembershipEntries");

				var _s = new LdapHelperLib.Searcher(getClientConfiguration());

				Log.Information(string.Format("Base DN: {0}", _s.BaseDN));
				Log.Information(string.Format("Obteniendo grupos a los que pertenece el {0}: {1}...", attribute.ToString(), attributeFilter));
				Console.WriteLine();

				var _results = await _s.SearchGroupMembershipEntries(attribute, attributeFilter, LdapHelperDTO.RequiredEntryAttributes.MinimunWithMemberAndMemberOf);

				foreach (var _entry in _results.OrderBy(f => f.distinguishedName))
					Log.Information(_entry.distinguishedName);

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
	}
}