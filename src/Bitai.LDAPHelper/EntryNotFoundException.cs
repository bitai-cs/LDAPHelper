using System;
using System.Collections.Generic;
using System.Text;

namespace Bitai.LDAPHelper
{
	public class EntryNotFoundException : Exception
	{
		public EntryNotFoundException(string message) : base(message)
		{
		}

		public EntryNotFoundException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}
