namespace AMESWEB.Models.Masters
{
    public class EmployeeViewModel
    {
        public Int16 EmployeeId { get; set; }
        public Int16 CompanyId { get; set; }
        public string? EmployeeCode { get; set; }
        public string? EmployeeName { get; set; }
        public string? EmployeeOtherName { get; set; }
        public string? EmployeePhoto { get; set; }
        public string? EmployeeSignature { get; set; }
        public Int16 DepartmentId { get; set; }
        public string? DepartmentCode { get; set; }
        public string? DepartmentName { get; set; }
        public string? EmployeeSex { get; set; }
        public string? MartialStatus { get; set; }
        public DateTime EmployeeDOB { get; set; }
        public DateTime EmployeeJoinDate { get; set; }
        public DateTime EmployeeLastDate { get; set; }
        public string? EmployeeOffEmailAdd { get; set; }
        public string? EmployeeOtherEmailAdd { get; set; }
        public string? Remarks { get; set; }
        public bool IsActive { get; set; }
        public Int16? CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public Int16? EditById { get; set; }
        public DateTime? EditDate { get; set; }
        public string? CreateBy { get; set; }
        public string? EditBy { get; set; }
    }

    public class SaveEmployeeViewModel
    {
        public EmployeeViewModel employee { get; set; }
        public string? companyId { get; set; }
    }

    public class EmployeeViewModelCount
    {
        public Int16 responseCode { get; set; }
        public string? responseMessage { get; set; }
        public Int64 totalRecords { get; set; }
        public List<EmployeeViewModel> data { get; set; }
    }
}