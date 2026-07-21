using Novell.Directory.Ldap;

namespace Bitai.LDAPHelper.LdapAdapters.Novell;

/// <summary>
/// Adapter for <see cref="LdapEntry"/>.
/// </summary>
public class NovellLdapEntryAdapter : ILdapEntryAdapter
{
    private readonly LdapEntry _entry;

    /// <summary>
    /// Initializes a new instance of the <see cref="NovellLdapEntryAdapter"/> class.
    /// </summary>
    /// <param name="entry">Wrapped Novell LDAP entry.</param>
    public NovellLdapEntryAdapter(LdapEntry entry) {
        _entry = entry;
    }

    /// <inheritdoc/>
    public string DistinguishedName => _entry.Dn;

    /// <inheritdoc/>
    public ILdapAttributeSetAdapter GetAttributeSet() {
        return new NovellLdapAttributeSetAdapter(_entry.GetAttributeSet());
    }
}
