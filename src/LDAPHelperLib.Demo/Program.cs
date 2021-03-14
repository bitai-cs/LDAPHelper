using System;
using System.Threading.Tasks;
using Serilog;
using Serilog.Sinks.SystemConsole;
using System.Linq;

namespace LDAPHelperLib.Demo
{
	public enum BaseDNEnum
	{
		com,
		cl_com,
		br_com,
		pe_com,
		ar_com,
	}

	public class Program
	{
		#region Private Static Methods
		private static void configLog()
		{
			Log.Logger = new LoggerConfiguration()
				.WriteTo.Console()
				.CreateLogger();
		}

		private static void Log_TestTitle(string title)
		{
			Console.WriteLine();
			Console.WriteLine("**********************************************");
			Console.WriteLine("  " + title);
			Console.WriteLine("**********************************************");
		}

		private static LdapHelperLib.LdapConnectionPipeline getConnectionPipeline()
		{
			return new LdapHelperLib.LdapConnectionPipeline(false, 10);
		}

		private static LdapHelperLib.LdapServerSettings getServerSettings(bool useGlobalCatalog, BaseDNEnum baseDNtype, out string baseDN)
		{
			switch (baseDNtype)
			{
				case BaseDNEnum.pe_com:
					baseDN = "DC=pe, DC=lan, DC=com";
					break;
				case BaseDNEnum.br_com:
					baseDN = "DC=br, DC=lan, DC=com";
					break;
				case BaseDNEnum.cl_com:
					baseDN = "DC=cl, DC=lan, DC=com";
					break;
				case BaseDNEnum.ar_com:
					baseDN = "DC=ar, DC=lan, DC=com";
					break;
				case BaseDNEnum.com:
					baseDN = "DC=lan, DC=com";
					break;
				default:
					throw new Exception("Tipo de BaseDN no reconocido.");
			}

			return new LdapHelperLib.LdapServerSettings("8kpelimdc01.pe.lan.com", useGlobalCatalog ? (int)LdapHelperLib.DefaultServerPorts.GlobalCatalogPort : (int)LdapHelperLib.DefaultServerPorts.DefaultPort);
		}

		private static LdapHelperLib.LdapServerSettings getServerSettingsForGlobalCatalog(BaseDNEnum baseDNtype, out string baseDN)
		{
			switch (baseDNtype)
			{
				case BaseDNEnum.pe_com:
					baseDN = "DC=pe, DC=lan, DC=com";
					break;
				case BaseDNEnum.br_com:
					baseDN = "DC=br, DC=lan, DC=com";
					break;
				case BaseDNEnum.cl_com:
					baseDN = "DC=cl, DC=lan, DC=com";
					break;
				case BaseDNEnum.ar_com:
					baseDN = "DC=ar, DC=lan, DC=com";
					break;
				case BaseDNEnum.com:
					baseDN = "DC=lan, DC=com";
					break;
				default:
					throw new Exception("Tipo de BaseDN no reconocido.");
			}

			return new LdapHelperLib.LdapServerSettings("8kpelimdc01.pe.lan.com", (int)LdapHelperLib.DefaultServerPorts.GlobalCatalogPort);
		}

		private static LdapHelperLib.LdapUserCredentials getCredentials()
		{
			return new LdapHelperLib.LdapUserCredentials("LANPERU\\usr_ext01", "L4t4m2018");
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

				await Test_AuthenticateUser(false, BaseDNEnum.pe_com);

				//await Test_AuthenticateUser(true, BaseDNEnum.br_com);



				await Test_GetUsersAndGroupsByAttributeEnumerable(true, LdapHelperDTO.EntryAttribute.sAMAccountName, "usr_ext01", BaseDNEnum.com);

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


		public static async Task<bool> Test_AuthenticateUser(bool useGlobalCatalog, BaseDNEnum baseDN = BaseDNEnum.com)
		{
			try
			{
				if (useGlobalCatalog)
					Log_TestTitle("Test00_Authenticate (Global Catalog)");
				else
					Log_TestTitle("Test00_Authenticate");

				var _connPipeline = getConnectionPipeline();
				string _baseDN = string.Empty;
				var _serverSettings = useGlobalCatalog ? getServerSettingsForGlobalCatalog(baseDN, out _baseDN) : getServerSettings(baseDN, out _baseDN);
				var _userCredentials = getCredentials();

				Log.Information(_baseDN);
				Log.Information("Autenticando usuario...");
				var _c = new LdapHelperLib.Authenticator(null, _connPipeline, _serverSettings, _userCredentials);
				var _auth = await _c.AuthenticateUser(_userCredentials);
				if (_auth)
					Log.Information("Usuario autententicado!");
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


		public static async Task<bool> Test_GetUsersAndGroupsByAttributeEnumerable(bool useGlobalCatalog, LdapHelperDTO.EntryAttribute attribute, string attributeFiler, BaseDNEnum baseDN)
		{
			try
			{
				if (useGlobalCatalog)
					Log_TestTitle("Test_GetUsersAndGroupsByAttributeEnumerable (Global Catalog)");
				else
					Log_TestTitle("Test_GetUsersAndGroupsByAttributeEnumerable");

				var _connPipeline = getConnectionPipeline();
				string _baseDN = string.Empty;
				var _serverSettings = useGlobalCatalog ? getServerSettingsForGlobalCatalog(baseDN, out _baseDN) : getServerSettings(baseDN, out _baseDN);
				var _userCredentials = getCredentials();

				var _s = new LdapHelperLib.Searcher(null, _connPipeline, _serverSettings, _baseDN, useGlobalCatalog, _userCredentials);

				Log.Information(_baseDN);
				Log.Information(string.Format("Búscando con el atributo {0}: {1}...", attribute.ToString(), attributeFiler));
				Console.WriteLine();

				var _result = await _s.SearchUsersAndGroupsByAttributeAsync(attribute, attributeFiler, LdapHelperDTO.RequiredEntryAttributes.MinimunWithMemberAndMemberOf);
				//if (string.IsNullOrEmpty(_result.ErrorType)) {
				//	Log.Error(_result.ErrorType);
				//	Log.Error(_result.ErrorMessage);
				//	//Log.Error(_result.Error.StackTrace);
				//	Console.WriteLine();
				//}

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

		public static async Task<bool> Test_GetUsersAndGroupsByAttributeQueuedResult(bool useGlobalCatalog, LdapHelperDTO.EntryAttribute attribute, string attributeFiler, BaseDNEnum baseDN)
		{
			try
			{
				if (useGlobalCatalog)
					Log_TestTitle("Test_GetUsersAndGroupsByAttributeQueuedResult (Global Catalog)");
				else
					Log_TestTitle("Test_GetUsersAndGroupsByAttributeQueuedResult");

				var _connPipeline = getConnectionPipeline();
				string _baseDN = string.Empty;
				var _serverSettings = useGlobalCatalog ? getServerSettingsForGlobalCatalog(baseDN, out _baseDN) : getServerSettings(baseDN, out _baseDN);
				var _userCredentials = getCredentials();

				var _s = new LdapHelperLib.Searcher(null, _connPipeline, _serverSettings, _baseDN, useGlobalCatalog, _userCredentials);

				Log.Information(_baseDN);
				Log.Information(string.Format("Búscando con el atributo {0}: {1}...", attribute.ToString(), attributeFiler));
				Console.WriteLine();

				var _result = await _s.SearchUsersAndGroupsByAttributeAsyncModeAsync(attribute, attributeFiler, LdapHelperDTO.RequiredEntryAttributes.MinimunWithMemberAndMemberOf);
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


		public static async Task<bool> Test_GetUsersAndGroupsBy2Attributes(bool useGlobalCatalog, LdapHelperDTO.EntryAttribute attribute, string attributeFilter, LdapHelperDTO.EntryAttribute attribute2, string attributeFilter2, bool conjuntion, BaseDNEnum baseDN)
		{
			try
			{
				if (useGlobalCatalog)
					Log_TestTitle("Test_GetUsersAndGroupsBy2Attributes (Global Catalog)");
				else
					Log_TestTitle("Test_GetUsersAndGroupsBy2Attributes");

				var _connPipeline = getConnectionPipeline();
				string _baseDN = string.Empty;
				var _serverSettings = useGlobalCatalog ? getServerSettingsForGlobalCatalog(baseDN, out _baseDN) : getServerSettings(baseDN, out _baseDN);
				var _userCredentials = getCredentials();

				var _s = new LdapHelperLib.Searcher(null, _connPipeline, _serverSettings, _baseDN, useGlobalCatalog, _userCredentials)
				{
					ServerSettings = _serverSettings,
					UserCredentials = _userCredentials,
					BaseDN = _baseDN
				};

				Log.Information(_baseDN);
				Log.Information(string.Format("Búscando con el atributo {1}: {2} {0} {3}: {4}", (conjuntion ? " Y " : " O "), attribute.ToString(), attributeFilter, attribute2.ToString(), attributeFilter2));
				Console.WriteLine();

				var _results = await _s.SearchUsersAndGroupsBy2AttributesAsync(attribute, attributeFilter, attribute2, attributeFilter2, conjuntion, LdapHelperDTO.RequiredEntryAttributes.MinimunWithMemberAndMemberOf);
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


		public static async Task<bool> Test_GetEntriesByAttribute(bool useGlobalCatalog, LdapHelperDTO.EntryAttribute attribute, string attributeFilter, BaseDNEnum baseDN)
		{
			try
			{
				if (useGlobalCatalog)
					Log_TestTitle("Test_GetEntriesByAttribute (Global Catalog)");
				else
					Log_TestTitle("Test_GetEntriesByAttribute");

				var _connPipeline = getConnectionPipeline();
				var _baseDN = string.Empty;
				var _serverSettings = useGlobalCatalog ? getServerSettingsForGlobalCatalog(baseDN, out _baseDN) : getServerSettings(baseDN, out _baseDN);
				var _userCredentials = getCredentials();

				var _s = new LdapHelperLib.Searcher(null, _connPipeline, _serverSettings, _baseDN, useGlobalCatalog, _userCredentials)
				{
					ServerSettings = _serverSettings,
					UserCredentials = _userCredentials,
					BaseDN = _baseDN
				};

				Log.Information(_baseDN);
				Log.Information(string.Format("Búscando en por {0}: {1}...", attribute.ToString(), attributeFilter));
				Console.WriteLine();

				var _result = await _s.SearchEntriesByAttributeAsync(attribute, attributeFilter, LdapHelperDTO.RequiredEntryAttributes.MinimunWithMemberAndMemberOf);
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


		public static async Task<bool> Test_GetGroupMembershipEntriesForEntry(bool useGlobalCatalog, LdapHelperDTO.KeyEntryAttribute keyAttribute, string keyAttributeFilter, BaseDNEnum baseDN = BaseDNEnum.com)
		{
			try
			{
				if (useGlobalCatalog)
					Log_TestTitle("Test_GetGroupMembershipEntriesForEntry (GLOBAL CATALOG)");
				else
					Log_TestTitle("Test_GetGroupMembershipEntriesForEntry");

				var _connPipeline = getConnectionPipeline();
				string _baseDN = string.Empty;
				var _serverSettings = useGlobalCatalog ? getServerSettingsForGlobalCatalog(baseDN, out _baseDN) : getServerSettings(baseDN, out _baseDN);
				var _userCredentials = getCredentials();

				var _s = new LdapHelperLib.Searcher(null, _connPipeline, _serverSettings, _baseDN, useGlobalCatalog, _userCredentials)
				{
					ServerSettings = _serverSettings,
					UserCredentials = _userCredentials,
					BaseDN = _baseDN
				};

				Log.Information(_baseDN);
				Log.Information(string.Format("Obteniendo grupos a los que pertenece el {0}: {1}...", keyAttribute.ToString(), keyAttributeFilter));
				Console.WriteLine();

				var _results = await _s.SearchGroupMembershipEntriesForEntry(keyAttribute, keyAttributeFilter, LdapHelperDTO.RequiredEntryAttributes.MinimunWithMemberAndMemberOf);

				foreach (var _entry in _results.OrderBy(f => f.distinguishedName))
				{
					Log.Information(_entry.distinguishedName);
				}

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


		public static async Task<bool> Test_GetGroupMembershipCNsForEntry(bool useGlobalCatalog, LdapHelperDTO.KeyEntryAttribute keyAttribute, string keyAttributeFilter, BaseDNEnum baseDN = BaseDNEnum.com)
		{
			try
			{
				if (useGlobalCatalog)
					Log_TestTitle("Test_GetGroupMembershipCNsForEntry (Global Catalog)");
				else
					Log_TestTitle("Test_GetGroupMembershipCNsForEntry");

				var _connPipeline = getConnectionPipeline();
				string _baseDN = string.Empty;
				var _serverSettings = useGlobalCatalog ? getServerSettingsForGlobalCatalog(baseDN, out _baseDN) : getServerSettings(baseDN, out _baseDN);
				var _userCredentials = getCredentials();

				var _s = new LdapHelperLib.Searcher(null, _connPipeline, _serverSettings, _baseDN, useGlobalCatalog, _userCredentials)
				{
					ServerSettings = _serverSettings,
					UserCredentials = _userCredentials,
					BaseDN = _baseDN
				};

				Log.Information(_baseDN);
				Log.Information(string.Format("Obteniendo grupos a los que pertenece el {0}: {1}...", keyAttribute, keyAttributeFilter));
				Console.WriteLine();

				var _results = await _s.SearchGroupMembershipCNsForEntry(keyAttribute, keyAttributeFilter);

				foreach (var _entry in _results)
				{
					Log.Information(_entry);
				}

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


		public static async Task<bool> Test_GetGroupMembershipEntries(bool useGlobalCatalog, LdapHelperDTO.EntryAttribute attribute, string attributeFilter, BaseDNEnum baseDN = BaseDNEnum.com)
		{
			try
			{
				if (useGlobalCatalog)
					Log_TestTitle("Test_GetGroupMembershipEntries (GLOBAL CATALOG)");
				else
					Log_TestTitle("Test_GetGroupMembershipEntries");

				var _connPipeline = getConnectionPipeline();
				string _baseDN = string.Empty;
				var _serverSettings = useGlobalCatalog ? getServerSettingsForGlobalCatalog(baseDN, out _baseDN) : getServerSettings(baseDN, out _baseDN);
				var _userCredentials = getCredentials();

				var _s = new LdapHelperLib.Searcher(null, _connPipeline, _serverSettings, _baseDN, useGlobalCatalog, _userCredentials)
				{
					ServerSettings = _serverSettings,
					UserCredentials = _userCredentials,
					BaseDN = _baseDN
				};

				Log.Information(_baseDN);
				Log.Information(string.Format("Obteniendo grupos a los que pertenece el {0}: {1}...", attribute.ToString(), attributeFilter));
				Console.WriteLine();

				var _results = await _s.SearchGroupMembershipEntries(attribute, attributeFilter, LdapHelperDTO.RequiredEntryAttributes.MinimunWithMemberAndMemberOf);

				foreach (var _entry in _results.OrderBy(f => f.distinguishedName))
				{
					Log.Information(_entry.distinguishedName);
				}

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