namespace AEMSWEB.Models.Masters
{
    public class AccountSetupViewModel
    {
        public Int16 AccSetupId { get; set; }
        public Int16 CompanyId { get; set; }
        public string? AccSetupCode { get; set; }
        public string? AccSetupName { get; set; }
        public Int16 AccSetupCategoryId { get; set; }
        public string? AccSetupCategoryCode { get; set; }
        public string? AccSetupCategoryName { get; set; }
        public string? Remarks { get; set; }
        public bool IsActive { get; set; }
        public Int16? CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public Int16? EditById { get; set; }
        public DateTime? EditDate { get; set; }
        public string? CreateBy { get; set; }
        public string? EditBy { get; set; }
    }

    public class SaveAccountSetupViewModel
    {
        public AccountSetupViewModel accountSetup { get; set; }
        public string? companyId { get; set; }
    }

    public class AccountSetupDtViewModel
    {
        public Int16 CompanyId { get; set; }
        public Int16 AccSetupId { get; set; }
        public string? AccSetupCode { get; set; }
        public string? AccSetupName { get; set; }
        public Int16 CurrencyId { get; set; }
        public string? CurrencyCode { get; set; }
        public string? CurrencyName { get; set; }
        public Int16 GLId { get; set; }
        public string? GLCode { get; set; }
        public string? GLName { get; set; }
        public bool ApplyAllCurr { get; set; }
        public Int16? CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public Int16? EditById { get; set; }
        public DateTime? EditDate { get; set; }
        public string? CreateBy { get; set; }
        public string? EditBy { get; set; }
    }

    public class SaveAccountSetupDtViewModel
    {
        public AccountSetupDtViewModel? accountSetupDt { get; set; }
        public string? companyId { get; set; }
    }

    public class AccountSetupCategoryViewModel
    {
        public Int16 AccSetupCategoryId { get; set; }
        public string? AccSetupCategoryCode { get; set; }
        public string? AccSetupCategoryName { get; set; }
        public string? Remarks { get; set; }
        public bool IsActive { get; set; }
        public Int16? CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public Int16? EditById { get; set; }
        public DateTime? EditDate { get; set; }
        public string? CreateBy { get; set; }
        public string? EditBy { get; set; }
    }

    public class SaveAccountSetupCategoryViewModel
    {
        public AccountSetupCategoryViewModel accountSetupCategory { get; set; }
        public string? companyId { get; set; }
    }

    public class AccountSetupViewModelCount
    {
        public Int16 responseCode { get; set; }
        public string? responseMessage { get; set; }
        public Int64 totalRecords { get; set; }
        public List<AccountSetupViewModel> data { get; set; }
    }

    public class AccountSetupDtViewModelCount
    {
        public Int16 responseCode { get; set; }
        public string? responseMessage { get; set; }
        public Int64 totalRecords { get; set; }
        public List<AccountSetupDtViewModel> data { get; set; }
    }

    public class AccountSetupCategoryViewModelCount
    {
        public Int16 responseCode { get; set; }
        public string? responseMessage { get; set; }
        public Int64 totalRecords { get; set; }
        public List<AccountSetupCategoryViewModel> data { get; set; }
    }
}