using System.ComponentModel.DataAnnotations;

namespace AMESWEB.Areas.HRM.Models
{
    public class Attendance
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }

        [Required]
        public DateTime Date { get; set; }

        public TimeSpan InTime { get; set; }
        public TimeSpan OutTime { get; set; }
        public string Status { get; set; } // e.g., Present, Absent, Late

        public Employee Employee { get; set; }
    }
}