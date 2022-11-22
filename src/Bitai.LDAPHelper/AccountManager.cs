using Novell.Directory.Ldap;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Bitai.LDAPHelper
{
	public class AccountManager : BaseHelper
	{
		#region Constructors
		public AccountManager(ClientConfiguration clientConfiguration) : base(clientConfiguration)
		{
		}

		public AccountManager(ConnectionInfo connectionInfo, SearchLimits searchLimits, DTO.LDAPDomainAccountCredential domainAccountCredential) : base(connectionInfo, searchLimits, domainAccountCredential)
		{
		}
		#endregion



		public async Task<DTO.LDAPPasswordUpdateResult> SetAccountPassword(DTO.LDAPDistinguishedNameCredential credential, string requestLabel = null, bool postUpdateTestAuthentication = true)
		{
			try
			{
				if (string.IsNullOrEmpty(credential.Password))
					throw new InvalidOperationException($"The password to be assigned to the {credential.DistinguishedName} account is required.");

				//Create password modification request
				string newPassword = $"\"{credential.Password}\"";
				byte[] encodedNewPasswordBytes = Encoding.Unicode.GetBytes(newPassword);
				string newPasswordEncodedString = Convert.ToBase64String(encodedNewPasswordBytes);
				var pwdAttribute = new LdapAttribute(DTO.EntryAttribute.unicodePwd.ToString(), encodedNewPasswordBytes);
				var pwdModification = new LdapModification(LdapModification.Replace, pwdAttribute);

				using (var ldapConnection = await GetLdapConnection(this.ConnectionInfo, this.DomainAccountCredential))
				{
					ldapConnection.Modify(credential.DistinguishedName, pwdModification);

					if (postUpdateTestAuthentication)
					{
						var authenticator = new Authenticator(ConnectionInfo);
						var authenticationResult = await authenticator.AuthenticateAsync(credential, requestLabel);

						if (authenticationResult.IsSuccessfulOperation)
						{
							if (authenticationResult.IsAuthenticated)
								return new DTO.LDAPPasswordUpdateResult(requestLabel, $"Password set successfully for {credential.DistinguishedName}");
							else
								return new DTO.LDAPPasswordUpdateResult(requestLabel, $"Could not set password for {credential.DistinguishedName} distinguished name.", false);
						}
						else {
							if (authenticationResult.HasErrorObject)
								throw new Exception(authenticationResult.OperationMessage, authenticationResult.ErrorObject);
							else
								throw new Exception(authenticationResult.OperationMessage);
						}
					}
					else
						return new DTO.LDAPPasswordUpdateResult(requestLabel, $"Password set successfully for {credential.DistinguishedName}");
				}
			}
			catch (Exception ex)
			{
				return new DTO.LDAPPasswordUpdateResult("Unexpected error trying to replace password.", ex, requestLabel);
			}
		}
	}
}
