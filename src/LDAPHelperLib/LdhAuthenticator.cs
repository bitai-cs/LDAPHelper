using Novell.Directory.Ldap;
using System;
using System.Threading.Tasks;
using System.Linq;

namespace LDAPHelper
{
	public class LdhAuthenticator : BaseHelper
	{
		#region Constructors
		public LdhAuthenticator(LdhConnectionInfo connectionInfo) : base(connectionInfo)
		{
		}
		#endregion


		#region Public methods
		public async Task<bool> AuthenticateUser(LdhUserCredentials credentials)
		{
			using (var connection = await GetLdapConnection(this.ConnectionInfo, credentials, false))
			{
				if (connection.Bound)
					return true;
				else
					return false;
			}
		}
		#endregion
	}
}