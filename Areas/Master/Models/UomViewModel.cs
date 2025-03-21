namespace AEMSWEB.Models.Masters
{
    public class UomViewModel
    {
        public Int16 UomId { get; set; }
        public Int16 CompanyId { get; set; }
        public string? UomCode { get; set; }
        public string? UomName { get; set; }
        public string? Remarks { get; set; }
        public bool IsActive { get; set; }
        public Int16? CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public Int16? EditById { get; set; }
        public DateTime? EditDate { get; set; }
        public string? CreateBy { get; set; }
        public string? EditBy { get; set; }
    }

    public class SaveUomViewModel
    {
        public UomViewModel Uom { get; set; }
        public string? CompanyId { get; set; }
    }

    public class UomDtViewModel
    {
        public Int16 CompanyId { get; set; }
        public Int16 UomId { get; set; }
        public string? UomCode { get; set; }
        public string? UomName { get; set; }
        public Int16 PackUomId { get; set; }
        public string? PackUomCode { get; set; }
        public string? PackUomName { get; set; }
        public decimal UomFactor { get; set; }
        public Int16? CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public Int16? EditById { get; set; }
        public DateTime? EditDate { get; set; }
        public string? CreateBy { get; set; }
        public string? EditBy { get; set; }
    }

    public class SaveUomDtViewModel
    {
        public UomDtViewModel? uomDt { get; set; }
        public string? companyId { get; set; }
    }

    public class UomViewModelCount
    {
        public Int16 responseCode { get; set; }
        public string? responseMessage { get; set; }
        public Int64 totalRecords { get; set; }
        public List<UomViewModel> data { get; set; }
    }

    public class UomDtViewModelCount
    {
        public Int16 responseCode { get; set; }
        public string? responseMessage { get; set; }
        public Int64 totalRecords { get; set; }
        public List<UomDtViewModel> data { get; set; }
    }
}