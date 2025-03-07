namespace AEMSWEB.Models.Masters
{
    public class DesignationViewModel
    {
        public Int16 DesignationId { get; set; }
        public Int16 CompanyId { get; set; }
        public string DesignationCode { get; set; }
        public string DesignationName { get; set; }
        public string Remarks { get; set; }
        public bool IsActive { get; set; }
        public Int16? CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public Int16? EditById { get; set; }
        public DateTime? EditDate { get; set; }
        public string CreateBy { get; set; }
        public string EditBy { get; set; }
    }

    public class SaveDesignationViewModel
    {
        public DesignationViewModel Designation { get; set; }
        public string CompanyId { get; set; }
    }
}