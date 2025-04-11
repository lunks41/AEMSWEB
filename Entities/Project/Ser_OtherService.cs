using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AMESWEB.Entities.Project
{
    public class Ser_OtherService
    {
        [Key]
        public long OtherServiceId { get; set; }

        public byte CompanyId { get; set; }
        public long JobOrderId { get; set; }
        public string JobOrderNo { get; set; } // Nullable
        public short TaskId { get; set; }
        public long? DebitNoteId { get; set; } // Nullable
        public string DebitNoteNo { get; set; } // Nullable
        public decimal TotAmt { get; set; } = 0m;
        public decimal GstAmt { get; set; } = 0m;
        public decimal TotAmtAftGst { get; set; } = 0m;
        public DateTime OtherServiceDate { get; set; }
        public string ServiceProvider { get; set; } // Not nullable
        public decimal? Amount { get; set; } // Nullable
        public short GLId { get; set; }
        public decimal Quantity { get; set; } = 0m; // Default zero
        public short ChargeId { get; set; }
        public short StatusId { get; set; }
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