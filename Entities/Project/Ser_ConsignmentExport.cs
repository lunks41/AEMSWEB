using System.ComponentModel.DataAnnotations;

namespace AMESWEB.Entities.Project
{
    public class Ser_ConsignmentExport
    {
        [Key]
        public long ConsignmentExportId { get; set; }

        public byte CompanyId { get; set; }
        public long JobOrderId { get; set; }
        public string JobOrderNo { get; set; } // Nullable
        public short TaskId { get; set; }
        public long? DebitNoteId { get; set; } // Nullable
        public string DebitNoteNo { get; set; } // Nullable
        public decimal TotAmt { get; set; } = 0m;
        public decimal GstAmt { get; set; } = 0m;
        public decimal TotAmtAftGst { get; set; } = 0m;
        public long? ConsignmentNo { get; set; } // Nullable
        public string AWBNumber { get; set; } // Nullable
        public string DeclarationNumber { get; set; } // Nullable
        public DateTime? BookingDate { get; set; } // Nullable
        public decimal Weight { get; set; }
        public string PickupLocation { get; set; } // Nullable
        public string DeliveryLocation { get; set; } // Nullable
        public short GLId { get; set; }
        public short ChargeId { get; set; }
        public int? SupplierId { get; set; } // Nullable
        public string ReferenceNumber { get; set; } // Nullable
        public string BillEntryNumber { get; set; } // Nullable
        public decimal? Quantity { get; set; } = 0m; // Nullable and default value
        public string PickupLocationAddress { get; set; } // Nullable
        public string DeliveryLocationAddress { get; set; } // Nullable
        public short? CargoType { get; set; } // Nullable
        public int? NoOfPcs { get; set; } // Nullable
        public DateTime? DateReceived { get; set; } // Nullable
        public DateTime? DateDelivered { get; set; } // Nullable
        public short StatusId { get; set; }
        public short? ModeId { get; set; } // Nullable
        public string ClearedBy { get; set; } // Nullable
        public string TransporterName { get; set; } // Nullable
        public bool? IsDepositRefundReceived { get; set; } // Nullable
        public decimal? AmountDeposited { get; set; } // Nullable
        public string RefundInstrumentNumber { get; set; } // Nullable
        public bool? IsWarehouseCharges { get; set; } // Nullable
        public short? LocationLookupId { get; set; } // Nullable
        public short TypeId { get; set; }
        public decimal? RefundAmt { get; set; } = 0m; // Nullable and default value
        public short? UomId { get; set; } // Nullable
        public string HawbNumber { get; set; } // Nullable
        public bool IsRefundedReceived { get; set; }
        public bool IsRefundedExpected { get; set; }
        public string Remarks { get; set; } = string.Empty; // Default value
        public short CreateById { get; set; }
        public DateTime CreateDate { get; set; } = DateTime.Now; // Default current date
        public short? EditById { get; set; } // Nullable
        public DateTime? EditDate { get; set; } // Nullable
        public byte EditVersion { get; set; }
    }
}