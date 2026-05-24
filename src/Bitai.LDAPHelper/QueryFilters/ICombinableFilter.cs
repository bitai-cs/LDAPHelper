namespace Bitai.LDAPHelper.QueryFilters
{
    public interface ICombinableFilter
    {
        bool Generated { get; }

        void Reset();

        string ToString();
    }
}