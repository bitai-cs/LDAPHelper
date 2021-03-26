using System;
using System.Collections.Generic;
using System.Text;

namespace LDAPHelper
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
