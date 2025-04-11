using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AMESWEB.Entities.Project
{
    [Table("Ser_CrewMiscellaneous")]
    public class Ser_CrewMiscellaneous
    {
        public byte CompanyId { get; set; }

        [Key]
        public long CrewMiscellaneousId { get; set; }

        public long JobOrderId { get; set; }
        public string JobOrderNo { get; set; } // Nullable
        public short TaskId { get; set; }
        public long? DebitNoteId { get; set; } // Nullable
        public string DebitNoteNo { get; set; } // Nullable
        public decimal TotAmt { get; set; } = 0m;
        public decimal GstAmt { get; set; } = 0m;
        public decimal TotAmtAftGst { get; set; } = 0m;
        public string MiscDescription { get; set; } // Not nullable
        public decimal MiscAmount { get; set; } = 0m; // Default value
        public short GLId { get; set; }
        public decimal Quantity { get; set; } = 0m; // Default value
        public int? ChargeId { get; set; } // Nullable
        public string Remarks { get; set; } = string.Empty; // Default value
        public short CreateById { get; set; }
        public DateTime CreateDate { get; set; } = DateTime.Now; // Default current date
        public short? EditById { get; set; } // Nullable
        public DateTime? EditDate { get; set; } // Nullable
        public byte EditVersion { get; set; }
    }
}