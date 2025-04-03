namespace AMESWEB.Models.Masters
{
    public class PortRegionViewModel
    {
        public Int16 PortRegionId { get; set; }
        public Int16 CompanyId { get; set; }
        public string? PortRegionCode { get; set; }
        public string? PortRegionName { get; set; }
        public string? Remarks { get; set; }
        public bool IsActive { get; set; }
        public Int32? CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public Int16? EditById { get; set; }
        public DateTime? EditDate { get; set; }
        public string? CreateBy { get; set; }
        public string? EditBy { get; set; }
    }

    public class SavePortRegionViewModel
    {
        public PortRegionViewModel portRegion { get; set; }
        public string? companyId { get; set; }
    }

    public class PortRegionViewModelCount
    {
        public Int16 responseCode { get; set; }
        public string? responseMessage { get; set; }
        public Int64 totalRecords { get; set; }
        public List<PortRegionViewModel> data { get; set; }
    }
}