using Novell.Directory.Ldap;
using System;
using System.Threading.Tasks;
using System.Linq;

namespace LdapHelperLib
{
	public class Authenticator : BaseHelper
	{
		#region Constructors
		public Authenticator(LdapClientConfiguration clientConfiguration) : base(clientConfiguration)
		{
		}

		public Authenticator(LdapConnectionInfo serverSettings, LdapUserCredentials userCredentials) : base(serverSettings, null, userCredentials)
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