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

		public AccountManager(ConnectionInfo connectionInfo, SearchLimits searchLimits, DomainAccountCredential domainAccountCredential) : base(connectionInfo, searchLimits, domainAccountCredential)
		{
		}
		#endregion



		public async Task<LDAPPasswordUpdateResult> SetAccountPassword(DistinguishedNameCredential credential, string requestTag = null, bool postUpdateTestAuthentication = true)
		{
			try
			{
				if (string.IsNullOrEmpty(credential.Password))
					throw new InvalidOperationException($"The password to be assigned to the {credential.DistinguishedName} account is required.");

				////Create search filter
				//var onlyUsersFilterCombiner = QueryFilters.AttributeFilterCombiner.CreateOnlyUsersFilterCombiner();
				//var attributeFilter = new QueryFilters.AttributeFilter(DTO.EntryAttribute.sAMAccountName, new QueryFilters.FilterValue(credential.DistinguishedName));
				//var searchFilterCombiner = new QueryFilters.AttributeFilterCombiner(false, true, new List<QueryFilters.ICombinableFilter> { onlyUsersFilterCombiner, attributeFilter });

				//var searcher = new Searcher(ConnectionInfo, SearchLimits, DomainAccountCredential);
				//var searchResult = await searcher.SearchEntriesAsync(searchFilterCombiner, DTO.RequiredEntryAttributes.Minimun, null);
				////Validate search result
				//if (searchResult.HasErrorInfo)
				//	throw searchResult.ErrorObject;
				//else if (searchResult.Entries.Count() == 0)
				//	throw new EntryNotFoundException($"{credential.DistinguishedName} not found.");
				//else if (searchResult.Entries.Count() > 1)
				//	throw new DuplicateNameException($"Multiple entries found for account identifier {credential.DistinguishedName}");
				////Get the only one entry
				//var ldapEntry = searchResult.Entries.Single();

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
						var authenticated = await authenticator.AuthenticateAsync(credential.DistinguishedName, credential.Password);

						if (!authenticated)
							return new LDAPPasswordUpdateResult(requestTag, $"Could not set password for {credential.DistinguishedName} distinguished name.", false);
					}
				}

				return new LDAPPasswordUpdateResult(requestTag, $"Password set successfully for {credential.DistinguishedName}");
			}
			catch(Exception ex)
			{
				return new LDAPPasswordUpdateResult("Unexpected error trying to replace password.", ex, requestTag);
			}
		}
	}
}
