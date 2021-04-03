using System;
using System.Collections.Generic;
using System.Text;

namespace Bitai.LDAPHelper.QueryFilters
{
    public class FilterValue
    {
        public string Value { get; }


        public FilterValue(string value)
        {
            if (string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value))
                throw new ArgumentNullException(nameof(value));

            Value = value;
        }


        public override string ToString()
        {
            return Value;
        }
    }
}
