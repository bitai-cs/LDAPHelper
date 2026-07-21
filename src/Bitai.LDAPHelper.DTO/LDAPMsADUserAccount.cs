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



		/// <summary>
		/// Gets or sets the distinguished name of the container where the user account should be created.
		/// </summary>
		public string DistinguishedNameOfContainer { get => distinguishedNameOfContainer; set => distinguishedNameOfContainer = value; }

		/// <summary>
		/// Gets or sets the user's given name.
		/// </summary>
		public string GivenName { get => givenName; set => givenName = value; }

		/// <summary>
		/// Gets or sets the user's surname.
		/// </summary>
		public string Sn { get => sn; set => sn = value; }

		/// <summary>
		/// Gets or sets the user common name (CN).
		/// </summary>
		public string Cn { get => cn; set => cn = value; }

		/// <summary>
		/// Gets or sets the LDAP <c>name</c> attribute.
		/// </summary>
		public string Name { get => name; set => name = value; }

		/// <summary>
		/// Gets or sets the display name.
		/// </summary>
		public string DisplayName { get => displayName; set => displayName = value; }

		/// <summary>
		/// Gets or sets the account description.
		/// </summary>
		public string Description { get => description; set => description = value; }

		/// <summary>
		/// Gets or sets the full distinguished name of the user account.
		/// </summary>
		public string DistinguishedName { get => distinguishedName; set => distinguishedName = value; }

		/// <summary>
		/// Gets or sets parent groups (distinguished names) this account belongs to.
		/// </summary>
		public string[] MemberOf { get => memberOf; set => memberOf = value; }

		/// <summary>
		/// Gets or sets LDAP object classes for this account.
		/// </summary>
		public string[] ObjectClass { get => objectClass; set => objectClass = value; }

		/// <summary>
		/// Gets or sets the sAMAccountName value.
		/// </summary>
		public string SAMAccountName { get => samAccountName; set => samAccountName = value; }

		/// <summary>
		/// Gets or sets the user principal name (UPN).
		/// </summary>
		public string UserPrincipalName { get => userPrincipalName; set => userPrincipalName = value; }
		/// <summary>
		/// Gets or sets user-account-control flags as a comma-separated list of <see cref="UserAccountControlFlagsForMsAD"/> names.
		/// </summary>
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
		/// <summary>
		/// Gets or sets the department.
		/// </summary>
		public string Department { get => department; set => department = value; }

		/// <summary>
		/// Gets or sets the phone number.
		/// </summary>
		public string TelephoneNumber { get => telephoneNumber; set => telephoneNumber = value; }

		/// <summary>
		/// Gets or sets the email address.
		/// </summary>
		public string Mail { get => mail; set => mail = value; }

		/// <summary>
		/// Gets or sets the account password.
		/// </summary>
		public string Password { get => password; set => password = value; }

		/// <summary>
		/// Gets parsed account-control flags derived from <see cref="UserAccountControl"/>.
		/// </summary>
		public UserAccountControlFlagsForMsAD? UserAccountControlFlags { get => userAccountControlFlags; }



		/// <summary>
		/// Creates a secure clone with password masked.
		/// </summary>
		/// <returns>A cloned account suitable for logging/transport.</returns>
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
