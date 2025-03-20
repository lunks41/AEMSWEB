using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AEMSWEB.Entities.Masters
{
    public class M_Employee
    {
        [Key]
        public Int16 EmployeeId { get; set; }

        public Int16 CompanyId { get; set; }
        public string? EmployeeCode { get; set; }
        public string? EmployeeName { get; set; }
        public string? EmployeeOtherName { get; set; }
        public string? EmployeePhoto { get; set; }
        public string? EmployeeSignature { get; set; }

        [ForeignKey("DepartmentId")]
        public Int16 DepartmentId { get; set; }

        public string? EmployeeSex { get; set; }
        public string? MartialStatus { get; set; }
        public DateTime EmployeeDOB { get; set; }
        public DateTime EmployeeJoinDate { get; set; }
        public DateTime EmployeeLastDate { get; set; }
        public string? EmployeeOffEmailAdd { get; set; }
        public string? EmployeeOtherEmailAdd { get; set; }
        public string? Remarks { get; set; }
        public bool IsActive { get; set; }
        public Int16 CreateById { get; set; }

        [NotMapped]
        public DateTime CreateDate { get; set; }

        public Int16? EditById { get; set; }
        public DateTime? EditDate { get; set; }
    }
}