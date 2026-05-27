using System;
using System.Linq;
using System.Threading.Tasks;
using Bitai.LDAPHelper.LdapAdapters;

namespace Bitai.LDAPHelper
{
    public class GroupMembershipValidator : BaseHelper
    {
        #region Constructors
        public GroupMembershipValidator(ClientConfiguration clientConfiguration, ILdapConnectionFactoryAdapter connectionFactory)
            : base(clientConfiguration, connectionFactory) {
        }

        public GroupMembershipValidator(
            ConnectionInfo connectionInfo,
            SearchLimits searchLimits,
            DTO.LDAPDomainAccountCredential domainAccountCredential,
            ILdapConnectionFactoryAdapter connectionFactory)
            : base(connectionInfo, searchLimits, domainAccountCredential, connectionFactory) {
        }
        #endregion


        /// <summary>
        /// Determines whether the directory entry identified by the specified 
        /// <paramref name="sAMAccountName"/> is a member (directly or indirectly) of the group whose 
        /// common name (CN) is specified by <paramref name="parentGroupCN"/>.
        /// </summary>
        /// <param name="sAMAccountName">The sAMAccountName of the entry to check. Must not be null, empty 
        /// or contain '*'. 
        /// </param><param name="parentGroupCN">The CN (common name) of the parent group to check membership against. Must not be null, empty or contain '*'.</param>
        /// <returns>
        /// <see langword="true"/> if the entry is a member (direct or nested) of the specified group; 
        /// otherwise <see langword="false"/>.
        /// If the entry is not found, <see langword="false"/> is returned.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="sAMAccountName"/> or 
        /// <paramref name="parentGroupCN"/> is null or empty.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="sAMAccountName"/> or 
        /// <paramref name="parentGroupCN"/> contains the '*' character.</exception>
        /// <exception cref="Exception">Thrown when the underlying LDAP search operation fails. If 
        /// available, the original search error object is thrown instead.</exception>
        public async Task<bool> CheckGroupMembershipAsync(string sAMAccountName, string parentGroupCN)
        {
            if (string.IsNullOrEmpty(sAMAccountName))
                throw new ArgumentNullException(nameof(sAMAccountName));

            if (sAMAccountName.Contains("*"))
                throw new ArgumentException($"{nameof(sAMAccountName)} cannot contain the character *.");

            if (string.IsNullOrEmpty(parentGroupCN))
                throw new ArgumentNullException(nameof(parentGroupCN));

            if (parentGroupCN.Contains("*"))
                throw new ArgumentException($"{nameof(parentGroupCN)} cannot contain the character *.");

            var attributeFilter = new QueryFilters.AttributeFilter(DTO.EntryAttribute.sAMAccountName, new QueryFilters.FilterValue(sAMAccountName));

            var searcher = new Searcher(this.ConnectionInfo, this.SearchLimits, this.DomainAccountCredential, ConnectionFactory);

            var searchResult = await searcher.SearchParentEntriesAsync(attributeFilter, DTO.RequiredEntryAttributes.OnlyCN, requestLabel: nameof(GroupMembershipValidator));
            if (!searchResult.IsSuccessfulOperation) {
                if (searchResult.HasErrorObject)
                    throw searchResult.ErrorObject;
                else
                    throw new EntryNotFoundException($"{searchResult.OperationMessage} {nameof(sAMAccountName)}: {sAMAccountName}");
            }

            if (searchResult.Entries.Count() == 0)
            {
                //if (searchResult.HasErrorObject)
                //    throw searchResult.ErrorObject;
                //else
                //    throw new EntryNotFoundException($"{sAMAccountName} not found.");

                return false; // Entry not found, so it cannot be a member of the group
            }

            return searchResult.Entries.Any(entry => entry.cn.Equals(parentGroupCN, StringComparison.OrdinalIgnoreCase) == true);
        }

        /// <summary>
        /// Retrieves all group memberships (common names - CNs) for the directory entry identified by 
        /// the specified <paramref name="sAMAccountName"/>. This method performs a parent-entry search 
        /// (resolving memberOf relationships) and returns the distinct CN values of all parent/group 
        /// entries discovered.
        /// </summary>
        /// <param name="sAMAccountName">
        /// The sAMAccountName of the entry whose group memberships are to be retrieved. This parameter 
        /// must not be null or empty and must not contain the '*' character.
        /// </param>
        /// <returns>
        /// An array of group common names (CNs) the specified entry is a member of. If the entry is not 
        /// found, an empty string array is returned. The returned CNs are unique and compared 
        /// case-insensitively.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="sAMAccountName"/> is null 
        /// or empty.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="sAMAccountName"/> contains the '*' character.</exception>
        /// <exception cref="Exception">
        /// Thrown when the underlying LDAP search operation fails. If the search result contains an error 
        /// object, that original exception object will be thrown instead.
        /// </exception>
        public async Task<string[]> GetAllGroupMembershipsAsync(string sAMAccountName) {
            if (string.IsNullOrEmpty(sAMAccountName))
                throw new ArgumentNullException(nameof(sAMAccountName));

            if (sAMAccountName.Contains("*"))
                throw new ArgumentException($"{nameof(sAMAccountName)} cannot contain the character *.");

            var attributeFilter = new QueryFilters.AttributeFilter(DTO.EntryAttribute.sAMAccountName, new QueryFilters.FilterValue(sAMAccountName));

            var searcher = new Searcher(this.ConnectionInfo, this.SearchLimits, this.DomainAccountCredential, ConnectionFactory);

            var searchResult = await searcher.SearchParentEntriesAsync(attributeFilter, DTO.RequiredEntryAttributes.OnlyCN, requestLabel: null);

            if (!searchResult.IsSuccessfulOperation) {
                if (searchResult.HasErrorObject)
                    throw searchResult.ErrorObject;
                else
                    throw new EntryNotFoundException($"{searchResult.OperationMessage} {nameof(sAMAccountName)}: {sAMAccountName}");
            }

            if (searchResult.Entries.Count() == 0) {
                return Array.Empty<string>(); // Entry not found
            }

            // Extract all unique group CNs
            var groups = searchResult.Entries
                .Where(entry => !string.IsNullOrEmpty(entry.cn))
                .Select(entry => entry.cn)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();

            return groups;
        }
    }
}