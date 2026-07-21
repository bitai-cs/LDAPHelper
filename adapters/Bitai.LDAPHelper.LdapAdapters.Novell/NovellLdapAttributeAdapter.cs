using Novell.Directory.Ldap;

namespace Bitai.LDAPHelper.LdapAdapters.Novell;

/// <summary>
/// Adapter for <see cref="LdapAttribute"/>.
/// </summary>
public class NovellLdapAttributeAdapter : ILdapAttributeAdapter
{
    private readonly LdapAttribute _attribute;

    /// <summary>
    /// Initializes a new instance of the <see cref="NovellLdapAttributeAdapter"/> class.
    /// </summary>
    /// <param name="attribute">Wrapped Novell LDAP attribute.</param>
    public NovellLdapAttributeAdapter(LdapAttribute attribute) {
        _attribute = attribute;
    }

    /// <inheritdoc/>
    public byte[] ByteValue => _attribute.ByteValue;

    /// <inheritdoc/>
    public string StringValue => _attribute.StringValue;

    /// <inheritdoc/>
    public string[] StringValueArray => _attribute.StringValueArray;

    /// <inheritdoc/>
    public byte[][] ByteValueArray => _attribute.ByteValueArray;
}
