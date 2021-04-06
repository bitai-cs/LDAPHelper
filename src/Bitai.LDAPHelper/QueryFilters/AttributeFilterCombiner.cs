using Bitai.LDAPHelper.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bitai.LDAPHelper.QueryFilters
{
    public class AttributeFilterCombiner : List<ICombinableFilter>, ICombinableFilter
    {
        #region Static methods
        public static AttributeFilterCombiner CreateOnlyUsersFilterCombiner()
        {
            var noComputerFilter = new QueryFilters.AttributeFilter(true, EntryAttribute.objectClass, new QueryFilters.FilterValue("computer"));

            var noGroupFilter = new QueryFilters.AttributeFilter(true, EntryAttribute.objectClass, new QueryFilters.FilterValue("group"));

            var userFilter = new QueryFilters.AttributeFilter(false, EntryAttribute.objectClass, new QueryFilters.FilterValue("user"));

            return new QueryFilters.AttributeFilterCombiner(false, true, new List<QueryFilters.AttributeFilter> { noComputerFilter, noGroupFilter, userFilter });
        }

        public static AttributeFilterCombiner CreateOnlyGroupsFilterCombiner()  
        {
            var noComputerFilter = new QueryFilters.AttributeFilter(true, EntryAttribute.objectClass, new QueryFilters.FilterValue("computer"));

            var noGroupFilter = new QueryFilters.AttributeFilter(false, EntryAttribute.objectClass, new QueryFilters.FilterValue("group"));

            var userFilter = new QueryFilters.AttributeFilter(true, EntryAttribute.objectClass, new QueryFilters.FilterValue("user"));

            return new QueryFilters.AttributeFilterCombiner(false, true, new List<QueryFilters.AttributeFilter> { noComputerFilter, noGroupFilter, userFilter });
        }
        #endregion


        private string _generatedFilterText = null;


        public bool IsCombinerNegated { get; set; }
        public bool ConjunctiveFilters { get; set; }
        public bool Generated { get; private set; }


        public AttributeFilterCombiner() : base()
        {
            IsCombinerNegated = false;
            ConjunctiveFilters = true;
        }

        public AttributeFilterCombiner(bool isCombinerNegated, bool conjunctiveFilters) : this()
        {
            IsCombinerNegated = isCombinerNegated;
            ConjunctiveFilters = conjunctiveFilters;
        }

        public AttributeFilterCombiner(bool isCombinerNegated, bool conjunctiveFilters, IEnumerable<ICombinableFilter> filters) : this(isCombinerNegated, conjunctiveFilters)
        {
            AddRange(filters);
        }


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

        public void Reset()
        {
            _generatedFilterText = null;

            Generated = false;
        }
    }
}
