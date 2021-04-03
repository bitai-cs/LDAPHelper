using Novell.Directory.Ldap;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Bitai.LDAPHelper.Extensions;

namespace Bitai.LDAPHelper
{
    public class Searcher : BaseHelper
    {
        #region Constructor 
        public Searcher(ClientConfiguration clientConfiguration) : base(clientConfiguration)
        {
        }

        public Searcher(ConnectionInfo serverSettings, SearchLimits searchLimits, Credentials userCredentials) : base(serverSettings, searchLimits, userCredentials)
        {
        }
        #endregion


        #region Private methods
        private async Task<DTO.LDAPEntry> getEntryFromAttributeSet(LdapAttributeSet attributeSet, DTO.RequiredEntryAttributes requiredEntryAttributes, string requestTag)
        {
            var ldapEntry = new DTO.LDAPEntry(requestTag);

            LdapAttribute attribute;
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

                    var searchResult = await this.SearchEntriesAsync(filter, requiredEntryAttributes, requestTag);

                    //Parent DN could be out of Base DN
                    if (searchResult.Entries.Count() > 0)
                        parentEntries.Add(searchResult.Entries.First());
                }

                ldapEntry.memberOfEntries = parentEntries.ToArray();
            }

            return ldapEntry;
        }

        [Obsolete("This method could be removed in future updates")]
        private async Task<IEnumerable<DTO.LDAPEntry>> getResultListAsync(DTO.RequiredEntryAttributes requiredEntryAttributes, string searchFilter, string requestTag)
        {
            var _attributesToLoad = this.GetRequiredAttributeNames(requiredEntryAttributes);

            var _results = new List<DTO.LDAPEntry>();

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

                        var _ldapEntry = await getEntryFromAttributeSet(_entry.GetAttributeSet(), requiredEntryAttributes, requestTag);

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

        private async Task<DTO.LDAPSearchResult> getSearchResultAsync(DTO.RequiredEntryAttributes requiredEntryAttributes, string searchFilter, string requestTag)
        {
            var attributesToLoad = this.GetRequiredAttributeNames(requiredEntryAttributes);

            var searchResult = new DTO.LDAPSearchResult(requestTag);
            var entries = new List<DTO.LDAPEntry>();
            using (var connection = await GetLdapConnection(this.ConnectionInfo, this.UserCredentials))
            {
                var searchConstraints = new LdapSearchConstraints
                {
                    ServerTimeLimit = this.SearchLimits.MaxSearchTimeout,
                    MaxResults = this.SearchLimits.MaxSearchResults,
                };

                LdapSearchQueue searchQueue = connection.Search(this.SearchLimits.BaseDN, LdapConnection.ScopeSub, searchFilter, attributesToLoad.ToArray(), false, (LdapSearchQueue)null, searchConstraints);

                LdapMessage responseMessage = null;
                try
                {
                    while ((responseMessage = searchQueue.GetResponse()) != null)
                    {
                        if (responseMessage is Novell.Directory.Ldap.LdapSearchResult)
                        {
                            var _ldapEntry = await getEntryFromAttributeSet(((Novell.Directory.Ldap.LdapSearchResult)responseMessage).Entry.GetAttributeSet(), requiredEntryAttributes, requestTag);

                            entries.Add(_ldapEntry);
                        }
                    }

                    connection.Disconnect();
                }
                catch (Exception ex)
                {
                    searchResult.SetError(ex);
                }
            }

            searchResult.Entries = entries;

            return searchResult;
        }
        #endregion


        #region Public methods
        public async Task<DTO.LDAPSearchResult> SearchEntriesAsync(QueryFilters.ICombinableFilter searchFilter, DTO.RequiredEntryAttributes requiredEntryAttributes, string requestTag)
        {
            return await getSearchResultAsync(requiredEntryAttributes, searchFilter.ToString(), requestTag);
        }

        public async Task<DTO.LDAPSearchResult> SearchParentEntriesAsync(QueryFilters.ICombinableFilter searchFilter, DTO.RequiredEntryAttributes requiredEntryAttributes, string requestTag)
        {
            var searchResult = await this.SearchEntriesAsync(searchFilter, DTO.RequiredEntryAttributes.OnlyMemberOf, requestTag);

            if (searchResult.Entries.Count().Equals(0))
            {
                if (searchResult.HasErrorInfo)
                    throw searchResult.ErrorObject;
                else
                    throw new EntryNotFoundException("No entry was found according to the search filter.");
            }

            var collectedEntries = searchResult.Entries.SelectAllMemberOfEntriesRecursively();

            var resultEntries = new List<DTO.LDAPEntry>();
            foreach (var entry in collectedEntries)
            {
                var distinguishedNameFilter = new QueryFilters.AttributeFilter(DTO.EntryAttribute.distinguishedName, new QueryFilters.FilterValue(entry.distinguishedName.ReplaceSpecialCharsToScapedChars()));
               
                var partialResult = await SearchEntriesAsync(distinguishedNameFilter, requiredEntryAttributes, requestTag);

                if (partialResult.HasErrorInfo)
                    throw partialResult.ErrorObject;

                resultEntries.AddRange(partialResult.Entries);
            }

            return new DTO.LDAPSearchResult(requestTag)
            {
                Entries = resultEntries
            };
        }
        #endregion
    }
}