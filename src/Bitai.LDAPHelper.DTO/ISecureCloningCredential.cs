using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitai.LDAPHelper.DTO
{
	/// <summary>
	/// Defines secure cloning behavior used to remove or mask sensitive credential values.
	/// </summary>
	/// <typeparam name="T">Credential type returned by the secure clone operation.</typeparam>
	internal interface ISecureCloningCredential<T>
	{
		/// <summary>
		/// Creates a secure clone suitable for logging or transport without exposing secrets.
		/// </summary>
		/// <returns>A cloned credential with sensitive data removed or masked.</returns>
		T SecureClone();
	}
}
