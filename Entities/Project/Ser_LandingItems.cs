using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AMESWEB.Entities.Project
{
    public class Ser_LandingItems
    {
        [Key]
        public long LandingItemId { get; set; }

        public byte CompanyId { get; set; }
        public long JobOrderId { get; set; }
        public string JobOrderNo { get; set; } // Nullable
        public short TaskId { get; set; }
        public long? DebitNoteId { get; set; } // Nullable
        public string DebitNoteNo { get; set; } // Nullable
        public decimal TotAmt { get; set; } = 0m;
        public decimal GstAmt { get; set; } = 0m;
        public decimal TotAmtAftGst { get; set; } = 0m;

        public DateTime LandingItemDate { get; set; }
        public string LandingItemName { get; set; } // Nullable
        public string Location { get; set; } // Not nullable
        public decimal? Quantity { get; set; } = 0m; // Default zero
        public decimal? Weight { get; set; } // Nullable
        public short GLId { get; set; }
        public short ChargeId { get; set; }
        public short StatusId { get; set; }
        public string ItemName { get; set; } // Not nullable
        public string LocationName { get; set; } // Not nullable
        public bool? IsDepositRefundReceived { get; set; } // Nullable
        public short? ForwardExportId { get; set; } // Nullable
        public short? UomId { get; set; } // Nullable
        public string Remarks { get; set; } = string.Empty; // Default empty
        public short CreateById { get; set; }

        [NotMapped]
        public DateTime CreateDate { get; set; } = DateTime.Now; // Default current date

        public short? EditById { get; set; } // Nullable
        public DateTime? EditDate { get; set; } // Nullable
        public byte EditVersion { get; set; }
    }
}