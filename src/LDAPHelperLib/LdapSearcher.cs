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
	public class LdapSearcher : BaseHelper
	{
		#region Constructor 
		public LdapSearcher(LdapClientConfiguration clientConfiguration) : base(clientConfiguration)
		{
		}

		public LdapSearcher(LdapConnectionInfo serverSettings, LdapSearchLimits searchLimits, LdapUserCredentials userCredentials) : base(serverSettings, searchLimits, userCredentials)
		{
		}
		#endregion


		#region Private methods
		private async Task<LdapHelperDTO.LdapEntry> getLdapEntryFromAttributeSet(LdapAttributeSet attributeSet, string customTag)
		{
			var _ldapEntry = new LdapHelperDTO.LdapEntry(customTag);

			LdapAttribute _attr;
			byte[] _bytes;

			_attr = attributeSet.GetAttribute("objectSid");
			if (_attr != null)
			{
				_bytes = (byte[])(Array)_attr.ByteValue;
				_ldapEntry.objectSidBytes = _bytes;
				_ldapEntry.objectSid = ConvertByteToStringSid(_bytes);
			}

			_attr = attributeSet.GetAttribute("objectGUID");
			if (_attr != null)
			{
				_bytes = (byte[])(Array)_attr.ByteValue;
				_ldapEntry.objectGuidBytes = _bytes;
				_ldapEntry.objectGuid = new Guid(_bytes).ToString();
			}

			_ldapEntry.objectCategory = attributeSet.GetAttribute("objectCategory")?.StringValue;

			_ldapEntry.objectClass = attributeSet.GetAttribute("objectClass")?.StringValueArray;

			_ldapEntry.company = attributeSet.GetAttribute("company")?.StringValue;

			_ldapEntry.manager = attributeSet.GetAttribute("manager")?.StringValue;

			_attr = attributeSet.GetAttribute("whenCreated");
			if (_attr == null)
				_ldapEntry.whenCreated = null;
			else
				_ldapEntry.whenCreated = DateTime.ParseExact(_attr.StringValue, "yyyyMMddHHmmss.0Z", System.Globalization.CultureInfo.InvariantCulture);

			_attr = attributeSet.GetAttribute("lastLogonTimestamp");
			_ldapEntry.lastLogon = (_attr == null) ? null : new DateTime?(DateTime.FromFileTime(Convert.ToInt64(_attr.StringValue)));

			_ldapEntry.department = attributeSet.GetAttribute("department")?.StringValue;

			_ldapEntry.cn = attributeSet.GetAttribute("cn")?.StringValue;

			_ldapEntry.name = attributeSet.GetAttribute("name")?.StringValue;

			_ldapEntry.samAccountName = attributeSet.GetAttribute("sAMAccountName")?.StringValue;

			_ldapEntry.userPrincipalName = attributeSet.GetAttribute("userPrincipalName")?.StringValue;

			_ldapEntry.distinguishedName = attributeSet.GetAttribute("distinguishedName")?.StringValue;

			_ldapEntry.displayName = attributeSet.GetAttribute("displayName")?.StringValue;

			_ldapEntry.givenName = attributeSet.GetAttribute("givenName")?.StringValue;

			_ldapEntry.sn = attributeSet.GetAttribute("sn")?.StringValue;

			_ldapEntry.description = attributeSet.GetAttribute("description")?.StringValue;

			_ldapEntry.telephoneNumber = attributeSet.GetAttribute("telephoneNumber")?.StringValue;

			_ldapEntry.mail = attributeSet.GetAttribute("mail")?.StringValue;

			_ldapEntry.title = attributeSet.GetAttribute("title")?.StringValue;

			_ldapEntry.l = attributeSet.GetAttribute("l")?.StringValue;

			_ldapEntry.c = attributeSet.GetAttribute("c")?.StringValue;

			_attr = attributeSet.GetAttribute("sAMAccountType");
			_ldapEntry.samAccountType = GetSAMAccountTypeName(_attr?.StringValue);

			_ldapEntry.member = attributeSet.GetAttribute("member")?.StringValueArray;

			_ldapEntry.memberOf = attributeSet.GetAttribute("memberOf")?.StringValueArray;

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

						var _searchResult = await this.SearchEntriesByAttributeAsync(EntryAttribute.distinguishedName, _parentDN.ConvertspecialCharsToScapedChars(), RequiredEntryAttributes.Minimun, customTag);
						if (_searchResult.Entries.Count() > 0) //Podría ser 0 si el _parentDN esta fuera del baseDN de búsqueda
							_parentEntries.Add((LdapHelperDTO.LdapEntry)_searchResult.Entries.First());
					}
				}

				_ldapEntry.memberOfEntries = _parentEntries.ToArray();
			}

			return _ldapEntry;
		}

		private IEnumerable<LdapHelperDTO.LdapEntry> enumerate_MemberOfProperty_Entries(IEnumerable<LdapHelperDTO.LdapEntry> entries, bool recursive)
		{
			var _memberOfEntries = new List<LdapHelperDTO.LdapEntry>();

			foreach (var _entry in entries)
			{
				_memberOfEntries.AddRange(enumerate_MemberOfProperty_Entries(_entry, recursive));
			}

			return _memberOfEntries;
		}

		/// <summary>
		/// Enumerate "memberOf" property entries
		/// </summary>
		/// <param name="entry">LdapEntry to evaluate</param>
		/// <param name="recursive">Recursive mode</param>
		/// <returns></returns>
		private IEnumerable<LdapHelperDTO.LdapEntry> enumerate_MemberOfProperty_Entries(LdapHelperDTO.LdapEntry entry, bool recursive)
		{
			var _memberOfEntries = new List<LdapHelperDTO.LdapEntry>();

			if (entry.memberOfEntries != null)
				foreach (var _memberOfEntry in entry.memberOfEntries)
				{
					_memberOfEntries.Add(_memberOfEntry);

					if (recursive)
						_memberOfEntries.AddRange(enumerate_MemberOfProperty_Entries(_memberOfEntry, true));
				}

			return _memberOfEntries;
		}

		private async Task<IEnumerable<LdapHelperDTO.LdapEntry>> enumerate_MemberOfProperty_Entries(string[] distinguishedNames, RequiredEntryAttributes requiredEntryAttributes, string customTag, bool recursive)
		{
			var _memberOfEntries = new List<LdapHelperDTO.LdapEntry>();

			if (distinguishedNames != null && distinguishedNames.Length > 0)
			{
				foreach (var _dn in distinguishedNames)
				{
					var _searchResult = await this.SearchEntriesByAttributeAsync(EntryAttribute.distinguishedName, _dn, requiredEntryAttributes, customTag);

					_memberOfEntries.Add(_searchResult.Entries.Single());

					if (recursive)
					{
						var _range = await enumerate_MemberOfProperty_Entries(_dn, requiredEntryAttributes, customTag, recursive);
						_memberOfEntries.AddRange(_range);
					}
				}
			}

			return _memberOfEntries;
		}

		private async Task<IEnumerable<LdapHelperDTO.LdapEntry>> enumerate_MemberOfProperty_Entries(string distinguishedName, RequiredEntryAttributes requiredEntryAttributes, string customTag, bool recursive)
		{
			var _memberOfEntries = new List<LdapHelperDTO.LdapEntry>();

			var _searchResult = await this.SearchEntriesByAttributeAsync(EntryAttribute.distinguishedName, distinguishedName, requiredEntryAttributes, customTag);

			if (_searchResult.Entries.Count() > 0)
			{
				var _range = await enumerate_MemberOfProperty_Entries(_searchResult.Entries.Single().memberOf, requiredEntryAttributes, customTag, recursive);
				_memberOfEntries.AddRange(_range);
			}

			return _memberOfEntries;
		}

		private async Task<IEnumerable<LdapHelperDTO.LdapEntry>> getResultListAsync(RequiredEntryAttributes requiredEntryAttributes, string searchFilter, string customTag)
		{
			var _attributesToLoad = this.GetRequiredAttributeNames(requiredEntryAttributes);

			var _results = new List<LdapHelperDTO.LdapEntry>();

			using (var _connection = await GetLdapConnection(this.ConnectionInfo, this.UserCredentials))
			{
				var _searchConstraints = new LdapSearchConstraints
				{
					MaxResults = this.SearchLimits.MaxSearchResults,
					ServerTimeLimit = this.SearchLimits.MaxSearchTimeout
				};
				var search = _connection.Search(this.SearchLimits.BaseDN, LdapConnection.ScopeSub, searchFilter, _attributesToLoad.ToArray(), false);

				Novell.Directory.Ldap.LdapEntry _entry = null;
				while (search.HasMore())
				{
					try
					{
						_entry = search.Next();

						var _ldapEntry = await getLdapEntryFromAttributeSet(_entry.GetAttributeSet(), customTag);

						_results.Add(_ldapEntry);
					}
					catch (LdapReferralException)
					{
						//This exception occurs when there is no more data when iterating through the result. We just let the exception go by.

						_connection.Disconnect();
					}
					catch (Exception ex)
					{
						throw ex;
					}
				}
			}

			return _results;
		}

		private async Task<LdapHelperDTO.LdapSearchResult> getSearchResultAsync(RequiredEntryAttributes requiredEntryAttributes, string searchFilter, string customTag)
		{
			var _attributesToLoad = this.GetRequiredAttributeNames(requiredEntryAttributes);

			var _searchResult = new LdapHelperDTO.LdapSearchResult(customTag);
			var _entries = new List<LdapHelperDTO.LdapEntry>();
			using (var _connection = await GetLdapConnection(this.ConnectionInfo, this.UserCredentials))
			{
				var _searchConstraints = new LdapSearchConstraints
				{
					ServerTimeLimit = this.SearchLimits.MaxSearchTimeout,
					MaxResults = this.SearchLimits.MaxSearchResults
				};
				LdapSearchQueue _searchQueue = _connection.Search(this.SearchLimits.BaseDN, LdapConnection.ScopeSub, searchFilter, _attributesToLoad.ToArray(), false, (LdapSearchQueue)null, _searchConstraints);
				LdapMessage _responseMsg = null;
				try
				{
					while ((_responseMsg = _searchQueue.GetResponse()) != null)
					{
						if (_responseMsg is Novell.Directory.Ldap.LdapSearchResult)
						{
							var _ldapEntry = await getLdapEntryFromAttributeSet(((Novell.Directory.Ldap.LdapSearchResult)_responseMsg).Entry.GetAttributeSet(), customTag);

							_entries.Add(_ldapEntry);
						}
					}
				}
				catch (Exception ex)
				{
					_searchResult.SetError(ex);
				}
			}

			_searchResult.Entries = _entries;

			return _searchResult;
		}
		#endregion


		#region Public methods
		public async Task<LdapHelperDTO.LdapSearchResult> SearchUsersAndGroupsByAttributeAsync(EntryAttribute filterAttribute, string filterValue, RequiredEntryAttributes requiredEntryAttributes, string customTag)
		{
			if (string.IsNullOrEmpty(filterValue) || string.IsNullOrWhiteSpace(filterValue))
				throw new ArgumentException("Debe de especificar el valor para filtrar el atributo");

			string _searchFilter = string.Format("(&(!(objectClass=computer))(&(|(objectClass=user)(objectClass=group)))({0}={1}))", filterAttribute.ToString(), filterValue);

			var _result = await getSearchResultAsync(requiredEntryAttributes, _searchFilter, customTag);

			return _result;
		}

		public async Task<LdapHelperDTO.LdapSearchResult> SearchUsersAndGroupsBy2AttributesAsync(EntryAttribute filterAttribute,
string filterValue, EntryAttribute secondFilterAttribute, string secondFilterValue, bool conjunctiveFilters, RequiredEntryAttributes requiredEntryAttributes, string customTag)
		{
			if (string.IsNullOrEmpty(filterValue) || string.IsNullOrWhiteSpace(filterValue))
				throw new ArgumentException("Debe de especificar el valor para filtrar el primer atributo");

			if (string.IsNullOrEmpty(secondFilterValue) || string.IsNullOrWhiteSpace(secondFilterValue))
				throw new ArgumentException("Debe de especificar el valor para filtrar el segundo atributo");

			string _searchFilter = string.Format("(&(!(objectClass=computer))(|(objectClass=user)(objectClass=group))(" + (conjunctiveFilters ? "&" : "|") + "({0}={1})({2}={3})))", filterAttribute.ToString(), filterValue.Trim(), secondFilterAttribute.ToString(), secondFilterValue.Trim());

			var _result = await getSearchResultAsync(requiredEntryAttributes, _searchFilter, customTag);

			return _result;
		}

		public async Task<LdapHelperDTO.LdapSearchResult> SearchEntriesByAttributeAsync(EntryAttribute filterAttribute, string filterValue, RequiredEntryAttributes requiredEntryAttributes, string customTag)
		{
			if (string.IsNullOrEmpty(filterValue) || string.IsNullOrWhiteSpace(filterValue))
				throw new ArgumentException("Debe de especificar el valor para filtrar el atributo");

			string _searchFilter = string.Format("({0}={1})", filterAttribute, filterValue);

			var _result = await getSearchResultAsync(requiredEntryAttributes, _searchFilter, customTag);

			return _result;
		}

		public async Task<LdapHelperDTO.LdapSearchResult> SearchGroupMembershipEntries(EntryAttribute filterAttribute, string filterValue, RequiredEntryAttributes requiredEntryAttributes, string customTag, bool recursive = true)
		{
			if (string.IsNullOrEmpty(filterValue) || string.IsNullOrWhiteSpace(filterValue))
				throw new ArgumentException("Debe de especificar el valor para filtrar el atributo.");

			var _searchResult = await this.SearchEntriesByAttributeAsync(filterAttribute, filterValue, RequiredEntryAttributes.OnlyMemberOf, customTag);

			if (_searchResult.HasErrorInfo && _searchResult.Entries.Count().Equals(0))
				throw _searchResult.ErrorObject;

			if (_searchResult.Entries.Count().Equals(0))
				throw new LdapEntryNotFoundException(string.Format("No se puede obtener la membresia a los grupos. No se encontraron entradas según el filtro {0}={1}.", filterAttribute.ToString(), filterValue));

			//Por cada Entry, combinar memberOf[] en uno solo
			IEnumerable<string> _combined = new string[0];
			foreach (var _entry in _searchResult.Entries.Where(f => f.memberOf != null))
				_combined = _combined.Concat(_entry.memberOf);

			if (_combined.Count() > 1)
				_combined = _combined.Distinct();

			var _groupMemberships = await this.enumerate_MemberOfProperty_Entries(_combined.ToArray(), requiredEntryAttributes, customTag, recursive);

			//Distinct by DistinguishedName
			var _return = _groupMemberships
				 .GroupBy(f => new { f.distinguishedName, f.objectSid })
				 .Select(g => g.First());

			return new LdapHelperDTO.LdapSearchResult(customTag)
			{
				Entries = _return
			};
		}

		public async Task<LdapHelperDTO.LdapSearchResult> SearchGroupMembershipEntries(EntryKeyAttribute filterAttribute, string filterValue, RequiredEntryAttributes requiredEntryAttributes, string customTag, bool recursive = true)
		{
			if (string.IsNullOrEmpty(filterValue) || string.IsNullOrWhiteSpace(filterValue))
				throw new ArgumentException("Debe de especificar el valor para filtrar el atributo.");

			if (filterValue.Contains("*"))
				throw new ArgumentOutOfRangeException("No puede utilizar caracteres comodines para la búsqueda de una entrada en específica. Solo se puede realizar una búsqueda exacta.");

			EntryAttribute _attribute;
			switch (filterAttribute)
			{
				case EntryKeyAttribute.distinguishedName:
					_attribute = EntryAttribute.distinguishedName;
					break;
				case EntryKeyAttribute.objectSid:
					_attribute = EntryAttribute.objectSid;
					break;
				case EntryKeyAttribute.sAMAccountName:
					_attribute = EntryAttribute.sAMAccountName;
					break;
				default:
					throw new ArgumentOutOfRangeException($"No se puede buscar una entrada especifica usando el atributo '{filterAttribute}' como filtro de bùsqueda.");
			}

			var _searchResult = await this.SearchEntriesByAttributeAsync(_attribute, filterValue, RequiredEntryAttributes.OnlyMemberOf, customTag);

			if (_searchResult.HasErrorInfo && _searchResult.Entries.Count().Equals(0))
				throw _searchResult.ErrorObject;

			if (_searchResult.Entries.Count().Equals(0))
				throw new KeyNotFoundException(string.Format("No se encontró una entrada con el filtro de búsqueda {0}={1}", _attribute.ToString(), filterValue));

			var _groupMemberships = await this.enumerate_MemberOfProperty_Entries(_searchResult.Entries.Single().memberOf, requiredEntryAttributes, customTag, recursive);

			//Distinct by DistinguishedName
			var _return = _groupMemberships
				 .GroupBy(f => f.distinguishedName)
				 .Select(g => g.First());

			return new LdapHelperDTO.LdapSearchResult(customTag)
			{
				Entries = _return
			};
		}

		public async Task<IEnumerable<string>> SearchGroupMembershipCNs(EntryAttribute filterAttribute, string filterValue, RequiredEntryAttributes requiredEntryAttributes, string customTag, bool recursive = true)
		{
			var _searchResult = await SearchGroupMembershipEntries(filterAttribute, filterValue, requiredEntryAttributes, customTag, recursive);
			var _CNs = (from c in _searchResult.Entries
						select c.cn).ToArray();

			return _CNs;
		}

		public async Task<IEnumerable<string>> SearchGroupMembershipCNs(EntryKeyAttribute filterAttribute, string filterValue, string customTag, bool recursive = true)
		{
			var _searchResults = await SearchGroupMembershipEntries(filterAttribute, filterValue, RequiredEntryAttributes.OnlyCN, customTag, recursive);
			var _CNs = (from c in _searchResults.Entries
						select c.cn).ToArray();

			return _CNs;
		}
		#endregion
	}
}