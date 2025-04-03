using System.ComponentModel.DataAnnotations;

namespace AMESWEB.Models
{
    public class AdmModule
    {
        [Key]
        public byte ModuleId { get; set; }

        public string? ModuleCode { get; set; }
        public string? ModuleName { get; set; }
        public byte SeqNo { get; set; }
        public string? Remarks { get; set; }
        public bool IsActive { get; set; }
        public Int16 CreateById { get; set; }
        public DateTime? CreateDate { get; set; } = DateTime.Now;
        public Int16? EditById { get; set; }
        public DateTime? EditDate { get; set; }

        // Navigation property
        public ICollection<AdmTransaction> Transactions { get; set; }
    }
}