using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitai.LDAPHelper.DTO
{
	public class LDAPDistinguishedNameCredential : ISecureCloningCredential<LDAPDistinguishedNameCredential>
	{
		public string DistinguishedName { get; set; }

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
				throw new InvalidOperationException("The account name must be specified.");

			DistinguishedName = distinguishedName;
			Password = password;
		}




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
