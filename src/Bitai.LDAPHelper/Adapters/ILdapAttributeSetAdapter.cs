// ILdapAttributeSetAdapter.cs
namespace Bitai.LDAPHelper.Adapters
{
    /// <summary>
    /// Target interface for LDAP attribute set operations
    /// </summary>
    public interface ILdapAttributeSetAdapter
    {
        bool ContainsKey(string attributeName);
        ILdapAttributeAdapter GetAttribute(string attributeName);
    }
}