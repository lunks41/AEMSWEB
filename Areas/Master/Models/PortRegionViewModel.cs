namespace AEMSWEB.Models.Masters
{
    public class PortRegionViewModel
    {
        public Int16 PortRegionId { get; set; }
        public Int16 CompanyId { get; set; }
        public string PortRegionCode { get; set; }
        public string PortRegionName { get; set; }
        public Int16 CountryId { get; set; }
        public string CountryCode { get; set; }
        public string CountryName { get; set; }
        public string Remarks { get; set; }
        public bool IsActive { get; set; }
        public Int32? CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public Int16? EditById { get; set; }
        public DateTime? EditDate { get; set; }
        public string CreateBy { get; set; }
        public string EditBy { get; set; }
    }

    public class SavePortRegionViewModel
    {
        public PortRegionViewModel PortRegion { get; set; }
        public string CompanyId { get; set; }
    }

    public class PortRegionViewModelCount
    {
        public Int16 responseCode { get; set; }
        public string responseMessage { get; set; }
        public Int64 totalRecords { get; set; }
        public List<PortRegionViewModel> data { get; set; }
    }
}