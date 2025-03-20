namespace AEMSWEB.Areas.Setting.Models
{
    public class DynamicLookupViewModel
    {
        public short CompanyId { get; set; }
        public bool IsBarge { get; set; }
        public bool IsVessel { get; set; }
        public bool IsVoyage { get; set; }
        public bool IsCustomer { get; set; }
        public bool IsSupplier { get; set; }
        public bool IsProduct { get; set; }
        public short CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public short? EditById { get; set; }
        public DateTime? EditDate { get; set; }
        public string? CreateBy { get; set; }
        public string? EditBy { get; set; }
    }
}