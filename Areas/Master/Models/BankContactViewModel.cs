namespace AEMSWEB.Models.Masters
{
    public class BankContactViewModel
    {
        public Int16 ContactId { get; set; }
        public Int32 BankId { get; set; }

        public string BankCode { get; set; }

        public string BankName { get; set; }
        public string ContactName { get; set; }
        public string OtherName { get; set; }
        public string MobileNo { get; set; }
        public string OffNo { get; set; }
        public string FaxNo { get; set; }
        public string EmailAdd { get; set; }
        public string MessId { get; set; }
        public string ContactMessType { get; set; }
        public bool IsDefault { get; set; }
        public bool IsFinance { get; set; }
        public bool IsSales { get; set; }
        public bool IsActive { get; set; }
        public Int16? CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public Int16? EditById { get; set; }
        public DateTime? EditDate { get; set; }
        public string CreateBy { get; set; }
        public string EditBy { get; set; }
    }

    public class BankContactViewModelCount
    {
        public Int16 responseCode { get; set; }
        public string responseMessage { get; set; }
        public Int64 totalRecords { get; set; }
        public List<BankContactViewModel> data { get; set; }
    }
}