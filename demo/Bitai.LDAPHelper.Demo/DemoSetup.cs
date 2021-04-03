using System;
using System.Collections.Generic;
using System.Text;

namespace Bitai.LDAPHelper.Demo
{
    public class DemoSetup
    {
        public string DomainAccountName { get; set; }
        public Ldapserver[] LdapServers { get; set; }
        public Basedn[] BaseDNs { get; set; }
        public short ConnectionTimeout { get; set; }
        public bool UseSSL { get; set; }
        public string Demo_Authenticator_Authenticate_DomainAccountName { get; set; }
        public string Demo_Searcher_SearchUsers_Filter_sAMAccountName { get; set; }
        public string Demo_Searcher_SearchUsers_Filter_cn { get; set; }
        public string Demo_Searcher_SearchUsersByTwoFilters_Filter1_sAMAccountName { get; set; }
        public string Demo_Searcher_SearchUsersByTwoFilters_Filter2_cn { get; set; }
        public string Demo_Searcher_SearchEntries_Filter_cn { get; set; }
        public string Demo_Searcher_SearchEntries_Filter_objectSid { get; set; }
        public string Demo_Searcher_SearchEntriesByTwoFilters_Filter1_cn { get; set; }
        public string Demo_Searcher_SearchEntriesByTwoFilters_Filter2_cn { get; set; }
        public string Demo_Searcher_SearchParentEntries_Filter_sAMAccountName { get; set; }
        public string Demo_GroupMembershipValidator_CheckGroupmembership_sAMAccountName { get; set; }
        public string Demo_GroupMembershipValidator_CheckGroupmembership_Check_GroupName { get; set; }
    }

    public class Ldapserver
    {
        public string Address { get; set; }
    }

    public class Basedn
    {
        public string DN { get; set; }
    }
}
