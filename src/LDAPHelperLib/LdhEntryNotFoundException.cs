using System;
using System.Collections.Generic;
using System.Text;

namespace LdapHelperLib
{
	public class LdhEntryNotFoundException : Exception
	{
		public LdhEntryNotFoundException(string message) : base(message)
		{
		}

		public LdhEntryNotFoundException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}
