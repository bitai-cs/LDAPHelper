// NovellLdapSearchConstraintsAdapter.cs
using Novell.Directory.Ldap;

namespace Bitai.LDAPHelper.Adapters.Novell
{
    /// <summary>
    /// Adapter for Novell.Directory.Ldap.LdapSearchConstraints
    /// </summary>
    public class NovellLdapSearchConstraintsAdapter : ILdapSearchConstraintsAdapter
    {
        private LdapSearchConstraints _constraints;

        public NovellLdapSearchConstraintsAdapter() {
            _constraints = new LdapSearchConstraints();
        }

        public NovellLdapSearchConstraintsAdapter(LdapSearchConstraints constraints) {
            _constraints = constraints;
        }

        public int ServerTimeLimit {
            get => _constraints.ServerTimeLimit;
            set => _constraints.ServerTimeLimit = value;
        }

        public int MaxResults {
            get => _constraints.MaxResults;
            set => _constraints.MaxResults = value;
        }

        public LdapSearchConstraints GetWrappedConstraints() {
            return _constraints;
        }
    }
}