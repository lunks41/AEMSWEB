namespace AEMSWEB.Models.Masters
{
    public class AccountSetupViewModel
    {
        public Int16 AccSetupId { get; set; }
        public Int16 CompanyId { get; set; }
        public string AccSetupCode { get; set; }
        public string AccSetupName { get; set; }
        public Int16 AccSetupCategoryId { get; set; }
        public string AccSetupCategoryCode { get; set; }
        public string AccSetupCategoryName { get; set; }
        public string Remarks { get; set; }
        public bool IsActive { get; set; }
        public Int16? CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public Int16? EditById { get; set; }
        public DateTime? EditDate { get; set; }
        public string CreateBy { get; set; }
        public string EditBy { get; set; }
    }

    public class SaveAccountSetupViewModel
    {
        public AccountSetupViewModel AccountSetup { get; set; }
        public string CompanyId { get; set; }
    }
}