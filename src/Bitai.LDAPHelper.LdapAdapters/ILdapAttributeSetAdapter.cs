namespace Bitai.LDAPHelper.LdapAdapters;

/// <summary>
/// Target interface for LDAP AttributeSet operations
/// </summary>
public interface ILdapAttributeSetAdapter
{
    void AddAttribute(string name, string value);
    void AddAttribute(string name, string[] values);
    void AddAttribute(string name, byte[] value);
    bool ContainsKey(string attributeName);
    ILdapAttributeAdapter GetAttribute(string attributeName);        
}