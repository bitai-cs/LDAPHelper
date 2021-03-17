using Novell.Directory.Ldap;
using System;
using System.Threading.Tasks;
using System.Linq;

namespace LdapHelperLib
{
	public class LdapAuthenticator : BaseHelper
	{
		#region Constructors
		public LdapAuthenticator(LdapConnectionInfo connectionInfo) : base(connectionInfo)
		{
		}
		#endregion


		#region Public methods
		public async Task<bool> AuthenticateUser(LdapUserCredentials credentials)
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