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

        public string co { get; set; }

        public string description { get; set; }

        public string department { get; set; }

        public string displayName { get; set; }

        public string distinguishedName { get; set; }

        /// <summary>
        /// First name
        /// </summary>
        public string givenName { get; set; }

        public string l { get; set; }

        public DateTime? lastLogon { get; set; }

        public string mail { get; set; }

        public string manager { get; set; }

        public string[] member { get; set; }

        public string[] memberOf { get; set; }

        public IEnumerable<LDAPEntry> memberOfEntries { get; set; }

        public string name { get; set; }

        public string objectCategory { get; set; }

        public string[] objectClass { get; set; }

        public string samAccountName { get; set; }

        public string samAccountType { get; set; }

        public string sn { get; set; }

        public string telephoneNumber { get; set; }

        public string title { get; set; }

        public string userPrincipalName { get; set; }

        public DateTime? whenCreated { get; set; }

        public string objectGuid { get; set; }

        public byte[] objectGuidBytes { get; set; }

        public string objectSid { get; set; }

        public byte[] objectSidBytes { get; set; }


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
