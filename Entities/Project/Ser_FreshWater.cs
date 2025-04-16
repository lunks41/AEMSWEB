using System.ComponentModel.DataAnnotations;

namespace AMESWEB.Entities.Project
{
    public class Ser_FreshWater
    {
        [Key]
        public int FreshWaterId { get; set; }

        public byte CompanyId { get; set; }
        public long JobOrderId { get; set; }
        public string JobOrderNo { get; set; } // Nullable
        public short TaskId { get; set; }
        public long? DebitNoteId { get; set; } // Nullable
        public string DebitNoteNo { get; set; } // Nullable
        public decimal TotAmt { get; set; } = 0m;
        public decimal GstAmt { get; set; } = 0m;
        public decimal TotAmtAftGst { get; set; } = 0m;
        public DateTime? DateOf { get; set; } // Nullable
        public string ReceiptNumber { get; set; } // Nullable
        public decimal? Distance { get; set; } // Nullable
        public decimal Quantity { get; set; } = 0m; // Default zero
        public short GLId { get; set; }
        public short ChargeId { get; set; }
        public short? BargeNameLookupId { get; set; } // Nullable
        public string BargeOperatorName { get; set; } // Nullable
        public bool? IsVesselCallingLocation { get; set; } // Nullable
        public short StatusId { get; set; }
        public short? UomId { get; set; } // Nullable
        public string Remarks { get; set; } = string.Empty; // Default empty
        public short CreateById { get; set; }
        public DateTime CreateDate { get; set; } = DateTime.Now; // Default current date
        public short? EditById { get; set; } // Nullable
        public DateTime? EditDate { get; set; } // Nullable
        public byte EditVersion { get; set; }
    }
}