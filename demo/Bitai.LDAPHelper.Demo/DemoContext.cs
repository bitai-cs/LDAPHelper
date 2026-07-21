using Bitai.LDAPHelper.LdapAdapters;
using Bitai.LDAPHelper.DTO;

namespace Bitai.LDAPHelper.Demo;

/// <summary>
/// Holds runtime context and resolved settings for demo execution.
/// </summary>
public class DemoContext
{
    /// <summary>
    /// Gets or sets which LDAP implementation to use.
    /// </summary>
    public ImplementationType Implementation { get; set; }

    /// <summary>
    /// Gets or sets the connection-factory implementation used by demos.
    /// </summary>
    public ILdapConnectionFactoryAdapter ConnectionFactory { get; set; }

    /// <summary>
    /// Gets or sets loaded demo configuration.
    /// </summary>
    public DemoSetup Configuration { get; set; }

    /// <summary>
    /// Gets or sets the request label used by demo operations.
    /// </summary>
    public string RequestLabel { get; set; } = "My Demo";
    
    // Connection settings
    public string SelectedLdapServer { get; set; }
    public int SelectedLdapServerPort { get; set; }
    public bool SelectedUseSsl { get; set; }
    public short SelectedConnectionTimeout { get; set; }
    public string SelectedDomainAccountName { get; set; }
    public string SelectedDomainAccountPassword { get; set; }
    public string SelectedBaseDN { get; set; }

    /// <summary>
    /// Builds a <see cref="ConnectionInfo"/> instance from selected demo settings.
    /// </summary>
    /// <returns>Connection settings.</returns>
    public ConnectionInfo GetConnectionInfo()
    {
        return new ConnectionInfo(
            SelectedLdapServer,
            SelectedLdapServerPort,
            SelectedUseSsl,
            SelectedConnectionTimeout);
    }

    /// <summary>
    /// Builds a domain-account credential from selected demo settings.
    /// </summary>
    /// <returns>Domain-account credential.</returns>
    public LDAPDomainAccountCredential GetDomainAccountCredential()
    {
        var parts = SelectedDomainAccountName.Split(new char[] { '\\' });
        return new LDAPDomainAccountCredential(
            parts[0],
            parts[1],
            SelectedDomainAccountPassword);
    }

    /// <summary>
    /// Builds search limits from selected demo settings.
    /// </summary>
    /// <returns>Search-limit settings.</returns>
    public SearchLimits GetSearchLimits()
    {
        return new SearchLimits(SelectedBaseDN)
        {
            MaxSearchResults = 1000,
            MaxSearchTimeout = 60
        };
    }

    /// <summary>
    /// Builds a full client configuration from selected demo settings.
    /// </summary>
    /// <returns>Client configuration for helper classes.</returns>
    public ClientConfiguration GetClientConfiguration()
    {
        return new ClientConfiguration(
            GetConnectionInfo(),
            GetDomainAccountCredential(),
            GetSearchLimits());
    }
}
