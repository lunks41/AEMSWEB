using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AMESWEB.Entities.Project
{
    [Table("Ser_CrewMiscellaneous")]
    public class Ser_CrewMiscellaneous
    {
        [Key]
        public Int64 CrewMiscellaneousId { get; set; }
        public Int64 JobOrderId { get; set; }
        public string? Description { get; set; }
        public decimal Amount { get; set; }
        public string? Remarks { get; set; }
        public string? Reference { get; set; }
        public DateTime? TransactionDate { get; set; }
        public bool IsDeleted { get; set; }
        public short CompanyId { get; set; }
        public short CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public short? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}