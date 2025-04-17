using System.ComponentModel.DataAnnotations;

namespace AMESWEB.Entities.Project
{
    public class Ser_ConsignmentImport
    {
        [Key]
        
            public long ConsignmentImportId { get; set; }
            public byte CompanyId { get; set; }
            public long JobOrderId { get; set; }
            public string JobOrderNo { get; set; }
            public short TaskId { get; set; }
            public short ChargeId { get; set; }
            public short GLId { get; set; }
            public string AWBNo { get; set; }
            public short CargoTypeId { get; set; }
            public short UomId { get; set; }
            public short? ModeId { get; set; } // Nullable
            public short TypeId { get; set; }
            public short? LocationLookupId { get; set; } // Nullable
            public short? NoOfPcs { get; set; } // Nullable
            public decimal Weight { get; set; } = 0; // Default value
            public string? PickupLocation { get; set; } // Nullable
            public string? DeliveryLocation { get; set; } // Nullable
            public string? ClearedBy { get; set; } // Nullable
            public string? BillEntryNo { get; set; } // Nullable
            public string? DeclarationNo { get; set; } // Nullable
            public DateTime? DateReceived { get; set; } // Nullable
            public DateTime? DateDelivered { get; set; } // Nullable
            public string? ReferenceNo { get; set; } // Nullable
            public bool? IsRefundedExpected { get; set; } // Nullable
            public decimal? AmountDeposited { get; set; } // Nullable
            public string? RefundInstrumentNo { get; set; } // Nullable
            public bool? IsRefundedReceived { get; set; } // Nullable
            public decimal RefundAmt { get; set; } = 0; // Default value
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