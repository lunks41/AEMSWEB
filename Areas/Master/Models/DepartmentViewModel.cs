namespace AEMSWEB.Models.Masters
{
    public class DepartmentViewModel
    {
        public Int16 DepartmentId { get; set; }
        public Int16 CompanyId { get; set; }
        public string DepartmentCode { get; set; }
        public string DepartmentName { get; set; }
        public string Remarks { get; set; }
        public bool IsActive { get; set; }
        public Int16? CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public Int16? EditById { get; set; }
        public DateTime? EditDate { get; set; }
        public string CreateBy { get; set; }
        public string EditBy { get; set; }
    }

    public class SaveDepartmentViewModel
    {
        public DepartmentViewModel Department { get; set; }
        public string CompanyId { get; set; }
    }
}