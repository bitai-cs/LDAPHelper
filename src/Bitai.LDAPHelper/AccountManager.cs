﻿using Bitai.LDAPHelper.DTO;
using Novell.Directory.Ldap;
using System;
using System.IO;
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




		/// <summary>
		/// Create a user account in Ms Active Directory
		/// https://www.rlmueller.net/Name_Attributes.htm
		/// </summary>
		/// <param name="newUserAccount"><see cref="LDAPMsADUserAccount"/></param>
		/// <param name="distinguishedNameOfContainer">DN of the container in which the user account will be created.</param>
		/// <param name="requestLabel">Optional tag to mark the request and/or response.</param>
		/// <returns>A Task of <see cref="LDAPCreateMsADUserAccountResult"/></returns>
		public async Task<LDAPCreateMsADUserAccountResult> CreateUserAccountForMsAD(LDAPMsADUserAccount newUserAccount, string requestLabel = null)
		{
			try
			{
				#region Validate minimally required properties
				if (string.IsNullOrEmpty(newUserAccount.DistinguishedNameOfContainer))
					throw new InvalidOperationException($"{nameof(newUserAccount.DistinguishedNameOfContainer)} is required.");

				if (string.IsNullOrEmpty(newUserAccount.Cn))
					throw new InvalidOperationException($"{nameof(newUserAccount.Cn)} is required.");

				if (string.IsNullOrEmpty(newUserAccount.DisplayName))
					throw new InvalidOperationException($"{nameof(newUserAccount.DisplayName)} is required.");

				if (string.IsNullOrEmpty(newUserAccount.SAMAccountName))
					throw new InvalidOperationException($"{nameof(newUserAccount.SAMAccountName)} is required.");

				if (newUserAccount.ObjectClass == null || newUserAccount.ObjectClass.Length == 0)
					throw new InvalidOperationException($"{nameof(newUserAccount.ObjectClass)} is required.");
				#endregion

				//Generate user account DistinguishedName LDAP attribute
				newUserAccount.DistinguishedName = $"CN={newUserAccount.Cn},{newUserAccount.DistinguishedNameOfContainer}";

				#region Initialize and populate LDAP attribute set
				LdapAttributeSet ldapAttributeSet = new LdapAttributeSet();

				if (!string.IsNullOrEmpty(newUserAccount.GivenName))
					ldapAttributeSet.Add(new LdapAttribute(EntryAttribute.givenName.ToString(), newUserAccount.GivenName));

				if (!string.IsNullOrEmpty(newUserAccount.Sn))
					ldapAttributeSet.Add(new LdapAttribute(EntryAttribute.sn.ToString(), newUserAccount.Sn));

				if (!string.IsNullOrEmpty(newUserAccount.Cn))
					ldapAttributeSet.Add(new LdapAttribute(EntryAttribute.cn.ToString(), newUserAccount.Cn));

				if (!string.IsNullOrEmpty(newUserAccount.Name))
					ldapAttributeSet.Add(new LdapAttribute(EntryAttribute.name.ToString(), newUserAccount.Name));

				if (!string.IsNullOrEmpty(newUserAccount.DisplayName))
					ldapAttributeSet.Add(new LdapAttribute(EntryAttribute.displayName.ToString(), newUserAccount.DisplayName));

				if (!string.IsNullOrEmpty(newUserAccount.Description))
					ldapAttributeSet.Add(new LdapAttribute(EntryAttribute.description.ToString(), newUserAccount.Description));

				if (newUserAccount.MemberOf != null && newUserAccount.MemberOf.Length > 0)
					ldapAttributeSet.Add(new LdapAttribute(EntryAttribute.memberOf.ToString(), newUserAccount.MemberOf));

				if (newUserAccount.ObjectClass != null && newUserAccount.ObjectClass.Length > 0)
					ldapAttributeSet.Add(new LdapAttribute(EntryAttribute.objectClass.ToString(), newUserAccount.ObjectClass));

				if (!string.IsNullOrEmpty(newUserAccount.SAMAccountName))
					ldapAttributeSet.Add(new LdapAttribute(EntryAttribute.sAMAccountName.ToString(), newUserAccount.SAMAccountName));

				if (!string.IsNullOrEmpty(newUserAccount.UserPrincipalName))
					ldapAttributeSet.Add(new LdapAttribute(EntryAttribute.userPrincipalName.ToString(), newUserAccount.UserPrincipalName));

				ldapAttributeSet.Add(new LdapAttribute(EntryAttribute.userAccountControl.ToString(), ((int)newUserAccount.UserAccountControl).ToString()));

				if (!string.IsNullOrEmpty(newUserAccount.Department))
					ldapAttributeSet.Add(new LdapAttribute(EntryAttribute.department.ToString(), newUserAccount.Department));

				if (!string.IsNullOrEmpty(newUserAccount.TelephoneNumber))
					ldapAttributeSet.Add(new LdapAttribute(EntryAttribute.telephoneNumber.ToString(), newUserAccount.TelephoneNumber));

				if (!string.IsNullOrEmpty(newUserAccount.Mail))
					ldapAttributeSet.Add(new LdapAttribute(EntryAttribute.mail.ToString(), newUserAccount.Mail));

				if (!string.IsNullOrEmpty(newUserAccount.Password))
				{
					byte[] encodedNewPasswordBytes = Encoding.Unicode.GetBytes($"\"{newUserAccount.Password}\"");
					ldapAttributeSet.Add(new LdapAttribute(EntryAttribute.unicodePwd.ToString(), encodedNewPasswordBytes));
				}
				#endregion

				using (var ldapConnection = await GetLdapConnection(this.ConnectionInfo, this.DomainAccountCredential))
				{
					LdapEntry newEntry = new LdapEntry(newUserAccount.DistinguishedName, ldapAttributeSet);

					ldapConnection.Add(newEntry);
				}

				return new LDAPCreateMsADUserAccountResult(newUserAccount.SecureClone(), requestLabel)
				{
					OperationMessage = $"User account created at {newUserAccount.DistinguishedName} with {EntryAttribute.sAMAccountName.ToString()}: {newUserAccount.SAMAccountName}"
				};
			}
			catch (Exception ex)
			{
				return new LDAPCreateMsADUserAccountResult("Error creating user account.", ex, requestLabel)
				{
					UserAccount = newUserAccount.SecureClone()
				};
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="credential"><see cref="LDAPDistinguishedNameCredential"/></param>
		/// <param name="requestLabel">Optional tag to mark the request and/or response.</param>
		/// <param name="postUpdateTestAuthentication">True if the account will be tested to be able to authenticate with the new password. False if only the password will be assigned, authentication will not be tested.</param>
		/// <returns></returns>
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
						else
						{
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
