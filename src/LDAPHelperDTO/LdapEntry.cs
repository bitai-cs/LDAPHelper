using System;
using System.Collections.Generic;
using System.Text;

namespace LdapHelperDTO
{
	public class LdapEntry : IComparable
	{
		#region Constructor
		public LdapEntry(string customTag = null)
		{
			_customTag = customTag;
		}
		#endregion

		private string _customTag;
		public string CustomTag
		{
			set
			{
				_customTag = value;
			}
			get
			{
				return _customTag;
			}
		}

		private string _directoryEntryPath;
		public string DirectoryEntryPath
		{
			set
			{
				_directoryEntryPath = value;
			}
			get
			{
				return _directoryEntryPath;
			}
		}

		//Country Abrev.
		private string _c;
		public string c
		{
			get
			{
				return _c;
			}
			set
			{
				_c = value;
			}
		}

		//Common name
		private string _cn;
		public string cn
		{
			get
			{
				return _cn;
			}
			set
			{
				_cn = value;
			}
		}

		//Company name
		private string _company;
		public string company
		{
			get
			{
				return _company;
			}
			set
			{
				_company = value;
			}
		}

		private string _description;
		public string description
		{
			get
			{
				return _description;
			}
			set
			{
				_description = value;
			}
		}

		private string _department;
		public string department
		{
			get
			{
				return _department;
			}
			set
			{
				_department = value;
			}
		}

		private string _displayName;
		public string displayName
		{
			get
			{
				return _displayName;
			}
			set
			{
				_displayName = value;
			}
		}

		private string _distinguishedName;
		public string distinguishedName
		{
			get
			{
				return _distinguishedName;
			}
			set
			{
				_distinguishedName = value;
			}
		}

		private string _givenName;
		/// <summary>
		/// First name
		/// </summary>
		public string givenName
		{
			get
			{
				return _givenName;
			}
			set
			{
				_givenName = value;
			}
		}

		private string _l;
		public string l { set => _l = value; get => _l; }

		private DateTime? _lastLogon;
		public DateTime? lastLogon
		{
			get
			{
				return _lastLogon;
			}
			set
			{
				_lastLogon = value;
			}
		}

		private string _mail;
		public string mail
		{
			get
			{
				return _mail;
			}
			set
			{
				_mail = value;
			}
		}

		private string _manager;
		public string manager
		{
			get
			{
				return _manager;
			}
			set
			{
				_manager = value;
			}
		}

		private string[] _member;
		public string[] member
		{
			get
			{
				return _member;
			}
			set
			{
				_member = value;
			}
		}

		private string[] _memberOf;
		public string[] memberOf
		{
			get
			{
				return _memberOf;
			}
			set
			{
				_memberOf = value;
			}
		}

		private IEnumerable<LdapEntry> _memberOfEntries;
		public IEnumerable<LdapEntry> memberOfEntries
		{
			get => _memberOfEntries;
			set => _memberOfEntries = value;
		}

		private string _name;
		public string name { set => _name = value; get => _name; }

		private string _objectCategory;
		public string objectCategory
		{
			get
			{
				return _objectCategory;
			}
			set
			{
				_objectCategory = value;
			}
		}

		private string[] _objectClass;
		public string[] objectClass
		{
			get
			{
				return _objectClass;
			}
			set
			{
				_objectClass = value;
			}
		}

		private string _samAccountName;
		public string samAccountName
		{
			get
			{
				return _samAccountName;
			}
			set
			{
				_samAccountName = value;
			}
		}

		private string _samAccountType;
		public string samAccountType { set => _samAccountType = value; get => _samAccountType; }

		//Surname
		private string _sn;
		public string sn
		{
			get
			{
				return _sn;
			}
			set
			{
				_sn = value;
			}
		}

		private string _telephoneNumber;
		public string telephoneNumber
		{
			get
			{
				return _telephoneNumber;
			}
			set
			{
				_telephoneNumber = value;
			}
		}

		private string _title;
		public string title
		{
			get
			{
				return _title;
			}
			set
			{
				_title = value;
			}
		}

		private string _userPrincipalName;
		public string userPrincipalName
		{
			get
			{
				return _userPrincipalName;
			}
			set
			{
				_userPrincipalName = value;
			}
		}

		private DateTime? _whenCreated;
		public DateTime? whenCreated
		{
			get
			{
				return _whenCreated;
			}
			set
			{
				_whenCreated = value;
			}
		}

		private string _objectGuid;
		public string objectGuid
		{
			get
			{
				return _objectGuid;
			}
			set
			{
				_objectGuid = value;
			}
		}

		private byte[] _objectGuidBytes;
		public byte[] objectGuidBytes
		{
			get
			{
				return _objectGuidBytes;
			}
			set
			{
				_objectGuidBytes = value;
			}
		}

		private string _objectSid;
		public string objectSid
		{
			get
			{
				return _objectSid;
			}
			set
			{
				_objectSid = value;
			}
		}

		private byte[] _objectSidBytes;
		public byte[] objectSidBytes
		{
			get
			{
				return _objectSidBytes;
			}
			set
			{
				_objectSidBytes = value;
			}
		}

		#region IComparable Members
		public int CompareTo(object obj)
		{
			if (obj is LdapEntry)
			{
				var u2 = (LdapEntry)obj;
				return _distinguishedName.CompareTo(u2.distinguishedName);
			}
			else
				throw new ArgumentException("Object has not implemented ILdapEntry interface.");
		}
		#endregion
	}
}
