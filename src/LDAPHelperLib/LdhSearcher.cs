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
	public class LdhSearcher : BaseHelper
	{
		#region Constructor 
		public LdhSearcher(LdhClientConfiguration clientConfiguration) : base(clientConfiguration)
		{
		}

		public LdhSearcher(LdhConnectionInfo serverSettings, LdhSearchLimits searchLimits, LdhUserCredentials userCredentials) : base(serverSettings, searchLimits, userCredentials)
		{
		}
		#endregion


		#region Private methods
		private async Task<LdapHelperDTO.LdhEntry> getEntryFromAttributeSet(LdapAttributeSet attributeSet, string customTag)
		{
			var _ldapEntry = new LdapHelperDTO.LdhEntry(customTag);

			LdapAttribute _attr;
			byte[] _bytes;
			string _temp;

			if (attributeSet.ContainsKey(LdapHelperDTO.EntryAttribute.objectSid.ToString()))
			{
				_attr = attributeSet.GetAttribute(LdapHelperDTO.EntryAttribute.objectSid.ToString());
				if (_attr != null)
				{
					_bytes = (byte[])(Array)_attr.ByteValue;
					_ldapEntry.objectSidBytes = _bytes;
					_ldapEntry.objectSid = ConvertByteToStringSid(_bytes);
				}
			}

			if (attributeSet.ContainsKey(LdapHelperDTO.EntryAttribute.objectGuid.ToString()))
			{
				_attr = attributeSet.GetAttribute(LdapHelperDTO.EntryAttribute.objectGuid.ToString());
				if (_attr != null)
				{
					_bytes = (byte[])(Array)_attr.ByteValue;
					_ldapEntry.objectGuidBytes = _bytes;
					_ldapEntry.objectGuid = new Guid(_bytes).ToString();
				}
			}

			_ldapEntry.objectCategory = attributeSet.ContainsKey(EntryAttribute.objectCategory.ToString()) ? attributeSet.GetAttribute(EntryAttribute.objectCategory.ToString()).StringValue : null;

			_ldapEntry.objectClass = attributeSet.ContainsKey(EntryAttribute.objectClass.ToString()) ? attributeSet.GetAttribute(EntryAttribute.objectClass.ToString()).StringValueArray : null;

			_ldapEntry.company = attributeSet.ContainsKey(EntryAttribute.company.ToString()) ? attributeSet.GetAttribute(EntryAttribute.company.ToString()).StringValue : null;

			_ldapEntry.co = attributeSet.ContainsKey(EntryAttribute.co.ToString()) ? attributeSet.GetAttribute(EntryAttribute.co.ToString()).StringValue : null;

			_ldapEntry.manager = attributeSet.ContainsKey(EntryAttribute.manager.ToString()) ? attributeSet.GetAttribute(EntryAttribute.manager.ToString()).StringValue : null;

			if (attributeSet.ContainsKey(EntryAttribute.whenCreated.ToString()))
			{
				_attr = attributeSet.GetAttribute(EntryAttribute.whenCreated.ToString());
				if (_attr == null)
					_ldapEntry.whenCreated = null;
				else
					_ldapEntry.whenCreated = DateTime.ParseExact(_attr.StringValue, "yyyyMMddHHmmss.0Z", System.Globalization.CultureInfo.InvariantCulture);
			}

			if (attributeSet.ContainsKey(EntryAttribute.lastLogonTimestamp.ToString()))
			{
				_attr = attributeSet.GetAttribute(EntryAttribute.lastLogonTimestamp.ToString());
				_ldapEntry.lastLogon = (_attr == null) ? null : new DateTime?(DateTime.FromFileTime(Convert.ToInt64(_attr.StringValue)));
			}

			_ldapEntry.department = attributeSet.ContainsKey(EntryAttribute.department.ToString()) ? attributeSet.GetAttribute(EntryAttribute.department.ToString()).StringValue : null;

			_ldapEntry.cn = attributeSet.ContainsKey(EntryAttribute.cn.ToString()) ? attributeSet.GetAttribute(EntryAttribute.cn.ToString()).StringValue : null;

			_ldapEntry.name = attributeSet.ContainsKey(EntryAttribute.name.ToString()) ? attributeSet.GetAttribute(EntryAttribute.name.ToString()).StringValue : null;

			_ldapEntry.samAccountName = attributeSet.ContainsKey(EntryAttribute.sAMAccountName.ToString()) ? attributeSet.GetAttribute(EntryAttribute.sAMAccountName.ToString()).StringValue : null;

			_ldapEntry.userPrincipalName = attributeSet.ContainsKey(EntryAttribute.userPrincipalName.ToString()) ? attributeSet.GetAttribute(EntryAttribute.userPrincipalName.ToString()).StringValue : null;

			_ldapEntry.distinguishedName = attributeSet.ContainsKey(EntryAttribute.displayName.ToString()) ? attributeSet.GetAttribute(EntryAttribute.displayName.ToString()).StringValue : null;

			_ldapEntry.displayName = attributeSet.ContainsKey(EntryAttribute.displayName.ToString()) ? attributeSet.GetAttribute(EntryAttribute.displayName.ToString()).StringValue : null;

			_ldapEntry.givenName = attributeSet.ContainsKey(EntryAttribute.givenName.ToString()) ? attributeSet.GetAttribute(EntryAttribute.givenName.ToString()).StringValue : null;

			_ldapEntry.sn = attributeSet.ContainsKey(EntryAttribute.sn.ToString()) ? attributeSet.GetAttribute(EntryAttribute.sn.ToString()).StringValue : null;

			_ldapEntry.description = attributeSet.ContainsKey(EntryAttribute.description.ToString()) ? attributeSet.GetAttribute(EntryAttribute.description.ToString()).StringValue : null;

			_ldapEntry.telephoneNumber = attributeSet.ContainsKey(EntryAttribute.telephoneNumber.ToString()) ? attributeSet.GetAttribute(EntryAttribute.telephoneNumber.ToString()).StringValue : null;

			_ldapEntry.mail = attributeSet.ContainsKey(EntryAttribute.mail.ToString()) ? attributeSet.GetAttribute(EntryAttribute.mail.ToString()).StringValue : null;

			_ldapEntry.title = attributeSet.ContainsKey(EntryAttribute.title.ToString()) ? attributeSet.GetAttribute(EntryAttribute.title.ToString()).StringValue : null;

			_ldapEntry.l = attributeSet.ContainsKey(EntryAttribute.l.ToString()) ? attributeSet.GetAttribute(EntryAttribute.l.ToString()).StringValue : null;

			_ldapEntry.c = attributeSet.ContainsKey(EntryAttribute.c.ToString()) ? attributeSet.GetAttribute(EntryAttribute.c.ToString()).StringValue : null;

			_temp = attributeSet.ContainsKey(EntryAttribute.sAMAccountType.ToString()) ? attributeSet.GetAttribute(EntryAttribute.sAMAccountType.ToString()).StringValue : null;
			_ldapEntry.samAccountType = GetSAMAccountTypeName(_temp);

			if (attributeSet.ContainsKey(EntryAttribute.member.ToString()))
			{
				_ldapEntry.member = attributeSet.GetAttribute(EntryAttribute.member.ToString()).StringValueArray;
			}

			if (attributeSet.ContainsKey(EntryAttribute.memberOf.ToString()))
			{
				_ldapEntry.memberOf = attributeSet.GetAttribute(EntryAttribute.memberOf.ToString()).StringValueArray;
			}

			if (_ldapEntry.memberOf != null)
			{
				var _parentEntries = new List<LdapHelperDTO.LdhEntry>();
				//VBG: Se obvia la siguiente búsqueda para optimizar la ejecuciòn del còdigo 
				if (true == false)
				{
					foreach (var _parentDN in _ldapEntry.memberOf)
					{
						//VBG: Se puede dar el caso a _ldapEntry que se le ha 
						//asignado a si mismo como contenedor.
						if (_ldapEntry.distinguishedName.Equals(_parentDN.ToLower(), StringComparison.OrdinalIgnoreCase))
							continue; //Pasar al siguiente objeto.

						var _searchResult = await this.SearchEntriesByAttributeAsync(EntryAttribute.distinguishedName, _parentDN.ReplaceSpecialCharsToScapedChars(), RequiredEntryAttributes.Minimun, customTag);
						if (_searchResult.Entries.Count() > 0) //Podría ser 0 si el _parentDN esta fuera del baseDN de búsqueda
							_parentEntries.Add((LdapHelperDTO.LdhEntry)_searchResult.Entries.First());
					}
				}

				_ldapEntry.memberOfEntries = _parentEntries.ToArray();
			}

			return _ldapEntry;
		}

		private IEnumerable<LdapHelperDTO.LdhEntry> enumerate_MemberOfProperty_Entries(IEnumerable<LdapHelperDTO.LdhEntry> entries, bool recursive)
		{
			var _memberOfEntries = new List<LdapHelperDTO.LdhEntry>();

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
		private IEnumerable<LdapHelperDTO.LdhEntry> enumerate_MemberOfProperty_Entries(LdapHelperDTO.LdhEntry entry, bool recursive)
		{
			var _memberOfEntries = new List<LdapHelperDTO.LdhEntry>();

			if (entry.memberOfEntries != null)
				foreach (var _memberOfEntry in entry.memberOfEntries)
				{
					_memberOfEntries.Add(_memberOfEntry);

					if (recursive)
						_memberOfEntries.AddRange(enumerate_MemberOfProperty_Entries(_memberOfEntry, true));
				}

			return _memberOfEntries;
		}

		private async Task<IEnumerable<LdapHelperDTO.LdhEntry>> enumerate_MemberOfProperty_Entries(string[] distinguishedNames, RequiredEntryAttributes requiredEntryAttributes, string customTag, bool recursive)
		{
			var _memberOfEntries = new List<LdapHelperDTO.LdhEntry>();

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

		private async Task<IEnumerable<LdapHelperDTO.LdhEntry>> enumerate_MemberOfProperty_Entries(string distinguishedName, RequiredEntryAttributes requiredEntryAttributes, string customTag, bool recursive)
		{
			var _memberOfEntries = new List<LdapHelperDTO.LdhEntry>();

			var _searchResult = await this.SearchEntriesByAttributeAsync(EntryAttribute.distinguishedName, distinguishedName, requiredEntryAttributes, customTag);

			if (_searchResult.Entries.Count() > 0)
			{
				var _range = await enumerate_MemberOfProperty_Entries(_searchResult.Entries.Single().memberOf, requiredEntryAttributes, customTag, recursive);
				_memberOfEntries.AddRange(_range);
			}

			return _memberOfEntries;
		}

		[Obsolete("Could be removed in future updates")]
		private async Task<IEnumerable<LdapHelperDTO.LdhEntry>> getResultListAsync(RequiredEntryAttributes requiredEntryAttributes, string searchFilter, string customTag)
		{
			var _attributesToLoad = this.GetRequiredAttributeNames(requiredEntryAttributes);

			var _results = new List<LdapHelperDTO.LdhEntry>();

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

						var _ldapEntry = await getEntryFromAttributeSet(_entry.GetAttributeSet(), customTag);

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

		private async Task<LdapHelperDTO.LdhSearchResult> getSearchResultAsync(RequiredEntryAttributes requiredEntryAttributes, string searchFilter, string customTag)
		{
			var _attributesToLoad = this.GetRequiredAttributeNames(requiredEntryAttributes);

			var _searchResult = new LdapHelperDTO.LdhSearchResult(customTag);
			var _entries = new List<LdapHelperDTO.LdhEntry>();
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
							var _ldapEntry = await getEntryFromAttributeSet(((Novell.Directory.Ldap.LdapSearchResult)_responseMsg).Entry.GetAttributeSet(), customTag);

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
		public async Task<LdapHelperDTO.LdhSearchResult> SearchUsersAndGroupsByAttributeAsync(EntryAttribute filterAttribute, string filterValue, RequiredEntryAttributes requiredEntryAttributes, string customTag)
		{
			if (string.IsNullOrEmpty(filterValue) || string.IsNullOrWhiteSpace(filterValue))
				throw new ArgumentException("Debe de especificar el valor para filtrar el atributo");

			string _searchFilter = string.Format("(&(!(objectClass=computer))(&(|(objectClass=user)(objectClass=group)))({0}={1}))", filterAttribute.ToString(), filterValue.ReplaceSpecialCharsToScapedChars());

			var _result = await getSearchResultAsync(requiredEntryAttributes, _searchFilter, customTag);

			return _result;
		}

		public async Task<LdapHelperDTO.LdhSearchResult> SearchUsersAndGroupsBy2AttributesAsync(EntryAttribute filterAttribute,
string filterValue, EntryAttribute secondFilterAttribute, string secondFilterValue, bool conjunctiveFilters, RequiredEntryAttributes requiredEntryAttributes, string customTag)
		{
			if (string.IsNullOrEmpty(filterValue) || string.IsNullOrWhiteSpace(filterValue))
				throw new ArgumentException("Debe de especificar el valor para filtrar el primer atributo");

			if (string.IsNullOrEmpty(secondFilterValue) || string.IsNullOrWhiteSpace(secondFilterValue))
				throw new ArgumentException("Debe de especificar el valor para filtrar el segundo atributo");

			string _searchFilter = string.Format("(&(!(objectClass=computer))(|(objectClass=user)(objectClass=group))(" + (conjunctiveFilters ? "&" : "|") + "({0}={1})({2}={3})))", filterAttribute.ToString(), filterValue.ReplaceSpecialCharsToScapedChars(), secondFilterAttribute.ToString(), secondFilterValue.ReplaceSpecialCharsToScapedChars());

			var _result = await getSearchResultAsync(requiredEntryAttributes, _searchFilter, customTag);

			return _result;
		}

		public async Task<LdapHelperDTO.LdhSearchResult> SearchEntriesByAttributeAsync(EntryAttribute filterAttribute, string filterValue, RequiredEntryAttributes requiredEntryAttributes, string customTag)
		{
			if (string.IsNullOrEmpty(filterValue) || string.IsNullOrWhiteSpace(filterValue))
				throw new ArgumentException("Debe de especificar el valor para filtrar el atributo");

			string _searchFilter = string.Format("({0}={1})", filterAttribute, filterValue.ReplaceSpecialCharsToScapedChars());

			var _result = await getSearchResultAsync(requiredEntryAttributes, _searchFilter, customTag);

			return _result;
		}

		public async Task<LdapHelperDTO.LdhSearchResult> SearchGroupMembershipEntries(EntryAttribute filterAttribute, string filterValue, RequiredEntryAttributes requiredEntryAttributes, string customTag, bool recursive = true)
		{
			if (string.IsNullOrEmpty(filterValue) || string.IsNullOrWhiteSpace(filterValue))
				throw new ArgumentException("Debe de especificar el valor para filtrar el atributo.");

			var _searchResult = await this.SearchEntriesByAttributeAsync(filterAttribute, filterValue, RequiredEntryAttributes.OnlyMemberOf, customTag);

			if (_searchResult.HasErrorInfo && _searchResult.Entries.Count().Equals(0))
				throw _searchResult.ErrorObject;

			if (_searchResult.Entries.Count().Equals(0))
				throw new LdhEntryNotFoundException(string.Format("No se puede obtener la membresia a los grupos. No se encontraron entradas según el filtro {0}={1}.", filterAttribute.ToString(), filterValue));

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

			return new LdapHelperDTO.LdhSearchResult(customTag)
			{
				Entries = _return
			};
		}

		public async Task<LdapHelperDTO.LdhSearchResult> SearchGroupMembershipEntries(EntryKeyAttribute filterAttribute, string filterValue, RequiredEntryAttributes requiredEntryAttributes, string customTag, bool recursive = true)
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

			return new LdapHelperDTO.LdhSearchResult(customTag)
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