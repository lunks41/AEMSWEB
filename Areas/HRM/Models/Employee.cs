using System.ComponentModel.DataAnnotations;

namespace AMESWEB.Areas.HRM.Models
{
    // Models/Employee.cs
    public class Employee
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } // Links to ASP.NET Identity user

        [Required]
        public string EmployeeCode { get; set; } // Unique code for Excel mapping

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        public string Email { get; set; }
        public int DepartmentId { get; set; }
        public int PositionId { get; set; }
        public DateTime HireDate { get; set; }
        public decimal BaseSalary { get; set; } // For payroll

        public Department Department { get; set; }
        public Position Position { get; set; }
    }

    public enum EmploymentStatus
    {
        Active,
        OnLeave,
        Terminated
    }
}