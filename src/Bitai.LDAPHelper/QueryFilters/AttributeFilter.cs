using Bitai.LDAPHelper.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Bitai.LDAPHelper.QueryFilters
{
    public class AttributeFilter : ICombinableFilter
    {
        private string _generatedFilterText = null;


        public EntryAttribute FilterAttribute { get; }
        public FilterValue FilterValue { get; }
        public bool IsFilterNegated { get; }
        public bool Generated { get; private set; }


        public AttributeFilter(DTO.EntryAttribute filterAttribute, FilterValue filterValue)
        {
            FilterAttribute = filterAttribute;
            FilterValue = filterValue;
            IsFilterNegated = false;
        }

        public AttributeFilter(bool isFilterNegated, DTO.EntryAttribute filterAttribute, FilterValue filterValue) : this(filterAttribute, filterValue)
        {
            FilterAttribute = filterAttribute;
            FilterValue = filterValue;
            IsFilterNegated = isFilterNegated;
        }


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

        public void Reset()
        {
            _generatedFilterText = null;

            Generated = false;
        }
    }
}
