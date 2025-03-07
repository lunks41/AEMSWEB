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
}