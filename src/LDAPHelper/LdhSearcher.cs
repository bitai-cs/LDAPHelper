using Novell.Directory.Ldap;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using LDAPHelper.DTO;
using LDAPHelper.Extensions;

namespace LDAPHelper
{
    public class LdhSearcher : BaseHelper
    {
        #region Constructor 
        public LdhSearcher(LdhClientConfiguration clientConfiguration) : base(clientConfiguration)
        {
        }

        public LdhSearcher(LdhConnectionInfo serverSettings, LdhSearchLimits searchLimits, LdhCredentials userCredentials) : base(serverSettings, searchLimits, userCredentials)
        {
        }
        #endregion


        #region Private methods
        private async Task<LDAPHelper.DTO.LDAPEntry> getEntryFromAttributeSet(LdapAttributeSet attributeSet, RequiredEntryAttributes requiredEntryAttributes, string customTag)
        {
            var ldapEntry = new LDAPHelper.DTO.LDAPEntry(customTag);

            LdapAttribute _attr;
            byte[] _bytes;
            string _temp;

            if (attributeSet.ContainsKey(LDAPHelper.DTO.EntryAttribute.objectSid.ToString()))
            {
                _attr = attributeSet.GetAttribute(LDAPHelper.DTO.EntryAttribute.objectSid.ToString());
                if (_attr != null)
                {
                    _bytes = (byte[])(Array)_attr.ByteValue;
                    ldapEntry.objectSidBytes = _bytes;
                    ldapEntry.objectSid = ConvertByteToStringSid(_bytes);
                }
            }

            if (attributeSet.ContainsKey(LDAPHelper.DTO.EntryAttribute.objectGuid.ToString()))
            {
                _attr = attributeSet.GetAttribute(LDAPHelper.DTO.EntryAttribute.objectGuid.ToString());
                if (_attr != null)
                {
                    _bytes = (byte[])(Array)_attr.ByteValue;
                    ldapEntry.objectGuidBytes = _bytes;
                    ldapEntry.objectGuid = new Guid(_bytes).ToString();
                }
            }

            ldapEntry.objectCategory = attributeSet.ContainsKey(EntryAttribute.objectCategory.ToString()) ? attributeSet.GetAttribute(EntryAttribute.objectCategory.ToString()).StringValue : null;

            ldapEntry.objectClass = attributeSet.ContainsKey(EntryAttribute.objectClass.ToString()) ? attributeSet.GetAttribute(EntryAttribute.objectClass.ToString()).StringValueArray : null;

            ldapEntry.company = attributeSet.ContainsKey(EntryAttribute.company.ToString()) ? attributeSet.GetAttribute(EntryAttribute.company.ToString()).StringValue : null;

            ldapEntry.co = attributeSet.ContainsKey(EntryAttribute.co.ToString()) ? attributeSet.GetAttribute(EntryAttribute.co.ToString()).StringValue : null;

            ldapEntry.manager = attributeSet.ContainsKey(EntryAttribute.manager.ToString()) ? attributeSet.GetAttribute(EntryAttribute.manager.ToString()).StringValue : null;

            if (attributeSet.ContainsKey(EntryAttribute.whenCreated.ToString()))
            {
                _attr = attributeSet.GetAttribute(EntryAttribute.whenCreated.ToString());
                if (_attr == null)
                    ldapEntry.whenCreated = null;
                else
                    ldapEntry.whenCreated = DateTime.ParseExact(_attr.StringValue, "yyyyMMddHHmmss.0Z", System.Globalization.CultureInfo.InvariantCulture);
            }

            if (attributeSet.ContainsKey(EntryAttribute.lastLogonTimestamp.ToString()))
            {
                _attr = attributeSet.GetAttribute(EntryAttribute.lastLogonTimestamp.ToString());
                ldapEntry.lastLogon = (_attr == null) ? null : new DateTime?(DateTime.FromFileTime(Convert.ToInt64(_attr.StringValue)));
            }

            ldapEntry.department = attributeSet.ContainsKey(EntryAttribute.department.ToString()) ? attributeSet.GetAttribute(EntryAttribute.department.ToString()).StringValue : null;

            ldapEntry.cn = attributeSet.ContainsKey(EntryAttribute.cn.ToString()) ? attributeSet.GetAttribute(EntryAttribute.cn.ToString()).StringValue : null;

            ldapEntry.name = attributeSet.ContainsKey(EntryAttribute.name.ToString()) ? attributeSet.GetAttribute(EntryAttribute.name.ToString()).StringValue : null;

            ldapEntry.samAccountName = attributeSet.ContainsKey(EntryAttribute.sAMAccountName.ToString()) ? attributeSet.GetAttribute(EntryAttribute.sAMAccountName.ToString()).StringValue : null;

            ldapEntry.userPrincipalName = attributeSet.ContainsKey(EntryAttribute.userPrincipalName.ToString()) ? attributeSet.GetAttribute(EntryAttribute.userPrincipalName.ToString()).StringValue : null;

            ldapEntry.distinguishedName = attributeSet.ContainsKey(EntryAttribute.distinguishedName.ToString()) ? attributeSet.GetAttribute(EntryAttribute.distinguishedName.ToString()).StringValue : null;

            ldapEntry.displayName = attributeSet.ContainsKey(EntryAttribute.displayName.ToString()) ? attributeSet.GetAttribute(EntryAttribute.displayName.ToString()).StringValue : null;

            ldapEntry.givenName = attributeSet.ContainsKey(EntryAttribute.givenName.ToString()) ? attributeSet.GetAttribute(EntryAttribute.givenName.ToString()).StringValue : null;

            ldapEntry.sn = attributeSet.ContainsKey(EntryAttribute.sn.ToString()) ? attributeSet.GetAttribute(EntryAttribute.sn.ToString()).StringValue : null;

            ldapEntry.description = attributeSet.ContainsKey(EntryAttribute.description.ToString()) ? attributeSet.GetAttribute(EntryAttribute.description.ToString()).StringValue : null;

            ldapEntry.telephoneNumber = attributeSet.ContainsKey(EntryAttribute.telephoneNumber.ToString()) ? attributeSet.GetAttribute(EntryAttribute.telephoneNumber.ToString()).StringValue : null;

            ldapEntry.mail = attributeSet.ContainsKey(EntryAttribute.mail.ToString()) ? attributeSet.GetAttribute(EntryAttribute.mail.ToString()).StringValue : null;

            ldapEntry.title = attributeSet.ContainsKey(EntryAttribute.title.ToString()) ? attributeSet.GetAttribute(EntryAttribute.title.ToString()).StringValue : null;

            ldapEntry.l = attributeSet.ContainsKey(EntryAttribute.l.ToString()) ? attributeSet.GetAttribute(EntryAttribute.l.ToString()).StringValue : null;

            ldapEntry.c = attributeSet.ContainsKey(EntryAttribute.c.ToString()) ? attributeSet.GetAttribute(EntryAttribute.c.ToString()).StringValue : null;

            _temp = attributeSet.ContainsKey(EntryAttribute.sAMAccountType.ToString()) ? attributeSet.GetAttribute(EntryAttribute.sAMAccountType.ToString()).StringValue : null;
            ldapEntry.samAccountType = GetSAMAccountTypeName(_temp);

            if (attributeSet.ContainsKey(EntryAttribute.member.ToString()))
            {
                ldapEntry.member = attributeSet.GetAttribute(EntryAttribute.member.ToString()).StringValueArray;
            }

            if (attributeSet.ContainsKey(EntryAttribute.memberOf.ToString()))
            {
                ldapEntry.memberOf = attributeSet.GetAttribute(EntryAttribute.memberOf.ToString()).StringValueArray;
            }

            if (ldapEntry.memberOf != null && ldapEntry.memberOf.Length > 0)
            {
                var parentEntries = new List<LDAPEntry>();

                foreach (var parentDN in ldapEntry.memberOf)
                {
                    //Avoid circular reference
                    if (ldapEntry.distinguishedName.Equals(parentDN, StringComparison.OrdinalIgnoreCase))
                        continue; //Pasar al siguiente objeto.

                    var _searchResult = await this.SearchEntriesAsync(EntryAttribute.distinguishedName, parentDN.ReplaceSpecialCharsToScapedChars(), requiredEntryAttributes, customTag);

                    //Parent DN could be out of Base DN
                    if (_searchResult.Entries.Count() > 0)
                        parentEntries.Add(_searchResult.Entries.First());
                }

                ldapEntry.memberOfEntries = parentEntries.ToArray();
            }

            return ldapEntry;
        }

        [Obsolete("This method could be removed in future updates")]
        private async Task<IEnumerable<LDAPHelper.DTO.LDAPEntry>> getResultListAsync(RequiredEntryAttributes requiredEntryAttributes, string searchFilter, string customTag)
        {
            var _attributesToLoad = this.GetRequiredAttributeNames(requiredEntryAttributes);

            var _results = new List<LDAPHelper.DTO.LDAPEntry>();

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

                        var _ldapEntry = await getEntryFromAttributeSet(_entry.GetAttributeSet(), requiredEntryAttributes, customTag);

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

        private async Task<LDAPHelper.DTO.LDAPSearchResult> getSearchResultAsync(RequiredEntryAttributes requiredEntryAttributes, string searchFilter, string customTag)
        {
            var _attributesToLoad = this.GetRequiredAttributeNames(requiredEntryAttributes);

            var _searchResult = new LDAPHelper.DTO.LDAPSearchResult(customTag);
            var _entries = new List<LDAPHelper.DTO.LDAPEntry>();
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
                            var _ldapEntry = await getEntryFromAttributeSet(((Novell.Directory.Ldap.LdapSearchResult)_responseMsg).Entry.GetAttributeSet(), requiredEntryAttributes, customTag);

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
        public async Task<LDAPHelper.DTO.LDAPSearchResult> SearchUsersAndGroupsAsync(EntryAttribute filterAttribute, string filterValue, RequiredEntryAttributes requiredEntryAttributes, string customTag)
        {
            if (string.IsNullOrEmpty(filterValue) || string.IsNullOrWhiteSpace(filterValue))
                throw new ArgumentNullException(nameof(filterValue));

            string _searchFilter = string.Format("(&(!(objectClass=computer))(&(|(objectClass=user)(objectClass=group)))({0}={1}))", filterAttribute.ToString(), filterValue);

            var _result = await getSearchResultAsync(requiredEntryAttributes, _searchFilter, customTag);

            return _result;
        }

        public async Task<LDAPHelper.DTO.LDAPSearchResult> SearchUsersAndGroupsAsync(EntryAttribute filterAttribute,
string filterValue, EntryAttribute secondFilterAttribute, string secondFilterValue, bool conjunctiveFilters, RequiredEntryAttributes requiredEntryAttributes, string customTag)
        {
            if (string.IsNullOrEmpty(filterValue) || string.IsNullOrWhiteSpace(filterValue))
                throw new ArgumentNullException(nameof(filterValue));

            if (string.IsNullOrEmpty(secondFilterValue) || string.IsNullOrWhiteSpace(secondFilterValue))
                throw new ArgumentNullException(nameof(secondFilterValue));

            string _searchFilter = string.Format("(&(!(objectClass=computer))(|(objectClass=user)(objectClass=group))(" + (conjunctiveFilters ? "&" : "|") + "({0}={1})({2}={3})))", filterAttribute.ToString(), filterValue, secondFilterAttribute.ToString(), secondFilterValue);

            var _result = await getSearchResultAsync(requiredEntryAttributes, _searchFilter, customTag);

            return _result;
        }

        public async Task<LDAPHelper.DTO.LDAPSearchResult> SearchEntriesAsync(EntryAttribute filterAttribute, string filterValue, RequiredEntryAttributes requiredEntryAttributes, string customTag)
        {
            if (string.IsNullOrEmpty(filterValue) || string.IsNullOrWhiteSpace(filterValue))
                throw new ArgumentNullException(nameof(filterValue));

            string _searchFilter = string.Format("({0}={1})", filterAttribute, filterValue);

            return await getSearchResultAsync(requiredEntryAttributes, _searchFilter, customTag);
        }

        public async Task<LDAPHelper.DTO.LDAPSearchResult> SearchEntriesAsync(EntryAttribute filterAttribute,
string filterValue, EntryAttribute secondFilterAttribute, string secondFilterValue, bool conjunctiveFilters, RequiredEntryAttributes requiredEntryAttributes, string customTag)
        {
            if (string.IsNullOrEmpty(filterValue) || string.IsNullOrWhiteSpace(filterValue))
                throw new ArgumentNullException(nameof(filterValue));

            if (string.IsNullOrEmpty(secondFilterValue) || string.IsNullOrWhiteSpace(secondFilterValue))
                throw new ArgumentNullException(nameof(secondFilterValue));

            string _searchFilter = string.Format("(" + (conjunctiveFilters ? "&" : "|") + "({0}={1})({2}={3}))", filterAttribute.ToString(), filterValue, secondFilterAttribute.ToString(), secondFilterValue);

            return await getSearchResultAsync(requiredEntryAttributes, _searchFilter, customTag);
        }

        public async Task<LDAPHelper.DTO.LDAPSearchResult> SearchParentEntriesAsync(EntryAttribute filterAttribute, string filterValue, RequiredEntryAttributes requiredEntryAttributes, string customTag)
        {
            if (string.IsNullOrEmpty(filterValue) || string.IsNullOrWhiteSpace(filterValue))
                throw new ArgumentNullException(nameof(filterValue));

            var searchResult = await this.SearchEntriesAsync(filterAttribute, filterValue, RequiredEntryAttributes.OnlyMemberOf, customTag);

            if (searchResult.Entries.Count().Equals(0))
            {
                if (searchResult.HasErrorInfo)
                    throw searchResult.ErrorObject;
                else
                    throw new LdhEntryNotFoundException($"No entry was found according to the search criteria {filterAttribute}={filterValue}.");
            }

            var collectedEntries = searchResult.Entries.SelectAllMemberOfEntriesRecursively();

            var resultEntries = new List<LDAPEntry>();
            foreach (var entry in collectedEntries)
            {
                var partialResult = await SearchEntriesAsync(EntryAttribute.distinguishedName, entry.distinguishedName.ReplaceSpecialCharsToScapedChars(), requiredEntryAttributes, customTag);

                if (partialResult.HasErrorInfo)
                    throw partialResult.ErrorObject;

                resultEntries.AddRange(partialResult.Entries);
            }

            return new LDAPHelper.DTO.LDAPSearchResult(customTag)
            {
                Entries = resultEntries
            };
        }
        #endregion
    }
}