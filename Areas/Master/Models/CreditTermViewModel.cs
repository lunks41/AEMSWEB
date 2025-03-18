namespace AEMSWEB.Models.Masters
{
    public class CreditTermViewModel
    {
        public Int16 CreditTermId { get; set; }
        public Int16 CompanyId { get; set; }
        public string CreditTermCode { get; set; }
        public string CreditTermName { get; set; }
        public Int32 NoDays { get; set; }
        public string Remarks { get; set; }
        public bool IsActive { get; set; }
        public Int16? CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public Int16? EditById { get; set; }
        public DateTime? EditDate { get; set; }
        public string CreateBy { get; set; }
        public string EditBy { get; set; }
    }

    public class SaveCreditTermViewModel
    {
        public CreditTermViewModel CreditTerm { get; set; }
        public string CompanyId { get; set; }
    }

    public class CreditTermViewModelCount
    {
        public Int16 responseCode { get; set; }
        public string responseMessage { get; set; }
        public Int64 totalRecords { get; set; }
        public List<CreditTermViewModel> data { get; set; }
    }

    public class CreditTermDtViewModelCount
    {
        public Int16 responseCode { get; set; }
        public string responseMessage { get; set; }
        public Int64 totalRecords { get; set; }
        public List<CreditTermDtViewModel> data { get; set; }
    }

    public class CreditTermDtViewModel
    {
        public Int16 CreditTermId { get; set; }
        public string CreditTermCode { get; set; }
        public string CreditTermName { get; set; }
        public Int16 CompanyId { get; set; }
        public Int16 FromDay { get; set; }
        public Int16 ToDay { get; set; }
        public bool IsEndOfMonth { get; set; }
        public Int16 DueDay { get; set; }
        public Int16 NoMonth { get; set; }
        public Int16? CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public Int16? EditById { get; set; }
        public DateTime? EditDate { get; set; }
        public string CreateBy { get; set; }
        public string EditBy { get; set; }
    }
}