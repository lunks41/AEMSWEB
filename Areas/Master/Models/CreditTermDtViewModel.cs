namespace AEMSWEB.Models.Masters
{
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