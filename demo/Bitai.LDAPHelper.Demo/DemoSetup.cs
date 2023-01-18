using System;
using System.Collections.Generic;
using System.Text;

namespace Bitai.LDAPHelper.Demo
{
	public class DemoSetup
	{
		public Ldapserver[] LdapServers { get; set; }
		public Basedn[] BaseDNs { get; set; }
		public short ConnectionTimeout { get; set; }

		public string DomainUserAccountForRunTests { get; set; }

		public bool Demo_AccountManager_CreateUserAccount_RunTest { get; set; }
		public string Demo_AccountManager_CreateUserAccount_UserAccountName { get; set; }
		public string Demo_AccountManager_CreateUserAccount_Password { get; set; }
		public string Demo_AccountManager_CreateUserAccount_ContainerDN { get; set; }
		public string Demo_AccountManager_CreateUserAccount_Name { get; set; }
		public string Demo_AccountManager_CreateUserAccount_Surname { get; set; }
		public string Demo_AccountManager_CreateUserAccount_DNSDomainName { get; set; }
		public string Demo_AccountManager_CreateUserAccount_MemberOf { get; set; }
		public string Demo_AccountManager_CreateUserAccount_ObjectClasses { get; set; }
		public string Demo_AccountManager_CreateUserAccount_UserAccountControlFlags { get; set; }

		public bool Demo_AccountManager_SetAccountPassword_RunTest { get; set; }
		public string Demo_AccountManager_SetAccountPassword_DistinguishedName { get; set; }

		public bool Demo_Authenticator_Authenticate_RunTest { get; set; }
		public string Demo_Authenticator_Authenticate_DomainAccountName { get; set; }

		public bool Demo_Searcher_SearchUsers_RunTest { get; set; }
		public string Demo_Searcher_SearchUsers_Filter_sAMAccountName { get; set; }
		public string Demo_Searcher_SearchUsers_Filter_cn { get; set; }
		public string Demo_Searcher_SearchUsersByTwoFilters_Filter1_sAMAccountName { get; set; }
		public string Demo_Searcher_SearchUsersByTwoFilters_Filter2_cn { get; set; }

		public bool Demo_Searcher_SearchEntries_RunTest { get; set; }
		public string Demo_Searcher_SearchEntries_Filter_cn { get; set; }
		public string Demo_Searcher_SearchEntries_Filter_objectSid { get; set; }
		public string Demo_Searcher_SearchEntriesByTwoFilters_Filter1_cn { get; set; }
		public string Demo_Searcher_SearchEntriesByTwoFilters_Filter2_cn { get; set; }

		public bool Demo_Searcher_SearchParentEntries_RunTest { get; set; }
		public string Demo_Searcher_SearchParentEntries_Filter_sAMAccountName { get; set; }

		public bool Demo_GroupMembershipValidator_RunTest { get; set; }
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
