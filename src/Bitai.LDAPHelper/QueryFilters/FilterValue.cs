using System;

namespace Bitai.LDAPHelper.QueryFilters
{
    /// <summary>
    /// Represents a validated LDAP filter value.
    /// </summary>
    public class FilterValue
    {
        /// <summary>
        /// Gets the raw value used in LDAP filters.
        /// </summary>
        public string Value { get; }


        /// <summary>
        /// Initializes a new instance of the <see cref="FilterValue"/> class.
        /// </summary>
        /// <param name="value">Filter value.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is null, empty, or whitespace.</exception>
        public FilterValue(string value)
        {
            if (string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value))
                throw new ArgumentNullException(nameof(value));

            Value = value;
        }


        /// <summary>
        /// Returns the filter value text.
        /// </summary>
        /// <returns>The raw filter value.</returns>
        public override string ToString()
        {
            return Value;
        }
    }
}
