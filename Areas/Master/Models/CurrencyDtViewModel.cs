namespace AEMSWEB.Models.Masters
{
    public class CurrencyDtViewModel
    {
        public Int16 CurrencyId { get; set; }
        public string CurrencyCode { get; set; }
        public string CurrencyName { get; set; }
        public Int16 CompanyId { get; set; }
        public decimal ExhRate { get; set; }
        public string ValidFrom;
        public Int16? CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public Int16? EditById { get; set; }
        public DateTime? EditDate { get; set; }
        public string CreateBy { get; set; }
        public string EditBy { get; set; }
    }
}