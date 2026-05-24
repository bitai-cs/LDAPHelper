using Bitai.LDAPHelper.LdapAdapters;
using Bitai.LDAPHelper.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitai.LDAPHelper
{
	public class AccountManager : BaseHelper
	{
        #region Constructors
        public AccountManager(ClientConfiguration clientConfiguration, ILdapConnectionFactoryAdapter connectionFactory)
            : base(clientConfiguration, connectionFactory) {
        }

        public AccountManager(ConnectionInfo connectionInfo, SearchLimits searchLimits, DTO.LDAPDomainAccountCredential domainAccountCredential, ILdapConnectionFactoryAdapter connectionFactory)
            : base(connectionInfo, searchLimits, domainAccountCredential, connectionFactory) {
        }
        #endregion




        public void InitializeMissingMsADUserAccountDN(LDAPMsADUserAccount userAccount)
		{
			if (string.IsNullOrEmpty(userAccount.DistinguishedName))
				userAccount.DistinguishedName = $"CN={userAccount.Cn},{userAccount.DistinguishedNameOfContainer}";
		}

		/// <summary>
		/// Create a user account in MS Active Directory service
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
				InitializeMissingMsADUserAccountDN(newUserAccount);

                using (var ldapConnection = await GetLdapConnection(this.ConnectionInfo, this.DomainAccountCredential)) {
                    #region Initialize and populate LDAP attribute set
                    var attributeSet = ldapConnection.CreateAttributeSet();

                    if (!string.IsNullOrEmpty(newUserAccount.GivenName))
                        attributeSet.AddAttribute(EntryAttribute.givenName.ToString(), newUserAccount.GivenName);

                    if (!string.IsNullOrEmpty(newUserAccount.Sn))
                        attributeSet.AddAttribute(EntryAttribute.sn.ToString(), newUserAccount.Sn);

                    if (!string.IsNullOrEmpty(newUserAccount.Cn))
                        attributeSet.AddAttribute(EntryAttribute.cn.ToString(), newUserAccount.Cn);

                    if (!string.IsNullOrEmpty(newUserAccount.Name))
                        attributeSet.AddAttribute(EntryAttribute.name.ToString(), newUserAccount.Name);

                    if (!string.IsNullOrEmpty(newUserAccount.DisplayName))
                        attributeSet.AddAttribute(EntryAttribute.displayName.ToString(), newUserAccount.DisplayName);

                    if (!string.IsNullOrEmpty(newUserAccount.Description))
                        attributeSet.AddAttribute(EntryAttribute.description.ToString(), newUserAccount.Description);

                    if (newUserAccount.MemberOf != null && newUserAccount.MemberOf.Length > 0)
                        attributeSet.AddAttribute(EntryAttribute.memberOf.ToString(), newUserAccount.MemberOf);

                    if (newUserAccount.ObjectClass != null && newUserAccount.ObjectClass.Length > 0)
                        attributeSet.AddAttribute(EntryAttribute.objectClass.ToString(), newUserAccount.ObjectClass);

                    if (!string.IsNullOrEmpty(newUserAccount.SAMAccountName))
                        attributeSet.AddAttribute(EntryAttribute.sAMAccountName.ToString(), newUserAccount.SAMAccountName);

                    if (!string.IsNullOrEmpty(newUserAccount.UserPrincipalName))
                        attributeSet.AddAttribute(EntryAttribute.userPrincipalName.ToString(), newUserAccount.UserPrincipalName);

                    if (newUserAccount.UserAccountControlFlags.HasValue)
                        attributeSet.AddAttribute(EntryAttribute.userAccountControl.ToString(), ((int)newUserAccount.UserAccountControlFlags).ToString());

                    if (!string.IsNullOrEmpty(newUserAccount.Department))
                        attributeSet.AddAttribute(EntryAttribute.department.ToString(), newUserAccount.Department);

                    if (!string.IsNullOrEmpty(newUserAccount.TelephoneNumber))
                        attributeSet.AddAttribute(EntryAttribute.telephoneNumber.ToString(), newUserAccount.TelephoneNumber);

                    if (!string.IsNullOrEmpty(newUserAccount.Mail))
                        attributeSet.AddAttribute(EntryAttribute.mail.ToString(), newUserAccount.Mail);

                    if (!string.IsNullOrEmpty(newUserAccount.Password)) {
                        byte[] encodedNewPasswordBytes = Encoding.Unicode.GetBytes($"\"{newUserAccount.Password}\"");
                        attributeSet.AddAttribute(EntryAttribute.unicodePwd.ToString(), encodedNewPasswordBytes);
                    }                   
                    #endregion

                    //Add new user account entry to the directory
                    await ldapConnection.AddEntryAsync(newUserAccount.DistinguishedName, attributeSet);                    
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
        /// Set a password for a user account in MS Active Directory service. This method will verify the authenticity of the user account by its distinguished name before trying to set the password. If the user account is not valid, the operation will not be attempted and an error will be returned.
        /// </summary>
        /// <param name="credential"><see cref="LDAPDistinguishedNameCredential"/></param>
        /// <param name="requestLabel">Optional tag to mark the request and/or response.</param>
        /// <param name="postUpdateTestAuthentication">True if the account will be tested to be able to authenticate with the new password. False if only the password will be assigned, authentication will not be tested.</param>
        /// <returns></returns>
        public async Task<LDAPPasswordUpdateResult> SetUserAccountPasswordForMsAD(DTO.LDAPDistinguishedNameCredential credential, string requestLabel = null, bool postUpdateTestAuthentication = true)
		{
			try
			{
				if (string.IsNullOrEmpty(credential.DistinguishedName))
					throw new ArgumentNullException("The distinguished name of the user account is required.");

				if (string.IsNullOrEmpty(credential.Password))
					throw new ArgumentNullException($"The password to be assigned is required.");

				var entry = await verifyUserAccountAuthenticity(credential.DistinguishedName, requestLabel);

				//Create password modification request
				string newPassword = $"\"{credential.Password}\"";
				byte[] encodedNewPasswordBytes = Encoding.Unicode.GetBytes(newPassword);
				//string newPasswordEncodedString = Convert.ToBase64String(encodedNewPasswordBytes);
				//var pwdAttribute = new LdapAttribute(DTO.EntryAttribute.unicodePwd.ToString(), encodedNewPasswordBytes);
				//var pwdModification = new LdapModification(LdapModification.Replace, pwdAttribute);

				using (var ldapConnection = await GetLdapConnection(this.ConnectionInfo, this.DomainAccountCredential))
				{
                    var modification = ldapConnection.CreateModification(LdapModificationType.Replace, EntryAttribute.unicodePwd.ToString(), encodedNewPasswordBytes);

                    //Send modification request to the directory
                    await ldapConnection.ModifyEntryAsync(entry.distinguishedName, new[] { modification });
                    //await ldapConnection.ModifyAsync(entry.distinguishedName, pwdModification);

                    if (postUpdateTestAuthentication)
					{
						var authenticator = new Authenticator(ConnectionInfo, ConnectionFactory);
						var authenticationResult = await authenticator.AuthenticateAsync(credential, requestLabel);

						if (authenticationResult.IsSuccessfulOperation)
						{
							if (authenticationResult.IsAuthenticated)
								return createSuccessfulResult(requestLabel, entry.distinguishedName);
							else
								return new DTO.LDAPPasswordUpdateResult(requestLabel, $"Could not set password for {entry.distinguishedName} distinguished name.", false);
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
                        return createSuccessfulResult(requestLabel, entry.distinguishedName);
                }
			}
			catch (Bitai.LDAPHelper.EntryNotFoundException ex) {
                return new DTO.LDAPPasswordUpdateResult(ex.Message, ex, requestLabel);
            }
			catch (Exception ex)
			{
				return new DTO.LDAPPasswordUpdateResult("Unexpected error trying to replace password.", ex, requestLabel);
			}

            DTO.LDAPPasswordUpdateResult createSuccessfulResult(string label, string name) {
                return new DTO.LDAPPasswordUpdateResult(label, $"Password set successfully for {name}");
            }
        }

        /// <summary>
        /// Remove a user account from MS Active Directory service. This method will verify the authenticity of the user account by its distinguished name before trying to remove it. If the user account is not valid, the operation will not be attempted and an error will be returned.
        /// </summary>
        /// <param name="distinguishedName">User account distinguished name</param>
        /// <param name="requestLabel">Optional tag to mark the request and/or response.</param>
        /// <returns><see cref="LDAPDisableUserAccountOperationResult"/></returns>
        public async Task<LDAPDisableUserAccountOperationResult> DisableUserAccountForMsAD(string distinguishedName, string requestLabel)
		{
			try
			{
				if (string.IsNullOrEmpty(distinguishedName))
					throw new ArgumentNullException(nameof(distinguishedName));

				var entry = await verifyUserAccountAuthenticity(distinguishedName, requestLabel);

                using (var ldapConnection = await GetLdapConnection(this.ConnectionInfo, this.DomainAccountCredential)) {
                    //To disable a user account in MS AD, the userAccountControl attribute needs to be set with the appropriate flags. The flag for disabling an account is ACCOUNTDISABLE (0x0002). However, when setting the userAccountControl attribute, it is important to preserve the existing flags that are set for the account, and only add the ACCOUNTDISABLE flag without removing any of the existing flags. This is because other flags may be set for the account that are necessary for its proper functioning, and removing them could cause unintended consequences. Therefore, when disabling a user account, you should retrieve the current value of the userAccountControl attribute, add the ACCOUNTDISABLE flag to it, and then update the attribute with the new value that includes both the existing flags and the ACCOUNTDISABLE flag.
                    UserAccountControlFlagsForMsAD userAccountControlFlags = UserAccountControlFlagsForMsAD.NORMAL_ACCOUNT | UserAccountControlFlagsForMsAD.ACCOUNTDISABLE;
                    //var userAccountControlAttribute = new LdapAttribute(DTO.EntryAttribute.userAccountControl.ToString(), ((int)userAccountControlFlags).ToString());

                    //Create modification request to disable the account by setting the userAccountControl attribute with the appropriate flags
                    var modification = ldapConnection.CreateModification(LdapModificationType.Replace, EntryAttribute.userAccountControl.ToString(),
                        ((int)userAccountControlFlags).ToString());
                    //var userAccountControlModification = new LdapModification(LdapModification.Replace, userAccountControlAttribute);

                    //Send modification request to the directory
                    await ldapConnection.ModifyEntryAsync(entry.distinguishedName, new[] { modification });
                    //await ldapConnection.ModifyAsync(entry.distinguishedName, userAccountControlModification);
                }

				return new DTO.LDAPDisableUserAccountOperationResult(requestLabel)
				{
					OperationMessage = $"User account {entry.samAccountName} has been disabled."
				};
			}
            catch (Bitai.LDAPHelper.EntryNotFoundException ex) {
                return new DTO.LDAPDisableUserAccountOperationResult(ex.Message, ex, requestLabel);
            }
            catch (Exception ex)
			{
				return new LDAPDisableUserAccountOperationResult($"Error trying to disable user account with DN: {distinguishedName}", ex, requestLabel);
			}
		}

        /// <summary>
        /// Remove a user account in MS Active Directory service. This operation will permanently delete the user account entry from the directory, so it should be used with caution.
        /// </summary>
        /// <param name="distinguishedName">User account distinguished name.</param>
        /// <param name="requestLabel">Optional tag to mark the request and/or response.</param>
        /// <returns><see cref="LDAPRemoveMsADUserAccountResult"/></returns>
        public async Task<LDAPRemoveMsADUserAccountResult> RemoveUserAccountForMsAD(string distinguishedName, string requestLabel = null)
		{
			try
			{
				if (string.IsNullOrEmpty(distinguishedName))
					throw new ArgumentNullException("The DN of the account to be removed must be provided.");

				var entry = await verifyUserAccountAuthenticity(distinguishedName, requestLabel);

				using (var ldapConnection = await GetLdapConnection(this.ConnectionInfo, this.DomainAccountCredential))
				{
                    //To remove a user account from MS AD, the user account entry needs to be deleted from the directory. This operation will permanently delete the user account entry, so it should be used with caution.
                    await ldapConnection.DeleteEntryAsync(entry.distinguishedName);
                    //await ldapConnection.DeleteAsync(entry.distinguishedName);
                }

				return new LDAPRemoveMsADUserAccountResult(requestLabel)
				{
					OperationMessage = $"The user account {entry.samAccountName} has been successfully removed."
				};
			}
            catch (Bitai.LDAPHelper.EntryNotFoundException ex) {
                return new LDAPRemoveMsADUserAccountResult(ex.Message, ex, requestLabel);
            }
            catch (Exception ex)
			{
				return new LDAPRemoveMsADUserAccountResult($"Error trying to remove user account with DN: {distinguishedName}", ex, requestLabel);
			}
		}




		private async Task<LDAPEntry> verifyUserAccountAuthenticity(string distinguishedName, string requestLabel = null)
		{
			var onlyUsersFilterCombiner = QueryFilters.AttributeFilterCombiner.CreateOnlyUsersFilterCombiner();
			var attributeFilter = new QueryFilters.AttributeFilter(EntryAttribute.distinguishedName, new QueryFilters.FilterValue(distinguishedName));
			var searchFilterCombiner = new QueryFilters.AttributeFilterCombiner(false, true, new List<QueryFilters.ICombinableFilter> { onlyUsersFilterCombiner, attributeFilter });

			var searcher = new Searcher(this.ConnectionInfo, this.SearchLimits, this.DomainAccountCredential, ConnectionFactory);
			var searchResult = await searcher.SearchEntriesAsync(searchFilterCombiner, RequiredEntryAttributes.Few, requestLabel);
			if (!searchResult.IsSuccessfulOperation)
			{
				if (searchResult.HasErrorObject)
					throw searchResult.ErrorObject;
				else
					throw new Exception(searchResult.OperationMessage);
			}

			if (searchResult.Entries.Count() == 0)
				throw new EntryNotFoundException($"DN {distinguishedName} does not exist.");

			var entry = searchResult.Entries.Single();

			if (!entry.objectClass.Contains("user"))
				throw new InvalidOperationException($"DN {distinguishedName} is not a user account.");

			return entry;
		}
	}
}
