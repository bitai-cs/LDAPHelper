using Novell.Directory.Ldap;
using System;
using System.Threading.Tasks;
using System.Linq;

namespace LdapHelperLib
{
	public class Authenticator : BaseHelper
	{
		#region Constructors
		public Authenticator(string requestTag, LdapClientConfiguration clientConfiguration) : base(requestTag, clientConfiguration)
		{
		}

		public Authenticator(string requestTag, LdapConnectionPipeline connectionPipeline, LdapServerSettings serverSettings, LdapUserCredentials userCredentials) : base(requestTag, connectionPipeline, serverSettings, null, false, userCredentials)
		{
		}
		#endregion


		#region Public methods
		public async Task<bool> AuthenticateUser(LdapUserCredentials credentials)
		{
			using (var connection = await GetBoundLdapConnection(this.ServerSettings, credentials, false))
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