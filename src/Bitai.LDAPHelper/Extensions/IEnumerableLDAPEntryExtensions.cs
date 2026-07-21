using System.Collections.Generic;
using System.Linq;

namespace Bitai.LDAPHelper.Extensions
{
    /// <summary>
    /// Provides extension methods for LDAP entry collections.
    /// </summary>
    public static class IEnumerableLDAPEntryExtensions
    {
        /// <summary>
        /// Flattens all <c>memberOfEntries</c> hierarchies recursively for every entry in the sequence.
        /// </summary>
        /// <param name="entries">Source LDAP entries.</param>
        /// <returns>A distinct flattened sequence of parent/group entries.</returns>
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
