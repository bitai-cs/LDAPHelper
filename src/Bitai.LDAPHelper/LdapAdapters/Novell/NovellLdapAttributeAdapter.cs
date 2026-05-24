using Novell.Directory.Ldap;

namespace Bitai.LDAPHelper.LdapAdapters.Novell;

/// <summary>
/// Adapter for Novell.Directory.Ldap.LdapAttribute
/// </summary>
public class NovellLdapAttributeAdapter : ILdapAttributeAdapter
{
    private readonly LdapAttribute _attribute;

    public NovellLdapAttributeAdapter(LdapAttribute attribute) {
        _attribute = attribute;
    }

    public byte[] ByteValue => _attribute.ByteValue;

    public string StringValue => _attribute.StringValue;

    public string[] StringValueArray => _attribute.StringValueArray;

    public byte[][] ByteValueArray => _attribute.ByteValueArray;
}