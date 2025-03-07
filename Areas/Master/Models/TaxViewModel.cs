namespace AEMSWEB.Models.Masters
{
    public class TaxViewModel
    {
        public Int16 TaxId { get; set; }
        public Int16 CompanyId { get; set; }
        public Int16 TaxCategoryId { get; set; }
        public string TaxCategoryCode { get; set; }
        public string TaxCategoryName { get; set; }
        public string TaxCode { get; set; }
        public string TaxName { get; set; }
        public string Remarks { get; set; }
        public bool IsActive { get; set; }
        public Int16? CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public Int16? EditById { get; set; }
        public DateTime? EditDate { get; set; }
        public string CreateBy { get; set; }
        public string EditBy { get; set; }
    }

    public class SaveTaxViewModel
    {
        public TaxViewModel Tax { get; set; }
        public string CompanyId { get; set; }
    }
}