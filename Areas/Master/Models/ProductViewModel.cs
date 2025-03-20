namespace AEMSWEB.Models.Masters
{
    public class ProductViewModel
    {
        public Int16 ProductId { get; set; }
        public Int16 CompanyId { get; set; }
        public string? ProductCode { get; set; }
        public string? ProductName { get; set; }
        public string? Remarks { get; set; }
        public bool IsActive { get; set; }
        public Int16? CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public Int16? EditById { get; set; }
        public DateTime? EditDate { get; set; }
        public string? CreateBy { get; set; }
        public string? EditBy { get; set; }
    }

    public class SaveProductViewModel
    {
        public ProductViewModel product { get; set; }
        public string? companyId { get; set; }
    }

    public class ProductViewModelCount
    {
        public Int16 responseCode { get; set; }
        public string? responseMessage { get; set; }
        public Int64 totalRecords { get; set; }
        public List<ProductViewModel> data { get; set; }
    }
}