# Bitai.LDAPHelper.Tests.Mocks

`Bitai.LDAPHelper.Tests.Mocks` provides in-memory LDAP adapter implementations for testing and demo scenarios in the Bitai LDAP Helper ecosystem. It implements the adapter contracts consumed by `Bitai.LDAPHelper`, allowing authentication, search, account management, and group membership workflows to be exercised without a live LDAP or Active Directory server.

This project is intended for tests, demos, and local development support. It should not be used as a production LDAP provider.

## Purpose

Use this project when you need to:

- Unit test LDAP-dependent code without network access.
- Verify how `Bitai.LDAPHelper` services call LDAP adapter contracts.
- Seed predictable directory entries for repeatable test cases.
- Record add, modify, and delete operations performed by higher-level services.
- Run demo flows in an offline mock mode.

## Target Framework

- .NET 10.0
- Nullable reference types enabled
- Implicit global usings enabled

## Package Metadata

- Package ID: `Bitai.LDAPHelper.Tests.Mocks`
- Version: `10.0.0`
- Repository: `https://github.com/bitai-cs/LDAPHelper`
- License: MIT

## Project Dependencies

This project references:

- `Bitai.LDAPHelper`
- `Bitai.LDAPHelper.LdapAdapters`
- `Bitai.LDAPHelper.DTO`

Those references provide the public helper services, adapter contracts, DTOs, and LDAP-related domain types used by the mocks.

## Main Components

### Adapter Mocks

- `MockLdapConnectionAdapter` provides a configurable in-memory LDAP connection.
- `MockLdapConnectionFactoryAdapter` returns a supplied mock connection and simulates connect and bind behavior.
- `MockLdapPersistenConnectionAdapter` extends the basic mock connection with a shared in-memory data store.
- `MockLdapPersistentConnectionFactoryAdapter` creates the persistent mock connection used by richer demo scenarios.
- `MockLdapEntryAdapter`, `MockLdapAttributeSetAdapter`, `MockLdapAttributeAdapter`, `MockLdapMessageAdapter`, `MockLdapSearchQueueAdapter`, and `MockLdapModificationAdapter` implement the lower-level LDAP contracts.

### Mock Data

- `MockLdapDataStore` is a thread-safe singleton store keyed by distinguished name.
- `MockLdapDataSeeder` populates the store with representative domains, organizational units, users, computers, groups, and group memberships.

## Basic Unit Test Usage

The simplest pattern is to create a mock connection, register search results for expected filters, and pass a mock factory into the service under test.

```csharp
using Bitai.LDAPHelper;
using Bitai.LDAPHelper.DTO;
using Bitai.LDAPHelper.Tests.Mocks.LdapAdapters;

var connection = new MockLdapConnectionAdapter();

var userEntry = new MockLdapEntryAdapter("CN=John Smith,OU=Users,DC=example,DC=com");
userEntry.AddAttribute("cn", "John Smith");
userEntry.AddAttribute("sAMAccountName", "john.smith");
userEntry.AddAttribute("distinguishedName", userEntry.DistinguishedName);

connection.AddSearchResult("(sAMAccountName=john.smith)", new List<MockLdapEntryAdapter>
{
    userEntry
});

var factory = new MockLdapConnectionFactoryAdapter(connection);

var connectionInfo = new ConnectionInfo("localhost", 389, useSSL: false, connectionTimeout: 15);
var credential = new LDAPDomainAccountCredential("EXAMPLE", "service.account", "StrongPassword!");
var searchLimits = new SearchLimits("DC=example,DC=com")
{
    MaxSearchResults = 10,
    MaxSearchTimeout = 15
};

var searcher = new Searcher(connectionInfo, searchLimits, credential, factory);
```

## Verifying Write Operations

`MockLdapConnectionAdapter` records created, modified, and deleted entries so tests can assert side effects.

```csharp
var connection = new MockLdapConnectionAdapter();
var factory = new MockLdapConnectionFactoryAdapter(connection);

// Execute code that creates, modifies, or deletes LDAP entries.

Assert.NotEmpty(connection.CreatedEntries);
Assert.NotEmpty(connection.Modifications);
Assert.NotEmpty(connection.DeletedEntries);
```

## Seeded Directory Usage

For integration-style tests or demos, use the persistent connection factory with `MockLdapDataSeeder`.

```csharp
using Bitai.LDAPHelper.Tests.Mocks.LdapAdapters;
using Bitai.LDAPHelper.Tests.Mocks.LdapData;
using Microsoft.Extensions.Logging.Abstractions;

var seeder = new MockLdapDataSeeder(NullLogger<MockLdapDataSeeder>.Instance);
seeder.SeedAllData();

var factory = new MockLdapPersistentConnectionFactoryAdapter();
```

The seeded data includes representative LATAM domain roots, organizational units, user accounts, groups, computers, and nested memberships used by the repository tests and demo project.

## Mock Behavior Notes

- `MockLdapConnectionAdapter.ConnectAsync` rejects empty hosts, `unknown`, `0.0.0.0`, and non-positive ports.
- `MockLdapConnectionAdapter.BindAsync` rejects empty credentials and intentionally weak or suspicious sample credentials used by tests.
- Search matching in the basic mock connection is filter-pattern based; register the expected filter text before executing the service under test.
- The persistent mock connection searches the shared `MockLdapDataStore` and applies a richer in-memory behavior model.
- `ServerCertificateValidationByPass` throws in mock classes because certificate bypass behavior is not meaningful in an in-memory implementation.

## Build

From the repository root:

```powershell
dotnet build tests/Bitai.LDAPHelper.Tests.Mocks/Bitai.LDAPHelper.Tests.Mocks.csproj
```

## Related Projects

- `Bitai.LDAPHelper.LdapAdapters`: Adapter contracts implemented by this project.
- `Bitai.LDAPHelper`: Core LDAP helper services tested with these mocks.
- `Bitai.LDAPHelper.Tests`: Test suite that consumes these mocks.
- `Bitai.LDAPHelper.Demo`: Demo application that can run against the persistent mock data store.

## License

This project is licensed under the MIT License. See [LICENSE.md](LICENSE.md).
