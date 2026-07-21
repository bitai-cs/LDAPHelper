using Bitai.LDAPHelper.DTO;

namespace Bitai.LDAPHelper.QueryFilters
{
    /// <summary>
    /// Represents a single LDAP attribute filter expression.
    /// </summary>
    public class AttributeFilter : ICombinableFilter
    {
        private string _generatedFilterText = null;


        /// <summary>
        /// Gets the target LDAP attribute.
        /// </summary>
        public EntryAttribute FilterAttribute { get; }

        /// <summary>
        /// Gets the filter value.
        /// </summary>
        public FilterValue FilterValue { get; }

        /// <summary>
        /// Gets a value indicating whether this filter is negated.
        /// </summary>
        public bool IsFilterNegated { get; }

        /// <summary>
        /// Gets a value indicating whether the LDAP text has already been generated.
        /// </summary>
        public bool Generated { get; private set; }


        /// <summary>
        /// Initializes a new non-negated attribute filter.
        /// </summary>
        /// <param name="filterAttribute">Target LDAP attribute.</param>
        /// <param name="filterValue">Value for the attribute comparison.</param>
        public AttributeFilter(DTO.EntryAttribute filterAttribute, FilterValue filterValue)
        {
            FilterAttribute = filterAttribute;
            FilterValue = filterValue;
            IsFilterNegated = false;
        }

        /// <summary>
        /// Initializes a new attribute filter.
        /// </summary>
        /// <param name="isFilterNegated">Whether the generated filter is negated.</param>
        /// <param name="filterAttribute">Target LDAP attribute.</param>
        /// <param name="filterValue">Value for the attribute comparison.</param>
        public AttributeFilter(bool isFilterNegated, DTO.EntryAttribute filterAttribute, FilterValue filterValue) : this(filterAttribute, filterValue)
        {
            FilterAttribute = filterAttribute;
            FilterValue = filterValue;
            IsFilterNegated = isFilterNegated;
        }


        /// <summary>
        /// Generates the LDAP filter text for this instance.
        /// </summary>
        /// <returns>LDAP filter expression.</returns>
        public override string ToString()
        {
            if (Generated)
                return this._generatedFilterText;

            const string negatedFilterFormat = "(!{0})";
            const string filterFormat = "({0}={1})";

            _generatedFilterText = string.Format(filterFormat, FilterAttribute, FilterValue);

            if (IsFilterNegated)
                _generatedFilterText = string.Format(negatedFilterFormat, _generatedFilterText);

            Generated = true;

            return _generatedFilterText;
        }

        /// <summary>
        /// Resets generated-state so the filter text is rebuilt on next call.
        /// </summary>
        public void Reset()
        {
            _generatedFilterText = null;

            Generated = false;
        }
    }
}
