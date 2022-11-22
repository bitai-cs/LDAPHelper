using System;
using System.Threading.Tasks;

namespace Bitai.LDAPHelper
{
	public class Authenticator : BaseHelper
	{
		#region Constructors
		public Authenticator(ConnectionInfo connectionInfo) : base(connectionInfo)
		{
		}
		#endregion


		#region Public methods
		/// <summary>
		/// Authenticate <see cref="Credentials"/> on the LDAP Server
		/// </summary>
		/// <param name="domainAccountCredential"><see cref="Credentials"/> to connect and authenticate on the LDAP Server.</param>
		/// <returns>True or false, if authenticated or no.</returns>
		public async Task<DTO.LDAPDomainAccountAuthenticationResult> AuthenticateAsync(DTO.LDAPDomainAccountCredential domainAccountCredential, string requestLabel = null)
		{
			try
			{
				bool? authenticated;
				using (var connection = await GetLdapConnection(this.ConnectionInfo, domainAccountCredential, false))
				{
					if (connection.Bound)
						authenticated = true;
					else
						authenticated = false;
				}

				return new DTO.LDAPDomainAccountAuthenticationResult(domainAccountCredential.SecureClone(), authenticated.Value, requestLabel);
			}
			catch (Exception ex)
			{
				return new DTO.LDAPDomainAccountAuthenticationResult(domainAccountCredential.SecureClone(), $"Failed to authenticate {domainAccountCredential.DomainAccountName}", ex, requestLabel);
			}
		}

		public async Task<DTO.LDAPDistinguishedNameAuthenticationResult> AuthenticateAsync(DTO.LDAPDistinguishedNameCredential distinguishedNameCredential, string requestLabel = null)
		{
			try
			{
				bool? authenticated;
				using (var connection = await GetLdapConnection(this.ConnectionInfo, distinguishedNameCredential, false))
				{
					if (connection.Bound)
						authenticated = true;
					else
						authenticated = false;
				}

				return new DTO.LDAPDistinguishedNameAuthenticationResult(distinguishedNameCredential.SecureClone(), authenticated.Value, requestLabel);
			}
			catch (Exception ex)
			{
				return new DTO.LDAPDistinguishedNameAuthenticationResult(distinguishedNameCredential.SecureClone(), $"Failed to authenticate {distinguishedNameCredential.DistinguishedName}", ex, requestLabel);
			}
		}
		#endregion
	}
}