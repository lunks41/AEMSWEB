namespace AEMSWEB.Areas.Master.Models
{
    public class BankViewModel
    {
        public Int16 BankId { get; set; }
        public Int16 CompanyId { get; set; }
        public string BankCode { get; set; }
        public string BankName { get; set; }
        public Int16 CurrencyId { get; set; }
        public string CurrencyCode { get; set; }
        public string CurrencyName { get; set; }
        public string AccountNo { get; set; }
        public string SwiftCode { get; set; }
        public string Remarks1 { get; set; }
        public string Remarks2 { get; set; }
        public Int16 GLId { get; set; }
        public string GLCode { get; set; }
        public string GLName { get; set; }
        public bool IsActive { get; set; }
        public bool IsOwnBank { get; set; }
        public Int16 CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public Int16? EditById { get; set; }
        public DateTime? EditDate { get; set; }
        public string CreateBy { get; set; }
        public string EditBy { get; set; }
    }

    public class SaveBankViewModel
    {
        public BankViewModel Bank { get; set; }
        public string CompanyId { get; set; }
    }
}