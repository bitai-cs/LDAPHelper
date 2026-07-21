namespace Bitai.LDAPHelper
{
	/// <summary>
	/// Encapsulates LDAP client runtime configuration.
	/// </summary>
	public class ClientConfiguration
	{
		/// <summary>
		/// Gets or sets LDAP server connection settings.
		/// </summary>
		public ConnectionInfo ServerSettings { get; set; }

		/// <summary>
		/// Gets or sets the service account used for LDAP operations.
		/// </summary>
		public DTO.LDAPDomainAccountCredential DomainAccountCredential { get; set; }

        /// <summary>
		/// Gets or sets LDAP search limits.
        /// </summary>
        public SearchLimits SearchLimits { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="serverSettings"><see cref="ConnectionInfo"/></param>
		/// <param name="domainAccountCredentials">Domain account credential used for bind/search operations.</param>
        /// <param name="searchLimits"><see cref="LDAPHelper.SearchLimits"/></param>
        public ClientConfiguration(ConnectionInfo serverSettings, DTO.LDAPDomainAccountCredential domainAccountCredentials, SearchLimits searchLimits)
		{
			this.ServerSettings = serverSettings;
			this.DomainAccountCredential = domainAccountCredentials;
			this.SearchLimits = searchLimits;
		}
	}
}
