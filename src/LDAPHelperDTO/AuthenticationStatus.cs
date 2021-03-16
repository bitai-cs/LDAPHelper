using System;
using System.Collections.Generic;
using System.Text;

namespace LdapHelperDTO
{
	public class AuthenticationStatus
	{
		public string CustomTag { get; set; }
		public bool UseGC { get; set; }
		public string DomainName { get; set; }
		public string User { get; set; }
		public bool IsAuthenticated { get; set; }
		public string Message { get; set; }
	}
}