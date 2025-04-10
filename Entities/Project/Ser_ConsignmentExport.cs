using System.ComponentModel.DataAnnotations;

namespace AMESWEB.Entities.Project
{
    public class Ser_ConsignmentExport
    {
        public long ConsignmentExportId { get; set; }
        public byte CompanyId { get; set; }
        public long JobOrderId { get; set; }
        public string JobOrderNo { get; set; } = string.Empty;
        public short TaskId { get; set; }
        public string? TaskName { get; set; } = string.Empty;
        public long? DebitNoteId { get; set; }
        public string? DebitNoteNo { get; set; } = string.Empty;
        public decimal TotAmt { get; set; } = 0M;
        public decimal GstAmt { get; set; } = 0M;
        public decimal TotAmtAftGst { get; set; } = 0M;

        // Consignment specific fields
        public long? ConsignmentNo { get; set; }

        public string? AWBNumber { get; set; }
        public string? DeclarationNumber { get; set; }
        public DateTime? BookingDate { get; set; }
        public decimal Weight { get; set; } = 0M;
        public string? PickupLocation { get; set; }
        public string? DeliveryLocation { get; set; }
        public short GLId { get; set; }
        public string? GlName { get; set; } = string.Empty;
        public short ChargeId { get; set; }
        public string? ChargeName { get; set; } = string.Empty;
        public int? SupplierId { get; set; }
        public string? SupplierName { get; set; } = string.Empty;
        public string? ReferenceNumber { get; set; }
        public string? BillEntryNumber { get; set; }
        public decimal? Quantity { get; set; } = 0M;
        public string? PickupLocationAddress { get; set; }
        public string? DeliveryLocationAddress { get; set; }
        public short? CargoType { get; set; }
        public string? CargoTypeName { get; set; } = string.Empty;
        public int? NoOfPcs { get; set; }
        public DateTime? DateReceived { get; set; }
        public DateTime? DateDelivered { get; set; }
        public short StatusId { get; set; }
        public string? StatusName { get; set; } = string.Empty;
        public short? ModeId { get; set; }
        public string? ModeName { get; set; } = string.Empty;
        public string? ClearedBy { get; set; }
        public string? TransporterName { get; set; }

        // Deposit and refund fields
        public bool? IsDepositRefundReceived { get; set; }

        public decimal? AmountDeposited { get; set; }
        public string? RefundInstrumentNumber { get; set; }
        public bool? IsWarehouseCharges { get; set; }
        public short? LocationLookupId { get; set; }
        public string? LocationName { get; set; } = string.Empty;
        public short TypeId { get; set; }
        public string? TypeName { get; set; } = string.Empty;
        public decimal? RefundAmt { get; set; } = 0M;
        public short? UomId { get; set; }
        public string? UomName { get; set; } = string.Empty;
        public string? HawbNumber { get; set; }
        public bool IsRefundedReceived { get; set; }
        public bool IsRefundedExpected { get; set; }

        // Common fields
        public string? Remarks { get; set; } = string.Empty;

        public short CreateById { get; set; }
        public DateTime CreateDate { get; set; } = DateTime.Now;
        public short? EditById { get; set; }
        public DateTime? EditDate { get; set; }
        public byte EditVersion { get; set; } = 0;

        // Additional fields for UI display
        public string? CreateBy { get; set; } = string.Empty;

        public string? EditBy { get; set; } = string.Empty;
    }
}