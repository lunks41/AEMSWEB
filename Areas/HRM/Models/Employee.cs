using System.ComponentModel.DataAnnotations;

namespace AEMSWEB.Areas.HRM.Models
{
    // Models/Employee.cs
    public class Employee
    {
        public int Id { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [Phone]
        public string Phone { get; set; }

        [DataType(DataType.Date)]
        public DateTime HireDate { get; set; } = DateTime.Today;

        public int DepartmentId { get; set; }
        public Department Department { get; set; }

        public string JobTitle { get; set; }
        public decimal Salary { get; set; }
        public EmploymentStatus Status { get; set; }
    }

    public enum EmploymentStatus
    {
        Active,
        OnLeave,
        Terminated
    }
}
