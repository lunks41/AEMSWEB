namespace AEMSWEB.Models.Masters
{
    public class VesselViewModel
    {
        public Int32 VesselId { get; set; }
        public Int16 CompanyId { get; set; }
        public string VesselCode { get; set; }
        public string VesselName { get; set; }
        public string CallSign { get; set; }
        public string IMOCode { get; set; }
        public string GRT { get; set; }
        public string LicenseNo { get; set; }
        public string VesselType { get; set; }
        public string Flag { get; set; }
        public string Remarks { get; set; }
        public bool IsActive { get; set; }
        public Int16? CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public Int16? EditById { get; set; }
        public DateTime? EditDate { get; set; }
        public string CreateBy { get; set; }
        public string EditBy { get; set; }
    }

    public class SaveVesselViewModel
    {
        public VesselViewModel Vessel { get; set; }
        public string CompanyId { get; set; }
    }
}