using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bitai.LDAPHelper.Extensions
{
    public static class IEnumerableLDAPEntryExtensions
    {
        public static IEnumerable<DTO.LDAPEntry> SelectAllMemberOfEntriesRecursively(this IEnumerable<DTO.LDAPEntry> entries)
        {
            var partialList = new List<DTO.LDAPEntry>();

            foreach (var entry in entries)
            {
                partialList.AddRange(entry.GetMemberOfEntriesRecursively());
            }

            return partialList.Distinct();
        }
    }
}
