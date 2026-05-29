# Bitai.LDAPHelper.DTO

[![NuGet Version](https://img.shields.io/nuget/v/Bitai.LDAPHelper.DTO.svg?style=flat-sign)](https://www.nuget.org/packages/Bitai.LDAPHelper.DTO/)
[![.NET Core](https://img.shields.io/badge/.NET-8.0%20%7C%20Standard%202.0-blue.svg)](https://dotnet.microsoft.com)
[![Platform](https://img.shields.io/badge/Platform-Windows%20%7C%20Linux%20%7C%20macOS-lightgrey.svg)]()

`Bitai.LDAPHelper.DTO` is a lightweight, high-performance C# class library containing the **Data Transfer Objects (DTOs)**, models, credentials, and enumeration flags used across the `Bitai.LDAPHelper` ecosystem. 

Designed with API-driven architectures in mind, all DTOs are fully serializable (supporting JSON/XML serialization) and decouple LDAP operations from underlying directory services protocol layers.

---

## 🌟 Key Features

- **Decoupled Entities:** Models standard LDAP entries (`LDAPEntry`) and MS Active Directory Specific structures (`LDAPMsADUserAccount`) cleanly.
- **Secure Credentials Handling:** Implements secure credential structures for Distinguished Name (DN) and domain-based accounts.
- **Unified Result Pattern:** Uses a robust, standardized wrapper (`LDAPOperationResult`) for handling LDAP success states, errors, exceptions, and execution tracking labels across all operations.
- **MS Active Directory Flags:** Built-in enum representation for `UserAccountControl` flags (`UAC`) to easily manipulate AD-specific account states.
- **Fully Serializable:** Properties are designed to integrate seamlessly into web service boundaries (e.g., ASP.NET Web API, gRPC, WCF).

---

## 📂 Library Architecture & DTO Catalog

### 1. Entities & Core Models

*   **`LDAPEntry`**: Models a generic LDAP entry containing standard schema attributes.
    *   *Properties:* `distinguishedName`, `cn`, `displayName`, `mail`, `givenName`, `sn`, `member`, `memberOf`, `objectGuid`, `objectSid`, etc.
    *   *Helpers:* Provides `GetMemberOfEntriesRecursively()` to resolve nested group memberships effortlessly.
    *   *Sorting:* Implements `IComparable` for deterministic sorting based on `distinguishedName`.
*   **`LDAPMsADUserAccount`**: Represents a specialized model mapping Microsoft Active Directory User accounts.

### 2. Credentials

*   **`LDAPDistinguishedNameCredential`**: Holds Distinguished Name credentials.
*   **`LDAPDomainAccountCredential`**: Holds traditional domain account credentials.
*   *Both credentials implement the `ISecureCloningCredential` interface.*

### 3. Operation Results

Every operation in `Bitai.LDAPHelper` returns a specialized derivative of **`LDAPOperationResult`**:

| Result Class | Description |
| :--- | :--- |
| **`LDAPSearchResult`** | Returns a collection of matched `LDAPEntry` objects. |
| **`LDAPDomainAccountAuthenticationResult`** | The outcome of validating a domain user. |
| **`LDAPDistinguishedNameAuthenticationResult`** | The outcome of validating a DN account. |
| **`LDAPCreateMsADUserAccountResult`** | Detailed results on new AD account creations. |
| **`LDAPRemoveMsADUserAccountResult`** | Details of account deletion actions. |
| **`LDAPDisableUserAccountOperationResult`** | Outcome of disabling an active directory user account. |
| **`LDAPPasswordUpdateResult`** | Status of password updates, password policies, and security updates. |

### 4. Enumerations

*   **`EntryAttribute`**: Strongly typed mapped attributes representing standard and custom Active Directory and generic LDAP properties (`sAMAccountName`, `mail`, `unicodePwd`, `userAccountControl`, etc.).
*   **`RequiredEntryAttributes`**: Preset levels to optimize data retrieval bandwidth (e.g., `Minimum`, `Few`, `All`, `MemberAndMemberOf`).
*   **`UserAccountControlFlagsForMsAD`**: `[Flags]` bitwise enum assisting in the reading/writing of Microsoft Active Directory user states (e.g., `ACCOUNTDISABLE`, `NORMAL_ACCOUNT`, `DONT_EXPIRE_PASSWORD`, `PASSWORD_EXPIRED`).

---

## 🚀 Installation

Install via the NuGet Package Manager Console:

```bash
dotnet add package Bitai.LDAPHelper.DTO
```

Or via the NuGet CLI:

```bash
nuget install Bitai.LDAPHelper.DTO
```

---

## 💻 Code Examples

### Standard Operation Error Handling

All result objects derive from `LDAPOperationResult`, exposing intuitive status flags:

```csharp
using Bitai.LDAPHelper.DTO;

public void ProcessOperation(LDAPOperationResult result)
{
    if (result.IsSuccessfulOperation)
    {
        Console.WriteLine($"Operation succeeded: {result.OperationMessage}");
    }
    else
    {
        Console.WriteLine($"Operation failed: {result.OperationMessage}");
        if (result.HasErrorObject)
        {
            Console.WriteLine($"Exception Type: {result.ErrorType}");
            Console.WriteLine($"Stack Trace: {result.ErrorObject?.StackTrace}");
        }
    }
}
```

### Resolving Nested Group Memberships

```csharp
using Bitai.LDAPHelper.DTO;

public void ListAllGroups(LDAPEntry userEntry)
{
    // Retrieve parent groups recursively
    IEnumerable<LDAPEntry> allGroups = userEntry.GetMemberOfEntriesRecursively();

    Console.WriteLine($"User {userEntry.cn} is member of:");
    foreach (var group in allGroups)
    {
        Console.WriteLine($" - {group.distinguishedName}");
    }
}
```