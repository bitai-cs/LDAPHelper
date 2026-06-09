using Bitai.LDAPHelper.LdapAdapters;
using Bitai.LDAPHelper.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

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
        /// Create a username in MS Active Directory service
        /// https://www.rlmueller.net/Name_Attributes.htm
        /// </summary>
        /// <param name="newUserAccount"><see cref="LDAPMsADUserAccount"/></param>
        /// <param name="distinguishedNameOfContainer">DN of the container in which the username will be created.</param>
        /// <param name="requestLabel">Optional tag to mark the request and/or response.</param>
        /// <returns>A Task of <see cref="LDAPCreateMsADUserAccountResult"/></returns>
        public async Task<LDAPCreateMsADUserAccountResult> CreateUserAccountForMsAD(LDAPMsADUserAccount newUserAccount, string requestLabel = null)
        {
            try
            {
                #region Validate minimally required properties
                if (string.IsNullOrEmpty(newUserAccount.DistinguishedNameOfContainer))
                    throw new DataValidationException($"{nameof(newUserAccount.DistinguishedNameOfContainer)} is required.");

                if (string.IsNullOrEmpty(newUserAccount.Cn))
                    throw new DataValidationException($"{nameof(newUserAccount.Cn)} is required.");

                if (string.IsNullOrEmpty(newUserAccount.DisplayName))
                    throw new DataValidationException($"{nameof(newUserAccount.DisplayName)} is required.");

                if (string.IsNullOrEmpty(newUserAccount.SAMAccountName))
                    throw new DataValidationException($"{nameof(newUserAccount.SAMAccountName)} is required.");

                if (newUserAccount.ObjectClass == null || newUserAccount.ObjectClass.Length == 0)
                    throw new DataValidationException($"{nameof(newUserAccount.ObjectClass)} is required.");
                #endregion

                //Generate username DistinguishedName LDAP attribute
                InitializeMissingMsADUserAccountDN(newUserAccount);

                LDAPEntry checkUserAccount = null;
                // Check whether an entry with the same distinguished name already exists in the directory before creating a new user. Since the distinguished name must be unique, a duplicate entry cannot be created. If one already exists, return an error or ask for a different name.
                try
                {
                    checkUserAccount = await verifyMsADEntryAccountAuthenticity(EntryAttribute.distinguishedName, newUserAccount.DistinguishedName, false, requestLabel);
                }
                catch (Exception)
                {
                    // Do nothing
                }
                finally
                {
                    if (checkUserAccount != null)
                        throw new DuplicateNameException($"The {EntryAttribute.displayName}: {newUserAccount.DistinguishedName} already exists in the directory.");
                }
                // Check whether an entry with the same sAMAccountName already exists in the directory before creating a new user. Since the sAMAccountName must be unique, a duplicate entry cannot be created. If one already exists, return an error or ask for a different name.
                try
                {
                    checkUserAccount = await verifyMsADEntryAccountAuthenticity(EntryAttribute.sAMAccountName, newUserAccount.SAMAccountName, false, requestLabel);
                }
                catch (Exception)
                {
                    // Do nothing
                }
                finally
                {
                    if (checkUserAccount != null)
                        throw new DuplicateNameException($"The {EntryAttribute.sAMAccountName}: {newUserAccount.SAMAccountName} already exists in the directory.");
                }

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

                    //Add new username entry to the directory
                    await ldapConnection.AddEntryAsync(newUserAccount.DistinguishedName, attributeSet);
                }

                return new LDAPCreateMsADUserAccountResult(newUserAccount.SecureClone(), requestLabel)
                {
                    OperationMessage = $"MS AD user account created at {newUserAccount.DistinguishedName} with {EntryAttribute.sAMAccountName.ToString()}: {newUserAccount.SAMAccountName}"
                };
            }
            catch (DataValidationException ex)
            {
                return new LDAPCreateMsADUserAccountResult("Unable to create user account.", ex, requestLabel)
                {
                    UserAccount = newUserAccount.SecureClone()
                };
            }
            catch (Exception ex)
			{
				return new LDAPCreateMsADUserAccountResult("Unexpected error while attempting to create user account.", ex, requestLabel)
				{
					UserAccount = newUserAccount.SecureClone()
				};
			}
		}

        /// <summary>
        /// Set a password for a username in MS Active Directory service. This method will verify the authenticity of the username by its distinguished name before trying to set the password. If the username is not valid, the operation will not be attempted and an error will be returned.
        /// </summary>
        /// <param name="credential"><see cref="LDAPDistinguishedNameCredential"/></param>
        /// <param name="requestLabel">Optional tag to mark the request and/or response.</param>
        /// <param name="postUpdateTestAuthentication">True if the MS AD user account will be tested to verify authentication with the new password. False if the password will simply be assigned and authentication will not be tested.</param>
        /// <returns></returns>
        public async Task<LDAPPasswordUpdateResult> SetUserAccountPasswordForMsAD(LDAPDistinguishedNameCredential credential, string requestLabel = null, bool postUpdateTestAuthentication = true)
		{
			try
			{
				if (string.IsNullOrEmpty(credential.DistinguishedName))
					throw new ArgumentNullException("The distinguished name of the username is required.");

				if (string.IsNullOrEmpty(credential.Password))
					throw new ArgumentNullException($"The password to be assigned is required.");

				var entry = await verifyMsADEntryAccountAuthenticity(EntryAttribute.distinguishedName, credential.DistinguishedName, true, requestLabel);

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
                    //await ldapConnection.ModifyAsync(entry.identifierValue, pwdModification);

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
			catch (EntryNotFoundException ex) {
                return new LDAPPasswordUpdateResult("User account not found.", ex, requestLabel);
            }
            catch (DataValidationException ex)
            {
                return new LDAPPasswordUpdateResult("Invalid data found.", ex, requestLabel);
            }
            catch (Exception ex)
			{
				return new LDAPPasswordUpdateResult("Unexpected error while attempting to replace password.", ex, requestLabel);
			}

            LDAPPasswordUpdateResult createSuccessfulResult(string label, string name) {
                return new LDAPPasswordUpdateResult(label, $"Password set successfully for {name}");
            }
        }

        /// <summary>
        /// Remove a username from MS Active Directory service. This method will verify the authenticity of the username by its distinguished name before trying to remove it. If the username is not valid, the operation will not be attempted and an error will be returned.
        /// </summary>
        /// <param name="identifierValue">Distinguished name of the username</param>
        /// <param name="requestLabel">Optional tag to mark the request and/or response.</param>
        /// <returns><see cref="LDAPDisableUserAccountOperationResult"/></returns>
        public async Task<LDAPDisableUserAccountOperationResult> DisableUserAccountForMsAD(EntryAttribute identifierAttribute, string identifierValue, string requestLabel)
		{
			try
			{
                if (EntryAttribute.sAMAccountName != identifierAttribute && EntryAttribute.distinguishedName != identifierAttribute)
                    throw new ArgumentException($"The identifier attribute must be {EntryAttribute.sAMAccountName} or {EntryAttribute.distinguishedName} for disabling a user account.");

                if (string.IsNullOrEmpty(identifierValue))
					throw new ArgumentNullException($"The user account's {identifierAttribute} value is required.");

                var entry = await verifyMsADEntryAccountAuthenticity(identifierAttribute, identifierValue, true, requestLabel);

                using (var ldapConnection = await GetLdapConnection(this.ConnectionInfo, this.DomainAccountCredential)) {
                    //To disable a MS AD user account, the userAccountControl attribute needs to be set with the appropriate flags. The flag for disabling an account is ACCOUNTDISABLE (0x0002). However, when setting the userAccountControl attribute, it is important to preserve the existing flags that are set for the account, and only add the ACCOUNTDISABLE flag without removing any of the existing flags. This is because other flags may be set for the account that are necessary for its proper functioning, and removing them could cause unintended consequences. Therefore, when disabling a username, you should retrieve the current value of the userAccountControl attribute, add the ACCOUNTDISABLE flag to it, and then update the attribute with the new value that includes both the existing flags and the ACCOUNTDISABLE flag.
                    UserAccountControlFlagsForMsAD userAccountControlFlags = UserAccountControlFlagsForMsAD.NORMAL_ACCOUNT | UserAccountControlFlagsForMsAD.ACCOUNTDISABLE;
                    //var userAccountControlAttribute = new LdapAttribute(DTO.EntryAttribute.userAccountControl.ToString(), ((int)userAccountControlFlags).ToString());

                    //Create modification request to disable the account by setting the userAccountControl attribute with the appropriate flags
                    var modification = ldapConnection.CreateModification(LdapModificationType.Replace, EntryAttribute.userAccountControl.ToString(),
                        ((int)userAccountControlFlags).ToString());
                    //var userAccountControlModification = new LdapModification(LdapModification.Replace, userAccountControlAttribute);

                    //Send modification request to the directory
                    await ldapConnection.ModifyEntryAsync(entry.distinguishedName, new[] { modification });
                    //await ldapConnection.ModifyAsync(entry.identifierValue, userAccountControlModification);
                }

				return new DTO.LDAPDisableUserAccountOperationResult(requestLabel)
				{
					OperationMessage = $"Username {entry.samAccountName} has been disabled."
				};
			}
            catch (EntryNotFoundException ex)
            {
                return new LDAPDisableUserAccountOperationResult("User account not found.", ex, requestLabel);
            }
            catch (DataValidationException ex)
            {
                return new LDAPDisableUserAccountOperationResult("Invalid data found.", ex, requestLabel);
            }
            catch (Exception ex)
			{
				return new LDAPDisableUserAccountOperationResult($"Error trying to disable username with DN: {identifierValue}", ex, requestLabel);
			}
		}

        /// <summary>
        /// Remove a username in MS Active Directory service. This operation will permanently delete the username entry from the directory, so it should be used with caution.
        /// </summary>
        /// <param name="identifierValue">Distinguished name of the username</param>
        /// <param name="requestLabel">Optional tag to mark the request and/or response.</param>
        /// <returns><see cref="LDAPRemoveMsADUserAccountResult"/></returns>
        public async Task<LDAPRemoveMsADUserAccountResult> RemoveUserAccountForMsAD(EntryAttribute identifierAttribute, string identifierValue, string requestLabel = null)
		{
			try
			{
                if (identifierAttribute != EntryAttribute.sAMAccountName && identifierAttribute != EntryAttribute.distinguishedName)
                    throw new ArgumentException($"The identifier attribute must be {EntryAttribute.sAMAccountName} or {EntryAttribute.distinguishedName} for removing a user account.");

                if (string.IsNullOrEmpty(identifierValue))
                    throw new ArgumentNullException($"The user account's {identifierAttribute} value is required.");

                var entry = await verifyMsADEntryAccountAuthenticity(identifierAttribute, identifierValue, true, requestLabel);

				using (var ldapConnection = await GetLdapConnection(this.ConnectionInfo, this.DomainAccountCredential))
				{
                    //To remove a username from MS AD, the username entry needs to be deleted from the directory. This operation will permanently delete the username entry, so it should be used with caution.
                    await ldapConnection.DeleteEntryAsync(entry.distinguishedName);
                    //await ldapConnection.DeleteAsync(entry.identifierValue);
                }

				return new LDAPRemoveMsADUserAccountResult(requestLabel)
				{
					OperationMessage = $"The username {entry.samAccountName} has been successfully removed."
				};
			}
            catch (EntryNotFoundException ex)
            {
                return new LDAPRemoveMsADUserAccountResult("User account not found.", ex, requestLabel);
            }
            catch (DataValidationException ex)
            {
                return new LDAPRemoveMsADUserAccountResult("Invalid data found.", ex, requestLabel);
            }
            catch (Exception ex)
			{
				return new LDAPRemoveMsADUserAccountResult($"Error trying to remove username with DN: {identifierValue}", ex, requestLabel);
			}
		}




		private async Task<LDAPEntry> verifyMsADEntryAccountAuthenticity(EntryAttribute identifierAttribute, string identifierValue, bool validateObjectClass, string requestLabel = null)
		{
            if (identifierAttribute != EntryAttribute.sAMAccountName && identifierAttribute != EntryAttribute.distinguishedName)
                throw new ArgumentException($"The identifier attribute must be {EntryAttribute.sAMAccountName} or {EntryAttribute.distinguishedName} for verifying the authenticity of a user account.");

            var onlyUsersFilterCombiner = QueryFilters.AttributeFilterCombiner.CreateOnlyUsersFilterCombiner();
			var attributeFilter = new QueryFilters.AttributeFilter(identifierAttribute, new QueryFilters.FilterValue(identifierValue));
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
				throw new EntryNotFoundException($"{identifierAttribute} {identifierValue} does not exist.");

			var entry = searchResult.Entries.Single();

			if (validateObjectClass && !entry.objectClass.Contains("user"))
				throw new DataValidationException($"{identifierAttribute} {identifierValue} is not a user entry.");

			return entry;
		}
	}
}
