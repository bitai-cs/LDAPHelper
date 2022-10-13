using Novell.Directory.Ldap;
using System;
using System.Threading.Tasks;
using System.Linq;
using Bitai.LDAPHelper.DTO;
using System.Diagnostics.CodeAnalysis;

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
		public async Task<LDAPDomainAccountAuthenticationResult> AuthenticateAsync(DTO.LDAPDomainAccountCredential domainAccountCredential, string requestTag = null)
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

				return new LDAPDomainAccountAuthenticationResult(domainAccountCredential.SecureClone(), authenticated.Value, requestTag);
			}
			catch (Exception ex)
			{
				return new LDAPDomainAccountAuthenticationResult(domainAccountCredential.SecureClone(), $"Failed to authenticate {domainAccountCredential.DomainAccountName}" ,ex, requestTag);
			}
		}

		public async Task<LDAPDistinguishedNameAuthenticationResult> AuthenticateAsync(DTO.LDAPDistinguishedNameCredential distinguishedNameCredential, string requestTag = null)
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

				return new LDAPDistinguishedNameAuthenticationResult(distinguishedNameCredential.SecureClone(), authenticated.Value, requestTag);
			}
			catch(Exception ex)
			{
				return new LDAPDistinguishedNameAuthenticationResult(distinguishedNameCredential.SecureClone(), $"Failed to authenticate {distinguishedNameCredential.DistinguishedName}", ex, requestTag);
			}
		}
		#endregion
	}
}