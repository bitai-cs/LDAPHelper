using System;

namespace Bitai.LDAPHelper;

public class LdapOperationException : Exception
{
	public LdapOperationException(string message) : base(message)
	{
	}

	public LdapOperationException(string message, Exception innerException) : base(message, innerException)
	{
	}
}
