using System;
using System.Collections.Generic;
using System.Text;

namespace Bitai.LDAPHelper.DTO
{
    /// <summary>
    /// This class represents an LDAP Entry.
    /// </summary>
    public class LDAPEntry : IComparable
    {
        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="requestLabel">Optional tag to label this object.</param>
        public LDAPEntry(string requestLabel = null)
        {
            this.RequestLabel = requestLabel;
        }
        #endregion

        /// <summary>
        /// Gets or sets an optional caller-defined label used to correlate requests and results.
        /// </summary>
        public string RequestLabel { get; set; }

        /// <summary>
        /// Country name abbreviation.
        /// </summary>
        public string c { get; set; }

        /// <summary>
        /// Common name.
        /// </summary>
        public string cn { get; set; }

        /// <summary>
        /// Company name
        /// </summary>        
        public string company { get; set; }

        /// <summary>
        /// Country full name.
        /// </summary>
        public string co { get; set; }

        /// <summary>
        /// Description.
        /// </summary>
        public string description { get; set; }

        /// <summary>
        /// Department name.
        /// </summary>
        public string department { get; set; }

        /// <summary>
        /// Display name.
        /// </summary>
        public string displayName { get; set; }

        /// <summary>
        /// Distinguished name.
        /// </summary>
        public string distinguishedName { get; set; }

        /// <summary>
        /// First name
        /// </summary>
        public string givenName { get; set; }

        /// <summary>
        /// Locality/city.
        /// </summary>
        public string l { get; set; }

        /// <summary>
        /// Last logon date/time.
        /// </summary>
        public DateTime? lastLogon { get; set; }

        /// <summary>
        /// Email address.
        /// </summary>
        public string mail { get; set; }

        /// <summary>
        /// Manager distinguished name.
        /// </summary>
        public string manager { get; set; }

        /// <summary>
        /// Members of this entry (for group entries).
        /// </summary>
        public string[] member { get; set; }

        /// <summary>
        /// Parent groups distinguished names.
        /// </summary>
        public string[] memberOf { get; set; }

        /// <summary>
        /// Parent entries expanded from <see cref="memberOf"/>.
        /// </summary>
        public IEnumerable<LDAPEntry> memberOfEntries { get; set; }

        /// <summary>
        /// LDAP <c>name</c> attribute.
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// Object category.
        /// </summary>
        public string objectCategory { get; set; }

        /// <summary>
        /// Object classes.
        /// </summary>
        public string[] objectClass { get; set; }

        /// <summary>
        /// sAMAccountName.
        /// </summary>
        public string samAccountName { get; set; }

        /// <summary>
        /// sAMAccountType symbolic value.
        /// </summary>
        public string samAccountType { get; set; }

        /// <summary>
        /// Surname.
        /// </summary>
        public string sn { get; set; }

        /// <summary>
        /// Telephone number.
        /// </summary>
        public string telephoneNumber { get; set; }

        /// <summary>
        /// Job title.
        /// </summary>
        public string title { get; set; }

        /// <summary>
        /// User principal name.
        /// </summary>
        public string userPrincipalName { get; set; }

        /// <summary>
        /// Entry creation date/time.
        /// </summary>
        public DateTime? whenCreated { get; set; }

        /// <summary>
        /// Object GUID string.
        /// </summary>
        public string objectGuid { get; set; }

        /// <summary>
        /// Object GUID bytes.
        /// </summary>
        public byte[] objectGuidBytes { get; set; }

        /// <summary>
        /// Object SID string.
        /// </summary>
        public string objectSid { get; set; }

        /// <summary>
        /// Object SID bytes.
        /// </summary>
        public byte[] objectSidBytes { get; set; }

        /// <summary>
        /// Raw userAccountControl attribute value.
        /// </summary>
        public string? userAccountControl { get; set; }

        /// <summary>
        /// Recursively gets all parent entries from the <see cref="memberOfEntries"/> hierarchy.
        /// </summary>
        /// <returns>A flattened sequence of parent entries.</returns>
        public IEnumerable<LDAPEntry> GetMemberOfEntriesRecursively()
        {
            var _list = new List<LDAPEntry>();

            if (this.memberOfEntries == null)
                return _list;

            foreach (var memberOfEntry in this.memberOfEntries)
            {
                _list.Add(memberOfEntry);
                _list.AddRange(memberOfEntry.GetMemberOfEntriesRecursively());
            }

            return _list;
        }


        #region IComparable Members
        /// <summary>
        /// Compares this entry with another by distinguished name.
        /// </summary>
        /// <param name="obj">Entry to compare against.</param>
        /// <returns>Comparison result.</returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="obj"/> is not an <see cref="LDAPEntry"/>.</exception>
        public int CompareTo(object obj)
        {
            if (obj is LDAPEntry)
            {
                var u2 = (LDAPEntry)obj;
                return this.distinguishedName.CompareTo(u2.distinguishedName);
            }
            else
                throw new ArgumentException("Object has not implemented ILdapEntry interface.");
        }
        #endregion
    }
}
