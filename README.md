# LDAP Helper ![Logo](resources/hierarchy_32.png)

**.NET Standard 2.0** library which wraps  **Novell.Directory.Ldap.NETStandard** functionality to make LDAP searches and also authenticate users against a Directory Service.

This libary is part of [Bitai.LDAPWebApi](https://github.com/bitai-cs/LDAPWebApi) solution 

### Requirements

- **.NET Standard 2.0**
  - Have at least [.NET Core 2.0 SDK installed](https://dotnet.microsoft.com/download/dotnet/2.0) on your system.  

### How to use the library?

- See the project **Bitai.LDAPHelper.Demo** to get a reference  

### How to execute Demo?

- First, make a copy, on your disk, of the  [Bitai.LDAPHelper.Demo/ldaphelper_demosetup.json](src/Bitai.LDAPHelper.Demo/ldaphelper_demosetup.json) file.
- Edit the copied file and set the parameters as needed.
```json
{
    "DomainAccountName": "DOMAIN\\AccountName",
    "LdapServers": [
        {
            "Address": "10.113.58.132"
        },
        {
            "Address": "140.141.33.65"
        },
        {
            "Address": "55.55.55.13"
        },
        {
            "Address": "34.11.58.13"
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
            "DN": "DC=company, DC=com"
        },
        {
            "DN": "DC=us, DC=company, DC=com"
        },
        {
            "DN": "DC=uk, DC=company, DC=com"
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
    "UseSSL": false,
    "Demo_Authenticator_Authenticate_DomainAccountName": "DOMAIN\\AccountName",
    "Demo_Searcher_SearchUsersAndGroups_Filter_sAMAccountName": "usr_ext01",
    "Demo_Searcher_SearchUsersAndGroups_Filter_cn": "*Bastidas*",
    "Demo_Searcher_SearchUsersAndGroupsByTwoFilters_Filter1_sAMAccountName": "*smith",
    "Demo_Searcher_SearchUsersAndGroupsByTwoFilters_Filter2_cn": "*Smith*",
    "Demo_Searcher_SearchEntries_Filter_cn": "DB Server",
    "Demo_Searcher_SearchEntries_Filter_objectSid": "S-1-5-21-638406840-1180129177-883519231-179439",
    "Demo_Searcher_SearchEntriesByTwoFilters_Filter1_cn": "*server*",
    "Demo_Searcher_SearchEntriesByTwoFilters_Filter2_cn": "*database*",
    "Demo_Searcher_SearchParentEntries_Filter_sAMAccountName": "usr_ext01",
    "Demo_GroupMembershipValidator_CheckGroupmembership_sAMAccountName": "4439690",
    "Demo_GroupMembershipValidator_CheckGroupmembership_Check_GroupName": "Software Administrators"
}
```
- After you have edited the parameter values in the configuration file, you can run the demo. 

### You may also be interested in:

- [Bitai.IdentityServer4.Admin](https://github.com/bitai-cs/IdentityServer4.Admin) forked from [skoruba.IdentityServer4.Admin](https://github.com/skoruba/IdentityServer4.Admin). This forked repository proposes to implement user authentication on an LDAP server.
