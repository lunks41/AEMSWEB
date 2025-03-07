namespace AEMSWEB.Models.Masters
{
    public class AccountSetupDtViewModel
    {
        public Int16 CompanyId { get; set; }
        public Int16 AccSetupId { get; set; }
        public string AccSetupCode { get; set; }
        public string AccSetupName { get; set; }
        public Int16 CurrencyId { get; set; }
        public string CurrencyCode { get; set; }
        public string CurrencyName { get; set; }
        public Int16 GLId { get; set; }
        public string GLCode { get; set; }
        public string GLName { get; set; }
        public bool ApplyAllCurr { get; set; }
        public Int16? CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public Int16? EditById { get; set; }
        public DateTime? EditDate { get; set; }
        public string CreateBy { get; set; }
        public string EditBy { get; set; }
    }
}