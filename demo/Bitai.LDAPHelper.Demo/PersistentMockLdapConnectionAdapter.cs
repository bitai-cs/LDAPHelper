using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bitai.LDAPHelper.Adapters;
using Bitai.LDAPHelper.Tests.Mocks;
using Microsoft.VisualStudio.TestPlatform.Common.DataCollection;

namespace Bitai.LDAPHelper.Demo
{
    public class PersistentMockLdapConnectionAdapter : MockLdapConnectionAdapter
    {
        private readonly MockDataStore _dataStore;

        public PersistentMockLdapConnectionAdapter() : base()
        {
            _dataStore = MockDataStore.Instance;
        }

        public override Task AddEntryAsync(string distinguishedName, ILdapAttributeSetAdapter attributes)
        {
            var mockAttributes = (MockLdapAttributeSetAdapter)attributes;
            var entry = new MockLdapEntryAdapter(distinguishedName);

            // Copy all attributes
            var allAttributes = mockAttributes.GetAllAttributes();
            foreach (var attr in allAttributes)
            {
                var value = attr.Value.StringValue;
                var arrayValue = attr.Value.StringValueArray;
                var byteValue = attr.Value.ByteValue;

                if (byteValue is byte[] bytes)
                    entry.AddAttribute(attr.Key, bytes);
                else if (arrayValue != null && arrayValue.Length > 1)
                    entry.AddAttribute(attr.Key, arrayValue);
                else if (value != null)
                    entry.AddAttribute(attr.Key, value);
            }

            _dataStore.AddOrUpdateEntry(entry);
            
            // Also add to search results
            AddSearchResult($"({entry.DistinguishedName})", new List<MockLdapEntryAdapter> { entry });
            
            return Task.CompletedTask;
        }

        public override Task ModifyEntryAsync(string distinguishedName, IEnumerable<ILdapModificationAdapter> modifications)
        {
            var entry = _dataStore.GetEntry(distinguishedName);
            if (entry == null)
                throw new Exception($"Entry {distinguishedName} not found");

            var entryAttrSet = (MockLdapAttributeSetAdapter)entry.GetAttributeSet();

            foreach (var mod in modifications)
            {
                var mockMod = mod as MockLdapModificationAdapter;
                if (mockMod != null)
                {
                    MockLdapAttributeAdapter attr;
                    attr = (MockLdapAttributeAdapter)entry.GetAttributeSet().GetAttribute(mockMod.AttributeName);

                    if (mockMod.ModificationType == LdapModificationType.Add) {
                        if (attr == null) {
                            entry.AddAttribute(mockMod.AttributeName, mockMod.Value);
                        }
                        else {
                            attr.AddValue(mockMod.Value);
                        }
                    }
                    else if (mockMod.ModificationType == LdapModificationType.Delete) {
                        if (attr != null) {
                            if (mockMod.Value is string stringValue) {
                                attr.RemoveValue(stringValue);
                            }
                            else if (mockMod.Value is string[] stringArray) {
                                foreach (var val in stringArray)
                                    attr.RemoveValue(val);
                            }
                            else if (mockMod.Value is byte[] byteValue) {
                                attr.RemoveValue(byteValue);
                            }
                            else 
                                throw new InvalidOperationException($"Unsupported value type for delete modification: {mockMod.Value?.GetType().Name}");

                            if (attr.ValueCount == 0) {
                                entryAttrSet.RemoveAttribute(mockMod.AttributeName);
                            }
                        }
                    }
                    else if (mockMod.ModificationType == LdapModificationType.Replace) {
                        if (attr != null) {
                            if (mockMod.Value != null) {
                                // Remove existing attribute before adding new value(s)
                                entryAttrSet.RemoveAttribute(mockMod.AttributeName);

                                // Add new value(s) if not null or empty
                                if (mockMod.Value is byte[] bytes) {
                                    if (bytes.Length > 0) {
                                        entry.AddAttribute(mockMod.AttributeName, bytes);
                                        continue;
                                    }
                                }
                                else if (mockMod.Value is string[] stringArray) {
                                    if (stringArray.Length > 0) {
                                        entry.AddAttribute(mockMod.AttributeName, stringArray);
                                        continue;
                                    }
                                }
                                else if (mockMod.Value is string stringValue) {
                                    if (stringValue.Length > 0) {
                                        entry.AddAttribute(mockMod.AttributeName, stringValue);
                                        continue;
                                    }
                                }
                                else
                                    throw new InvalidOperationException($"Unsupported value type for replace modification: {mockMod.Value.GetType().Name}");
                            }
                            else { //Attribute EXISTS and Mod.Value is NULL!
                                // Remove existing attribute since replace with null means delete all values
                                entryAttrSet.RemoveAttribute(mockMod.AttributeName);
                            }
                        }
                        else {
                            // Attribute DOESN'T EXIST ignored if the attribute does not exist.
                        }
                    }
                    
                    Modifications.Add(new MockModification
                    {
                        DistinguishedName = distinguishedName,
                        ModificationType = mockMod.ModificationType,
                        AttributeName = mockMod.AttributeName,
                        Value = mockMod.Value
                    });
                }
            }

            _dataStore.AddOrUpdateEntry(entry);

            return Task.CompletedTask;
        }

        public override Task DeleteEntryAsync(string distinguishedName)
        {
            if (_dataStore.RemoveEntry(distinguishedName))
            {
                DeletedEntries.Add(distinguishedName);
            }

            return Task.CompletedTask;
        }

        public override Task<ILdapSearchQueueAdapter> SearchAsync(
            string searchBase,
            int searchScope,
            string searchFilter,
            string[] attributeNames,
            bool typesOnly,
            ILdapSearchConstraintsAdapter constraints)
        {
            // Search in persistent store
            var allEntries = _dataStore.GetAllEntries();
            var matchingEntries = new List<MockLdapEntryAdapter>();

            foreach (var entry in allEntries)
            {
                // Simple filter matching (supports wildcards)
                if (MatchesFilter(entry, searchFilter))
                {
                    matchingEntries.Add(entry);
                }
            }

            var mockQueue = new MockLdapSearchQueueAdapter();
            foreach (var entry in matchingEntries)
            {
                mockQueue.AddSearchResult(new MockLdapMessageAdapter(entry));
            }

            return Task.FromResult<ILdapSearchQueueAdapter>(mockQueue);
        }

        private bool MatchesFilter(MockLdapEntryAdapter entry, string filter)
        {
            // Simple wildcard matching for demo purposes
            if (filter.Contains("(sAMAccountName="))
            {
                var value = ExtractFilterValue(filter, "sAMAccountName");
                var samAccountName = entry.GetAttributeSet().GetAttribute("sAMAccountName")?.StringValue;
                return MatchWildcard(samAccountName, value);
            }
            else if (filter.Contains("(distinguishedName="))
            {
                var value = ExtractFilterValue(filter, "distinguishedName");
                return MatchWildcard(entry.DistinguishedName, value);
            }
            else if (filter.Contains("(cn="))
            {
                var value = ExtractFilterValue(filter, "cn");
                var cn = entry.GetAttributeSet().GetAttribute("cn")?.StringValue;
                return MatchWildcard(cn, value);
            }
            
            // Default - return true for complex filters in demo
            return true;
        }

        private string ExtractFilterValue(string filter, string attributeName)
        {
            var pattern = $"({attributeName}=";
            var start = filter.IndexOf(pattern) + pattern.Length;
            var end = filter.IndexOf(")", start);
            return filter.Substring(start, end - start);
        }

        private bool MatchWildcard(string value, string pattern)
        {
            if (string.IsNullOrEmpty(value) || string.IsNullOrEmpty(pattern))
                return false;

            if (pattern.Contains("*"))
            {
                var parts = pattern.Split('*');
                int index = 0;
                foreach (var part in parts)
                {
                    if (!string.IsNullOrEmpty(part))
                    {
                        index = value.IndexOf(part, index, StringComparison.OrdinalIgnoreCase);
                        if (index == -1) return false;
                        index += part.Length;
                    }
                }
                return true;
            }
            
            return value.Equals(pattern, StringComparison.OrdinalIgnoreCase);
        }
    }
}