// ILdapEntryAdapter.cs
namespace Bitai.LDAPHelper.Adapters
{
    /// <summary>
    /// Target interface for LDAP entry operations
    /// </summary>
    public interface ILdapEntryAdapter
    {
        string DistinguishedName { get; }
        ILdapAttributeSetAdapter GetAttributeSet();
    }
}