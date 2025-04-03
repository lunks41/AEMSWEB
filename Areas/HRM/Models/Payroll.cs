using System.ComponentModel.DataAnnotations;

namespace AMESWEB.Areas.HRM.Models
{
    public class Payroll
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        [Required]
        public DateTime PayPeriodStart { get; set; }
        [Required]
        public DateTime PayPeriodEnd { get; set; }
        public decimal GrossSalary { get; set; }
        public decimal Deductions { get; set; }
        public decimal NetSalary { get; set; }

        public Employee Employee { get; set; }
    }
}
