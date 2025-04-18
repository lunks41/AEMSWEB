﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AMESWEB.Entities.Project
{
    public class Ser_LandingItems
    {
        [Key]
        public long LandingItemId { get; set; }

        public DateTime LandingDate { get; set; }
        public byte CompanyId { get; set; }
        public long JobOrderId { get; set; }
        public string JobOrderNo { get; set; } = string.Empty; // Default value as per SQL
        public short TaskId { get; set; }
        public short GLId { get; set; }
        public short ChargeId { get; set; }
        public string Name { get; set; } = string.Empty; // Not Nullable
        public decimal Quantity { get; set; } = 0; // Default value
        public decimal Weight { get; set; } = 0; // Default value
        public short LandingTypeId { get; set; }
        public string LocationName { get; set; } = string.Empty; // Not Nullable
        public short UomId { get; set; }
        public DateTime? ReturnDate { get; set; } // Nullable
        public short StatusId { get; set; }
        public string Remarks { get; set; } = string.Empty; // Default value
        public long? DebitNoteId { get; set; } // Nullable
        public string? DebitNoteNo { get; set; } // Nullable
        public decimal TotAmt { get; set; } = 0; // Default value
        public decimal GstAmt { get; set; } = 0; // Default value
        public decimal TotAmtAftGst { get; set; } = 0; // Default value
        public short CreateById { get; set; }
        public DateTime CreateDate { get; set; } = DateTime.Now; // Default value
        public short? EditById { get; set; } // Nullable
        public DateTime? EditDate { get; set; } // Nullable
        public byte EditVersion { get; set; }
    }
}