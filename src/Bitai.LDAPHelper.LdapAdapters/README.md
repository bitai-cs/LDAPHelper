# Bitai.LDAPHelper.LdapAdapters

`Bitai.LDAPHelper.LdapAdapters` defines the LDAP adapter contracts used by the Bitai LDAP Helper libraries. It is a small abstraction layer that keeps the core helper logic independent from any specific LDAP client implementation, such as `Novell.Directory.Ldap.NETStandard`.

This package contains interfaces and enums only. It does not open LDAP connections by itself and does not include a concrete LDAP provider.

## Purpose

Use this project when you need to:

- Implement a concrete LDAP provider for `Bitai.LDAPHelper`.
- Test LDAP-dependent code without binding directly to a real directory server.
- Isolate application logic from vendor-specific LDAP client APIs.
- Standardize LDAP operations such as bind, search, add, modify, delete, and attribute access.

## Target Framework

- .NET 10.0
- Nullable reference types enabled
- Implicit global usings enabled

## Package Metadata

- Package ID: `Bitai.LDAPHelper.LdapAdapters`
- Version: `10.0.0`
- Repository: `https://github.com/bitai-cs/LDAPHelper`
- License: MIT

## Main Contracts

### Connection and Factory

- `ILdapConnectionFactoryAdapter` creates configured LDAP connections from `IConnectionInfo` and credentials.
- `ILdapConnectionAdapter` represents a connected LDAP session and exposes bind, search, add, modify, delete, disconnect, and dispose operations.
- `IConnectionInfo` describes LDAP server address, port, SSL usage, and timeout settings.

### Search

- `ISearchLimits` describes base DN, search scope, result limits, and timeout limits.
- `ILdapSearchQueueAdapter` abstracts queued LDAP search responses.
- `ILdapMessageAdapter` distinguishes search result entries from search completion messages.

### Entries and Attributes

- `ILdapEntryAdapter` exposes an LDAP entry distinguished name and its attribute set.
- `ILdapAttributeSetAdapter` creates and retrieves LDAP attributes by name.
- `ILdapAttributeAdapter` exposes attribute values as string, string array, byte array, or byte array collection.

### Modifications

- `ILdapModificationAdapter` describes an LDAP attribute modification.
- `LdapModificationType` supports add, delete, and replace operations.
- `LdapSearchScope` supports base, one-level, and subtree LDAP searches.

## Implementing an Adapter

A concrete provider typically implements `ILdapConnectionFactoryAdapter` and `ILdapConnectionAdapter`, then maps the Bitai contracts to the provider-specific LDAP client.

```csharp
using Bitai.LDAPHelper.LdapAdapters;

public sealed class CustomLdapConnectionFactoryAdapter : ILdapConnectionFactoryAdapter
{
    public async Task<ILdapConnectionAdapter> CreateConnectionAsync(
        IConnectionInfo connectionInfo,
        string userAccount,
        string password,
        bool bindRequired = true)
    {
        var connection = new CustomLdapConnectionAdapter();
        connection.ConnectionTimeout = connectionInfo.ConnectionTimeout;
        connection.SecureSocketLayer = connectionInfo.UseSSL;

        await connection.ConnectAsync(connectionInfo.Server, connectionInfo.ServerPort);

        if (bindRequired)
        {
            await connection.BindAsync(userAccount, password);
        }

        return connection;
    }
}
```

The adapter is then passed into higher-level `Bitai.LDAPHelper` services such as authentication, search, account management, and group membership validation.

## Design Notes

- The package intentionally contains no dependency on a specific LDAP SDK.
- Adapter implementations should preserve provider exceptions only when callers can handle them meaningfully; otherwise, translate them into domain-level exceptions used by the consuming library.
- `ILdapConnectionAdapter` implements `IDisposable`; concrete implementations should release network resources and provider-specific handles consistently.
- `ServerCertificateValidationByPass` exists for compatibility with provider adapters. Use it only in controlled development or test environments.

## Build

From the repository root:

```powershell
dotnet build src/Bitai.LDAPHelper.LdapAdapters/Bitai.LDAPHelper.LdapAdapters.csproj
```

## Related Projects

- `Bitai.LDAPHelper`: Core LDAP helper services that consume these contracts.
- `Bitai.LDAPHelper.Tests.Mocks`: In-memory mock implementations for tests and demos.

## License

This project is licensed under the MIT License. See [LICENSE.md](LICENSE.md).
