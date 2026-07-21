namespace Bitai.LDAPHelper.Demo;

/// <summary>
/// Selects the LDAP implementation used by demo scenarios.
/// </summary>
public enum ImplementationType {
	Novell,
	Mock
}

/// <summary>
/// Represents configuration values used by demo scenarios.
/// </summary>
public class DemoSetup
{
	/// <summary>
	/// Gets or sets available LDAP server endpoints for selection.
	/// </summary>
	public Ldapserver[] LdapServers { get; set; }

	/// <summary>
	/// Gets or sets available Base DN values for selection.
	/// </summary>
	public Basedn[] BaseDNs { get; set; }

	/// <summary>
	/// Gets or sets default LDAP connection timeout in seconds.
	/// </summary>
	public short ConnectionTimeout { get; set; }

	/// <summary>
	/// Gets or sets the domain account used as operator account during demo execution.
	/// </summary>
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
	public bool Demo_Authenticator_Authenticate_RunTest_Simple { get; set; }
	public string Demo_Authenticator_Authenticate_DomainAccountName { get; set; }

	public bool Demo_AccountManager_DisableUserAccount_RunTest { get; set; }
	public string Demo_AccountManager_DisableUserAccount_UserAccountDistinguishedName { get; set; }

	public bool Demo_AccountManager_RemoveUserAccount_RunTest { get; set; }
	public string Demo_AccountManager_RemoveUserAccount_UserAccountDistinguishedName { get; set; }

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

/// <summary>
/// Represents an LDAP server option in demo configuration.
/// </summary>
public class Ldapserver
{
	/// <summary>
	/// Gets or sets the LDAP server address.
	/// </summary>
	public string Address { get; set; }
}

/// <summary>
/// Represents a Base DN option in demo configuration.
/// </summary>
public class Basedn
{
	/// <summary>
	/// Gets or sets the base distinguished name.
	/// </summary>
	public string DN { get; set; }
}
