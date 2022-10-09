using Bitai.LDAPHelper.DTO;
using Bitai.LDAPHelper.QueryFilters;
using Microsoft.VisualBasic;
using Novell.Directory.Ldap;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
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

		public AccountManager(ConnectionInfo connectionInfo, SearchLimits searchLimits, LDAPDomainAccountCredential domainAccountCredential) : base(connectionInfo, searchLimits, domainAccountCredential)
		{
		}
		#endregion



		public async Task<LDAPPasswordUpdateResult> SetAccountPassword(LDAPDistinguishedNameCredential credential, string requestTag = null, bool postUpdateTestAuthentication = true)
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
						var authenticationResult = await authenticator.AuthenticateAsync(credential, requestTag);

						if (authenticationResult.IsSuccessfulOperation)
						{
							if (authenticationResult.IsAuthenticated)
								return new LDAPPasswordUpdateResult(requestTag, $"Password set successfully for {credential.DistinguishedName}");
							else
								return new LDAPPasswordUpdateResult(requestTag, $"Could not set password for {credential.DistinguishedName} distinguished name.", false);
						}
						else {
							if (authenticationResult.HasErrorObject)
								throw new Exception(authenticationResult.OperationMessage, authenticationResult.ErrorObject);
							else
								throw new Exception(authenticationResult.OperationMessage);
						}
					}
					else
						return new LDAPPasswordUpdateResult(requestTag, $"Password set successfully for {credential.DistinguishedName}");
				}
			}
			catch (Exception ex)
			{
				return new LDAPPasswordUpdateResult("Unexpected error trying to replace password.", ex, requestTag);
			}
		}
	}
}
