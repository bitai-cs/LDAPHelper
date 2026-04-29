// ILdapSearchConstraintsAdapter.cs
namespace Bitai.LDAPHelper.Adapters
{
    /// <summary>
    /// Target interface for LDAP search constraints
    /// </summary>
    public interface ILdapSearchConstraintsAdapter
    {
        int ServerTimeLimit { get; set; }
        int MaxResults { get; set; }
    }
}