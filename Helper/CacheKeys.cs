namespace AEMSWEB.Helper
{
    public static class CacheKeys
    {
        public static string UserCompanies(int userId) => $"UserCompanies_{userId}";

        public static string AccountSetupCategoryLookup => "AccountSetupCategoryLookup";
    }
}