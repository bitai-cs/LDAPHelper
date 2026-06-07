namespace Bitai.LDAPHelper.LdapAdapters;

public interface IConnectionInfo
{
    short ConnectionTimeout { get; }
    string Server { get; }
    int ServerPort { get; }
    bool UseSSL { get; }
}
