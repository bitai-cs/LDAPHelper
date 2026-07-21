namespace Bitai.LDAPHelper.QueryFilters
{
    /// <summary>
    /// Defines a filter component that can generate LDAP filter text and be combined with others.
    /// </summary>
    public interface ICombinableFilter
    {
        /// <summary>
        /// Gets a value indicating whether the filter text has already been generated.
        /// </summary>
        bool Generated { get; }

        /// <summary>
        /// Resets generated-state so filter text is rebuilt on next <see cref="ToString"/> call.
        /// </summary>
        void Reset();

        /// <summary>
        /// Generates LDAP filter text.
        /// </summary>
        /// <returns>LDAP filter expression.</returns>
        string ToString();
    }
}
