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
        /// <summary>
        /// Authenticate <see cref="LdhCredentials"/> on the LDAP Server
        /// </summary>
        /// <param name="credentials"><see cref="LdhCredentials"/> to connect and authenticate on the LDAP Server.</param>
        /// <returns>True or false, if authenticated or no.</returns>
        public async Task<bool> Authenticate(LdhCredentials credentials)
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