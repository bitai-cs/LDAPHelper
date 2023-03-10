# LDAP Helper ![Logo](resources/hierarchy_32.png)

**.NET 7.0** library which wraps  **Novell.Directory.Ldap.NETStandard** functionality to make LDAP searches and also authenticate users against a Directory Service.

This libary is part of [Bitai.LDAPWebApi](https://github.com/bitai-cs/LDAPWebApi) solution

## Requirements

- **.NET 7.0**
  - Have at least [.NET 6.0 SDK installed](https://dotnet.microsoft.com/en-us/download/dotnet/6.0) on your system.  

## How to use the library?

- See the project **Bitai.LDAPHelper.Demo** to get a reference  

## How to execute Demo console application?

- First, make a copy on your desktop of the  [Bitai.LDAPHelper.Demo/ldaphelper_demosetup.json](src/Bitai.LDAPHelper.Demo/ldaphelper_demosetup.json) file.
- Edit the copied file and set the parameters as needed.
  
```json
{
    "LdapServers": [
        {
            "Address": "hr.company.com"
        },
        {
            "Address": "10.100.54.4"
        },
        {
            "Address": "55.55.55.13"
        },
        {
            "Address": "10.100.54.35"
        },
        {
            "Address": "11.11.11.13"
        },
        {
            "Address": "124.11.58.130"
        }
    ],
    "BaseDNs": [
        {
            "DN": "DC=school,DC=edu,DC=uk"
        },
        {
            "DN": "DC=university,DC=edu,DC=br"
        },
        {
            "DN": "DC=domain,CD=local"
        },
        {
            "DN": "DC=br, DC=company, DC=com"
        },
        {
            "DN": "DC=pe, DC=company, DC=com"
        },
        {
            "DN": "DC=es, DC=company, DC=com"
        }
    ],

    "ConnectionTimeout": 15,
    "DomainUserAccountForRunTests": "CERTUS\\usrrpa_mda",
    "AccountPassword": "@secret-password!",

    "Demo_AccountManager_CreateUserAccount_RunTest": true,
    "Demo_AccountManager_CreateUserAccount_UserAccountName": "vbastidas77",
    "Demo_AccountManager_CreateUserAccount_Password": "rpa2023@@",
    "Demo_AccountManager_CreateUserAccount_ContainerDN": "OU=TEST_RPA_MDA,OU=ADM,OU=CERTUS,DC=certus,DC=edu,DC=pe",
    "Demo_AccountManager_CreateUserAccount_Name": "Victor G.",
    "Demo_AccountManager_CreateUserAccount_Surname": "Bastidas G.",
    "Demo_AccountManager_CreateUserAccount_DNSDomainName": "certus.edu.pe",
    "Demo_AccountManager_CreateUserAccount_MemberOf": null,
    "Demo_AccountManager_CreateUserAccount_ObjectClasses": "user,top,person,organizationalPerson",
    "Demo_AccountManager_CreateUserAccount_UserAccountControlFlags": "NORMAL_ACCOUNT,DONT_EXPIRE_PASSWORD",

    "Demo_AccountManager_SetAccountPassword_RunTest": false,
    "Demo_AccountManager_SetAccountPassword_DistinguishedName": "CN=Victor Bastidas,OU=TEST_RPA_MDA,OU=ADM,OU=CERTUS,DC=certus,DC=edu,DC=pe",

    "Demo_Authenticator_Authenticate_RunTest": true,
    "Demo_Authenticator_Authenticate_RunTest_Simple": false,
    "Demo_Authenticator_Authenticate_DomainAccountName": "CERTUS\\vbastidas77",

    "Demo_AccountManager_DisableUserAccount_RunTest": false,
    "Demo_AccountManager_DisableUserAccount_UserAccountDistinguishedName": "CN=Victor Bastidas,OU=TEST_RPA_MDA,OU=ADM,OU=CERTUS,DC=certus,DC=edu,DC=pe",

    "Demo_AccountManager_RemoveUserAccount_RunTest": false,
    "Demo_AccountManager_RemoveUserAccount_UserAccountDistinguishedName": "CN=Victor German Bastidas Gonzales,OU=TEST_RPA_MDA,OU=ADM,OU=CERTUS,DC=certus,DC=edu,DC=pe",

    "Demo_Searcher_SearchUsers_RunTest": false,
    "Demo_Searcher_SearchUsers_Filter_sAMAccountName": "00000002",
    "Demo_Searcher_SearchUsers_Filter_cn": "*Isaac*",
    "Demo_Searcher_SearchUsersByTwoFilters_Filter1_sAMAccountName": "*cordoba",
    "Demo_Searcher_SearchUsersByTwoFilters_Filter2_cn": "*manuel*",

    "Demo_Searcher_SearchEntries_RunTest": false,
    "Demo_Searcher_SearchEntries_Filter_cn": "SRVWEBDEV",
    "Demo_Searcher_SearchEntries_Filter_objectSid": "S-1-5-21-3451337281-1996239963-2625140484-1000",
    "Demo_Searcher_SearchEntriesByTwoFilters_Filter1_cn": "*server*",
    "Demo_Searcher_SearchEntriesByTwoFilters_Filter2_cn": "*database*",

    "Demo_Searcher_SearchParentEntries_RunTest": false,
    "Demo_Searcher_SearchParentEntries_Filter_sAMAccountName": "00000002",

    "Demo_GroupMembershipValidator_RunTest": false,
    "Demo_GroupMembershipValidator_CheckGroupmembership_sAMAccountName": "Administrator",
    "Demo_GroupMembershipValidator_CheckGroupmembership_Check_GroupName": "Administrators"
}
```

- After you have edited the parameter values in the configuration file, you can run the demo.

## You may also be interested in

- [Bitai.IdentityServer4.Admin](https://github.com/bitai-cs/IdentityServer4.Admin) forked from [skoruba.IdentityServer4.Admin](https://github.com/skoruba/IdentityServer4.Admin). This forked repository proposes to implement user authentication on an LDAP server.
