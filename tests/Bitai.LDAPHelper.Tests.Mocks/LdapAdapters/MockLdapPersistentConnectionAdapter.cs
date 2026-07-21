using System.Text;
using Bitai.LDAPHelper.LdapAdapters;
using Bitai.LDAPHelper.Tests.Mocks.LdapData;

namespace Bitai.LDAPHelper.Tests.Mocks.LdapAdapters;

/// <summary>
/// Persistent mock connection backed by a shared in-memory LDAP data store.
/// </summary>
public class MockLdapPersistentConnectionAdapter : MockLdapConnectionAdapter
{
    private readonly MockLdapDataStore _dataStore;

    public MockLdapPersistentConnectionAdapter() : base()
    {
        _dataStore = MockLdapDataStore.Instance;
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
                    // For replace, we remove the existing attribute and add the new value(s)
                    entryAttrSet.RemoveAttribute(mockMod.AttributeName);

                    if (mockMod.Value == null)
                        continue;

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
        ISearchLimits searchLimits,
        string searchFilter,
        string[] attributeNames,
        bool typesOnly)
    {
        // Search in persistent store
        var allEntries = _dataStore.GetAllEntries();
        var matchingEntries = new List<MockLdapEntryAdapter>();

        foreach (var entry in allEntries)
        {
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
        else if (filter.Contains("(objectSid="))
        {
            var value = ExtractFilterValue(filter, "objectSid");
            var objectSidBytes = entry.GetAttributeSet().GetAttribute("objectSid")?.ByteValue;
            var objectSid = objectSidBytes == null ? null : ConvertByteToStringSid(objectSidBytes);
            return MatchWildcard(objectSid, value);
        }
        else if (filter.Contains("(objectClass="))
        {
            var value = ExtractFilterValue(filter, "objectClass");
            var objectClassValues = entry.GetAttributeSet().GetAttribute("objectClass")?.StringValueArray;
            if (objectClassValues == null)
                return false;

            foreach (var objectClass in objectClassValues) {
                if (MatchWildcard(objectClass, value))
                    return true;
            }

            return false;
        }
        
        return false;
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

    private static string ConvertByteToStringSid(byte[] sidBytes)
    {
        short subAuthorityCount = 0;
        var sid = new StringBuilder();
        sid.Append("S-");

        sid.Append(sidBytes[0].ToString());

        subAuthorityCount = Convert.ToInt16(sidBytes[1]);

        if (sidBytes[2] != 0 || sidBytes[3] != 0) {
            string authority = string.Format("0x{0:2x}{1:2x}{2:2x}{3:2x}{4:2x}{5:2x}",
                (short)sidBytes[2],
                (short)sidBytes[3],
                (short)sidBytes[4],
                (short)sidBytes[5],
                (short)sidBytes[6],
                (short)sidBytes[7]);
            sid.Append("-");
            sid.Append(authority);
        }
        else {
            long authority = sidBytes[7] +
                (sidBytes[6] << 8) +
                (sidBytes[5] << 16) +
                (sidBytes[4] << 24);
            sid.Append("-");
            sid.Append(authority.ToString());
        }

        for (int i = 0; i < subAuthorityCount; i++) {
            int offset = 8 + i * 4;
            uint subAuthority = BitConverter.ToUInt32(sidBytes, offset);
            sid.Append("-");
            sid.Append(subAuthority.ToString());
        }

        return sid.ToString();
    }
}
