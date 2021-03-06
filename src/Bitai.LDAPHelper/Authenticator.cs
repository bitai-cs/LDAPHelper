using Novell.Directory.Ldap;
using System;
using System.Threading.Tasks;
using System.Linq;

namespace Bitai.LDAPHelper
{
    public class Authenticator : BaseHelper
    {
        #region Constructors
        public Authenticator(ConnectionInfo connectionInfo) : base(connectionInfo)
        {
        }
        #endregion


        #region Public methods
        /// <summary>
        /// Authenticate <see cref="Credentials"/> on the LDAP Server
        /// </summary>
        /// <param name="credentials"><see cref="Credentials"/> to connect and authenticate on the LDAP Server.</param>
        /// <returns>True or false, if authenticated or no.</returns>
        public async Task<bool> AuthenticateAsync(Credentials credentials)
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