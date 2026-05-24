using Novell.Directory.Ldap;

namespace Bitai.LDAPHelper.LdapAdapters.Novell;

/// <summary>
/// Adapter that wraps a <see cref="LdapModification"/> instance and exposes it through the <see cref="ILdapModificationAdapter"/> interface.
/// </summary>
/// <remarks>
/// This adapter provides a lightweight wrapper around Novell.Directory.Ldap's <see cref="LdapModification"/>,
/// translating the native modification operation and attribute into the project's adapter abstractions.
/// </remarks>
public class NovellLdapModificationAdapter : ILdapModificationAdapter
{
    private readonly LdapModification _modification;

    /// <summary>
    /// Initializes a new instance of the <see cref="NovellLdapModificationAdapter"/> class.
    /// </summary>
    /// <param name="modification">The <see cref="LdapModification"/> instance to wrap. May be <c>null</c> only if callers handle it; this adapter does not perform null checks.</param>
    public NovellLdapModificationAdapter(LdapModification modification) {
        _modification = modification;
    }

    /// <summary>
    /// Gets the modification operation type mapped to the project's <see cref="LdapModificationType"/> enumeration.
    /// </summary>
    /// <remarks>
    /// The value is obtained from the wrapped <see cref="LdapModification.Op"/> and cast to <see cref="LdapModificationType"/>.
    /// </remarks>
    public LdapModificationType ModificationType => (LdapModificationType)_modification.Op;

    /// <summary>
    /// Gets an <see cref="ILdapAttributeAdapter"/> representing the attribute involved in this modification.
    /// </summary>
    /// <remarks>
    /// The returned adapter wraps the underlying <see cref="LdapAttribute"/> instance from the wrapped <see cref="LdapModification"/>.
    /// </remarks>
    public ILdapAttributeAdapter Attribute => new NovellLdapAttributeAdapter(_modification.Attribute);

    /// <summary>
    /// Returns the underlying <see cref="LdapModification"/> instance wrapped by this adapter.
    /// </summary>
    /// <returns>The original <see cref="LdapModification"/> object supplied to the adapter.</returns>
    public LdapModification GetWrappedModification() {
        return _modification;
    }
}