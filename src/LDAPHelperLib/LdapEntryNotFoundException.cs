using System;
using System.Collections.Generic;
using System.Text;

namespace LdapHelperLib
{
	public class LdapEntryNotFoundException : Exception
	{
		public LdapEntryNotFoundException(string message) : base(message)
		{
		}

		public LdapEntryNotFoundException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}
