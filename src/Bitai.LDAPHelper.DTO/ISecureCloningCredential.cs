using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitai.LDAPHelper.DTO
{
	internal interface ISecureCloningCredential<T>
	{
		T SecureClone();
	}
}
