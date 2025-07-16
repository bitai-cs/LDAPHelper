using System;
using System.Linq;
using System.Threading.Tasks;

namespace Bitai.LDAPHelper
{
    public class GroupMembershipValidator : BaseHelper
    {
        #region Constructors
        public GroupMembershipValidator(ClientConfiguration clientConfiguration) : base(clientConfiguration)
        {
        }

        public GroupMembershipValidator(ConnectionInfo connectionInfo, SearchLimits searchLimits,  DTO.LDAPDomainAccountCredential domainAccountCredential) : base(connectionInfo, searchLimits, domainAccountCredential) { }
        #endregion


        /// <summary>
        /// Verify, for an <paramref name="sAMAccountName"/>, if it is a direct or indirect member of the <paramref name="parentGroupName"/>
        /// </summary>
        /// <param name="sAMAccountName">samAccountName of the entry</param>
        /// <param name="parentGroupName">samAccountName of the group to be verified is related to the <paramref name="sAMAccountName"/></param>
        /// <returns></returns>
        public async Task<bool> CheckGroupMembershipAsync(string sAMAccountName, string parentGroupName)
        {
            if (string.IsNullOrEmpty(sAMAccountName))
                throw new ArgumentNullException(nameof(sAMAccountName));

            if (sAMAccountName.Contains("*"))
                throw new ArgumentException($"{nameof(sAMAccountName)} cannot contain the character *.");

            var attributeFilter = new QueryFilters.AttributeFilter(DTO.EntryAttribute.sAMAccountName, new QueryFilters.FilterValue(sAMAccountName));

            var searcher = new Searcher(this.ConnectionInfo, this.SearchLimits, this.DomainAccountCredential);

            var searchResult = await searcher.SearchParentEntriesAsync(attributeFilter, DTO.RequiredEntryAttributes.OnlyCN, null);
            if (!searchResult.IsSuccessfulOperation) 
            {
                if (searchResult.HasErrorObject)
                    throw searchResult.ErrorObject;
                else
                    throw new Exception($"An error occurred while searching for the entry with sAMAccountName: {sAMAccountName}. {searchResult.OperationMessage}");
            }
            else if (searchResult.Entries.Count() == 0)
            {
                //if (searchResult.HasErrorObject)
                //    throw searchResult.ErrorObject;
                //else
                //    throw new EntryNotFoundException($"{sAMAccountName} not found.");

                return false; // Entry not found, so it cannot be a member of the group
            }

            return searchResult.Entries.Where(entry => entry.cn.Equals(parentGroupName, StringComparison.OrdinalIgnoreCase)).Any();
        }
    }
}