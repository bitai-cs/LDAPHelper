#if DEBUG
using System.Reflection;
#endif
using Novell.Directory.Ldap;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using LdapHelperDTO;
using LdapHelperLib.Extensions;

namespace LdapHelperLib
{
	public class Searcher : BaseHelper
	{
		#region Constructor 
		public Searcher(string requestTag, LdapClientConfiguration clientConfiguration) : base(requestTag, clientConfiguration)
		{
		}

		public Searcher(string requestTag, LdapConnectionPipeline connectionPipeline, LdapServerSettings serverSettings, string baseDN, bool useGC, LdapUserCredentials userCredentials) : base(requestTag, connectionPipeline, serverSettings, baseDN, useGC, userCredentials)
		{
		}
		#endregion



		#region Private methods
		private async Task<LdapHelperDTO.LdapEntry> mapAttributeSetToObject(LdapAttributeSet attributeSet)
		{
			var _ldapEntry = new LdapHelperDTO.LdapEntry(this.RequestTag, this.UseGC);

			LdapAttribute _attr = null;
			byte[] _bytes = null;

			_attr = attributeSet.getAttribute("objectSid");
			if (_attr != null)
			{
				_bytes = (byte[])(Array)_attr.ByteValue;
				_ldapEntry.objectSidBytes = _bytes;
				_ldapEntry.objectSid = ConvertByteToStringSid(_bytes);
			}

			_attr = attributeSet.getAttribute("objectGUID");
			if (_attr != null)
			{
				_bytes = (byte[])(Array)_attr.ByteValue;
				_ldapEntry.objectGuidBytes = _bytes;
				_ldapEntry.objectGuid = new Guid(_bytes).ToString();
			}

			_ldapEntry.objectCategory = attributeSet.getAttribute("objectCategory")?.StringValue;

			_ldapEntry.objectClass = attributeSet.getAttribute("objectClass")?.StringValueArray;

			_ldapEntry.company = attributeSet.getAttribute("company")?.StringValue;

			_ldapEntry.manager = attributeSet.getAttribute("manager")?.StringValue;

			_attr = attributeSet.getAttribute("whenCreated");
			if (_attr == null)
				_ldapEntry.whenCreated = null;
			else
				_ldapEntry.whenCreated = DateTime.ParseExact(_attr.StringValue, "yyyyMMddHHmmss.0Z", System.Globalization.CultureInfo.InvariantCulture);

			_attr = attributeSet.getAttribute("lastLogonTimestamp");
			_ldapEntry.lastLogon = (_attr == null) ? null : new DateTime?(DateTime.FromFileTime(Convert.ToInt64(_attr.StringValue)));

			_ldapEntry.department = attributeSet.getAttribute("department")?.StringValue;

			_ldapEntry.cn = attributeSet.getAttribute("cn")?.StringValue;

			_ldapEntry.name = attributeSet.getAttribute("name")?.StringValue;

			_ldapEntry.samAccountName = attributeSet.getAttribute("sAMAccountName")?.StringValue;

			_ldapEntry.userPrincipalName = attributeSet.getAttribute("userPrincipalName")?.StringValue;

			_ldapEntry.distinguishedName = attributeSet.getAttribute("distinguishedName")?.StringValue;

			_ldapEntry.displayName = attributeSet.getAttribute("displayName")?.StringValue;

			_ldapEntry.givenName = attributeSet.getAttribute("givenName")?.StringValue;

			_ldapEntry.sn = attributeSet.getAttribute("sn")?.StringValue;

			_ldapEntry.description = attributeSet.getAttribute("description")?.StringValue;

			_ldapEntry.telephoneNumber = attributeSet.getAttribute("telephoneNumber")?.StringValue;

			_ldapEntry.mail = attributeSet.getAttribute("mail")?.StringValue;

			_ldapEntry.title = attributeSet.getAttribute("title")?.StringValue;

			_ldapEntry.l = attributeSet.getAttribute("l")?.StringValue;

			_ldapEntry.c = attributeSet.getAttribute("c")?.StringValue;

			_attr = attributeSet.getAttribute("sAMAccountType");
			_ldapEntry.samAccountType = GetSAMAccountTypeName(_attr?.StringValue);

			_ldapEntry.member = attributeSet.getAttribute("member")?.StringValueArray;

			_ldapEntry.memberOf = attributeSet.getAttribute("memberOf")?.StringValueArray;

			if (_ldapEntry.memberOf != null)
			{
				var _parentEntries = new List<LdapHelperDTO.LdapEntry>();
				//VBG: Se obvia la siguiente búsqueda para optimizar la ejecuciòn del còdigo 
				if (true == false)
				{
					foreach (var _parentDN in _ldapEntry.memberOf)
					{
						//VBG: Se puede dar el caso a _ldapEntry que se le ha 
						//asignado a si mismo como contenedor.
						if (_ldapEntry.distinguishedName.Equals(_parentDN.ToLower(), StringComparison.OrdinalIgnoreCase))
							continue; //Pasar al siguiente objeto.

						var _parentEntry = await this.SearchEntriesByAttributeAsync(EntryAttribute.distinguishedName, _parentDN.ConvertspecialCharsToScapedChars(), RequiredEntryAttributes.Minimun);
						////VBG: Old version
						//var _parentEntry = await this.GetEntriesByAttributeAsync(EntryAttribute.distinguishedName, LdapHelperLib.Utils.ConvertspecialCharsToScapedChars(_parentDN), RequiredEntryAttributes.Minimun);
						if (_parentEntry.Count() > 0) //Podría ser 0 si el _parentDN esta fuera del baseDN de búsqueda
							_parentEntries.Add((LdapHelperDTO.LdapEntry)_parentEntry.First());
					}
				}

				_ldapEntry.memberOfEntries = _parentEntries.ToArray();
			}

			return _ldapEntry;
		}

		private IEnumerable<LdapHelperDTO.LdapEntry> enumerateMemberOfEntriesForEachEntry(IEnumerable<LdapHelperDTO.LdapEntry> entries)
		{
			var _memberOfEntries = new List<LdapHelperDTO.LdapEntry>();

			foreach (var _entry in entries)
			{
				_memberOfEntries.AddRange(enumerateMemberOfEntriesForEntry(_entry));
			}

			return _memberOfEntries;
		}

		/// <summary>
		/// Enumerar recursivamente los memberOf
		/// </summary>
		/// <param name="entry">LdapEntry to evaluate</param>
		/// <returns></returns>
		private IEnumerable<LdapHelperDTO.LdapEntry> enumerateMemberOfEntriesForEntry(LdapHelperDTO.LdapEntry entry)
		{
			var _memberOfEntries = new List<LdapHelperDTO.LdapEntry>();

			if (entry.memberOfEntries != null)
				foreach (var _memberOfEntry in entry.memberOfEntries)
				{
					_memberOfEntries.Add(_memberOfEntry);
					_memberOfEntries.AddRange(enumerateMemberOfEntriesForEntry(_memberOfEntry));
				}

			return _memberOfEntries;
		}

		private async Task<IEnumerable<LdapHelperDTO.LdapEntry>> enumerateMemberOfEntriesForEachDN(string[] distinguishedNames, RequiredEntryAttributes requiredEntryAttributes)
		{
			var _memberOfEntries = new List<LdapHelperDTO.LdapEntry>();

			if (distinguishedNames != null && distinguishedNames.Length > 0)
			{
				foreach (var _dn in distinguishedNames)
				{
					var _result = await this.SearchEntriesByAttributeAsync(EntryAttribute.distinguishedName, _dn, requiredEntryAttributes);
					_memberOfEntries.Add(_result.Single());
					var _range = await enumerateMemberOfEntriesForDN(_dn, requiredEntryAttributes);
					_memberOfEntries.AddRange(_range);
				}
			}

			return _memberOfEntries;
		}

		private async Task<IEnumerable<LdapHelperDTO.LdapEntry>> enumerateMemberOfEntriesForDN(string distinguishedName, RequiredEntryAttributes requiredEntryAttributes)
		{
			var _memberOfEntries = new List<LdapHelperDTO.LdapEntry>();

			var _result = await this.SearchEntriesByAttributeAsync(EntryAttribute.distinguishedName, distinguishedName, requiredEntryAttributes);

			if (_result.Count() > 0)
			{
				var _range = await enumerateMemberOfEntriesForEachDN(_result.Single().memberOf, requiredEntryAttributes);
				_memberOfEntries.AddRange(_range);
			}

			return _memberOfEntries;
		}

		private async Task<IEnumerable<LdapHelperDTO.LdapEntry>> getResults(RequiredEntryAttributes requiredEntryAttributes, string searchFilter)
		{
			//Obtenemos el arreglo con los atributos que deseamos obtener
			var _attributesToLoad = this.GetRequiredAttributeNames(requiredEntryAttributes);

			var _results = new List<LdapHelperDTO.LdapEntry>();

			using (var _connection = await GetBoundLdapConnection(this.ServerSettings, this.UserCredentials))
			{
				var search = _connection.Search(this.BaseDN, LdapConnection.SCOPE_SUB, searchFilter, _attributesToLoad.ToArray(), false);

				Novell.Directory.Ldap.LdapEntry _entry = null;
				while (search.HasMore())
				{
					try
					{
						_entry = search.Next();

						var _ldapEntry = await mapAttributeSetToObject(_entry.getAttributeSet());

						_results.Add(_ldapEntry);
					}
					catch (LdapReferralException)
					{
						_connection.Disconnect();
						//LdapReferralException ocurre cuando no existe el siguiente item a iterar. Se debe obviar este error");
					}
					catch (Exception)
					{
						throw;
					}
				}
			}

			return _results;
		}

		private async Task<QueuedResult> getAsyncResult(RequiredEntryAttributes requiredEntryAttributes, string searchFilter)
		{
			//Obtenemos el arreglo con los atributos que deseamos obtener
			var _attributesToLoad = this.GetRequiredAttributeNames(requiredEntryAttributes);

			var _result = new QueuedResult(RequestTag);
			var _entries = new List<LdapHelperDTO.LdapEntry>();

			using (var _connection = await GetBoundLdapConnection(this.ServerSettings, this.UserCredentials))
			{
				LdapSearchQueue _queue = _connection.Search(this.BaseDN, LdapConnection.SCOPE_SUB, searchFilter, _attributesToLoad.ToArray(), false, (LdapSearchQueue)null, (LdapSearchConstraints)null);
				LdapMessage _responseMsg = null;
				try
				{
					while ((_responseMsg = _queue.getResponse()) != null)
					{
						if (_responseMsg is LdapSearchResult)
						{
							var _ldapEntry = await mapAttributeSetToObject(((LdapSearchResult)_responseMsg).Entry.getAttributeSet());

							_entries.Add(_ldapEntry);
						}
					}
				}
				catch (Exception ex)
				{
					_result.ErrorType = ex.GetType().FullName;
					_result.ErrorMessage = ex.Message;
				}
			}

			_result.Entries = _entries;

			return _result;
		}
		#endregion



		#region Public methods
		public async Task<IEnumerable<LdapHelperDTO.LdapEntry>> SearchUsersAndGroupsByAttributeAsync(EntryAttribute filterAttribute, string filterValue, RequiredEntryAttributes requiredEntryAttributes)
		{
			if (string.IsNullOrEmpty(filterValue) || string.IsNullOrWhiteSpace(filterValue))
				throw new ArgumentException("Debe de especificar el valor para filtrar el primer atributo");
#if DEBUG
			var _debugLine = MethodBase.GetCurrentMethod().Name + "({0}, {1})";

			System.Diagnostics.Debug.WriteLine(string.Format(_debugLine, filterAttribute.ToString(), filterValue));
#endif
			string _searchFilter = string.Format("(&(!(objectClass=computer))(&(|(objectClass=user)(objectClass=group)))({0}={1}))", filterAttribute.ToString(), filterValue);

			var _results = await getResults(requiredEntryAttributes, _searchFilter);

			return _results;
		}

		public async Task<QueuedResult> SearchUsersAndGroupsByAttributeAsyncModeAsync(EntryAttribute filterAttribute, string filterValue, RequiredEntryAttributes requiredEntryAttributes)
		{
			if (string.IsNullOrEmpty(filterValue) || string.IsNullOrWhiteSpace(filterValue))
				throw new ArgumentException("Debe de especificar el valor para filtrar el atributo");
#if DEBUG
			var _debugLine = MethodBase.GetCurrentMethod().Name + "({0}, {1})";

			System.Diagnostics.Debug.WriteLine(string.Format(_debugLine, filterAttribute.ToString(), filterValue));
#endif
			string _searchFilter = string.Format("(&(!(objectClass=computer))(&(|(objectClass=user)(objectClass=group)))({0}={1}))", filterAttribute.ToString(), filterValue);

			var _result = await getAsyncResult(requiredEntryAttributes, _searchFilter);

			return _result;
		}



		public async Task<IEnumerable<LdapHelperDTO.LdapEntry>> SearchUsersAndGroupsBy2AttributesAsync(EntryAttribute filterAttribute,
string filterValue, EntryAttribute filterAttribute2, string filterValue2, bool conjunction, RequiredEntryAttributes requiredEntryAttributes)
		{
			if (string.IsNullOrEmpty(filterValue) || string.IsNullOrWhiteSpace(filterValue))
				throw new ArgumentException("Debe de especificar el valor para filtrar el primer atributo");

			if (string.IsNullOrEmpty(filterValue2) || string.IsNullOrWhiteSpace(filterValue2))
				throw new ArgumentException("Debe de especificar el valor para filtrar el segundo atributo");

			string _searchFilter = string.Format("(&(!(objectClass=computer))(|(objectClass=user)(objectClass=group))(" + (conjunction ? "&" : "|") + "({0}={1})({2}={3})))", filterAttribute.ToString(), filterValue.Trim(), filterAttribute2.ToString(), filterValue2.Trim());

			var _results = await getResults(requiredEntryAttributes, _searchFilter);

			return _results;
		}

		public async Task<QueuedResult> SearchUsersAndGroupsBy2AttributesAsyncModeAsync(EntryAttribute filterAttribute,
string filterValue, EntryAttribute filterAttribute2, string filterValue2, bool conjunction, RequiredEntryAttributes requiredEntryAttributes)
		{
			if (string.IsNullOrEmpty(filterValue) || string.IsNullOrWhiteSpace(filterValue))
				throw new ArgumentException("Debe de especificar el valor para filtrar el primer atributo");

			if (string.IsNullOrEmpty(filterValue2) || string.IsNullOrWhiteSpace(filterValue2))
				throw new ArgumentException("Debe de especificar el valor para filtrar el segundo atributo");

			string _searchFilter = string.Format("(&(!(objectClass=computer))(|(objectClass=user)(objectClass=group))(" + (conjunction ? "&" : "|") + "({0}={1})({2}={3})))", filterAttribute.ToString(), filterValue.Trim(), filterAttribute2.ToString(), filterValue2.Trim());

			var _result = await getAsyncResult(requiredEntryAttributes, _searchFilter);

			return _result;
		}



		public async Task<IEnumerable<LdapHelperDTO.LdapEntry>> SearchEntriesByAttributeAsync(EntryAttribute filterAttribute, string filterValue, RequiredEntryAttributes requiredEntryAttributes)
		{
			if (string.IsNullOrEmpty(filterValue) || string.IsNullOrWhiteSpace(filterValue))
				throw new ArgumentException("Debe de especificar el valor para filtrar el atributo");
#if DEBUG
			var _debugLine = MethodInfo.GetCurrentMethod().Name + "({0}, {1})";

			System.Diagnostics.Debug.WriteLine(string.Format(_debugLine, filterAttribute.ToString(), filterValue));
#endif
			string _searchFilter = string.Format("({0}={1})", filterAttribute, filterValue);

			var _results = await getResults(requiredEntryAttributes, _searchFilter);

			return _results;
		}

		public async Task<QueuedResult> SearchEntriesByAttributeAsyncModeAsync(EntryAttribute filterAttribute, string filterValue, RequiredEntryAttributes requiredEntryAttributes)
		{
			if (string.IsNullOrEmpty(filterValue) || string.IsNullOrWhiteSpace(filterValue))
				throw new ArgumentException("Debe de especificar el valor para filtrar el atributo");
#if DEBUG
			var _debugLine = MethodInfo.GetCurrentMethod().Name + "({0}, {1})";

			System.Diagnostics.Debug.WriteLine(string.Format(_debugLine, filterAttribute.ToString(), filterValue));
#endif
			string _searchFilter = string.Format("({0}={1})", filterAttribute, filterValue);

			var _result = await getAsyncResult(requiredEntryAttributes, _searchFilter);

			return _result;
		}



		public async Task<IEnumerable<LdapHelperDTO.LdapEntry>> SearchGroupMembershipEntriesForEntry(KeyEntryAttribute keyFilterAttribute, string keyFilterValue, RequiredEntryAttributes requiredEntryAttributes)
		{
			if (string.IsNullOrEmpty(keyFilterValue) || string.IsNullOrWhiteSpace(keyFilterValue))
				throw new ArgumentException("Debe de especificar el valor para filtrar el atributo.");

			if (keyFilterValue.Contains("*"))
				throw new ArgumentOutOfRangeException("No puede utilizar caracteres comodines para la búsqueda de una entrada en específica. Solo se puede realizar una búsqueda exacta.");

			EntryAttribute _attribute;
			switch (keyFilterAttribute)
			{
				case KeyEntryAttribute.distinguishedName:
					_attribute = EntryAttribute.distinguishedName;
					break;
				case KeyEntryAttribute.objectSid:
					_attribute = EntryAttribute.objectSid;
					break;
				case KeyEntryAttribute.sAMAccountName:
					_attribute = EntryAttribute.sAMAccountName;
					break;
				default:
					throw new ArgumentOutOfRangeException($"No se puede buscar una entrada especifica usando el atributo '{keyFilterAttribute}' como filtro de bùsqueda.");
			}

			var _entries = await this.SearchEntriesByAttributeAsync(_attribute, keyFilterValue, RequiredEntryAttributes.OnlyMemberOf);
			if (_entries.Count().Equals(0))
				throw new KeyNotFoundException(string.Format("No se encontró una entrada con el filtro de búsqueda {0}={1}", _attribute.ToString(), keyFilterValue));

			var _groupMemberships = await this.enumerateMemberOfEntriesForEachDN(_entries.Single().memberOf, requiredEntryAttributes);

			//Distinct by DistinguishedName
			return _groupMemberships
				 .GroupBy(f => f.distinguishedName)
				 .Select(g => g.First());
		}

		public async Task<IEnumerable<string>> SearchGroupMembershipCNsForEntry(KeyEntryAttribute keyFilterAttribute, string keyFilterValue)
		{
			var _groupMemberships = await SearchGroupMembershipEntriesForEntry(keyFilterAttribute, keyFilterValue, RequiredEntryAttributes.OnlyCN);
			var _CNs = (from c in _groupMemberships
						select c.cn).ToArray();

			return _CNs;
		}



		public async Task<IEnumerable<LdapHelperDTO.LdapEntry>> SearchGroupMembershipEntries(EntryAttribute filterAttribute, string filterValue, RequiredEntryAttributes requiredEntryAttributes)
		{
			if (string.IsNullOrEmpty(filterValue) || string.IsNullOrWhiteSpace(filterValue))
				throw new ArgumentException("Debe de especificar el valor para filtrar el atributo.");

			var _entries = await this.SearchEntriesByAttributeAsync(filterAttribute, filterValue, RequiredEntryAttributes.OnlyMemberOf);
			if (_entries.Count().Equals(0))
				throw new LdapEntryNotFoundException(string.Format("No se puede obtener la membresia a los grupos. No se encontraron entradas según el filtro {0}={1}.", filterAttribute.ToString(), filterValue));

			//Por cada Entry, combinar memberOf[] en uno solo
			IEnumerable<string> _combined = new string[0];
			foreach (var _entry in _entries.Where(f => f.memberOf != null))
				_combined = _combined.Concat(_entry.memberOf);

			if (_combined.Count() > 1)
				_combined = _combined.Distinct();

			var _groupMemberships = await this.enumerateMemberOfEntriesForEachDN(_combined.ToArray(), requiredEntryAttributes);

			//Distinct by DistinguishedName
			return _groupMemberships
				 .GroupBy(f => new { f.distinguishedName, f.objectSid })
				 .Select(g => g.First());
		}

		public async Task<IEnumerable<string>> SearchGroupMembershipCNs(EntryAttribute filterAttribute, string filterValue, RequiredEntryAttributes requiredEntryAttributes)
		{
			var _groupMemberships = await SearchGroupMembershipEntries(filterAttribute, filterValue, requiredEntryAttributes);
			var _CNs = (from c in _groupMemberships
						select c.cn).ToArray();

			return _CNs;
		}
		#endregion
	}
}