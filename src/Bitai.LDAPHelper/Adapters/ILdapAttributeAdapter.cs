// ILdapAttributeAdapter.cs
namespace Bitai.LDAPHelper.Adapters
{
    /// <summary>
    /// Target interface for LDAP attribute operations
    /// </summary>
    public interface ILdapAttributeAdapter
    {
        object ByteValue { get; }
        string StringValue { get; }
        string[] StringValueArray { get; }
    }
}