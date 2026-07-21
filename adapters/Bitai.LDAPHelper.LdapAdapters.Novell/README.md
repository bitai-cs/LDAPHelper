# Bitai.LDAPHelper.LdapAdapters.Novell ![Logo](../../resources/hierarchy_32.png)

![.NET 10.0](https://img.shields.io/badge/.NET-10.0-512BD4)
![License: MIT](https://img.shields.io/badge/license-MIT-green)
![Package](https://img.shields.io/badge/package-Bitai.LDAPHelper.LdapAdapters.Novell-blue)

Production LDAP connection adapter for the **Bitai LDAP Helper** ecosystem, providing cross-platform connectivity to Active Directory, OpenLDAP, and RFC-compliant Directory Services via **Novell.Directory.Ldap.NETStandard**.

---

## Table of Contents

- [Overview](#overview)
- [Solution Architecture](#solution-architecture)
- [Key Features](#key-features)
- [Class Architecture & Native Mapping](#class-architecture--native-mapping)
- [Quick Start](#quick-start)
- [Detailed Usage Scenarios](#detailed-usage-scenarios)
  - [Scenario 1: Production Searcher Setup](#scenario-1-production-searcher-setup)
  - [Scenario 2: Secure SSL/LDAPS Domain Authentication](#scenario-2-secure-sslldaps-domain-authentication)
  - [Scenario 3: Directory Entry Management](#scenario-3-directory-entry-management)
- [Configuration & Security Guidance](#configuration--security-guidance)
  - [Connection Parameters](#connection-parameters)
  - [SSL/TLS & Certificate Validation](#ssltls--certificate-validation)
- [Build, Test & Package](#build-test--package)
- [Troubleshooting Matrix](#troubleshooting-matrix)
- [License](#license)

---

## Overview

`Bitai.LDAPHelper.LdapAdapters.Novell` bridges the core services of [Bitai.LDAPHelper](../../src/Bitai.LDAPHelper/README.md) with physical LDAP directory servers. 

By wrapping the open-source, fully cross-platform [Novell.Directory.Ldap.NETStandard](https://www.nuget.org/packages/Novell.Directory.Ldap.NETStandard) library (version 4.0.0), this adapter enables high-performance LDAP queries, user authentication, password updates, and Active Directory object management on Windows, Linux, and macOS without relying on Windows-only system libraries (`System.DirectoryServices`).

### Target Framework & Specifications

- **Target Framework:** .NET 10.0 (`net10.0`)
- **Nullable Context:** Enabled (`<Nullable>enable</Nullable>`)
- **Implicit Usings:** Enabled (`<ImplicitUsings>enable</ImplicitUsings>`)
- **Underlying Driver:** `Novell.Directory.Ldap.NETStandard` v4.0.0
- **Project Reference:** Depends on [Bitai.LDAPHelper](../../src/Bitai.LDAPHelper/Bitai.LDAPHelper.csproj)

---

## Solution Architecture

Core services in `Bitai.LDAPHelper` (such as `Searcher`, `Authenticator`, and `AccountManager`) depend on adapter interfaces (`ILdapConnectionFactoryAdapter`, `ILdapConnectionAdapter`, `ILdapEntryAdapter`) rather than specific LDAP drivers. `Bitai.LDAPHelper.LdapAdapters.Novell` implements these interfaces for live production environments.

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
                                ▼
┌─────────────────────────────────────────────────────────────────┐
│              Bitai.LDAPHelper.LdapAdapters.Novell               │
│               (Novell Production Adapter Layer)                 │
└─────────────────────────────────────────────────────────────────┘
                                │
                                ▼
┌─────────────────────────────────────────────────────────────────┐
│               Novell.Directory.Ldap.NETStandard                 │
│               (Cross-Platform Network Protocol)                 │
└─────────────────────────────────────────────────────────────────┘
                                │
                 LDAP (389) / LDAPS (636) Protocol
                                │
                                ▼
┌─────────────────────────────────────────────────────────────────┐
│        Active Directory / OpenLDAP / Directory Server           │
└─────────────────────────────────────────────────────────────────┘
```

### Repository Projects Overview

| Project | Path | Role & Purpose |
|---|---|---|
| **`Bitai.LDAPHelper.LdapAdapters.Novell`** | [`adapters/Bitai.LDAPHelper.LdapAdapters.Novell`](file:///c:/_bitai/Bitai.LDAPHelper/adapters/Bitai.LDAPHelper.LdapAdapters.Novell) | Production Novell LDAP adapter implementation |
| **`Bitai.LDAPHelper`** | [`src/Bitai.LDAPHelper`](file:///c:/_bitai/Bitai.LDAPHelper/src/Bitai.LDAPHelper) | Core helper library defining services and adapter interfaces |
| **`Bitai.LDAPHelper.DTO`** | [`src/Bitai.LDAPHelper.DTO`](file:///c:/_bitai/Bitai.LDAPHelper/src/Bitai.LDAPHelper.DTO) | Data transfer objects, credential models, and operation results |
| **`Bitai.LDAPHelper.LdapAdapters.LdapHelperMock`** | [`adapters/Bitai.LDAPHelper.LdapAdapters.LdapHelperMock`](file:///c:/_bitai/Bitai.LDAPHelper/adapters/Bitai.LDAPHelper.LdapAdapters.LdapHelperMock) | In-memory mock adapter for offline testing & CI pipelines |

---

## Key Features

1. **Production LDAP & LDAPS Protocol Support**
   - Connects to Active Directory and LDAP servers over standard unencrypted TCP (port 389) or secure SSL/TLS sockets (port 636).
2. **Automated Connection Lifecycle (`NovellLdapConnectionFactoryAdapter`)**
   - Instantiates, configures timeouts, enables SSL, establishes socket connections (`ConnectAsync`), and authenticates (`BindAsync`) automatically.
3. **Asynchronous Search Streaming (`SearchAsync`)**
   - Translates domain-level `ISearchLimits` (Base DN, Search Scope, Timeouts, Max Results) into Novell `LdapSearchConstraints`.
   - Streams LDAP search results asynchronously via `ILdapSearchQueueAdapter`.
4. **Complete Directory Object Lifecycle**
   - Supports creating (`AddEntryAsync`), updating (`ModifyEntryAsync`), and removing (`DeleteEntryAsync`) entries in directory trees.
5. **Multi-Type Attribute Translation**
   - Native support for reading and writing single string values, multi-valued string arrays, and raw binary byte arrays (e.g. `objectSid`, `objectGUID`, `userCertificate`).
6. **Certificate Validation Customization**
   - Provides `ServerCertificateValidationByPass()` for development, staging, or internal enterprise environments using self-signed PKI certificates.

---

## Class Architecture & Native Mapping

The adapter layer translates between Novell native types and `Bitai.LDAPHelper` abstractions:

| Native Novell Class | Adapter Class | Implemented Abstraction | Role |
|---|---|---|---|
| `Novell.Directory.Ldap.LdapConnection` | `NovellLdapConnectionAdapter` | `ILdapConnectionAdapter` | Manages server connection, authentication state, and LDAP operations. |
| *N/A (Factory)* | `NovellLdapConnectionFactoryAdapter` | `ILdapConnectionFactoryAdapter` | Dynamically creates bound `NovellLdapConnectionAdapter` instances. |
| `Novell.Directory.Ldap.LdapEntry` | `NovellLdapEntryAdapter` | `ILdapEntryAdapter` | Represents a directory entry (DN and attribute set). |
| `Novell.Directory.Ldap.LdapAttributeSet` | `NovellLdapAttributeSetAdapter` | `ILdapAttributeSetAdapter` | Dictionary-style attribute container for entries and operations. |
| `Novell.Directory.Ldap.LdapAttribute` | `NovellLdapAttributeAdapter` | `ILdapAttributeAdapter` | Encapsulates single/multi-valued string and binary attribute values. |
| `Novell.Directory.Ldap.LdapModification` | `NovellLdapModificationAdapter` | `ILdapModificationAdapter` | Represents attribute modifications (Add, Delete, Replace). |
| `Novell.Directory.Ldap.LdapMessage` | `NovellLdapMessageAdapter` | `ILdapMessageAdapter` | Wraps incoming LDAP responses and search results. |
| `Novell.Directory.Ldap.LdapSearchQueue` | `NovellLdapSearchQueueAdapter` | `ILdapSearchQueueAdapter` | Queue for consuming search result messages from asynchronous queries. |

---

## Quick Start

### Installation

Add `Bitai.LDAPHelper.LdapAdapters.Novell` to your .NET application:

```bash
dotnet add package Bitai.LDAPHelper.LdapAdapters.Novell
```

Or reference it in your `.csproj`:

```xml
<ItemGroup>
  <ProjectReference Include="..\..\adapters\Bitai.LDAPHelper.LdapAdapters.Novell\Bitai.LDAPHelper.LdapAdapters.Novell.csproj" />
</ItemGroup>
```

### Basic Minimal Example

```csharp
using Bitai.LDAPHelper;
using Bitai.LDAPHelper.DTO;
using Bitai.LDAPHelper.LdapAdapters.Novell;

// 1. Instanciate the Novell connection factory
var connectionFactory = new NovellLdapConnectionFactoryAdapter();

// 2. Configure connection settings and service account credentials
var connectionInfo = new ConnectionInfo("dc01.corp.contoso.com", 389, useSSL: false, connectionTimeout: 15);
var credential = new LDAPDomainAccountCredential("CONTOSO", "svc_ldap", "P@ssword123!");
var searchLimits = new SearchLimits("DC=corp,DC=contoso,DC=com");

// 3. Inject factory into core LDAPHelper Searcher service
var searcher = new Searcher(connectionInfo, searchLimits, credential, connectionFactory);

// 4. Query an entry from Active Directory
var user = await searcher.GetLdapEntryBySamAccountNameAsync("johndoe");

if (user != null)
{
    Console.WriteLine($"DN: {user.DistinguishedName}");
    Console.WriteLine($"Mail: {user.Mail}");
}
```

---

## Detailed Usage Scenarios

### Scenario 1: Production Searcher Setup

Searching for active directory user accounts or groups using standard LDAP filters:

```csharp
using Bitai.LDAPHelper;
using Bitai.LDAPHelper.DTO;
using Bitai.LDAPHelper.LdapAdapters.Novell;

public class DirectorySearchService
{
    private readonly Searcher _searcher;

    public DirectorySearchService()
    {
        var factory = new NovellLdapConnectionFactoryAdapter();
        
        var connectionInfo = new ConnectionInfo(
            server: "ldap.example.com",
            serverPort: 389,
            useSSL: false,
            connectionTimeout: 10
        );

        var searchLimits = new SearchLimits(
            baseDN: "OU=Employees,DC=example,DC=com",
            maxSearchResults: 500,
            maxSearchTimeout: 30,
            ldapSearchScope: LdapSearchScope.ScopeSub
        );

        var credential = new LDAPDomainAccountCredential("EXAMPLE", "ldap_reader", "SecurePassword!");

        _searcher = new Searcher(connectionInfo, searchLimits, credential, factory);
    }

    public async Task<LDAPEntry?> FindUserByEmailAsync(string email)
    {
        var filter = $"(&(objectClass=user)(mail={email}))";
        var results = await _searcher.GetLdapEntriesAsync(filter);
        return results?.FirstOrDefault();
    }
}
```

---

### Scenario 2: Secure SSL/LDAPS Domain Authentication

Authenticating user credentials over encrypted LDAPS (Port 636) using `Authenticator`:

```csharp
using Bitai.LDAPHelper;
using Bitai.LDAPHelper.DTO;
using Bitai.LDAPHelper.LdapAdapters.Novell;

public class AuthenticationService
{
    private readonly Authenticator _authenticator;

    public AuthenticationService()
    {
        var factory = new NovellLdapConnectionFactoryAdapter();

        // Configure LDAPS over SSL on Port 636
        var connectionInfo = new ConnectionInfo(
            server: "dc01.domain.com",
            serverPort: 636,
            useSSL: true,
            connectionTimeout: 15
        );

        var searchLimits = new SearchLimits("DC=domain,DC=com");
        var adminCreds = new LDAPDomainAccountCredential("DOMAIN", "svc_auth", "AdminSecret!");

        _authenticator = new Authenticator(connectionInfo, searchLimits, adminCreds, factory);
    }

    public async Task<bool> ValidateUserCredentialsAsync(string username, string password)
    {
        var userCreds = new LDAPDomainAccountCredential("DOMAIN", username, password);
        var result = await _authenticator.AuthenticateDomainAccountAsync(userCreds);

        return result.IsAuthenticated;
    }
}
```

---

### Scenario 3: Directory Entry Management

Adding, modifying, or deleting entries in Active Directory using `AccountManager`:

```csharp
using Bitai.LDAPHelper;
using Bitai.LDAPHelper.DTO;
using Bitai.LDAPHelper.LdapAdapters;
using Bitai.LDAPHelper.LdapAdapters.Novell;

public class DirectoryManagerService
{
    private readonly AccountManager _accountManager;

    public DirectoryManagerService()
    {
        var factory = new NovellLdapConnectionFactoryAdapter();
        
        var connectionInfo = new ConnectionInfo("dc01.company.local", 389, false, 15);
        var searchLimits = new SearchLimits("DC=company,DC=local");
        var adminCreds = new LDAPDomainAccountCredential("COMPANY", "DomainAdmin", "SuperSecure123!");

        _accountManager = new AccountManager(connectionInfo, searchLimits, adminCreds, factory);
    }

    public async Task UpdatePhoneNumberAsync(string userDn, string newPhone)
    {
        // 1. Create connection to modify entry
        var adapter = new NovellLdapConnectionFactoryAdapter();
        using var connection = await adapter.CreateConnectionAsync(
            new ConnectionInfo("dc01.company.local", 389, false, 15),
            "DomainAdmin",
            "SuperSecure123!"
        );

        // 2. Prepare modification
        var mod = connection.CreateModification(LdapModificationType.Replace, "telephoneNumber", newPhone);

        // 3. Apply modification
        await connection.ModifyEntryAsync(userDn, new[] { mod });
    }
}
```

---

## Configuration & Security Guidance

### Connection Parameters

`NovellLdapConnectionFactoryAdapter` maps `IConnectionInfo` properties as follows:

| `IConnectionInfo` Property | Type | Mapping / Novell Adapter Behavior |
|---|---|---|
| `Server` | `string` | Passed to `ConnectAsync(host, port)`. Hostname or IP address. |
| `ServerPort` | `int` | Passed to `ConnectAsync(host, port)`. Default `389` (LDAP) or `636` (LDAPS). |
| `UseSSL` | `bool` | Enables `SecureSocketLayer = true` and attaches SSL certificate handler. |
| `ConnectionTimeout` | `int` (sec) | Multiplied by 1000 and assigned to Novell `LdapConnection.ConnectionTimeout` (in milliseconds). |

### SSL/TLS & Certificate Validation

When `UseSSL` is enabled, `NovellLdapConnectionFactoryAdapter` sets `SecureSocketLayer = true` and invokes `ServerCertificateValidationByPass()`.

> [!WARNING]
> `ServerCertificateValidationByPass()` attaches a validation delegate that accepts any server certificate (bypassing SSL certificate chain checking). This is convenient for development and internal enterprise environments using self-signed Active Directory CA certificates. For strict security environments requiring explicit certificate chain validation, customize the delegate on `NovellLdapConnectionAdapter`.

---

## Build, Test & Package

### Prerequisites

- **.NET SDK 10.0** or newer installed.
- Network access to a test LDAP/Active Directory server (or execute repository tests using `LdapHelperMock`).

### Building the Project

```bash
dotnet build adapters/Bitai.LDAPHelper.LdapAdapters.Novell/Bitai.LDAPHelper.LdapAdapters.Novell.csproj -c Release
```

### Running Tests

```bash
dotnet test tests/Bitai.LDAPHelper.Tests/Bitai.LDAPHelper.Tests.csproj -c Release
```

### Packing the NuGet Package

```bash
dotnet pack adapters/Bitai.LDAPHelper.LdapAdapters.Novell/Bitai.LDAPHelper.LdapAdapters.Novell.csproj -c Release -o ./artifacts
```

---

## Troubleshooting Matrix

| Symptom / Error | Probable Cause | Recommended Fix |
|---|---|---|
| `LdapException: Connect Error` | Invalid host, closed port, or network firewall blocking port 389/636. | Verify LDAP server hostname, check DNS resolution, and test port open with `Test-NetConnection -ComputerName host -Port 389`. |
| `LdapException: Invalid Credentials` (Code 49) | Invalid distinguished name, domain user format, or incorrect password. | Use fully qualified UPN (`user@domain.com`) or DN (`CN=User,OU=...,DC=...`). |
| SSL Handshake Failure on Port 636 | `UseSSL` is `false` when connecting to SSL port 636, or SSL certificate mismatch. | Ensure `UseSSL = true` on port 636. If using self-signed certs, ensure certificate bypass is enabled. |
| `LdapException: Size Limit Exceeded` (Code 4) | Query returned more items than permitted by Active Directory policy (default 1000). | Adjust `SearchLimits.MaxSearchResults` or use paged LDAP searches. |
| `ArgumentOutOfRangeException: Unknown type of modification` | Invalid `LdapModificationType` value passed to `CreateModification`. | Ensure `LdapModificationType` is one of `Add`, `Delete`, or `Replace`. |

---

## License

This project is licensed under the **MIT License**. See [LICENSE.md](LICENSE.md) for details.

© 2026 **BITAI**. All rights reserved.
