﻿using Bitai.LDAPHelper.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Bitai.LDAPHelper
{
	public class Searcher : BaseHelper
	{
		#region Constructor 
		public Searcher(ClientConfiguration clientConfiguration) : base(clientConfiguration)
		{
		}

		public Searcher(ConnectionInfo connectionInfo, SearchLimits searchLimits, DTO.LDAPDomainAccountCredential domainAccountCredential) : base(connectionInfo, searchLimits, domainAccountCredential)
		{
		}
		#endregion



		#region Private methods
		private async Task<DTO.LDAPEntry> getEntryFromAttributeSet(Novell.Directory.Ldap.LdapAttributeSet attributeSet, DTO.RequiredEntryAttributes requiredEntryAttributes, string requestLabel)
		{
			var ldapEntry = new DTO.LDAPEntry(requestLabel);

			Novell.Directory.Ldap.LdapAttribute attribute;
			byte[] bytes;
			string tempValue;

			if (attributeSet.ContainsKey(DTO.EntryAttribute.objectSid.ToString()))
			{
				attribute = attributeSet.GetAttribute(DTO.EntryAttribute.objectSid.ToString());
				if (attribute != null)
				{
					bytes = (byte[])(Array)attribute.ByteValue;
					ldapEntry.objectSidBytes = bytes;
					ldapEntry.objectSid = ConvertByteToStringSid(bytes);
				}
			}

			if (attributeSet.ContainsKey(DTO.EntryAttribute.objectGuid.ToString()))
			{
				attribute = attributeSet.GetAttribute(DTO.EntryAttribute.objectGuid.ToString());
				if (attribute != null)
				{
					bytes = (byte[])(Array)attribute.ByteValue;
					ldapEntry.objectGuidBytes = bytes;
					ldapEntry.objectGuid = new Guid(bytes).ToString();
				}
			}

			ldapEntry.objectCategory = attributeSet.ContainsKey(DTO.EntryAttribute.objectCategory.ToString()) ? attributeSet.GetAttribute(DTO.EntryAttribute.objectCategory.ToString()).StringValue : null;

			ldapEntry.objectClass = attributeSet.ContainsKey(DTO.EntryAttribute.objectClass.ToString()) ? attributeSet.GetAttribute(DTO.EntryAttribute.objectClass.ToString()).StringValueArray : null;

			ldapEntry.company = attributeSet.ContainsKey(DTO.EntryAttribute.company.ToString()) ? attributeSet.GetAttribute(DTO.EntryAttribute.company.ToString()).StringValue : null;

			ldapEntry.co = attributeSet.ContainsKey(DTO.EntryAttribute.co.ToString()) ? attributeSet.GetAttribute(DTO.EntryAttribute.co.ToString()).StringValue : null;

			ldapEntry.manager = attributeSet.ContainsKey(DTO.EntryAttribute.manager.ToString()) ? attributeSet.GetAttribute(DTO.EntryAttribute.manager.ToString()).StringValue : null;

			if (attributeSet.ContainsKey(DTO.EntryAttribute.whenCreated.ToString()))
			{
				attribute = attributeSet.GetAttribute(DTO.EntryAttribute.whenCreated.ToString());
				if (attribute == null)
					ldapEntry.whenCreated = null;
				else
					ldapEntry.whenCreated = DateTime.ParseExact(attribute.StringValue, "yyyyMMddHHmmss.0Z", System.Globalization.CultureInfo.InvariantCulture);
			}

			if (attributeSet.ContainsKey(DTO.EntryAttribute.lastLogonTimestamp.ToString()))
			{
				attribute = attributeSet.GetAttribute(DTO.EntryAttribute.lastLogonTimestamp.ToString());
				ldapEntry.lastLogon = (attribute == null) ? null : new DateTime?(DateTime.FromFileTime(Convert.ToInt64(attribute.StringValue)));
			}

			ldapEntry.department = attributeSet.ContainsKey(DTO.EntryAttribute.department.ToString()) ? attributeSet.GetAttribute(DTO.EntryAttribute.department.ToString()).StringValue : null;

			ldapEntry.cn = attributeSet.ContainsKey(DTO.EntryAttribute.cn.ToString()) ? attributeSet.GetAttribute(DTO.EntryAttribute.cn.ToString()).StringValue : null;

			ldapEntry.name = attributeSet.ContainsKey(DTO.EntryAttribute.name.ToString()) ? attributeSet.GetAttribute(DTO.EntryAttribute.name.ToString()).StringValue : null;

			ldapEntry.samAccountName = attributeSet.ContainsKey(DTO.EntryAttribute.sAMAccountName.ToString()) ? attributeSet.GetAttribute(DTO.EntryAttribute.sAMAccountName.ToString()).StringValue : null;

			ldapEntry.userPrincipalName = attributeSet.ContainsKey(DTO.EntryAttribute.userPrincipalName.ToString()) ? attributeSet.GetAttribute(DTO.EntryAttribute.userPrincipalName.ToString()).StringValue : null;

			ldapEntry.distinguishedName = attributeSet.ContainsKey(DTO.EntryAttribute.distinguishedName.ToString()) ? attributeSet.GetAttribute(DTO.EntryAttribute.distinguishedName.ToString()).StringValue : null;

			ldapEntry.displayName = attributeSet.ContainsKey(DTO.EntryAttribute.displayName.ToString()) ? attributeSet.GetAttribute(DTO.EntryAttribute.displayName.ToString()).StringValue : null;

			ldapEntry.givenName = attributeSet.ContainsKey(DTO.EntryAttribute.givenName.ToString()) ? attributeSet.GetAttribute(DTO.EntryAttribute.givenName.ToString()).StringValue : null;

			ldapEntry.sn = attributeSet.ContainsKey(DTO.EntryAttribute.sn.ToString()) ? attributeSet.GetAttribute(DTO.EntryAttribute.sn.ToString()).StringValue : null;

			ldapEntry.description = attributeSet.ContainsKey(DTO.EntryAttribute.description.ToString()) ? attributeSet.GetAttribute(DTO.EntryAttribute.description.ToString()).StringValue : null;

			ldapEntry.telephoneNumber = attributeSet.ContainsKey(DTO.EntryAttribute.telephoneNumber.ToString()) ? attributeSet.GetAttribute(DTO.EntryAttribute.telephoneNumber.ToString()).StringValue : null;

			ldapEntry.mail = attributeSet.ContainsKey(DTO.EntryAttribute.mail.ToString()) ? attributeSet.GetAttribute(DTO.EntryAttribute.mail.ToString()).StringValue : null;

			ldapEntry.title = attributeSet.ContainsKey(DTO.EntryAttribute.title.ToString()) ? attributeSet.GetAttribute(DTO.EntryAttribute.title.ToString()).StringValue : null;

			ldapEntry.l = attributeSet.ContainsKey(DTO.EntryAttribute.l.ToString()) ? attributeSet.GetAttribute(DTO.EntryAttribute.l.ToString()).StringValue : null;

			ldapEntry.c = attributeSet.ContainsKey(DTO.EntryAttribute.c.ToString()) ? attributeSet.GetAttribute(DTO.EntryAttribute.c.ToString()).StringValue : null;

			tempValue = attributeSet.ContainsKey(DTO.EntryAttribute.sAMAccountType.ToString()) ? attributeSet.GetAttribute(DTO.EntryAttribute.sAMAccountType.ToString()).StringValue : null;
			ldapEntry.samAccountType = GetSAMAccountTypeName(tempValue);

			if (attributeSet.ContainsKey(DTO.EntryAttribute.member.ToString()))
			{
				ldapEntry.member = attributeSet.GetAttribute(DTO.EntryAttribute.member.ToString()).StringValueArray;
			}

			if (attributeSet.ContainsKey(DTO.EntryAttribute.memberOf.ToString()))
			{
				ldapEntry.memberOf = attributeSet.GetAttribute(DTO.EntryAttribute.memberOf.ToString()).StringValueArray;
			}

			if (ldapEntry.memberOf != null && ldapEntry.memberOf.Length > 0)
			{
				var parentEntries = new List<DTO.LDAPEntry>();

				foreach (var parentDN in ldapEntry.memberOf)
				{
					//Avoid circular reference
					if (ldapEntry.distinguishedName.Equals(parentDN, StringComparison.OrdinalIgnoreCase))
						continue; //Pasar al siguiente objeto.

					var filter = new QueryFilters.AttributeFilter(DTO.EntryAttribute.distinguishedName, new QueryFilters.FilterValue(parentDN.ReplaceSpecialCharsToScapedChars()));

					var searchResult = await this.SearchEntriesAsync(filter, requiredEntryAttributes, requestLabel);

					//Parent DN could be out of Base DN
					if (searchResult.Entries.Count() > 0)
						parentEntries.Add(searchResult.Entries.First());
				}

				ldapEntry.memberOfEntries = parentEntries.ToArray();
			}

            if (attributeSet.ContainsKey(DTO.EntryAttribute.userAccountControl.ToString())) {
                ldapEntry.userAccountControl = attributeSet.GetAttribute(DTO.EntryAttribute.userAccountControl.ToString()).StringValue;
            }

            return ldapEntry;
		}

		private async Task<DTO.LDAPSearchResult> getSearchResultAsync(DTO.RequiredEntryAttributes requiredEntryAttributes, string searchFilter, string requestLabel)
		{
			DTO.LDAPSearchResult searchResult;

			try {
				var attributesToLoad = this.GetRequiredAttributeNames(requiredEntryAttributes);
				var entries = new List<DTO.LDAPEntry>();
				using (var connection = await GetLdapConnection(this.ConnectionInfo, this.DomainAccountCredential)) {
					var searchConstraints = new Novell.Directory.Ldap.LdapSearchConstraints {
						ServerTimeLimit = this.SearchLimits.MaxSearchTimeout,
						MaxResults = this.SearchLimits.MaxSearchResults,
					};

					Novell.Directory.Ldap.LdapSearchQueue searchQueue = await connection.SearchAsync(this.SearchLimits.BaseDN, Novell.Directory.Ldap.LdapConnection.ScopeSub, searchFilter, attributesToLoad.ToArray(), false, (Novell.Directory.Ldap.LdapSearchQueue)null, searchConstraints);

					Novell.Directory.Ldap.LdapMessage responseMessage = null;
					while ((responseMessage = searchQueue.GetResponse()) != null) {
						if (responseMessage is Novell.Directory.Ldap.LdapSearchResult) {
							var _ldapEntry = await getEntryFromAttributeSet(((Novell.Directory.Ldap.LdapSearchResult)responseMessage).Entry.GetAttributeSet(), requiredEntryAttributes, requestLabel);

							entries.Add(_ldapEntry);
						}
					}

					connection.Disconnect();
				}

				searchResult = new DTO.LDAPSearchResult(requestLabel, entries, $"The search returned {entries.Count} entrie(s).");

				return searchResult;
			}
			catch (Novell.Directory.Ldap.LdapException ex) {
				searchResult = new DTO.LDAPSearchResult($"{ex.Message} ({ex.LdapErrorMessage})", ex, requestLabel);

				return searchResult;
			}
			catch (Exception ex) {
				searchResult = new DTO.LDAPSearchResult($"Unexpected error performing search. {ex.Message}", ex, requestLabel);

				return searchResult;
			}
		}
		#endregion



		#region Public methods
		public Task<DTO.LDAPSearchResult> SearchEntriesAsync(QueryFilters.ICombinableFilter searchFilter, DTO.RequiredEntryAttributes requiredEntryAttributes, string requestLabel)
		{
			return getSearchResultAsync(requiredEntryAttributes, searchFilter.ToString(), requestLabel);
		}

		public async Task<DTO.LDAPSearchResult> SearchParentEntriesAsync(QueryFilters.ICombinableFilter searchFilter, DTO.RequiredEntryAttributes requiredEntryAttributes, string requestLabel)
		{
			//DTO.LDAPSearchResult addTopExceptionToResult(DTO.LDAPSearchResult partialSearchResult)
			//{
			//	return new DTO.LDAPSearchResult("Error looking up parent LDAP entries.", new Exception(partialSearchResult.OperationMessage, partialSearchResult.ErrorObject), requestLabel);
			//}

			//DTO.LDAPSearchResult addTopMessageToResult(DTO.LDAPSearchResult partialSearchResult)
			//{
			//	partialSearchResult.SetUnsuccessfullOperation($"Error looking up parent LDAP entries. {partialSearchResult.OperationMessage}");

			//	return partialSearchResult;
			//}

			try
			{
				var partialSearchResult = await this.SearchEntriesAsync(searchFilter, DTO.RequiredEntryAttributes.OnlyMemberOf, requestLabel);

				if (!partialSearchResult.IsSuccessfulOperation)
				{
					//if (partialSearchResult.HasErrorObject)
					//	return addTopExceptionToResult(partialSearchResult);
					//else
					//	return addTopMessageToResult(partialSearchResult);

					return partialSearchResult;
                }
				else if (partialSearchResult.Entries.Count() == 0)
				{
					partialSearchResult.SetUnsuccessfullOperation("No entry was found according to the search filter.");

					return partialSearchResult;
				}

				var collectedEntries = partialSearchResult.Entries.SelectAllMemberOfEntriesRecursively();

				var resultEntries = new List<DTO.LDAPEntry>();
				foreach (var entry in collectedEntries)
				{
					var distinguishedNameFilter = new QueryFilters.AttributeFilter(DTO.EntryAttribute.distinguishedName, new QueryFilters.FilterValue(entry.distinguishedName.ReplaceSpecialCharsToScapedChars()));

					partialSearchResult = await SearchEntriesAsync(distinguishedNameFilter, requiredEntryAttributes, requestLabel);
					if (!partialSearchResult.IsSuccessfulOperation)
					{
						//if (partialSearchResult.HasErrorObject)
						//	return addTopExceptionToResult(partialSearchResult);
						//else
						//	return addTopMessageToResult(partialSearchResult);

						return partialSearchResult;
					}

					resultEntries.AddRange(partialSearchResult.Entries);
				}

				return new DTO.LDAPSearchResult(requestLabel, resultEntries);
			}
            catch (Novell.Directory.Ldap.LdapException ex) {
                var searchResult = new DTO.LDAPSearchResult($"{ex.Message} ({ex.LdapErrorMessage})", ex, requestLabel);

                return searchResult;
            }
            catch (Exception ex) {
                var searchResult = new DTO.LDAPSearchResult($"Unexpected error performing search. {ex.Message}", ex, requestLabel);

                return searchResult;
            }
        }
		#endregion
	}
}