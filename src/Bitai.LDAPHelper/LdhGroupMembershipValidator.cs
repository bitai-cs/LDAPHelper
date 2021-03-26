using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using LDAPHelper.DTO;

namespace LDAPHelper
{
    public class LdhGroupMembershipValidator : BaseHelper
    {
        #region Constructors
        public LdhGroupMembershipValidator(LdhClientConfiguration clientConfiguration) : base(clientConfiguration)
        {
        }

        public LdhGroupMembershipValidator(LdhConnectionInfo connectionInfo, LdhSearchLimits searchLimits, LdhCredentials userCredentials) : base(connectionInfo, searchLimits, userCredentials) { }
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

            var searcher = new LdhSearcher(this.ConnectionInfo, this.SearchLimits, this.UserCredentials);

            var searchResult = await searcher.SearchParentEntriesAsync(EntryAttribute.sAMAccountName, sAMAccountName, RequiredEntryAttributes.OnlyCN, null);

            if (searchResult.Entries.Count().Equals(0))
            {
                if (searchResult.HasErrorInfo)
                    throw searchResult.ErrorObject;
                else
                    throw new LDAPHelper.LdhEntryNotFoundException($"{sAMAccountName} not found.");
            }

            return searchResult.Entries.Where(entry => entry.cn.Equals(parentGroupName, StringComparison.OrdinalIgnoreCase)).Any();
        }
    }
}