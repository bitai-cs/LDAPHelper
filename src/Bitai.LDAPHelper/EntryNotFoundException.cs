using System;

namespace Bitai.LDAPHelper
{
	/// <summary>
	/// Represents errors when a requested LDAP entry cannot be found.
	/// </summary>
	public class EntryNotFoundException : Exception
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="EntryNotFoundException"/> class with a message.
		/// </summary>
		/// <param name="message">Error message.</param>
		public EntryNotFoundException(string message) : base(message)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="EntryNotFoundException"/> class with a message and inner exception.
		/// </summary>
		/// <param name="message">Error message.</param>
		/// <param name="innerException">Underlying cause of this exception.</param>
		public EntryNotFoundException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}
