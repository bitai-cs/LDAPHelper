using Bitai.LDAPHelper.DTO;
using System.Collections.Generic;
using System.Text;

namespace Bitai.LDAPHelper.QueryFilters
{
    /// <summary>
    /// Combines multiple LDAP filter components into a single LDAP expression.
    /// </summary>
    public class AttributeFilterCombiner : List<ICombinableFilter>, ICombinableFilter
    {
        #region Static methods
        /// <summary>
        /// Creates a filter combiner that targets user entries and excludes computers and groups.
        /// </summary>
        /// <returns>A composed LDAP filter combiner.</returns>
        public static AttributeFilterCombiner CreateOnlyUsersFilterCombiner()
        {
            var noComputerFilter = new QueryFilters.AttributeFilter(true, EntryAttribute.objectClass, new QueryFilters.FilterValue("computer"));

            var noGroupFilter = new QueryFilters.AttributeFilter(true, EntryAttribute.objectClass, new QueryFilters.FilterValue("group"));

            var userFilter = new QueryFilters.AttributeFilter(false, EntryAttribute.objectClass, new QueryFilters.FilterValue("user"));

            return new QueryFilters.AttributeFilterCombiner(false, true, new List<QueryFilters.AttributeFilter> { noComputerFilter, noGroupFilter, userFilter });
        }

        /// <summary>
        /// Creates a filter combiner that targets group entries and excludes users and computers.
        /// </summary>
        /// <returns>A composed LDAP filter combiner.</returns>
        public static AttributeFilterCombiner CreateOnlyGroupsFilterCombiner()  
        {
            var noComputerFilter = new QueryFilters.AttributeFilter(true, EntryAttribute.objectClass, new QueryFilters.FilterValue("computer"));

            var noGroupFilter = new QueryFilters.AttributeFilter(false, EntryAttribute.objectClass, new QueryFilters.FilterValue("group"));

            var userFilter = new QueryFilters.AttributeFilter(true, EntryAttribute.objectClass, new QueryFilters.FilterValue("user"));

            return new QueryFilters.AttributeFilterCombiner(false, true, new List<QueryFilters.AttributeFilter> { noComputerFilter, noGroupFilter, userFilter });
        }
        #endregion


        private string _generatedFilterText = null;


        /// <summary>
        /// Gets or sets a value indicating whether the combined expression is negated.
        /// </summary>
        public bool IsCombinerNegated { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether filters are combined conjunctively (AND) or disjunctively (OR).
        /// </summary>
        public bool ConjunctiveFilters { get; set; }

        /// <summary>
        /// Gets a value indicating whether the LDAP text has already been generated.
        /// </summary>
        public bool Generated { get; private set; }


        /// <summary>
        /// Initializes a new non-negated conjunctive combiner.
        /// </summary>
        public AttributeFilterCombiner() : base()
        {
            IsCombinerNegated = false;
            ConjunctiveFilters = true;
        }

        /// <summary>
        /// Initializes a new combiner.
        /// </summary>
        /// <param name="isCombinerNegated">Whether the full expression is negated.</param>
        /// <param name="conjunctiveFilters"><see langword="true"/> for AND-combination; <see langword="false"/> for OR-combination.</param>
        public AttributeFilterCombiner(bool isCombinerNegated, bool conjunctiveFilters) : this()
        {
            IsCombinerNegated = isCombinerNegated;
            ConjunctiveFilters = conjunctiveFilters;
        }

        /// <summary>
        /// Initializes a new combiner with initial filter components.
        /// </summary>
        /// <param name="isCombinerNegated">Whether the full expression is negated.</param>
        /// <param name="conjunctiveFilters"><see langword="true"/> for AND-combination; <see langword="false"/> for OR-combination.</param>
        /// <param name="filters">Initial filter components.</param>
        public AttributeFilterCombiner(bool isCombinerNegated, bool conjunctiveFilters, IEnumerable<ICombinableFilter> filters) : this(isCombinerNegated, conjunctiveFilters)
        {
            AddRange(filters);
        }


        /// <summary>
        /// Generates LDAP filter text for all contained filter components.
        /// </summary>
        /// <returns>LDAP filter expression.</returns>
        public override string ToString()
        {
            if (Generated)
                return _generatedFilterText;

            const string negatedFormat = "(!{0})";
            const string baseFormat = "({0}{1})";

            if (this.Count == 1)
            {
                if (IsCombinerNegated)
                {
                    _generatedFilterText = string.Format(negatedFormat, this[0].ToString());
                    Generated = true;
                    return _generatedFilterText;
                }
                else
                {
                    _generatedFilterText = this[0].ToString();
                    Generated = true;
                    return _generatedFilterText;
                }
            }
            else
            {
                var sb = new StringBuilder();
                foreach (var combinable in this)
                {
                    sb.Append(combinable.ToString());
                }

                _generatedFilterText = string.Format(baseFormat, ConjunctiveFilters ? "&" : "|", sb.ToString());

                if (IsCombinerNegated)
                    _generatedFilterText = string.Format(negatedFormat, _generatedFilterText);

                Generated = true;

                return _generatedFilterText;
            }
        }

        /// <summary>
        /// Resets generated-state so filter text is rebuilt on next call.
        /// </summary>
        public void Reset()
        {
            _generatedFilterText = null;

            Generated = false;
        }
    }
}
