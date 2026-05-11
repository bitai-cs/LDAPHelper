using Bitai.LDAPHelper.Adapters;
using Bitai.LDAPHelper.DTO;

namespace Bitai.LDAPHelper.Demo
{
    public class DemoContext
    {
        public ImplementationType Implementation { get; set; }
        public ILdapConnectionFactoryAdapter ConnectionFactory { get; set; }
        public DemoSetup Configuration { get; set; }
        public string RequestLabel { get; set; } = "My Demo";
        
        // Connection settings
        public string SelectedLdapServer { get; set; }
        public int SelectedLdapServerPort { get; set; }
        public bool SelectedUseSsl { get; set; }
        public short SelectedConnectionTimeout { get; set; }
        public string SelectedDomainAccountName { get; set; }
        public string SelectedDomainAccountPassword { get; set; }
        public string SelectedBaseDN { get; set; }

        public ConnectionInfo GetConnectionInfo()
        {
            return new ConnectionInfo(
                SelectedLdapServer,
                SelectedLdapServerPort,
                SelectedUseSsl,
                SelectedConnectionTimeout);
        }

        public LDAPDomainAccountCredential GetDomainAccountCredential()
        {
            var parts = SelectedDomainAccountName.Split(new char[] { '\\' });
            return new LDAPDomainAccountCredential(
                parts[0],
                parts[1],
                SelectedDomainAccountPassword);
        }

        public SearchLimits GetSearchLimits()
        {
            return new SearchLimits(SelectedBaseDN)
            {
                MaxSearchResults = 1000,
                MaxSearchTimeout = 60
            };
        }

        public ClientConfiguration GetClientConfiguration()
        {
            return new ClientConfiguration(
                GetConnectionInfo(),
                GetDomainAccountCredential(),
                GetSearchLimits());
        }
    }
}