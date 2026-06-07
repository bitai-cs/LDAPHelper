namespace Bitai.LDAPHelper.LdapAdapters;

public interface ISearchLimits
{
    string BaseDN { get; set; }
    LdapSearchScope LdapSearchScope { get; set; }
    int MaxSearchResults { get; set; }
    int MaxSearchTimeout { get; set; }
}
