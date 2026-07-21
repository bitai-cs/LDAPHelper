using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitai.LDAPHelper.DTO
{
	/// <summary>
	/// Represents credentials based on a distinguished name and password.
	/// </summary>
	public class LDAPDistinguishedNameCredential : ISecureCloningCredential<LDAPDistinguishedNameCredential>
	{
		/// <summary>
		/// Gets or sets the account distinguished name.
		/// </summary>
		public string DistinguishedName { get; set; }

		/// <summary>
		/// Gets or sets the account password.
		/// </summary>
		public string Password { get; set; }




		/// <summary>
		/// Default constructor. 
		/// </summary>
		public LDAPDistinguishedNameCredential()
		{
			//Do not remove this constructor, it is required to deserialize data.
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="distinguishedName">Account distinguished name</param>
		/// <param name="password">Account password</param>
		/// <exception cref="InvalidOperationException">Constructor exception.</exception>
		public LDAPDistinguishedNameCredential(string distinguishedName, string password)
		{
			if (string.IsNullOrEmpty(distinguishedName))
				throw new InvalidOperationException("A distinguished name is required: the account’s DN must be specified.");

			DistinguishedName = distinguishedName;
			Password = password;
		}




		/// <summary>
		/// Creates a clone with password removed.
		/// </summary>
		/// <returns>A cloned credential with sensitive information sanitized.</returns>
		public LDAPDistinguishedNameCredential SecureClone()
		{
			var clone = new LDAPDistinguishedNameCredential
			{
				DistinguishedName = this.DistinguishedName,
				Password = null
			};

			return clone;
		}
	}
}
