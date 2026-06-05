# Bitai.LDAPHelper ![Logo](resources/hierarchy_32.png)

A high-performance library wrapping **Novell.Directory.Ldap.NETStandard** functionality to interact with LDAP-compliant Directory Services (such as Microsoft Active Directory). It simplifies operations like searching, authenticating users, and creating, modifying, disabling, or deleting accounts.

This library is a key component of the [Bitai.LDAPWebApi](https://github.com/bitai-cs/LDAPWebApi) solution ecosystem.

---

## 🚀 Key Features

The library is organized into specialized helper classes targeting specific directory operations:

### 🛡️ 1. Authentication (`Authenticator`)
Enables quick and secure validation of user credentials on the LDAP/AD server.
- Supports both **User Distinguished Name (DN)** (`LDAPDistinguishedNameCredential`) and **Domain Username** (`LDAPDomainAccountCredential`) credential structures.
- Provides a simple authentication bind check (`AuthenticateAsync(credential)`).
- Provides a comprehensive, pre-validated authentication workflow that first searches the user entry in Active Directory to ensure uniqueness and validity before attempting the bind (`AuthenticateAsync(credential, searchLimits, searchCredential)`).

### 👤 2. Account Management (`AccountManager`)
Simplifies user provisioning and lifecycle management within Active Directory.
- **Create User Accounts** (`CreateUserAccountForMsAD`): Provision new Active Directory entries (`LDAPMsADUserAccount`) with extensive attribute mappings (such as UPN, sAMAccountName, unicodePwd, department, memberOf, object classes, and user control flags).
- **Set/Change Passwords** (`SetUserAccountPasswordForMsAD`): Safe password replacement utilizing secure Unicode encoding. It can optionally test immediate post-update authentication.
- **Disable Accounts** (`DisableUserAccountForMsAD`): Securely disables accounts by updating the `userAccountControl` attribute with the `ACCOUNTDISABLE` flag, dynamically preserving all other existing account name flags to prevent unintended configuration loss.
- **Remove Accounts** (`RemoveUserAccountForMsAD`): Permanently deletes user entries from the directory service.

### 🔍 3. Directory Searching (`Searcher`)
Facilitates fast, flexible searching using highly customizable LDAP search configurations.
- **Filtered Searching** (`SearchEntriesAsync`): Execute directory searches using strongly-typed, combinable query filters (`AttributeFilter`, `AttributeFilterCombiner`).
- **Parent Hierarchy Resolution** (`SearchParentEntriesAsync`): Traverses directory hierarchies and recursively resolves full parent group/container entries (from the `memberOf` attribute) rather than just returning raw string paths.
- **Performance Optimized**: Fine-tune query execution by specifying search size/timeout limits and choosing which attributes to load via the `RequiredEntryAttributes` enum (e.g. `OnlyCN`, `OnlyObjectSid`, `Few`, `All`).

### 👥 4. Group Membership Validation (`GroupMembershipValidator`)
Specialized operations to determine security group memberships and roles.
- **Membership Verification** (`CheckGroupMembershipAsync`): Resolves whether a specific user (`sAMAccountName`) is a member (either direct or via nested/inherited groups) of a given target group (`CN`).
- **Complete Group Resolution** (`GetAllGroupMembershipsAsync`): Recursively retrieves all unique group common names (`CN`s) a user belongs to.

---

## 🏛️ Project Architecture

The solution `LDAP Helper Libraries.sln` consists of the following projects:

1. **`Bitai.LDAPHelper`** (`src/Bitai.LDAPHelper`): The core library containing helper classes (`Authenticator`, `AccountManager`, `Searcher`, `GroupMembershipValidator`) and query construction engines.
2. **`Bitai.LDAPHelper.DTO`** (`src/Bitai.LDAPHelper.DTO`): Light-weight, structured Data Transfer Objects, interfaces, and enums (e.g., `LDAPEntry`, `LDAPMsADUserAccount`, results structures, and secure cloning models).
3. **`Bitai.LDAPHelper.Demo`** (`demo/Bitai.LDAPHelper.Demo`): An interactive console application demonstrating all features using both real LDAP connections and simulated in-memory mock data.
4. **`Bitai.LDAPHelper.Tests`** (`tests/Bitai.LDAPHelper.Tests`): xUnit testing suite verifying LDAP wrappers, filters, and helper methods.

---

## ⚙️ Requirements & Dependencies

- **.NET 10 SDK** or higher.
- **Novell.Directory.Ldap.NETStandard** (v4.0.0+) package dependency.

---

## 💻 Quick Start & Usage Examples

### 1. Authenticate a Domain User
```csharp
using Bitai.LDAPHelper;
using Bitai.LDAPHelper.DTO;
using Bitai.LDAPHelper.Adapters.Novell;

var connectionInfo = new ConnectionInfo("dc.company.com", 389, useSsl: false, timeoutSeconds: 15);
var factory = new NovellLdapConnectionFactoryAdapter();
var authenticator = new Authenticator(connectionInfo, factory);

var credential = new LDAPDomainAccountCredential("COMPANY", "john.doe", "MySecurePassword123!");
var authResult = await authenticator.AuthenticateAsync(credential);

if (authResult.IsSuccessfulOperation && authResult.IsAuthenticated)
{
    Console.WriteLine("User successfully authenticated!");
}
else
{
    Console.WriteLine($"Authentication failed: {authResult.OperationMessage}");
}
```

### 2. Search for Users
```csharp
using Bitai.LDAPHelper;
using Bitai.LDAPHelper.DTO;
using Bitai.LDAPHelper.QueryFilters;
using Bitai.LDAPHelper.Adapters.Novell;

var connectionInfo = new ConnectionInfo("dc.company.com", 389, useSsl: false, timeoutSeconds: 15);
var searchLimits = new SearchLimits("DC=company,DC=com", maxSearchResults: 100, maxSearchTimeoutSeconds: 30);
var adminCredential = new LDAPDomainAccountCredential("COMPANY", "admin.user", "AdminPassword!");
var factory = new NovellLdapConnectionFactoryAdapter();

var searcher = new Searcher(connectionInfo, searchLimits, adminCredential, factory);

// Build search filter: user must be active and have sAMAccountName match '*smith*'
var onlyUsersFilter = AttributeFilterCombiner.CreateOnlyUsersFilterCombiner();
var nameFilter = new AttributeFilter(EntryAttribute.sAMAccountName, new FilterValue("*smith*"));
var combinedFilter = new AttributeFilterCombiner(negate: false, conjunctive: true, new ICombinableFilter[] { onlyUsersFilter, nameFilter });

var searchResult = await searcher.SearchEntriesAsync(combinedFilter, RequiredEntryAttributes.Few, requestLabel: "UserSearch");

if (searchResult.IsSuccessfulOperation)
{
    foreach (var entry in searchResult.Entries)
    {
        Console.WriteLine($"User: {entry.cn} (sAMAccountName: {entry.samAccountName}), UPN: {entry.userPrincipalName}");
    }
}
```

### 3. Verify Nested Group Membership
```csharp
using Bitai.LDAPHelper;
using Bitai.LDAPHelper.DTO;
using Bitai.LDAPHelper.Adapters.Novell;

var connectionInfo = new ConnectionInfo("dc.company.com", 389, useSsl: false, timeoutSeconds: 15);
var searchLimits = new SearchLimits("DC=company,DC=com", maxSearchResults: 100, maxSearchTimeoutSeconds: 30);
var adminCredential = new LDAPDomainAccountCredential("COMPANY", "admin.user", "AdminPassword!");
var factory = new NovellLdapConnectionFactoryAdapter();

var validator = new GroupMembershipValidator(connectionInfo, searchLimits, adminCredential, factory);

bool isMember = await validator.CheckGroupMembershipAsync(sAMAccountName: "john.doe", parentGroupCN: "HR_Department_Managers");

Console.WriteLine($"Is member (direct or nested): {isMember}");
```

---

## 🛠️ Configuring and Running the Demo Application

The **`Bitai.LDAPHelper.Demo`** console application is a rich test bench designed to execute all primary library methods. 

### 💡 Two Execution Modes
The demo application provides two implementations to let developers test functionality seamlessly:
1. **Mock Mode (Simulated LDAP)**: **Does not require a live LDAP server.** It runs entirely in-memory using simulated directory entries seeded upon startup (`MockDataStore` & `MockDataSeeder`). This mode is ideal for local development, rapid feedback, and offline debugging.
2. **Novell Mode (Real LDAP)**: Connects directly to an active directory service. It prompts you interactively for server connection details and password bindings.

### 📋 Setup & Configuration Steps

#### Step 1: Deploy Configuration File
The demo application is programmed to load its configuration file named **`ldaphelper_demosetup.json`** directly from the logged-in user's **Desktop** folder.

1. Locate the sample setup file at: `demo/Bitai.LDAPHelper.Demo/ldaphelper_demosetup.json`
2. **Copy this file directly to your Desktop directory.**

#### Step 2: Configure Environment Parameters
Edit the copied **`ldaphelper_demosetup.json`** on your Desktop. Adjust parameters as needed for either Mock or Novell operations:

```json
{
   "LdapServers": [
      { "Address": "dcsrvr1.company.com" },
      { "Address": "10.100.54.4" }
   ],
   "BaseDNs": [
      { "DN": "DC=company,DC=com" }
   ],
   "ConnectionTimeout": 15,
   "DomainUserAccountForRunTests": "COMPANY\\service.account",
   "AccountPassword": "SecretPassword123!",

   // Set true to execute individual test cases when the app runs:
   "Demo_AccountManager_CreateUserAccount_RunTest": true,
   "Demo_AccountManager_CreateUserAccount_UserAccountName": "victor.bastidas",
   "Demo_AccountManager_CreateUserAccount_Password": "NewPassword2026!",
   "Demo_AccountManager_CreateUserAccount_ContainerDN": "OU=Users,DC=company,DC=com",
   "Demo_AccountManager_CreateUserAccount_Name": "Victor",
   "Demo_AccountManager_CreateUserAccount_Surname": "Bastidas",
   "Demo_AccountManager_CreateUserAccount_DNSDomainName": "company.com",
   "Demo_AccountManager_CreateUserAccount_MemberOf": null,
   "Demo_AccountManager_CreateUserAccount_ObjectClasses": "user,top,person,organizationalPerson",
   "Demo_AccountManager_CreateUserControlFlags": "NORMAL_ACCOUNT,DONT_EXPIRE_PASSWORD",

   "Demo_AccountManager_SetAccountPassword_RunTest": false,
   "Demo_AccountManager_SetAccountPassword_DistinguishedName": "CN=Victor Bastidas,OU=Users,DC=company,DC=com",

   "Demo_Authenticator_Authenticate_RunTest": false,
   "Demo_Authenticator_Authenticate_RunTest_Simple": false,
   "Demo_Authenticator_Authenticate_DomainAccountName": "COMPANY\\victor.bastidas",

   "Demo_AccountManager_DisableUserAccount_RunTest": false,
   "Demo_AccountManager_DisableUserAccount_UserAccountDistinguishedName": "CN=Victor Bastidas,OU=Users,DC=company,DC=com",

   "Demo_AccountManager_RemoveUserAccount_RunTest": false,
   "Demo_AccountManager_RemoveUserAccount_UserAccountDistinguishedName": "CN=Victor Bastidas,OU=Users,DC=company,DC=com",

   "Demo_Searcher_SearchUsers_RunTest": true,
   "Demo_Searcher_SearchUsers_Filter_sAMAccountName": "victor.bastidas",
   "Demo_Searcher_SearchUsers_Filter_cn": "*Bastidas*",
   
   "Demo_Searcher_SearchEntries_RunTest": false,
   "Demo_Searcher_SearchEntries_Filter_cn": "DEVSERVER01",
   
   "Demo_Searcher_SearchParentEntries_RunTest": false,
   "Demo_Searcher_SearchParentEntries_Filter_sAMAccountName": "victor.bastidas",

   "Demo_GroupMembershipValidator_RunTest": true,
   "Demo_GroupMembershipValidator_CheckGroupmembership_sAMAccountName": "victor.bastidas",
   "Demo_GroupMembershipValidator_CheckGroupmembership_Check_GroupName": "Administrators"
}
```

### 🏃 Running the Demo

Open a terminal at the root of the LDAPHelper solution folder and execute:

```powershell
dotnet run --project demo/Bitai.LDAPHelper.Demo/Bitai.LDAPHelper.Demo.csproj
```

#### Selection and Arguments:
- **Interactive Mode**: By default, starting the console will prompt you to select:
  - `1` for Novell (Real LDAP Connection)
  - `2` for Mock (Simulated LDAP, Offline)
- **Automatic / Scripted Mode**: You can bypass the interactive prompts by passing the `--implementation` or `-i` command line arguments:
  ```powershell
  # Directly start Mock implementation:
  dotnet run --project demo/Bitai.LDAPHelper.Demo/Bitai.LDAPHelper.Demo.csproj -- --implementation mock
  
  # Directly start Novell implementation:
  dotnet run --project demo/Bitai.LDAPHelper.Demo/Bitai.LDAPHelper.Demo.csproj -- --implementation novell
  ```

Once started, the console app will automatically execute the active test routines toggled as `true` in your Desktop's `ldaphelper_demosetup.json`, print step-by-step logs, record performance, and output a detailed execution summary table.

---

## 🤝 Community & Ecosystem

- Part of the wider [Bitai.LDAPWebApi](https://github.com/bitai-cs/LDAPWebApi) project.
- Feel free to check out [Bitai.IdentityServer4.Admin](https://github.com/bitai-cs/IdentityServer4.Admin) (forked from `Skoruba.IdentityServer4.Admin`) which provides an identity server administration UI backed by LDAP authentication pipelines.
