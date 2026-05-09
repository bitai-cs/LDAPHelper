using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bitai.LDAPHelper.Adapters;
using Bitai.LDAPHelper.DTO;
using Bitai.LDAPHelper.QueryFilters;
using Serilog;

namespace Bitai.LDAPHelper.Demo
{
    public partial class Program
    {        
        #region Authenticator Demos        
        public static async Task Demo_Authenticator_Authenticate_Simple(DemoContext context, string domainAccountCredential)
        {
            printDemoTitle(nameof(Demo_Authenticator_Authenticate_Simple));

            Log.Information($"To authenticate {domainAccountCredential} you need to enter a password (it's not necessary the real password).");

            var accountPassword = requestAccountPassword(domainAccountCredential);

            var authenticator = new Authenticator(context.GetConnectionInfo(), context.ConnectionFactory);

            var domainAccountCredentialParts = domainAccountCredential.Split(new char[] { '\\' });

            Log.Information("Authenticating account name...");
            var authenticationResult = await authenticator.AuthenticateAsync(
                new LDAPDomainAccountCredential(domainAccountCredentialParts[0], domainAccountCredentialParts[1], accountPassword), 
                context.RequestLabel);
            
            if (authenticationResult.IsSuccessfulOperation)
            {
                Log.Information("{@model}", authenticationResult);

                if (authenticationResult.IsAuthenticated)
                    Log.Information("Account name authenticated.");
                else
                    Log.Warning("Account name NOT authenticated.");
            }
            else
            {
                if (authenticationResult.HasErrorObject)
                    throw new Exception(authenticationResult.OperationMessage, authenticationResult.ErrorObject);
                else
                    throw new Exception(authenticationResult.OperationMessage);
            }
        }

        public static async Task Demo_Authenticator_Authenticate_WithAccountValidation(DemoContext context, string domainAccountCredential)
        {
            printDemoTitle(nameof(Demo_Authenticator_Authenticate_WithAccountValidation));

            Log.Information($"To authenticate {domainAccountCredential} you need to enter a password (it's not necessary the real password).");

            var accountPassword = requestAccountPassword(domainAccountCredential);

            var authenticator = new Authenticator(context.GetConnectionInfo(), context.ConnectionFactory);

            var domainAccountCredentialParts = domainAccountCredential.Split(new char[] { '\\' });

            Log.Information("Authenticating account name with account validation...");
            var authenticationResult = await authenticator.AuthenticateAsync(
                new LDAPDomainAccountCredential(domainAccountCredentialParts[0], domainAccountCredentialParts[1], accountPassword),
                context.GetSearchLimits(),
                context.GetDomainAccountCredential(),
                context.RequestLabel);
            
            if (authenticationResult.IsSuccessfulOperation)
            {
                Log.Information("{@model}", authenticationResult);

                if (authenticationResult.IsAuthenticated)
                    Log.Information("Account name authenticated.");
                else
                    Log.Warning("Account name NOT authenticated.");
            }
            else
            {
                if (authenticationResult.HasErrorObject)
                    throw new Exception(authenticationResult.OperationMessage, authenticationResult.ErrorObject);
                else
                    throw new Exception(authenticationResult.OperationMessage);
            }
        }
        #endregion

        #region Account Manager Demos
        public static async Task Demo_AccountManager_CreateUserAccount(
            DemoContext context,
            string userAccountName,
            string password,
            string distinguishedNameOfContainer,
            string name,
            string surName,
            string dnsDomainName,
            string[] memberOf,
            string[] objectClasses,
            string userAccountControlFlags)
        {
            printDemoTitle(nameof(Demo_AccountManager_CreateUserAccount));

            string fullName = $"{name} {surName}";
            var newUserAccount = new LDAPMsADUserAccount(distinguishedNameOfContainer)
            {
                GivenName = name,
                Sn = surName,
                Cn = fullName,
                Name = fullName,
                DisplayName = fullName,
                MemberOf = memberOf,
                ObjectClass = objectClasses,
                Password = password,
                SAMAccountName = userAccountName,
                UserAccountControl = userAccountControlFlags,
                UserPrincipalName = $"{userAccountName}@{dnsDomainName}"
            };
            
            Log.Information($"New user account data:");
            Log.Information("{@newUserAccount}", newUserAccount);

            var accountManager = new AccountManager(context.GetClientConfiguration(), context.ConnectionFactory);

            Log.Information("Creating new user account...");
            var result = await accountManager.CreateUserAccountForMsAD(newUserAccount, context.RequestLabel);

            if (result.IsSuccessfulOperation)
            {
                Log.Information("Operation completed.");
                Log.Information("{@result}", result);
            }
            else
            {
                Log.Error("Unsuccessful operation!");
                Log.Error("{@result}", result);
                throw new Exception(result.OperationMessage);
            }
        }

        public static async Task Demo_AccountManager_SetPassword(DemoContext context, string distinguishedName)
        {
            printDemoTitle(nameof(Demo_AccountManager_SetPassword));

            Log.Information($"Enter new password for {distinguishedName}");

            var password = requestAccountPassword(distinguishedName);
            var credential = new LDAPDistinguishedNameCredential(distinguishedName, password);
            var accountManager = new AccountManager(context.GetClientConfiguration(), context.ConnectionFactory);

            Log.Information("Setting account password...");
            var result = await accountManager.SetUserAccountPasswordForMsAD(credential, context.RequestLabel);

            if (result.IsSuccessfulOperation)
            {
                Log.Information(result.OperationMessage);
            }
            else
            {
                Log.Error(result.OperationMessage);
                if (result.HasErrorObject)
                {
                    Log.Error(result.ErrorObject.Message);
                    Log.Error(result.ErrorObject.StackTrace);
                }
                throw new Exception(result.OperationMessage);
            }
        }

        public static async Task Demo_AccountManager_DisableUserAccount(DemoContext context, string distinguishedName)
        {
            printDemoTitle(nameof(Demo_AccountManager_DisableUserAccount));

            var accountManager = new AccountManager(context.GetClientConfiguration(), context.ConnectionFactory);

            Log.Information("Disabling user account {dn}", distinguishedName);
            var result = await accountManager.DisableUserAccountForMsAD(distinguishedName, context.RequestLabel);

            if (result.IsSuccessfulOperation)
            {
                Log.Information(result.OperationMessage);
                Log.Information("{@r}", result);
            }
            else
            {
                Log.Error(result.OperationMessage);
                Log.Error("{@r}", result);
                throw new Exception(result.OperationMessage, result.ErrorObject);
            }
        }

        public static async Task Demo_AccountManager_RemoveUserAccount(DemoContext context, string distinguishedName)
        {
            printDemoTitle(nameof(Demo_AccountManager_RemoveUserAccount));

            var accountManager = new AccountManager(context.GetClientConfiguration(), context.ConnectionFactory);

            Log.Information("Removing user account {dn}", distinguishedName);
            var result = await accountManager.RemoveUserAccountForMsAD(distinguishedName, context.RequestLabel);

            if (result.IsSuccessfulOperation)
            {
                Log.Information(result.OperationMessage);
                Log.Information("{@r}", result);
            }
            else
            {
                Log.Error(result.OperationMessage);
                Log.Error("{@r}", result);
                throw new Exception(result.OperationMessage, result.ErrorObject);
            }
        }
        #endregion

        #region Searcher Demos

        public static async Task Demo_Searcher_SearchUsers(
            DemoContext context, 
            EntryAttribute filterAttribute, 
            string filterValue, 
            RequiredEntryAttributes requiredEntryAttributes)
        {
            printDemoTitle("Demo_Searcher_SearchUsers");

            // Create search filter
            var onlyUsersFilterCombiner = AttributeFilterCombiner.CreateOnlyUsersFilterCombiner();
            var attributeFilter = new AttributeFilter(filterAttribute, new FilterValue(filterValue));
            var searchFilterCombiner = new AttributeFilterCombiner(false, true, new List<ICombinableFilter> { onlyUsersFilterCombiner, attributeFilter });

            var searcher = new Searcher(context.GetClientConfiguration(), context.ConnectionFactory);

            Log.Information($"Base DN: {searcher.SearchLimits.BaseDN}");
            Log.Information($"Searching by {searchFilterCombiner}");
            Console.WriteLine();

            var searchResult = await searcher.SearchEntriesAsync(searchFilterCombiner, requiredEntryAttributes, context.RequestLabel);
            
            if (searchResult.IsSuccessfulOperation)
            {
                foreach (var entry in searchResult.Entries)
                {
                    Log.Information(entry.company);
                    Log.Information(entry.co);
                    Log.Information(entry.samAccountName);
                    Log.Information(entry.cn);
                    Log.Information(entry.displayName);
                    Log.Information(entry.distinguishedName);
                    Log.Information(entry.objectSid);
                    Log.Information(entry.userAccountControl);

                    if (entry.memberOfEntries != null && entry.memberOfEntries.Count() > 0)
                        Console.WriteLine();

                    printMemberOf(entry);

                    Console.WriteLine();
                }

                Log.Information($"{searchResult.Entries.Count()} entries found.");
            }
            else
            {
                if (searchResult.HasErrorObject)
                    throw new Exception(searchResult.OperationMessage, searchResult.ErrorObject);
                else
                    throw new Exception(searchResult.OperationMessage);
            }
        }

        public static async Task Demo_Searcher_SearchUsersByTwoFilters(
            DemoContext context,
            EntryAttribute filterAttribute, 
            string filterValue, 
            EntryAttribute secondFilterAttribute, 
            string secondFilterValue, 
            bool conjunctiveFilters, 
            RequiredEntryAttributes requiredEntryAttributes)
        {
            printDemoTitle("Demo_Searcher_SearchUsersByTwoFilters");

            // Create search filter
            var onlyUsersFilterCombiner = AttributeFilterCombiner.CreateOnlyUsersFilterCombiner();
            var attributeFilter1 = new AttributeFilter(filterAttribute, new FilterValue(filterValue));
            var attributeFilter2 = new AttributeFilter(secondFilterAttribute, new FilterValue(secondFilterValue));
            var filter1Filter2Combiner = new AttributeFilterCombiner(false, conjunctiveFilters, new List<ICombinableFilter> { attributeFilter1, attributeFilter2 });
            var searchFilterCombiner = new AttributeFilterCombiner(false, true, new List<ICombinableFilter> { onlyUsersFilterCombiner, filter1Filter2Combiner });

            var searcher = new Searcher(context.GetClientConfiguration(), context.ConnectionFactory);

            Log.Information($"Base DN: {searcher.SearchLimits.BaseDN}");
            Log.Information($"Searching by {searchFilterCombiner}");
            Console.WriteLine();

            var searchResult = await searcher.SearchEntriesAsync(searchFilterCombiner, requiredEntryAttributes, context.RequestLabel);
            
            if (searchResult.IsSuccessfulOperation)
            {
                foreach (var entry in searchResult.Entries)
                {
                    Log.Information(entry.company);
                    Log.Information(entry.co);
                    Log.Information(entry.samAccountName);
                    Log.Information(entry.cn);
                    Log.Information(entry.displayName);
                    Log.Information(entry.distinguishedName);
                    Log.Information(entry.objectSid);
                    Log.Information(entry.userAccountControl);

                    Console.WriteLine();
                }

                Log.Information($"{searchResult.Entries.Count()} entries found.");
            }
            else
            {
                if (searchResult.HasErrorObject)
                    throw new Exception(searchResult.OperationMessage, searchResult.ErrorObject);
                else
                    throw new Exception(searchResult.OperationMessage);
            }
        }

        public static async Task Demo_Searcher_SearchEntries(
            DemoContext context,
            EntryAttribute filterAttribute, 
            string filterValue, 
            bool filterValueNegated, 
            RequiredEntryAttributes requiredEntryAttributes)
        {
            printDemoTitle("Demo_Searcher_SearchEntries");

            // Create filter
            var filter = new AttributeFilter(filterValueNegated, filterAttribute, new FilterValue(filterValue));

            var searcher = new Searcher(context.GetClientConfiguration(), context.ConnectionFactory);

            Log.Information($"Base DN: {searcher.SearchLimits.BaseDN}");
            Log.Information($"Searching by {filter}");
            Console.WriteLine();

            var searchResult = await searcher.SearchEntriesAsync(filter, requiredEntryAttributes, context.RequestLabel);
            
            if (searchResult.IsSuccessfulOperation)
            {
                foreach (var entry in searchResult.Entries)
                {
                    Log.Information(entry.company);
                    Log.Information(entry.co);
                    Log.Information(entry.samAccountName);
                    Log.Information(entry.cn);
                    Log.Information(entry.displayName);
                    Log.Information(entry.distinguishedName);
                    Log.Information(entry.objectSid);
                    Log.Information(entry.userAccountControl);

                    Console.WriteLine();
                }

                Log.Information($"{searchResult.Entries.Count()} entries found.");
            }
            else
            {
                if (searchResult.HasErrorObject)
                    throw new Exception(searchResult.OperationMessage, searchResult.ErrorObject);
                else
                    throw new Exception(searchResult.OperationMessage);
            }
        }

        public static async Task Demo_Searcher_SearchEntriesByTwoFilters(
            DemoContext context,
            EntryAttribute filterAttribute, 
            string filterValue, 
            EntryAttribute secondFilterAttribute, 
            string secondFilterValue, 
            bool conjunctiveFilters, 
            RequiredEntryAttributes requiredEntryAttributes)
        {
            printDemoTitle("Demo_Searcher_SearchEntriesByTwoFilters");

            // Create filters
            var filter1 = new AttributeFilter(false, filterAttribute, new FilterValue(filterValue));
            var filter2 = new AttributeFilter(false, secondFilterAttribute, new FilterValue(secondFilterValue));
            var filterCombiner = new AttributeFilterCombiner(false, conjunctiveFilters, new List<ICombinableFilter> { filter1, filter2 });

            var searcher = new Searcher(context.GetClientConfiguration(), context.ConnectionFactory);

            Log.Information($"Base DN: {searcher.SearchLimits.BaseDN}");
            Log.Information($"Searching by {filterCombiner}");
            Console.WriteLine();

            var searchResult = await searcher.SearchEntriesAsync(filterCombiner, requiredEntryAttributes, context.RequestLabel);
            
            if (searchResult.IsSuccessfulOperation)
            {
                foreach (var entry in searchResult.Entries)
                {
                    Log.Information(entry.company);
                    Log.Information(entry.co);
                    Log.Information(entry.samAccountName);
                    Log.Information(entry.cn);
                    Log.Information(entry.displayName);
                    Log.Information(entry.distinguishedName);
                    Log.Information(entry.objectSid);
                    Log.Information(entry.userAccountControl);

                    Console.WriteLine();
                }

                Log.Information($"{searchResult.Entries.Count()} entries found.");
            }
            else
            {
                if (searchResult.HasErrorObject)
                    throw new Exception(searchResult.OperationMessage, searchResult.ErrorObject);
                else
                    throw new Exception(searchResult.OperationMessage);
            }
        }

        public static async Task Demo_Searcher_SearchParentEntries(
            DemoContext context,
            EntryAttribute filterAttribute, 
            string filterValue, 
            RequiredEntryAttributes requiredEntryAttributes)
        {
            printDemoTitle("Demo_Searcher_SearchParentEntries");

            // Create search filter
            var attributeFilter = new AttributeFilter(filterAttribute, new FilterValue(filterValue));

            var searcher = new Searcher(context.GetClientConfiguration(), context.ConnectionFactory);

            Log.Information($"Base DN: {searcher.SearchLimits.BaseDN}");
            Log.Information($"Searching parent entries for {filterAttribute}: {filterValue}");
            Console.WriteLine();

            var searchResult = await searcher.SearchParentEntriesAsync(attributeFilter, requiredEntryAttributes, context.RequestLabel);
            
            if (searchResult.IsSuccessfulOperation)
            {
                foreach (var entry in searchResult.Entries)
                {
                    Log.Information(entry.company);
                    Log.Information(entry.co);
                    Log.Information(entry.samAccountName);
                    Log.Information(entry.cn);
                    Log.Information(entry.displayName);
                    Log.Information(entry.distinguishedName);
                    Log.Information(entry.objectSid);
                    Log.Information(entry.userAccountControl);

                    Console.WriteLine();
                }

                Log.Information($"{searchResult.Entries.Count()} entries found.");
            }
            else
            {
                if (searchResult.HasErrorObject)
                    throw new Exception(searchResult.OperationMessage, searchResult.ErrorObject);
                else
                    throw new Exception(searchResult.OperationMessage);
            }
        }

        #endregion

        #region Group Membership Validator Demos

        public static async Task Demo_GroupMembershipValidator_CheckGroupMembership(
            DemoContext context,
            string sAMAccountName, 
            string groupName)
        {
            printDemoTitle("Demo_GroupMembershipValidator_CheckGroupMembership");

            var validator = new GroupMembershipValidator(context.GetClientConfiguration(), context.ConnectionFactory);

            Log.Information($"Base DN: {validator.SearchLimits.BaseDN}");
            Log.Information($"Checking {groupName} membership for {sAMAccountName}");
            Console.WriteLine();

            var result = await validator.CheckGroupMembershipAsync(sAMAccountName, groupName);

            if (result)
                Log.Information($"{sAMAccountName} BELONGS to the group {groupName}.");
            else
                Log.Information($"{sAMAccountName} DOES NOT BELONG to the group {groupName}.");
        }

        #endregion
    }
}