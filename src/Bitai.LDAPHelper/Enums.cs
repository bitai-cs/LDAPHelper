namespace Bitai.LDAPHelper
{
    /// <summary>
    /// LDAP default ports
    /// </summary>
	public enum LdapServerDefaultPorts : int
	{
		/// <summary>
		/// Default LDAP port.
		/// </summary>
		DefaultPort = 389,

		/// <summary>
		/// Default LDAP-over-SSL port.
		/// </summary>
		DefaultSslPort = 636,

		/// <summary>
		/// Default Global Catalog port.
		/// </summary>
		DefaultGlobalCatalogPort = 3268,

		/// <summary>
		/// Default Global Catalog SSL port.
		/// </summary>
		DefaultGlobalCatalogSslPort = 3269
	}    
}
