using System.ComponentModel.DataAnnotations;

namespace AMESWEB.Areas.PettyCash.Models
{
    // Models/PettyCashEntry.cs
    public enum ApprovalStatus
    {
        Pending,
        Approved,
        Rejected
    }

    public class PettyCashEntry
    {
        public int Id { get; set; }

        [Required]
        public DateTime TransactionDate { get; set; } = DateTime.Now;

        [Required]
        [StringLength(255)]
        public string Description { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Amount { get; set; }

        [Required]
        [StringLength(50)]
        public string Category { get; set; }

        [Required]
        public string EmployeeId { get; set; }

        [Required]
        public string RequestedBy { get; set; }

        public string ApprovedBy { get; set; }
        public DateTime? ApprovalDate { get; set; }
        public ApprovalStatus Status { get; set; } = ApprovalStatus.Pending;
        public string Comments { get; set; }
    }
}
