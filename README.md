# Bitai.LDAPHelper ![Logo](resources/hierarchy_32.png)

![.NET 10.0](https://img.shields.io/badge/.NET-10.0-512BD4)
![License: MIT](https://img.shields.io/badge/license-MIT-green)
![Package](https://img.shields.io/badge/package-Bitai.LDAPHelper-blue)
![Version](https://img.shields.io/badge/version-10.2.0-orange)

A high-performance, adapter-driven .NET 10 library that simplifies interacting with Microsoft Active Directory and RFC-compliant LDAP Directory Services. 

It abstracts complex low-level LDAP network protocol operations—such as binds, asynchronous searches, attribute modifications, Unicode password encoding, and Active Directory `userAccountControl` bitwise flags—into clean, async C# services.

---

## Table of Contents

- [Overview](#overview)
- [Solution Architecture](#solution-architecture)
- [Core Functional Modules](#core-functional-modules)
  - [1. Authentication (`Authenticator`)](#1-authentication-authenticator)
  - [2. Account Lifecycle Management (`AccountManager`)](#2-account-lifecycle-management-accountmanager)
  - [3. Directory Search Engine (`Searcher`)](#3-directory-search-engine-searcher)
  - [4. Group Membership Validation (`GroupMembershipValidator`)](#4-group-membership-validation-groupmembershipvalidator)
  - [5. Combinable Query Filters Engine (`Bitai.LDAPHelper.QueryFilters`)](#5-combinable-query-filters-engine-bitaildaphelperqueryfilters)
- [Adapter Infrastructure](#adapter-infrastructure)
- [Class Architecture Taxonomy](#class-architecture-taxonomy)
- [Quick Start](#quick-start)
- [Detailed Usage Scenarios](#detailed-usage-scenarios)
  - [Scenario 1: User Credentials Authentication](#scenario-1-user-credentials-authentication)
  - [Scenario 2: Active Directory Account Provisioning](#scenario-2-active-directory-account-provisioning)
  - [Scenario 3: Filtered Directory Searching](#scenario-3-filtered-directory-searching)
  - [Scenario 4: Nested Group Membership Resolution](#scenario-4-nested-group-membership-resolution)
  - [Scenario 5: Offline Unit Testing with Mock Adapter](#scenario-5-offline-unit-testing-with-mock-adapter)
- [Demo Console Application](#demo-console-application)
- [Build, Test & Package](#build-test--package)
- [Troubleshooting Matrix](#troubleshooting-matrix)
- [Security & Operational Guidance](#security--operational-guidance)
- [License & Community Ecosystem](#license--community-ecosystem)

---

## Overview

`Bitai.LDAPHelper` provides enterprise-grade infrastructure for enterprise applications requiring directory authentication, Active Directory user management, and organizational membership resolution.

### Primary Design Goals

- **Driver Decoupling via Adapters:** Decouples higher-level services from specific LDAP protocol drivers (`Novell.Directory.Ldap.NETStandard`) via pluggable adapter interfaces (`ILdapConnectionFactoryAdapter`, `ILdapConnectionAdapter`).
- **Production & Testing Parity:** Enables seamless switching between production directory servers (`Bitai.LDAPHelper.LdapAdapters.Novell`) and high-speed in-memory mock datasets (`Bitai.LDAPHelper.LdapAdapters.LdapHelperMock`).
- **Active Directory Specialization:** Built-in support for MS Active Directory specific attributes (`sAMAccountName`, `userPrincipalName`, `unicodePwd`, `userAccountControl`, `objectSid`).

---

## Solution Architecture

```
┌─────────────────────────────────────────────────────────────────────────┐
│                          Consumer Application                           │
│        (ASP.NET Core Web API, IdentityServer4, Background Worker)       │
└─────────────────────────────────────────────────────────────────────────┘
                                     │
                                     ▼
┌─────────────────────────────────────────────────────────────────────────┐
│                            Bitai.LDAPHelper                             │
│  ┌─────────────────┐ ┌─────────────────┐ ┌──────────┐ ┌──────────────┐  │
│  │ Authenticator   │ │ AccountManager  │ │ Searcher │ │ GroupVal...  │  │
│  └─────────────────┘ └─────────────────┘ └──────────┘ └──────────────┘  │
│        │                     │                 │             │          │
│        └─────────────────────┼─────────────────┴─────────────┘          │
│                              ▼                                          │
│                   QueryFilters Engine & BaseHelper                      │
└─────────────────────────────────────────────────────────────────────────┘
                                     │
                        ILdapConnectionFactoryAdapter
                        ILdapConnectionAdapter
                                     │
            ┌────────────────────────┴────────────────────────┐
            ▼                                                 ▼
┌───────────────────────────────────────┐ ┌───────────────────────────────┐
│ Bitai.LDAPHelper.LdapAdapters.Novell  │ │  ...LdapAdapters.LdapHelperMock│
│  (Production / Novell LDAP Driver)    │ │   (In-Memory Test / CI Mode)  │
└───────────────────────────────────────┘ └───────────────────────────────┘
                    │                                         │
                    ▼                                         ▼
┌───────────────────────────────────────┐ ┌───────────────────────────────┐
│     Live LDAP / Active Directory      │ │     MockLdapDataStore         │
└───────────────────────────────────────┘ └───────────────────────────────┘
```

### Solution Project Breakdown

| Project | Path | Purpose |
|---|---|---|
| **`Bitai.LDAPHelper`** | [`src/Bitai.LDAPHelper`](file:///c:/_bitai/Bitai.LDAPHelper/src/Bitai.LDAPHelper) | Core service framework, query engine, and adapter interfaces. |
| **`Bitai.LDAPHelper.DTO`** | [`src/Bitai.LDAPHelper.DTO`](file:///c:/_bitai/Bitai.LDAPHelper/src/Bitai.LDAPHelper.DTO) | Data Transfer Objects, credentials, and operation result models. |
| **`Bitai.LDAPHelper.LdapAdapters.Novell`** | [`adapters/Bitai.LDAPHelper.LdapAdapters.Novell`](file:///c:/_bitai/Bitai.LDAPHelper/adapters/Bitai.LDAPHelper.LdapAdapters.Novell) | Novell-backed production LDAP adapter for live network servers. |
| **`Bitai.LDAPHelper.LdapAdapters.LdapHelperMock`** | [`adapters/Bitai.LDAPHelper.LdapAdapters.LdapHelperMock`](file:///c:/_bitai/Bitai.LDAPHelper/adapters/Bitai.LDAPHelper.LdapAdapters.LdapHelperMock) | In-memory mock adapter and directory seeder for unit/integration tests. |
| **`Bitai.LDAPHelper.Demo`** | [`demo/Bitai.LDAPHelper.Demo`](file:///c:/_bitai/Bitai.LDAPHelper/demo/Bitai.LDAPHelper.Demo) | Interactive console test bench supporting both Mock and Novell modes. |
| **`Bitai.LDAPHelper.Tests`** | [`tests/Bitai.LDAPHelper.Tests`](file:///c:/_bitai/Bitai.LDAPHelper/tests/Bitai.LDAPHelper.Tests) | xUnit testing suite covering services, filters, and adapters. |

---

## Core Functional Modules

### 1. Authentication (`Authenticator`)

`Authenticator` validates user credentials against LDAP servers.

- **Direct Credentials Bind:** Supports both domain user account credentials (`LDAPDomainAccountCredential`) and explicit distinguished name credentials (`LDAPDistinguishedNameCredential`).
- **Pre-Validated Search & Bind:** Performs an initial directory search to verify user account existence, uniqueness, and active state before attempting the bind operation, guarding against ambiguous domain lookups.

### 2. Account Lifecycle Management (`AccountManager`)

`AccountManager` handles directory object provisioning and state updates for Active Directory:

- **User Provisioning (`CreateUserAccountForMsADAsync`)**: Creates Active Directory user objects (`LDAPMsADUserAccount`) with automatic mapping of attributes (`cn`, `sAMAccountName`, `userPrincipalName`, `givenName`, `sn`, `mail`, `department`, `unicodePwd`, `objectClass`, `userAccountControl`).
- **Password Updates (`SetUserAccountPasswordForMsADAsync`)**: Encodes passwords in double-quoted UTF-16LE binary format required by Active Directory `unicodePwd`. Supports optional immediate authentication verification.
- **Account Disabling (`DisableUserAccountForMsADAsync`)**: Performs bitwise OR operations on `userAccountControl` to add `ACCOUNTDISABLE` (0x0002) while preserving all existing account control flags (`NORMAL_ACCOUNT`, `DONT_EXPIRE_PASSWORD`, etc.).
- **Account Deletion (`RemoveUserAccountForMsADAsync`)**: Permanently removes specified user entries from Active Directory.
- **Generic Directory Operations (`AddEntryAsync`, `ModifyEntryAsync`, `DeleteEntryAsync`)**: Low-level CRUD operations on raw LDAP entries and attribute sets.

### 3. Directory Search Engine (`Searcher`)

`Searcher` executes optimized directory queries:

- **Targeted Lookups (`GetLdapEntryBySamAccountNameAsync`)**: Fast search by `sAMAccountName`.
- **Advanced Query Filtering (`SearchEntriesAsync`)**: Executes directory searches using combinable query filters.
- **Parent Hierarchy Resolution (`SearchParentEntriesAsync`)**: Traverses `memberOf` attributes to recursively fetch full `LDAPEntry` parent container objects rather than raw string DNs.
- **Attribute Projection Optimization (`RequiredEntryAttributes`)**: Controls memory and network overhead by selecting attribute projections (`OnlyCN`, `OnlyObjectSid`, `Few`, `All`).

### 4. Group Membership Validation (`GroupMembershipValidator`)

`GroupMembershipValidator` inspects security group assignments:

- **Recursive Membership Resolution (`CheckGroupMembershipAsync`)**: Determines if a user (`sAMAccountName`) belongs to a group (`CN`), automatically evaluating direct memberships and nested/inherited child group trees.
- **Full Group Membership Extraction (`GetAllGroupMembershipsAsync`)**: Returns a list of all unique group common names (`CN`s) assigned to a given account.

### 5. Combinable Query Filters Engine (`Bitai.LDAPHelper.QueryFilters`)

The query engine constructs clean, injection-safe LDAP filter strings:

```csharp
// Example: Active users in Engineering department matching 'smith*'
var activeFilter = new OnlyActiveAccountsFilterCombiner();
var userFilter = new OnlyUsersFilterCombiner();
var nameFilter = new AttributeFilter(EntryAttribute.sAMAccountName, new FilterValue("*smith*"));

var combined = new AttributeFilterCombiner(
    negate: false, 
    conjunctive: true, 
    new ICombinableFilter[] { activeFilter, userFilter, nameFilter }
);

string ldapQuery = combined.GetFilter(); 
// Output: (&(&(objectCategory=person)(objectClass=user))(!(userAccountControl:1.2.840.113556.1.4.803:=2))(sAMAccountName=*smith*))
```

---

## Adapter Infrastructure

To prevent direct coupling to specific LDAP drivers, `Bitai.LDAPHelper` relies on core adapter abstractions defined in `Bitai.LDAPHelper.LdapAdapters`:

```csharp
namespace Bitai.LDAPHelper.LdapAdapters
{
    public interface ILdapConnectionFactoryAdapter
    {
        Task<ILdapConnectionAdapter> CreateConnectionAsync(
            IConnectionInfo connectionInfo, 
            string userAccount, 
            string password, 
            bool bindRequired = true);
    }

    public interface ILdapConnectionAdapter : IDisposable
    {
        int ConnectionTimeout { get; set; }
        bool SecureSocketLayer { get; set; }
        bool IsBound { get; }
        
        Task ConnectAsync(string host, int port);
        Task BindAsync(string userDN, string password);
        Task<ILdapSearchQueueAdapter> SearchAsync(ISearchLimits searchLimits, string searchFilter, string[] attributeNames, bool typesOnly);
        Task AddEntryAsync(string distinguishedName, ILdapAttributeSetAdapter attributes);
        Task ModifyEntryAsync(string distinguishedName, IEnumerable<ILdapModificationAdapter> modifications);
        Task DeleteEntryAsync(string distinguishedName);
    }
}
```

---

## Class Architecture Taxonomy

| Class / Interface | Namespace | Role |
|---|---|---|
| `Authenticator` | `Bitai.LDAPHelper` | Handles domain account & DN credential authentication workflows. |
| `AccountManager` | `Bitai.LDAPHelper` | Provisions, updates passwords, disables, and deletes MS AD accounts. |
| `Searcher` | `Bitai.LDAPHelper` | Executes searches, projections, and parent group resolutions. |
| `GroupMembershipValidator` | `Bitai.LDAPHelper` | Validates direct and nested security group memberships. |
| `BaseHelper` | `Bitai.LDAPHelper` | Base class managing connection factory lifetimes, logging, and error handling. |
| `AttributeFilter` | `Bitai.LDAPHelper.QueryFilters` | Represents single attribute filter conditions (`attr=value`). |
| `AttributeFilterCombiner` | `Bitai.LDAPHelper.QueryFilters` | Combines multiple filters into AND (`&`) or OR (`\|`) expressions. |
| `LDAPEntry` | `Bitai.LDAPHelper.DTO` | DTO representing an LDAP directory record with parsed properties. |
| `LDAPMsADUserAccount` | `Bitai.LDAPHelper.DTO` | Detailed model representing Active Directory user account attributes. |
| `ConnectionInfo` | `Bitai.LDAPHelper` | Defines LDAP server host, port, SSL settings, and timeouts. |
| `SearchLimits` | `Bitai.LDAPHelper` | Defines search Base DN, scope, max results, and execution timeouts. |

---

## Quick Start

### Installation

Add `Bitai.LDAPHelper` and a connection adapter package (such as `Bitai.LDAPHelper.LdapAdapters.Novell`) to your .NET project:

```bash
dotnet add package Bitai.LDAPHelper
dotnet add package Bitai.LDAPHelper.LdapAdapters.Novell
```

---

## Detailed Usage Scenarios

### Scenario 1: User Credentials Authentication

```csharp
using Bitai.LDAPHelper;
using Bitai.LDAPHelper.DTO;
using Bitai.LDAPHelper.LdapAdapters.Novell;

var connectionInfo = new ConnectionInfo("dc01.contoso.com", 389, useSSL: false, connectionTimeout: 15);
var searchLimits = new SearchLimits("DC=contoso,DC=com");
var serviceCreds = new LDAPDomainAccountCredential("CONTOSO", "svc_auth", "ServicePass123!");

var factory = new NovellLdapConnectionFactoryAdapter();
var authenticator = new Authenticator(connectionInfo, searchLimits, serviceCreds, factory);

// Authenticate user
var userCreds = new LDAPDomainAccountCredential("CONTOSO", "jdoe", "UserPassword!");
var result = await authenticator.AuthenticateDomainAccountAsync(userCreds);

if (result.IsAuthenticated)
{
    Console.WriteLine($"Successfully authenticated user: {result.UserEntry?.CN}");
}
else
{
    Console.WriteLine($"Authentication failed: {result.ResultDetail}");
}
```

---

### Scenario 2: Active Directory Account Provisioning

```csharp
using Bitai.LDAPHelper;
using Bitai.LDAPHelper.DTO;
using Bitai.LDAPHelper.LdapAdapters.Novell;

var connectionInfo = new ConnectionInfo("dc01.contoso.com", 389, useSSL: false, connectionTimeout: 15);
var searchLimits = new SearchLimits("DC=contoso,DC=com");
var adminCreds = new LDAPDomainAccountCredential("CONTOSO", "DomainAdmin", "AdminSecret!");
var factory = new NovellLdapConnectionFactoryAdapter();

var accountManager = new AccountManager(connectionInfo, searchLimits, adminCreds, factory);

// Build new MS AD User Account model
var newUser = new LDAPMsADUserAccount
{
    SamAccountName = "alice.smith",
    UserPrincipalName = "alice.smith@contoso.com",
    Name = "Alice",
    Surname = "Smith",
    Email = "alice.smith@contoso.com",
    ContainerDN = "OU=Employees,DC=contoso,DC=com",
    Password = "P@ssword2026!Secure",
    UserAccountControlFlags = UserAccountControlFlags.NORMAL_ACCOUNT | UserAccountControlFlags.DONT_EXPIRE_PASSWORD
};

var createResult = await accountManager.CreateUserAccountForMsADAsync(newUser);

if (createResult.IsSuccess)
{
    Console.WriteLine($"User account created successfully: {createResult.DistinguishedName}");
}
```

---

### Scenario 3: Filtered Directory Searching

```csharp
using Bitai.LDAPHelper;
using Bitai.LDAPHelper.DTO;
using Bitai.LDAPHelper.QueryFilters;
using Bitai.LDAPHelper.LdapAdapters.Novell;

var connectionInfo = new ConnectionInfo("dc01.contoso.com", 389, useSSL: false, connectionTimeout: 15);
var searchLimits = new SearchLimits("DC=contoso,DC=com", maxSearchResults: 200, maxSearchTimeout: 30);
var adminCreds = new LDAPDomainAccountCredential("CONTOSO", "svc_reader", "Pass123!");
var factory = new NovellLdapConnectionFactoryAdapter();

var searcher = new Searcher(connectionInfo, searchLimits, adminCreds, factory);

// Query active accounts matching 'j*' in sAMAccountName
var userFilter = AttributeFilterCombiner.CreateOnlyUsersFilterCombiner();
var activeFilter = AttributeFilterCombiner.CreateOnlyActiveAccountsFilterCombiner();
var samFilter = new AttributeFilter(EntryAttribute.sAMAccountName, new FilterValue("j*"));

var combinedFilter = new AttributeFilterCombiner(
    negate: false, 
    conjunctive: true, 
    new ICombinableFilter[] { userFilter, activeFilter, samFilter }
);

var searchResult = await searcher.SearchEntriesAsync(combinedFilter, RequiredEntryAttributes.Few, requestLabel: "ActiveUserSearch");

foreach (var entry in searchResult.Entries)
{
    Console.WriteLine($"Found Account: {entry.SamAccountName} | Mail: {entry.Mail} | UPN: {entry.UserPrincipalName}");
}
```

---

### Scenario 4: Nested Group Membership Resolution

```csharp
using Bitai.LDAPHelper;
using Bitai.LDAPHelper.DTO;
using Bitai.LDAPHelper.LdapAdapters.Novell;

var connectionInfo = new ConnectionInfo("dc01.contoso.com", 389, useSSL: false, connectionTimeout: 15);
var searchLimits = new SearchLimits("DC=contoso,DC=com");
var adminCreds = new LDAPDomainAccountCredential("CONTOSO", "svc_reader", "Pass123!");
var factory = new NovellLdapConnectionFactoryAdapter();

var validator = new GroupMembershipValidator(connectionInfo, searchLimits, adminCreds, factory);

// Check if 'jdoe' belongs to 'Finance_Admins' (either directly or via nested group inheritance)
bool isFinanceAdmin = await validator.CheckGroupMembershipAsync("jdoe", "Finance_Admins");

Console.WriteLine($"User jdoe belongs to Finance_Admins: {isFinanceAdmin}");

// Fetch all unique group membership CNs for user
var groups = await validator.GetAllGroupMembershipsAsync("jdoe");
Console.WriteLine($"Assigned Groups: {string.Join(", ", groups)}");
```

---

### Scenario 5: Offline Unit Testing with Mock Adapter

```csharp
using Xunit;
using Bitai.LDAPHelper;
using Bitai.LDAPHelper.DTO;
using Bitai.LDAPHelper.LdapAdapters.LdapHelperMock;

public class SearcherTests
{
    [Fact]
    public async Task GetUser_UsingMockAdapter_ShouldReturnStubbedUser()
    {
        // 1. Instantiate in-memory mock connection
        var mockConnection = new MockLdapConnectionAdapter();

        // 2. Add stubbed search response
        var stubEntry = new MockLdapEntryAdapter("CN=Test User,OU=Users,DC=example,DC=com");
        stubEntry.AddAttribute("sAMAccountName", "testuser");
        stubEntry.AddAttribute("mail", "testuser@example.com");
        
        mockConnection.AddSearchResult("(sAMAccountName=testuser)", new List<MockLdapEntryAdapter> { stubEntry });

        // 3. Pass mock factory to Searcher
        var factory = new MockLdapConnectionFactoryAdapter(mockConnection);
        var searcher = new Searcher(
            new ConnectionInfo("localhost", 389, false, 5),
            new SearchLimits("DC=example,DC=com"),
            new LDAPDomainAccountCredential("EXAMPLE", "admin", "pass"),
            factory
        );

        // 4. Test service without network access
        var user = await searcher.GetLdapEntryBySamAccountNameAsync("testuser");

        Assert.NotNull(user);
        Assert.Equal("testuser", user.SamAccountName);
        Assert.Equal("testuser@example.com", user.Mail);
    }
}
```

---

## Demo Console Application

The repository includes an interactive console test bench at [`demo/Bitai.LDAPHelper.Demo`](file:///c:/_bitai/Bitai.LDAPHelper/demo/Bitai.LDAPHelper.Demo).

### Dual Execution Modes

1. **Mock Mode (Simulated / Offline)**: Runs in-memory using `MockLdapDataSeeder` data. Requires no live directory server or credentials.
2. **Novell Mode (Live LDAP / Active Directory)**: Connects to a physical Active Directory server using settings configured in `ldaphelper_demosetup.json`.

### Configuration Setup

Copy `demo/Bitai.LDAPHelper.Demo/ldaphelper_demosetup.json` to your user's **Desktop** directory and configure:

```json
{
   "LdapServers": [{ "Address": "dc01.contoso.com" }],
   "BaseDNs": [{ "DN": "DC=contoso,DC=com" }],
   "ConnectionTimeout": 15,
   "DomainUserAccountForRunTests": "CONTOSO\\svc_demo",
   "AccountPassword": "DemoPassword123!",
   "Demo_Searcher_SearchUsers_RunTest": true,
   "Demo_GroupMembershipValidator_RunTest": true
}
```

### Running the Demo

```bash
# Interactive mode (prompts for mode selection):
dotnet run --project demo/Bitai.LDAPHelper.Demo/Bitai.LDAPHelper.Demo.csproj

# Direct Mock mode execution:
dotnet run --project demo/Bitai.LDAPHelper.Demo/Bitai.LDAPHelper.Demo.csproj -- --implementation mock

# Direct Novell mode execution:
dotnet run --project demo/Bitai.LDAPHelper.Demo/Bitai.LDAPHelper.Demo.csproj -- --implementation novell
```

---

## Build, Test & Package

### Prerequisites

- **.NET SDK 10.0** or newer.

### Build Solution

```bash
dotnet build Bitai.Ldap.Helper.sln -c Release
```

### Execute Test Suite

```bash
dotnet test tests/Bitai.LDAPHelper.Tests/Bitai.LDAPHelper.Tests.csproj -c Release
```

### Generate NuGet Packages

```bash
dotnet pack src/Bitai.LDAPHelper/Bitai.LDAPHelper.csproj -c Release -o ./artifacts
dotnet pack adapters/Bitai.LDAPHelper.LdapAdapters.Novell/Bitai.LDAPHelper.LdapAdapters.Novell.csproj -c Release -o ./artifacts
dotnet pack adapters/Bitai.LDAPHelper.LdapAdapters.LdapHelperMock/Bitai.LDAPHelper.LdapAdapters.LdapHelperMock.csproj -c Release -o ./artifacts
```

---

## Troubleshooting Matrix

| Issue / Error | Cause | Fix |
|---|---|---|
| `LdapOperationException: Invalid credentials` | Server refused bind credentials or user DN format is incorrect. | Verify UPN (`user@domain.com`) or short domain account format (`DOMAIN\user`). |
| `EntryNotFoundException` | Specified DN or account filter yielded no matching records. | Verify `SearchLimits.BaseDN` and ensure target account exists in directory. |
| Password Change Fails (`CONSTRAINT_VIOLATION` / 0x13) | New password violates Active Directory password policy (length, complexity, history). | Ensure password meets Active Directory domain password policy requirements. |
| Password Change Fails (`UNWILLING_TO_PERFORM` / 0x35) | Active Directory requires LDAPS (SSL port 636) to update `unicodePwd`. | Enable `UseSSL = true` and connect over port 636. Active Directory rejects password changes over unencrypted LDAP. |
| Group Membership Check Returns False | Parent group name `CN` is incorrect or user belongs to nested group without subtree traversal. | Specify common name `CN` (not full DN) for target group. `GroupMembershipValidator` automatically evaluates nested inheritance. |

---

## Security & Operational Guidance

- **LDAPS Requirement for Passwords:** Active Directory strictly rejects `unicodePwd` updates over unencrypted LDAP (port 389). Always configure `UseSSL = true` on port 636 when updating credentials via `AccountManager`.
- **Credential Storage:** Store service account credentials in secure configuration providers (e.g. Azure Key Vault, AWS Secrets Manager, or User Secrets) rather than hardcoded source strings.
- **Bitwise Account Control:** When disabling accounts using `AccountManager.DisableUserAccountForMsADAsync`, existing control flags are preserved dynamically to avoid unintentionally stripping settings like `DONT_EXPIRE_PASSWORD`.

---

## License & Community Ecosystem

This project is licensed under the **MIT License**. See [LICENSE.md](LICENSE.md) for details.

- Part of the **Bitai LDAP Ecosystem**.
- Related Projects: [Bitai.LDAPWebApi](https://github.com/bitai-cs/LDAPWebApi), [IdentityServer4.Admin](https://github.com/bitai-cs/IdentityServer4.Admin).

© 2026 **BITAI**. All rights reserved.
