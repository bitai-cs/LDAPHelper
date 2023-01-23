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
	public class LDAPMsADUserAccount : ISecureCloningCredential<LDAPMsADUserAccount>
	{
		private string distinguishedNameOfContainer;
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
		private string userAccountControl;
		private UserAccountControlFlagsForMsAD? userAccountControlFlags;
		private string department;
		private string telephoneNumber;
		private string mail;
		private string password;



		/// <summary>
		/// Default constructor
		/// </summary>
		public LDAPMsADUserAccount()
		{
			UserAccountControl = $"{UserAccountControlFlagsForMsAD.NORMAL_ACCOUNT.ToString()},{UserAccountControlFlagsForMsAD.DONT_EXPIRE_PASSWORD}";
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="distinguishedNmaeOfContainer">Container distinguished name of user account.</param>
		public LDAPMsADUserAccount(string distinguishedNmaeOfContainer) : this()
		{
			if (string.IsNullOrEmpty(distinguishedNmaeOfContainer))
				throw new ArgumentNullException(nameof(distinguishedNmaeOfContainer));

			DistinguishedNameOfContainer = distinguishedNmaeOfContainer;
		}



		public string DistinguishedNameOfContainer { get => distinguishedNameOfContainer; set => distinguishedNameOfContainer = value; }
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
		public string UserAccountControl
		{
			get => userAccountControl;
			set
			{
				var tempValue = value;

				if (!string.IsNullOrEmpty(tempValue))
				{
					var flagNames = tempValue.Split(',');
					int totalFlagValue = 0;
					foreach (var flagName in flagNames)
					{
						UserAccountControlFlagsForMsAD parsedFlag;
						if (!Enum.TryParse(flagName, out parsedFlag))
							throw new InvalidCastException($"Unable to assign property {nameof(UserAccountControl)}. Can not parse {flagName} to {nameof(UserAccountControlFlagsForMsAD)}");

						totalFlagValue += (int)parsedFlag;
					}

					userAccountControlFlags = (UserAccountControlFlagsForMsAD)Enum.ToObject(typeof(UserAccountControlFlagsForMsAD), totalFlagValue);

					userAccountControl = tempValue;
				}
				else
				{
					userAccountControlFlags = null;
					userAccountControl = value;
				}
			}
		}
		public string Department { get => department; set => department = value; }
		public string TelephoneNumber { get => telephoneNumber; set => telephoneNumber = value; }
		public string Mail { get => mail; set => mail = value; }
		public string Password { get => password; set => password = value; }
		public UserAccountControlFlagsForMsAD? UserAccountControlFlags { get => userAccountControlFlags; }



		public LDAPMsADUserAccount SecureClone()
		{
			return new LDAPMsADUserAccount
			{
				Cn = Cn,
				Department = Department,
				Description = Description,
				DisplayName = DisplayName,
				DistinguishedName = DistinguishedName,
				DistinguishedNameOfContainer = DistinguishedNameOfContainer,
				GivenName = GivenName,
				Mail = Mail,
				MemberOf = (string[])MemberOf?.Clone(),
				Name = Name,
				ObjectClass = (string[])ObjectClass?.Clone(),
				Password = "*****",
				SAMAccountName = SAMAccountName,
				TelephoneNumber = TelephoneNumber,
				UserAccountControl = UserAccountControl,
				UserPrincipalName = UserPrincipalName
			};
		}
	}
}
