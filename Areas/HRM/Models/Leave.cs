using System.ComponentModel.DataAnnotations;

namespace AEMSWEB.Areas.HRM.Models
{
    public class Leave
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public int LeaveTypeId { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        public string Reason { get; set; }
        public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected
        public DateTime AppliedDate { get; set; } = DateTime.Now;
        public string ApprovedBy { get; set; } // UserId of approver
        public int Days { get; set; } // Calculated on application

        public Employee Employee { get; set; }
        public LeaveType LeaveType { get; set; }
    }

    public class LeaveType
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } // e.g., Annual, Sick, Casual
    }

    public class LeaveBalance
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public int LeaveTypeId { get; set; }
        public int Balance { get; set; }

        public Employee Employee { get; set; }
        public LeaveType LeaveType { get; set; }
    }
}
