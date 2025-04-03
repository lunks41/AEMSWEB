using System.ComponentModel.DataAnnotations;

namespace AMESWEB.Areas.HRM.Models
{
    // Models/LeaveRequest.cs
    public class LeaveRequest
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }

        [Required]
        public LeaveType LeaveType { get; set; }

        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        public string Reason { get; set; }
        public LeaveStatus Status { get; set; } = LeaveStatus.Pending;
        public string ApproverComments { get; set; }
    }

    public enum LeaveTypeV1
    {
        Vacation,
        Sick,
        Personal,
        Maternity,
        Paternity
    }

    public enum LeaveStatus
    {
        Pending,
        Approved,
        Rejected
    }
}
