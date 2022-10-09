using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Bitai.LDAPHelper.DTO;

namespace Bitai.LDAPHelper
{
    public class GroupMembershipValidator : BaseHelper
    {
        #region Constructors
        public GroupMembershipValidator(ClientConfiguration clientConfiguration) : base(clientConfiguration)
        {
        }

        public GroupMembershipValidator(ConnectionInfo connectionInfo, SearchLimits searchLimits, LDAPDomainAccountCredential domainAccountCredential) : base(connectionInfo, searchLimits, domainAccountCredential) { }
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

            var attributeFilter = new QueryFilters.AttributeFilter(EntryAttribute.sAMAccountName, new QueryFilters.FilterValue(sAMAccountName));

            var searcher = new Searcher(this.ConnectionInfo, this.SearchLimits, this.DomainAccountCredential);

            var searchResult = await searcher.SearchParentEntriesAsync(attributeFilter, RequiredEntryAttributes.OnlyCN, null);

            if (searchResult.Entries.Count().Equals(0))
            {
                if (searchResult.HasErrorObject)
                    throw searchResult.ErrorObject;
                else
                    throw new LDAPHelper.EntryNotFoundException($"{sAMAccountName} not found.");
            }

            return searchResult.Entries.Where(entry => entry.cn.Equals(parentGroupName, StringComparison.OrdinalIgnoreCase)).Any();
        }
    }
}