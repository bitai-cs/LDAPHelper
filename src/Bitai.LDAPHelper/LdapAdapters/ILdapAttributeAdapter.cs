namespace Bitai.LDAPHelper.LdapAdapters; 

/// <summary>
/// Target interface for LDAP attribute operations
/// </summary>
public interface ILdapAttributeAdapter
{
    byte[] ByteValue { get; }
    string StringValue { get; }
    string[] StringValueArray { get; }
    public byte[][] ByteValueArray { get; }
}