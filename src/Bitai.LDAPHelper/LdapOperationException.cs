using System;

namespace Bitai.LDAPHelper;

/// <summary>
/// Represents generic LDAP operation failures in helper workflows.
/// </summary>
public class LdapOperationException : Exception
{
	/// <summary>
	/// Initializes a new instance of the <see cref="LdapOperationException"/> class with a message.
	/// </summary>
	/// <param name="message">Error message.</param>
	public LdapOperationException(string message) : base(message)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="LdapOperationException"/> class with a message and inner exception.
	/// </summary>
	/// <param name="message">Error message.</param>
	/// <param name="innerException">Underlying cause of this exception.</param>
	public LdapOperationException(string message, Exception innerException) : base(message, innerException)
	{
	}
}
