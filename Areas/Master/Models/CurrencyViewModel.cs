namespace AEMSWEB.Models.Masters
{
    public class CurrencyViewModel
    {
        public Int16 CurrencyId { get; set; }
        public Int16 CompanyId { get; set; }
        public string CurrencyCode { get; set; }
        public string CurrencyName { get; set; }
        public bool IsMultiply { get; set; }
        public string Remarks { get; set; }
        public bool IsActive { get; set; }
        public Int16? CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public Int16? EditById { get; set; }
        public DateTime? EditDate { get; set; }
        public string CreateBy { get; set; }
        public string EditBy { get; set; }
    }

    public class SaveCurrencyViewModel
    {
        public CurrencyViewModel Currency { get; set; }
        public string CompanyId { get; set; }
    }
}