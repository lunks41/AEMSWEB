﻿using System.ComponentModel.DataAnnotations;

namespace AMESWEB.Entities.Project
{
    public class Ser_ThirdPartySupply
    {
        [Key]
        public long ThirdPartySupplyId { get; set; }

        public byte CompanyId { get; set; }
        public long JobOrderId { get; set; }
        public string JobOrderNo { get; set; } // Nullable
        public short TaskId { get; set; }
        public long? DebitNoteId { get; set; } // Nullable
        public string DebitNoteNo { get; set; } // Nullable
        public decimal TotAmt { get; set; } = 0m;
        public decimal GstAmt { get; set; } = 0m;
        public decimal TotAmtAftGst { get; set; } = 0m;

        public byte EditVersion { get; set; }
        public decimal Quantity { get; set; } = 0m; // Default zero
        public string SupplierName { get; set; } // Nullable
        public short GLId { get; set; }
        public short ChargeId { get; set; }
        public short StatusId { get; set; }
        public int SupplierId { get; set; }
        public string SupplierMobileNumber { get; set; } // Nullable
        public short UomId { get; set; }
        public DateTime? DateDelivered { get; set; } // Nullable
        public string Remarks { get; set; } = string.Empty; // Default empty
        public short CreateById { get; set; }
        public DateTime CreateDate { get; set; } = DateTime.Now; // Default current date
        public short? EditById { get; set; } // Nullable
        public DateTime? EditDate { get; set; } // Nullable
    }
}