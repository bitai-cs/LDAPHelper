using System;
using System.Collections.Generic;
using System.Text;

namespace LDAPHelper.DTO
{
	public class LdhEntry : IComparable
	{
		#region Constructor
		public LdhEntry(string customTag = null)
		{
			this.CustomTag = customTag;
		}
		#endregion

		public string CustomTag { get; set; }

		public string DirectoryEntryPath { get; set; }

		//Country Abrev.
		public string c { get; set; }

		//Common name
		public string cn { get; set; }

		//Company name
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

		public IEnumerable<LdhEntry> memberOfEntries { get; set; }

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

		#region IComparable Members
		public int CompareTo(object obj)
		{
			if (obj is LdhEntry)
			{
				var u2 = (LdhEntry)obj;
				return this.distinguishedName.CompareTo(u2.distinguishedName);
			}
			else
				throw new ArgumentException("Object has not implemented ILdapEntry interface.");
		}
		#endregion
	}
}
