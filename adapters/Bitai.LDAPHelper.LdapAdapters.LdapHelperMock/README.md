# Bitai.LDAPHelper.LdapAdapters.LdapHelperMock ![Logo](../../resources/hierarchy_32.png)

![.NET 10.0](https://img.shields.io/badge/.NET-10.0-512BD4)
![License: MIT](https://img.shields.io/badge/license-MIT-green)
![Package](https://img.shields.io/badge/package-Bitai.LDAPHelper.LdapAdapters.LdapHelperMock-blue)

An in-memory LDAP adapter implementation designed for unit testing, integration testing, offline prototyping, local development, and CI/CD pipelines in the **Bitai LDAP Helper** ecosystem.

---

## Table of Contents

- [Overview](#overview)
- [Solution Architecture](#solution-architecture)
- [Key Features](#key-features)
- [Project Structure & Class Taxonomy](#project-structure--class-taxonomy)
- [Quick Start](#quick-start)
- [Detailed Usage Scenarios](#detailed-usage-scenarios)
  - [Scenario 1: Isolated Unit Testing with Custom Search Results](#scenario-1-isolated-unit-testing-with-custom-search-results)
  - [Scenario 2: Integration Testing with Persistent Seeded Data](#scenario-2-integration-testing-with-persistent-seeded-data)
  - [Scenario 3: Verifying Account Management Side Effects](#scenario-3-verifying-account-management-side-effects)
- [Data Store & Seeding Infrastructure](#data-store--seeding-infrastructure)
  - [MockLdapDataStore](#mockldapdatastore)
  - [MockLdapDataSeeder](#mockldapdataseeder)
- [Build, Test & Package](#build-test--package)
- [Observability & Diagnostic Assertions](#observability--diagnostic-assertions)
- [Troubleshooting](#troubleshooting)
- [Security & Operational Guidance](#security--operational-guidance)
- [License](#license)

---

## Overview

`Bitai.LDAPHelper.LdapAdapters.LdapHelperMock` provides lightweight, fast, in-memory mock implementations of the LDAP adapter contracts defined in [Bitai.LDAPHelper](../../src/Bitai.LDAPHelper/README.md).

It enables developers to test LDAP-dependent components—such as authentication services (`Authenticator`), user search providers (`Searcher`), group membership validators (`GroupMembershipValidator`), and Active Directory account management routines (`AccountManager`)—without requiring access to a physical Active Directory, OpenLDAP, or containerized LDAP instance.

### Target Framework & Specifications

- **Target Framework:** .NET 10.0 (`net10.0`)
- **Nullable Context:** Enabled (`<Nullable>enable</Nullable>`)
- **Implicit Usings:** Enabled (`<ImplicitUsings>enable</ImplicitUsings>`)
- **Project Reference:** Depends on [Bitai.LDAPHelper](../../src/Bitai.LDAPHelper/Bitai.LDAPHelper.csproj)

---

## Solution Architecture

In the `Bitai.LDAPHelper` ecosystem, core services interact exclusively with LDAP contract abstractions (`ILdapConnectionFactoryAdapter`, `ILdapConnectionAdapter`, `ILdapEntryAdapter`, etc.) rather than concrete directory implementations. `LdapHelperMock` acts as a drop-in replacement for production adapters such as `Bitai.LDAPHelper.LdapAdapters.Novell`.

```
┌─────────────────────────────────────────────────────────────────┐
│                      Consumer Application                       │
└─────────────────────────────────────────────────────────────────┘
                                │
                                ▼
┌─────────────────────────────────────────────────────────────────┐
│                       Bitai.LDAPHelper                          │
│     (Authenticator, Searcher, AccountManager, Validator)        │
└─────────────────────────────────────────────────────────────────┘
                                │
                   ILdapConnectionFactoryAdapter
                   ILdapConnectionAdapter
                                │
        ┌───────────────────────┴───────────────────────┐
        ▼                                               ▼
┌──────────────────────────────┐       ┌──────────────────────────────┐
│     LdapAdapters.Novell      │       │       LdapHelperMock         │
│  (Production / Live LDAP)    │       │  (In-Memory / Test Suite)    │
└──────────────────────────────┘       └──────────────────────────────┘
                                                        │
                                                        ▼
                                       ┌──────────────────────────────┐
                                       │     MockLdapDataStore        │
                                       │   (In-Memory Directory)      │
                                       └──────────────────────────────┘
```

### Repository Projects Overview

| Project | Path | Role & Purpose |
|---|---|---|
| **`Bitai.LDAPHelper.LdapAdapters.LdapHelperMock`** | [`adapters/Bitai.LDAPHelper.LdapAdapters.LdapHelperMock`](file:///c:/_bitai/Bitai.LDAPHelper/adapters/Bitai.LDAPHelper.LdapAdapters.LdapHelperMock) | In-memory mock adapter implementation & test seeder |
| **`Bitai.LDAPHelper`** | [`src/Bitai.LDAPHelper`](file:///c:/_bitai/Bitai.LDAPHelper/src/Bitai.LDAPHelper) | Core helper library defining adapter interfaces & services |
| **`Bitai.LDAPHelper.DTO`** | [`src/Bitai.LDAPHelper.DTO`](file:///c:/_bitai/Bitai.LDAPHelper/src/Bitai.LDAPHelper.DTO) | Data transfer objects, credentials, and operation results |
| **`Bitai.LDAPHelper.LdapAdapters.Novell`** | [`adapters/Bitai.LDAPHelper.LdapAdapters.Novell`](file:///c:/_bitai/Bitai.LDAPHelper/adapters/Bitai.LDAPHelper.LdapAdapters.Novell) | Novell-backed LDAP connection adapter for live environments |
| **`Bitai.LDAPHelper.Tests`** | [`tests/Bitai.LDAPHelper.Tests`](file:///c:/_bitai/Bitai.LDAPHelper/tests/Bitai.LDAPHelper.Tests) | Unit and integration test suite consuming mock adapters |

---

## Key Features

1. **In-Memory Connection Simulation (`MockLdapConnectionAdapter`)**
   - Simulates `ConnectAsync`, `BindAsync`, `SearchAsync`, `AddEntryAsync`, `ModifyEntryAsync`, and `DeleteEntryAsync`.
   - Supports registerable search results via `AddSearchResult(filterPattern, entries)`.
2. **Persistent Shared State (`MockLdapPersistentConnectionAdapter`)**
   - Backed by `MockLdapDataStore` to persist entry creations, modifications, and deletions across multiple connections.
   - Evaluates search filters dynamically against stored records.
3. **Deterministic Directory Seeder (`MockLdapDataSeeder`)**
   - Populates realistic Active Directory objects: domain roots, OUs (Users, Groups, Computers), standard accounts, service accounts, and nested group memberships.
   - Includes thread-safe progressive RID generation and structured logging via `ILogger<MockLdapDataSeeder>`.
4. **Side-Effect Inspection & Assertions**
   - Tracks created entries (`CreatedEntries`), modifications (`Modifications`), and deletions (`DeletedEntries`) for assertion in unit tests.
5. **Multi-Attribute Filter Engine**
   - Evaluates LDAP search filter criteria including wildcard matching (`*`) against `sAMAccountName`, `distinguishedName`, `cn`, `objectSid`, and `objectClass`.
6. **Active Directory SID Conversion**
   - Includes binary SID parsing utilities (`ConvertByteToStringSid`) to match Windows Active Directory `objectSid` formatting (`S-1-5-...`).

---

## Project Structure & Class Taxonomy

```text
adapters/Bitai.LDAPHelper.LdapAdapters.LdapHelperMock/
├── Bitai.LDAPHelper.LdapAdapters.LdapHelperMock.csproj
├── README.md
├── LICENSE.md
├── MockLdapConnectionAdapter.cs
├── MockLdapConnectionFactoryAdapter.cs
├── MockLdapPersistentConnectionAdapter.cs
├── MockLdapPersistentConnectionFactoryAdapter.cs
├── MockLdapEntryAdapter.cs
├── MockLdapAttributeSetAdapter.cs
├── MockLdapAttributeAdapter.cs
├── MockLdapModificationAdapter.cs
├── MockLdapMessageAdapter.cs
├── MockLdapSearchQueueAdapter.cs
└── LdapData/
    ├── MockLdapDataStore.cs
    └── MockLdapDataSeeder.cs
```

### Class Roles Breakdown

| Class | Implemented Interface | Primary Responsibility |
|---|---|---|
| `MockLdapConnectionAdapter` | `ILdapConnectionAdapter` | Standard in-memory mock connection with configurable search results & side-effect trackers. |
| `MockLdapConnectionFactoryAdapter` | `ILdapConnectionFactoryAdapter` | Factory returning an instance of `MockLdapConnectionAdapter`. |
| `MockLdapPersistentConnectionAdapter` | `ILdapConnectionAdapter` | Persistent mock connection connected to the central `MockLdapDataStore`. |
| `MockLdapPersistentConnectionFactoryAdapter` | `ILdapConnectionFactoryAdapter` | Factory producing `MockLdapPersistentConnectionAdapter` instances. |
| `MockLdapEntryAdapter` | `ILdapEntryAdapter` | Represents an LDAP entry with a DN and attribute set. |
| `MockLdapAttributeSetAdapter` | `ILdapAttributeSetAdapter` | Dictionary-backed container for entry attributes. Includes verification helpers. |
| `MockLdapAttributeAdapter` | `ILdapAttributeAdapter` | Represents individual attribute key-value pairs (string, string array, or byte array). |
| `MockLdapModificationAdapter` | `ILdapModificationAdapter` | Represents an LDAP entry modification (Add, Delete, Replace). |
| `MockLdapMessageAdapter` | `ILdapMessageAdapter` | Wraps single search result entries for message queue delivery. |
| `MockLdapSearchQueueAdapter` | `ILdapSearchQueueAdapter` | In-memory queue storing search result messages returned by `SearchAsync`. |
| `MockLdapDataStore` | *N/A (Singleton)* | Thread-safe in-memory directory store guarded by `ReaderWriterLockSlim`. |
| `MockLdapDataSeeder` | *N/A* | Seeds `MockLdapDataStore` with domain hierarchies, OUs, users, groups, and SIDs. |

---

## Quick Start

### Installation

To add `Bitai.LDAPHelper.LdapAdapters.LdapHelperMock` to a test project:

```bash
dotnet add package Bitai.LDAPHelper.LdapAdapters.LdapHelperMock
```

Or via project reference:

```xml
<ItemGroup>
  <ProjectReference Include="..\..\adapters\Bitai.LDAPHelper.LdapAdapters.LdapHelperMock\Bitai.LDAPHelper.LdapAdapters.LdapHelperMock.csproj" />
</ItemGroup>
```

### Basic Minimal Example

```csharp
using Bitai.LDAPHelper;
using Bitai.LDAPHelper.DTO;
using Bitai.LDAPHelper.LdapAdapters.LdapHelperMock;

// 1. Create a mock connection and configure stubbed search results
var mockConnection = new MockLdapConnectionAdapter();

var userEntry = new MockLdapEntryAdapter("CN=John Doe,OU=Users,DC=example,DC=com");
userEntry.AddAttribute("cn", "John Doe");
userEntry.AddAttribute("sAMAccountName", "jdoe");
userEntry.AddAttribute("userPrincipalName", "jdoe@example.com");

mockConnection.AddSearchResult("(sAMAccountName=jdoe)", new List<MockLdapEntryAdapter> { userEntry });

// 2. Wrap in a factory
var factory = new MockLdapConnectionFactoryAdapter(mockConnection);

// 3. Initialize LDAPHelper services using the mock factory
var connectionInfo = new ConnectionInfo("localhost", 389, useSSL: false, connectionTimeout: 15);
var credential = new LDAPDomainAccountCredential("EXAMPLE", "service.account", "SecretPassword!");
var searchLimits = new SearchLimits("DC=example,DC=com");

var searcher = new Searcher(connectionInfo, searchLimits, credential, factory);

// 4. Perform search via Searcher
var user = await searcher.GetLdapEntryBySamAccountNameAsync("jdoe");
Console.WriteLine($"Found User: {user?.CN} ({user?.DistinguishedName})");
```

---

## Detailed Usage Scenarios

### Scenario 1: Isolated Unit Testing with Custom Search Results

For targeted unit tests where you want exact control over returned entries without global directory state:

```csharp
using Xunit;
using Bitai.LDAPHelper;
using Bitai.LDAPHelper.DTO;
using Bitai.LDAPHelper.LdapAdapters.LdapHelperMock;

public class UserServiceTests
{
    [Fact]
    public async Task GetUser_ShouldReturnMatchingLdapEntry()
    {
        // Arrange
        var mockConnection = new MockLdapConnectionAdapter();
        
        var expectedDn = "CN=Alice Smith,OU=Engineering,DC=corp,DC=local";
        var entry = new MockLdapEntryAdapter(expectedDn);
        entry.AddAttribute("sAMAccountName", "asmith");
        entry.AddAttribute("mail", "alice.smith@corp.local");
        entry.AddAttribute("givenName", "Alice");
        entry.AddAttribute("sn", "Smith");

        mockConnection.AddSearchResult("asmith", new List<MockLdapEntryAdapter> { entry });
        
        var factory = new MockLdapConnectionFactoryAdapter(mockConnection);
        var searcher = new Searcher(
            new ConnectionInfo("ldap.corp.local", 389, false, 10),
            new SearchLimits("DC=corp,DC=local"),
            new LDAPDomainAccountCredential("CORP", "admin", "pass"),
            factory
        );

        // Act
        var result = await searcher.GetLdapEntryBySamAccountNameAsync("asmith");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("asmith", result.SamAccountName);
        Assert.Equal(expectedDn, result.DistinguishedName);
    }
}
```

---

### Scenario 2: Integration Testing with Persistent Seeded Data

When testing complex workflows that execute sequential searches, authentications, or group membership checks against a shared directory dataset:

```csharp
using Xunit;
using Bitai.LDAPHelper;
using Bitai.LDAPHelper.DTO;
using Bitai.LDAPHelper.LdapAdapters.LdapHelperMock;
using Bitai.LDAPHelper.LdapAdapters.LdapHelperMock.LdapData;
using Microsoft.Extensions.Logging.Abstractions;

public class AuthenticationIntegrationTests
{
    public AuthenticationIntegrationTests()
    {
        // Seed the shared in-memory data store with realistic AD entities
        var seeder = new MockLdapDataSeeder(NullLogger<MockLdapDataSeeder>.Instance);
        seeder.SeedAllData();
    }

    [Fact]
    public async Task AuthenticateDomainAccount_WithSeededUser_ShouldSucceed()
    {
        // Arrange - use persistent connection factory
        var factory = new MockLdapPersistentConnectionFactoryAdapter();
        
        var connectionInfo = new ConnectionInfo("dc01.domain.com", 389, false, 30);
        var searchLimits = new SearchLimits("DC=domain,DC=com");
        var adminCreds = new LDAPDomainAccountCredential("DOMAIN", "Administrator", "AdminP@ss123");

        var authenticator = new Authenticator(connectionInfo, searchLimits, adminCreds, factory);

        // Act - Authenticate a user populated by MockLdapDataSeeder
        var targetUserCreds = new LDAPDomainAccountCredential("DOMAIN", "jdoe", "UserP@ssword1!");
        var authResult = await authenticator.AuthenticateDomainAccountAsync(targetUserCreds);

        // Assert
        Assert.NotNull(authResult);
        Assert.True(authResult.IsAuthenticated);
    }
}
```

---

### Scenario 3: Verifying Account Management Side Effects

When testing routines that create, modify, or delete directory objects (e.g. `AccountManager`), use `MockLdapConnectionAdapter` to verify generated modifications:

```csharp
using Xunit;
using Bitai.LDAPHelper;
using Bitai.LDAPHelper.DTO;
using Bitai.LDAPHelper.LdapAdapters;
using Bitai.LDAPHelper.LdapAdapters.LdapHelperMock;

public class AccountManagerTests
{
    [Fact]
    public async Task ModifyUserAttribute_ShouldRecordModification()
    {
        // Arrange
        var mockConnection = new MockLdapConnectionAdapter();
        var factory = new MockLdapConnectionFactoryAdapter(mockConnection);
        
        var connectionInfo = new ConnectionInfo("localhost", 389, false, 10);
        var searchLimits = new SearchLimits("DC=example,DC=com");
        var adminCreds = new LDAPDomainAccountCredential("EXAMPLE", "admin", "pass");

        var accountManager = new AccountManager(connectionInfo, searchLimits, adminCreds, factory);

        var userDn = "CN=Bob Jones,OU=Users,DC=example,DC=com";
        var modifications = new List<ILdapModificationAdapter>
        {
            new MockLdapModificationAdapter(LdapModificationType.Replace, "telephoneNumber", "+1-555-0199")
        };

        // Act
        await accountManager.ModifyEntryAsync(userDn, modifications);

        // Assert
        Assert.Single(mockConnection.Modifications);
        var recordedMod = mockConnection.Modifications[0];
        Assert.Equal(userDn, recordedMod.DistinguishedName);
        Assert.Equal("telephoneNumber", recordedMod.AttributeName);
        Assert.Equal("+1-555-0199", recordedMod.Value);
    }
}
```

---

## Data Store & Seeding Infrastructure

### `MockLdapDataStore`

`MockLdapDataStore` is a thread-safe singleton managing the in-memory LDAP hierarchy for persistent mock connections.

- **Thread-Safety:** Operations are protected by `ReaderWriterLockSlim`.
- **Case-Insensitive Keys:** Distinguished names are indexed using `StringComparer.OrdinalIgnoreCase`.
- **API Surface:**
  - `AddOrUpdateEntry(MockLdapEntryAdapter entry)`
  - `GetEntry(string distinguishedName)`
  - `SearchEntries(Func<MockLdapEntryAdapter, bool> predicate)`
  - `RemoveEntry(string distinguishedName)`
  - `GetAllEntries()`
  - `Clear()`
  - `Count`

### `MockLdapDataSeeder`

`MockLdapDataSeeder` generates a complete, realistic directory structure inside `MockLdapDataStore`.

#### Seeding Operations Executed by `SeedAllData()`

1. **Domain Structure:** Seeds root domain `DC=domain,DC=com`.
2. **Organizational Units:** `OU=Users`, `OU=Groups`, `OU=Computers`, `OU=IT`, `OU=HR`, `OU=Finance`.
3. **Standard Users:** Generates standard accounts with attributes (`cn`, `sAMAccountName`, `userPrincipalName`, `mail`, `givenName`, `sn`, `objectSid`, `userAccountControl`).
4. **Security Groups:** Seeds domain groups (`Domain Admins`, `Domain Users`, `VPN Users`, `HR Staff`, `Finance Managers`).
5. **Computer Objects:** Seeds workstation and server entries.
6. **Group Memberships:** Links users to groups (`member` and `memberOf` attributes).
7. **RID Allocation:** Thread-safe RID counters starting at:
   - Users: `1000`
   - Groups: `2000`
   - Computers: `3000`
   - Other Objects: `4000`

---

## Build, Test & Package

### Prerequisites

- **.NET SDK 10.0** or newer installed.
- PowerShell or standard Linux/macOS command shell.

### Building the Project

```bash
dotnet build adapters/Bitai.LDAPHelper.LdapAdapters.LdapHelperMock/Bitai.LDAPHelper.LdapAdapters.LdapHelperMock.csproj -c Release
```

### Running the Test Suite

Run the full repository test suite that consumes `LdapHelperMock`:

```bash
dotnet test tests/Bitai.LDAPHelper.Tests/Bitai.LDAPHelper.Tests.csproj -c Release
```

### Creating the NuGet Package

```bash
dotnet pack adapters/Bitai.LDAPHelper.LdapAdapters.LdapHelperMock/Bitai.LDAPHelper.LdapAdapters.LdapHelperMock.csproj -c Release -o ./artifacts
```

---

## Observability & Diagnostic Assertions

When debugging unit or integration tests, `MockLdapConnectionAdapter` provides diagnostic tracking properties:

```csharp
// Inspect entries created during the test run
List<MockLdapEntryAdapter> created = mockConnection.CreatedEntries;

// Inspect all entry modifications executed
List<MockModification> mods = mockConnection.Modifications;

// Inspect deleted distinguished names
List<string> deleted = mockConnection.DeletedEntries;
```

`MockLdapDataSeeder` accepts an `ILogger<MockLdapDataSeeder>` parameter, emitting structured log statements during data population for full visibility in console outputs or test runner logs.

---

## Troubleshooting

| Issue / Symptom | Probable Cause | Recommended Fix |
|---|---|---|
| `ConnectAsync` throws `Invalid server connection!` | `host` parameter is empty, `"unknown"`, or `"0.0.0.0"`, or `port <= 0`. | Provide a valid non-empty host string and positive port number (e.g. `"localhost"`, `389`). |
| `BindAsync` throws `Invalid credentials!` | Credentials contain `"hacker"` or `"123456"`, or are null/empty. | Use valid non-blacklisted mock credentials. |
| `SearchAsync` returns empty results in standard mode | The filter string was not registered via `AddSearchResult`. | Call `mockConnection.AddSearchResult(filterPattern, entries)` before executing queries. |
| Search returns empty results in persistent mode | `MockLdapDataSeeder` was not executed, or filter attribute is unsupported. | Execute `seeder.SeedAllData()` prior to testing. Ensure filter matches supported attributes (`sAMAccountName`, `distinguishedName`, `cn`, `objectSid`, `objectClass`). |
| `ServerCertificateValidationByPass` throws `InvalidOperationException` | Method called on mock class. | Certificate validation bypass is not applicable in mock mode. Remove call in test setups. |

---

## Security & Operational Guidance

- **Mock Credentials:** Never hardcode or commit production passwords or sensitive Active Directory credentials into test scripts or seeder logic.
- **Deterministic Testing:** Use fixed seeds or reset `MockLdapDataStore.Instance.Clear()` between tests to prevent inter-test state contamination.
- **Non-Production Purpose:** This package is explicitly designed for testing, CI/CD, and local prototyping. Do not deploy mock adapters to production environments.

---

## License

This project is licensed under the **MIT License**. See [LICENSE.md](LICENSE.md) for details.

© 2026 **BITAI**. All rights reserved.
