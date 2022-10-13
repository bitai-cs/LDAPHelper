﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitai.LDAPHelper.DTO
{
	public class LDAPDistinguishedNameCredential
	{
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


		public string DistinguishedName { get; }
		public string Password { get; }


		public LDAPDistinguishedNameCredential SecureClone()
		{
			return new LDAPDistinguishedNameCredential(this.DistinguishedName, null);
		}
	}
}
