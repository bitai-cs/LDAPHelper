using Bitai.LDAPHelper.LdapAdapters;
using Bitai.LDAPHelper.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Novell.Directory.Ldap;

namespace Bitai.LDAPHelper
{
    /// <summary>
    /// Performs LDAP search operations and maps results into DTO models.
    /// </summary>
	public class Searcher : BaseHelper
	{
		#region Constructor 
        /// <summary>
        /// Initializes a new instance of the <see cref="Searcher"/> class.
        /// </summary>
        /// <param name="clientConfiguration">Client configuration containing connection, credential, and search settings.</param>
        /// <param name="connectionFactory">LDAP connection factory abstraction.</param>
		public Searcher(ClientConfiguration clientConfiguration, ILdapConnectionFactoryAdapter connectionFactory) : base(clientConfiguration, connectionFactory)
		{
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="Searcher"/> class.
        /// </summary>
        /// <param name="connectionInfo">LDAP server connection settings.</param>
        /// <param name="searchLimits">LDAP search limits.</param>
        /// <param name="domainAccountCredential">Credential used to execute LDAP searches.</param>
        /// <param name="connectionFactory">LDAP connection factory abstraction.</param>
		public Searcher(ConnectionInfo connectionInfo, SearchLimits searchLimits, DTO.LDAPDomainAccountCredential domainAccountCredential, ILdapConnectionFactoryAdapter connectionFactory) : base(connectionInfo, searchLimits, domainAccountCredential, connectionFactory)
		{
		}
		#endregion




		#region Public methods
		/// <summary>
		/// Searches for entries matching the provided LDAP filter and loads the requested attributes.
		/// </summary>
        /// <param name="searchFilterObject">
		/// A combinable LDAP filter that identifies the entries to search for. This filter will be converted 
		/// to its string representation and used directly in the LDAP search operation.
		/// </param>
		/// <param name="requiredEntryAttributes">
		/// The set of attributes to load for each entry returned in the search result. Use this to limit 
		/// attributes loaded for performance (for example, OnlyMemberOf, Few, All, etc.).
		/// </param>
		/// <param name="requestLabel">
		/// Optional label/tag that will be set on the returned LDAPSearchResult and on created LDAPEntry 
		/// instances to help callers correlate operations and results.
		/// </param>
		/// <returns>
		/// A task that resolves to an <see cref="DTO.LDAPSearchResult"/> containing the entries found and 
		/// any operation message. If an error occurs, the returned LDAPSearchResult will have 
		/// IsSuccessfulOperation == false and contain error details.
		/// </returns>
		public async Task<DTO.LDAPSearchResult> SearchEntriesAsync(QueryFilters.ICombinableFilter searchFilterObject, DTO.RequiredEntryAttributes requiredEntryAttributes, string requestLabel)
		{
            try
            {
                string searchFilter = searchFilterObject.ToString();

                var attributesToLoad = this.GetRequiredAttributeNames(requiredEntryAttributes);
                var entries = new List<DTO.LDAPEntry>();
                using (var connection = await GetLdapConnection(this.ConnectionInfo, this.DomainAccountCredential))
                {
                    ILdapSearchQueueAdapter searchQueue = await connection.SearchAsync(this.SearchLimits, searchFilter, attributesToLoad.ToArray(), false);

                    ILdapMessageAdapter responseMessage = null;
                    while ((responseMessage = searchQueue.GetResponse()) != null)
                    {
                        //if (responseMessage is Novell.Directory.Ldap.LdapSearchResult) {
                        if (responseMessage.IsSearchResult && responseMessage.Entry != null)
                        {
                            var _ldapEntry = await GetEntryFromAttributeSet(responseMessage.Entry.GetAttributeSet(), requiredEntryAttributes, requestLabel);

                            entries.Add(_ldapEntry);
                        }
                    }

                    connection.Disconnect();
                }

                return new DTO.LDAPSearchResult(requestLabel, entries, $"The search returned {entries.Count} entries.");
            }
            catch (LdapException ex)
            {
                string msg = string.IsNullOrEmpty(ex.LdapErrorMessage) ? ex.Message : (string.IsNullOrEmpty(ex.Message) ? ex.LdapErrorMessage : $"{ex.Message} ({ex.LdapErrorMessage})");

                var searchResult = new DTO.LDAPSearchResult(msg, ex, requestLabel);

                return searchResult;
            }
            //// BITAI: Remain for future reference if we want to avoid direct dependency on Novell.Directory.Ldap in this class. The LdapException type is specific to the Novell library, so if we want to keep this class decoupled from that library, we can catch general Exception and check the type name as done in other parts of the code. However, if we are okay with referencing Novell.Directory.Ldap directly, catching LdapException is more straightforward and type-safe.
            //catch (Exception ex) when (ex.GetType().Name == "LdapException") {
            //	var ldapErrorMessageProp = ex.GetType().GetProperty("LdapErrorMessage");
            //	string ldapErrorMessage = ldapErrorMessageProp?.GetValue(ex) as string ?? "";
            //	string msg = string.IsNullOrEmpty(ldapErrorMessage) ? ex.Message : $"{ex.Message} ({ldapErrorMessage})";
            //	searchResult = new DTO.LDAPSearchResult(msg, ex, requestLabel);
            //	return searchResult;
            //}
            catch (Exception ex)
            {
                var searchResult = new DTO.LDAPSearchResult($"Unexpected error encountered while performing search.", ex, requestLabel);

                return searchResult;
            }
        }

        /// <summary>
        /// Searches for parent entries (groups or containers) for the entries matched by the provided filter,
        /// then loads those parent entries (listed in memberOf attibute) with the requested attributes.
        /// </summary>
        /// <param name="searchFilter">
        /// A combinable LDAP filter that identifies the starting entries whose parent membership (memberOf) 
		/// will be traversed. This filter is used only to find the initial set of entries; the method then 
		/// resolves their memberOf hierarchy.
        /// </param>
        /// <param name="requiredEntryAttributes">
        /// The set of attributes to load for each parent entry returned in the final result. Use this 
		/// to limit attributes loaded for performance (for example, OnlyMemberOf, Few, All, etc.).
        /// </param>
        /// <param name="requestLabel">
        /// Optional label/tag that will be set on the returned LDAPSearchResult and on created LDAPEntry 
		/// instances to help callers correlate operations and results.
        /// </param>
        /// <returns>
        /// A task that resolves to an <see cref="DTO.LDAPSearchResult"/> containing the parent entries found 
		/// and any operation message. If an error occurs, the returned LDAPSearchResult will have 
		/// IsSuccessfulOperation == false and contain error details.
        /// </returns>
        /// <remarks>
        /// The method first performs a partial search requesting only memberOf to discover 
		/// group/container relationships. It then traverses parents recursively (using the entry recursion 
		/// helper) and performs targeted searches by distinguishedName to load the requested attributes for 
		/// each discovered parent. Any LDAP or general exception is captured and returned as an unsuccessful 
		/// LDAPSearchResult rather than being thrown.
        /// </remarks>
        public async Task<DTO.LDAPSearchResult> SearchParentEntriesAsync(QueryFilters.ICombinableFilter searchFilter, DTO.RequiredEntryAttributes requiredEntryAttributes, string requestLabel) {
            try {
                // First, perform a partial search to get the memberOf attributes of the entries matching the provided filter. This is necessary to discover the parent entries (groups/containers) that we need to load with the requested attributes.
                var partialSearchResult = await this.SearchEntriesAsync(searchFilter, DTO.RequiredEntryAttributes.OnlyMemberOf, requestLabel);

                if (!partialSearchResult.IsSuccessfulOperation) {
                    return partialSearchResult;
                }
                else if (!partialSearchResult.Entries.Any()) {
                    throw new EntryNotFoundException("Unable to evaluate without an entry.");
                }

                var collectedEntries = partialSearchResult.Entries.SelectAllMemberOfEntriesRecursively();

                var resultEntries = new List<DTO.LDAPEntry>();
                foreach (var entry in collectedEntries) {
                    var distinguishedNameFilter = new QueryFilters.AttributeFilter(DTO.EntryAttribute.distinguishedName, new QueryFilters.FilterValue(entry.distinguishedName.ReplaceSpecialCharsToScapedChars()));

                    partialSearchResult = await SearchEntriesAsync(distinguishedNameFilter, requiredEntryAttributes, requestLabel);
                    if (!partialSearchResult.IsSuccessfulOperation) {
                        return partialSearchResult;
                    }

                    resultEntries.AddRange(partialSearchResult.Entries);
                }

                return new DTO.LDAPSearchResult(requestLabel, resultEntries);
            }
            catch (EntryNotFoundException ex)
            {
                var searchResult = new DTO.LDAPSearchResult("Nonexistent entry.", ex, requestLabel);

                return searchResult;
            }
            catch (LdapException ex)
            {
                string msg = string.IsNullOrEmpty(ex.LdapErrorMessage) ? ex.Message : (string.IsNullOrEmpty(ex.Message) ? ex.LdapErrorMessage : $"{ex.Message} ({ex.LdapErrorMessage})");
                var searchResult = new DTO.LDAPSearchResult(msg, ex, requestLabel);

                return searchResult;
            }
            //// BITAI: Remain for future reference if we want to avoid direct dependency on Novell.Directory.Ldap in this class. The LdapException type is specific to the Novell library, so if we want to keep this class decoupled from that library, we can catch general Exception and check the type name as done in other parts of the code. However, if we are okay with referencing Novell.Directory.Ldap directly, catching LdapException is more straightforward and type-safe.
            //catch (Exception ex) when (ex.GetType().Name == "LdapException") {
            //	var ldapErrorMessageProp = ex.GetType().GetProperty("LdapErrorMessage");
            //	string ldapErrorMessage = ldapErrorMessageProp?.GetValue(ex) as string ?? "";
            //	string msg = string.IsNullOrEmpty(ldapErrorMessage) ? ex.Message : $"{ex.Message} ({ldapErrorMessage})";
            //	var searchResult = new DTO.LDAPSearchResult(msg, ex, requestLabel);
            //	return searchResult;
            //}
            catch (Exception ex) {
				var searchResult = new DTO.LDAPSearchResult($"Unexpected error performing search. {ex.Message}", ex, requestLabel);

				return searchResult;
			}
        }
        #endregion




        #region Protected methods
        protected async Task<DTO.LDAPEntry> GetEntryFromAttributeSet(ILdapAttributeSetAdapter attributeSet, DTO.RequiredEntryAttributes requiredEntryAttributes, string requestLabel)
        {
            var ldapEntry = new DTO.LDAPEntry(requestLabel);

            //Novell.Directory.Ldap.LdapAttribute attribute;
            ILdapAttributeAdapter attribute;
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

            /// Load parent entries (groups/containers) if memberOf is requested and available. This allows callers to have the full parent objects with their attributes instead of just the distinguished names.
			/// IMPORTANT: Note that this will perform additional LDAP searches for each parent entry, so it may impact performance if there are many parent entries or if the LDAP server is slow. Callers should consider this when requesting memberOf and the expected number of parent entries.
            if (ldapEntry.memberOf != null && ldapEntry.memberOf.Length > 0)
            {
                var parentEntries = new List<DTO.LDAPEntry>();

                foreach (var parentDN in ldapEntry.memberOf)
                {
                    // Avoid circular reference. In some cases, an entry could be member of a group that is itself member of the entry (this is not common but possible), so we can end in a loop if we try to get the parent entry in that case. To avoid this, we check if the parent DN is the same that the entry DN, and if it is, we skip it.
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

            if (attributeSet.ContainsKey(DTO.EntryAttribute.userAccountControl.ToString()))
            {
                ldapEntry.userAccountControl = attributeSet.GetAttribute(DTO.EntryAttribute.userAccountControl.ToString()).StringValue;
            }

            return ldapEntry;
        }
        #endregion
    }
}
