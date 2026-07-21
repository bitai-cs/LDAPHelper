using Bitai.LDAPHelper.LdapAdapters;
using Bitai.LDAPHelper.DTO;
using Bitai.LDAPHelper.QueryFilters;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Bitai.LDAPHelper
{
    /// <summary>
    /// Provides LDAP authentication workflows for domain-account and distinguished-name credentials.
    /// </summary>
    public class Authenticator : BaseHelper
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="Authenticator"/> class.
        /// </summary>
        /// <param name="connectionInfo">LDAP server connection settings.</param>
        /// <param name="connectionFactory">LDAP connection factory abstraction.</param>
        public Authenticator(ConnectionInfo connectionInfo, ILdapConnectionFactoryAdapter connectionFactory) : base(connectionInfo, connectionFactory) {
        }
        #endregion


        #region Public methods
        /// <summary>
        /// Authenticates a domain account and validates that exactly one matching LDAP user exists.
        /// </summary>
        /// <param name="credential">Credential to authenticate.</param>
        /// <param name="searchLimits">Search limits used to locate the account before authentication.</param>
        /// <param name="credentialForSearching">Credential used to perform the validation search.</param>
        /// <param name="requestLabel">Optional label used to correlate request/response operations.</param>
        /// <returns>
        /// A result object containing operation status, authentication status, and optional error details.
        /// </returns>
        public async Task<LDAPDomainAccountAuthenticationResult> AuthenticateAsync(LDAPDomainAccountCredential credential, SearchLimits searchLimits, LDAPDomainAccountCredential credentialForSearching, string requestLabel = null) {
            LDAPDomainAccountAuthenticationResult authenticationResult;

            try {
                var searcher = new Searcher(ConnectionInfo, searchLimits, credentialForSearching, ConnectionFactory);

                var onlyUsersFilter = AttributeFilterCombiner.CreateOnlyUsersFilterCombiner();
                var attributeFilter = new AttributeFilter(EntryAttribute.sAMAccountName, new FilterValue(credential.AccountName));
                var searchFilter = new AttributeFilterCombiner(false, true, new ICombinableFilter[] { onlyUsersFilter, attributeFilter });

                var searchResult = await searcher.SearchEntriesAsync(searchFilter, RequiredEntryAttributes.OnlyObjectSid, requestLabel);

                if (!searchResult.IsSuccessfulOperation) {
                    if (searchResult.HasErrorObject) {
                        return new LDAPDomainAccountAuthenticationResult(credential, searchResult.OperationMessage, searchResult.ErrorObject, requestLabel);
                    }
                    else {
                        authenticationResult = new LDAPDomainAccountAuthenticationResult(credential, false, requestLabel, false)
                        {
                            OperationMessage = searchResult.OperationMessage
                        };

                        return authenticationResult;
                    }
                }
                else {
                    if (searchResult.Entries.Count() == 0) {
                        authenticationResult = new LDAPDomainAccountAuthenticationResult(credential.SecureClone(), false, requestLabel);
                        authenticationResult.SetSuccessfulOperation($"The domain username {credential.DomainName}\\{credential.AccountName} could not be found.");

                        return authenticationResult;
                    }
                    else if (searchResult.Entries.Count() > 1) {
                        authenticationResult = new LDAPDomainAccountAuthenticationResult(credential.SecureClone(), false, requestLabel);
                        authenticationResult.SetSuccessfulOperation($"Multiple {credential.DomainName}\\{credential.AccountName} accounts were found. Accounts must be unique. Verify the parameters with which the search for user accounts is carried out.");

                        return authenticationResult;
                    }
                }

                bool? authenticated;
                using (var connection = await GetLdapConnection(this.ConnectionInfo, credential, false)) {
                    if (connection.IsBound)
                        authenticated = true;
                    else
                        authenticated = false;
                }

                authenticationResult = new LDAPDomainAccountAuthenticationResult(credential.SecureClone(), authenticated.Value, requestLabel);
                if (authenticated.Value)
                    authenticationResult.SetSuccessfulOperation($"The domain username {credential.DomainName}\\{credential.AccountName} has been successfully authenticated.");
                else
                    authenticationResult.SetSuccessfulOperation("The password is wrong");

                return authenticationResult;
            }
            catch (Exception ex) {
                return new LDAPDomainAccountAuthenticationResult(credential.SecureClone(), $"Failed to authenticate {credential.DomainAccountName}", ex, requestLabel);
            }
        }

        /// <summary>
        /// Authenticates a domain account directly against the LDAP server.
        /// </summary>
        /// <param name="credential">Credential to authenticate.</param>
        /// <param name="requestLabel">Optional label used to correlate request/response operations.</param>
        /// <returns>
        /// A result object containing operation status, authentication status, and optional error details.
        /// </returns>
        public async Task<LDAPDomainAccountAuthenticationResult> AuthenticateAsync(LDAPDomainAccountCredential credential, string requestLabel = null) {
            try {
                bool? authenticated;
                using (var connection = await GetLdapConnection(this.ConnectionInfo, credential, false)) {
                    if (connection.IsBound)
                        authenticated = true;
                    else
                        authenticated = false;
                }

                var result = new LDAPDomainAccountAuthenticationResult(credential.SecureClone(), authenticated.Value, requestLabel);

                if (authenticated.Value)
                    result.SetSuccessfulOperation($"The domain username {credential.DomainAccountName} has been successfully authenticated.");
                else
                    result.SetSuccessfulOperation("Wrong username and/or password.");

                return result;
            }
            catch (Exception ex) {
                return new LDAPDomainAccountAuthenticationResult(credential.SecureClone(), $"Failed to authenticate {credential.DomainAccountName}", ex, requestLabel);
            }
        }

        /// <summary>
        /// Authenticates a distinguished-name account and validates that exactly one matching LDAP user exists.
        /// </summary>
        /// <param name="credential">Credential to authenticate.</param>
        /// <param name="searchLimits">Search limits used to locate the account before authentication.</param>
        /// <param name="credentialForSearching">Credential used to perform the validation search.</param>
        /// <param name="requestLabel">Optional label used to correlate request/response operations.</param>
        /// <returns>
        /// A result object containing operation status, authentication status, and optional error details.
        /// </returns>
        public async Task<LDAPDistinguishedNameAuthenticationResult> AuthenticateAsync(LDAPDistinguishedNameCredential credential, SearchLimits searchLimits, LDAPDomainAccountCredential credentialForSearching, string requestLabel = null) {
            LDAPDistinguishedNameAuthenticationResult authenticationResult;

            try {
                var searcher = new Searcher(ConnectionInfo, searchLimits, credentialForSearching, ConnectionFactory);

                var onlyUsersFilter = AttributeFilterCombiner.CreateOnlyUsersFilterCombiner();
                var attributeFilter = new AttributeFilter(EntryAttribute.distinguishedName, new FilterValue(credential.DistinguishedName));
                var searchFilter = new AttributeFilterCombiner(false, true, new ICombinableFilter[] { onlyUsersFilter, attributeFilter });

                var searchResult = await searcher.SearchEntriesAsync(searchFilter, RequiredEntryAttributes.OnlyObjectSid, requestLabel);

                if (!searchResult.IsSuccessfulOperation) {
                    if (searchResult.HasErrorObject) {
                        return new LDAPDistinguishedNameAuthenticationResult(credential, searchResult.OperationMessage, searchResult.ErrorObject, requestLabel);
                    }
                    else {
                        authenticationResult = new LDAPDistinguishedNameAuthenticationResult(credential, false, requestLabel, false);
                        authenticationResult.OperationMessage = searchResult.OperationMessage;

                        return authenticationResult;
                    }
                }
                else {
                    if (searchResult.Entries.Count() == 0) {
                        authenticationResult = new LDAPDistinguishedNameAuthenticationResult(credential.SecureClone(), false, requestLabel);
                        authenticationResult.SetSuccessfulOperation($"Unable to locate the account's DN: {credential.DistinguishedName}");

                        return authenticationResult;
                    }
                    else if (searchResult.Entries.Count() > 1) {
                        authenticationResult = new LDAPDistinguishedNameAuthenticationResult(credential.SecureClone(), false, requestLabel);
                        authenticationResult.SetSuccessfulOperation($"Multiple accounts were found. Accounts must be unique. Verify the parameters with which the search for user accounts is carried out.");

                        return authenticationResult;
                    }
                }

                bool? authenticated;
                using (var connection = await GetLdapConnection(this.ConnectionInfo, credential, false)) {
                    if (connection.IsBound)
                        authenticated = true;
                    else
                        authenticated = false;
                }

                authenticationResult = new LDAPDistinguishedNameAuthenticationResult(credential.SecureClone(), authenticated.Value, requestLabel);
                if (authenticated.Value)
                    authenticationResult.SetSuccessfulOperation($"The account with DN: {credential.DistinguishedName} has been successfully authenticated.");
                else
                    authenticationResult.SetSuccessfulOperation("Authentication failed: incorrect password.");

                return authenticationResult;
            }
            catch (Exception ex) {
                return new LDAPDistinguishedNameAuthenticationResult(credential.SecureClone(), $"Failed to authenticate {credential.DistinguishedName}", ex, requestLabel);
            }
        }

        /// <summary>
        /// Authenticates a distinguished-name account directly against the LDAP server.
        /// </summary>
        /// <param name="credential">Credential to authenticate.</param>
        /// <param name="requestLabel">Optional label used to correlate request/response operations.</param>
        /// <returns>
        /// A result object containing operation status, authentication status, and optional error details.
        /// </returns>
        public async Task<LDAPDistinguishedNameAuthenticationResult> AuthenticateAsync(LDAPDistinguishedNameCredential credential, string requestLabel = null) {
            try {
                bool? authenticated;
                using (var connection = await GetLdapConnection(this.ConnectionInfo, credential, false)) {
                    if (connection.IsBound)
                        authenticated = true;
                    else
                        authenticated = false;
                }

                var result = new LDAPDistinguishedNameAuthenticationResult(credential.SecureClone(), authenticated.Value, requestLabel);

                if (authenticated.Value)
                    result.SetSuccessfulOperation($"The account with DN: {credential.DistinguishedName} has been successfully authenticated.");
                else
                    result.SetSuccessfulOperation("Authentication failed: incorrect username or password.");

                return result;
            }
            catch (Exception ex) {
                return new LDAPDistinguishedNameAuthenticationResult(credential.SecureClone(), $"Failed to authenticate {credential.DistinguishedName}", ex, requestLabel);
            }
        }
        #endregion
    }
}
