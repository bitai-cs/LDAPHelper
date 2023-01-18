using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitai.LDAPHelper.DTO
{
	/// <summary>
	/// User account for Microsoft Active Directory
	/// 
	/// Resources of interest:
	/// - https://www.rlmueller.net/Name_Attributes.htm
	/// </summary>
	public class LDAPMsADUserAccount
	{
		private string givenName;
		private string sn;
		private string cn;
		private string name;
		private string displayName;
		private string description;
		private string distinguishedName;
		private string[] memberOf;
		private string[] objectClass;
		private string samAccountName;
		private string userPrincipalName;
		private UserAccountControlFlagsForMsAD userAccountControl;
		private string department;
		private string telephoneNumber;
		private string mail;
		private string password;



		/// <summary>
		/// Default constructor
		/// </summary>
		public LDAPMsADUserAccount()
		{
			userAccountControl = UserAccountControlFlagsForMsAD.NORMAL_ACCOUNT | UserAccountControlFlagsForMsAD.DONT_EXPIRE_PASSWORD;
		}




		public string GivenName { get => givenName; set => givenName = value; }
		public string Sn { get => sn; set => sn = value; }
		public string Cn { get => cn; set => cn = value; }
		public string Name { get => name; set => name = value; }
		public string DisplayName { get => displayName; set => displayName = value; }
		public string Description { get => description; set => description = value; }
		public string DistinguishedName { get => distinguishedName; set => distinguishedName = value; }
		public string[] MemberOf { get => memberOf; set => memberOf = value; }
		public string[] ObjectClass { get => objectClass; set => objectClass = value; }
		public string SAMAccountName { get => samAccountName; set => samAccountName = value; }
		public string UserPrincipalName { get => userPrincipalName; set => userPrincipalName = value; }
		public UserAccountControlFlagsForMsAD UserAccountControl { get => userAccountControl; set => userAccountControl = value; }
		public string Department { get => department; set => department = value; }
		public string TelephoneNumber { get => telephoneNumber; set => telephoneNumber = value; }
		public string Mail { get => mail; set => mail = value; }
		public string Password { get => password; set => password = value; }
	}
}
