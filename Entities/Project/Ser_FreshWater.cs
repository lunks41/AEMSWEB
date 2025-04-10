using System.ComponentModel.DataAnnotations;

namespace AMESWEB.Entities.Project
{
    public class Ser_FreshWater
    {
        public int FreshWaterId { get; set; }
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
        public string? Remarks { get; set; } = string.Empty;
        public short CreateById { get; set; }
        public DateTime CreateDate { get; set; } = DateTime.Now;
        public short? EditById { get; set; }
        public DateTime? EditDate { get; set; }
        public byte EditVersion { get; set; } = 0;

        // Fresh water specific fields
        public DateTime? DateOfSupply { get; set; }

        public string? ReceiptNumber { get; set; }
        public decimal? Distance { get; set; }
        public decimal Quantity { get; set; } = 0M;
        public short GLId { get; set; }
        public string? GlName { get; set; } = string.Empty;
        public short ChargeId { get; set; }
        public string? ChargeName { get; set; } = string.Empty;
        public short? BargeNameLookupId { get; set; }
        public string? BargeName { get; set; } = string.Empty;
        public string? BargeOperatorName { get; set; }
        public bool? IsVesselCallingLocation { get; set; }
        public short StatusId { get; set; }
        public string? StatusName { get; set; } = string.Empty;
        public short? UomId { get; set; }
        public string? UomName { get; set; } = string.Empty;

        // Additional fields for UI display
        public string? CreateBy { get; set; } = string.Empty;

        public string? EditBy { get; set; } = string.Empty;
    }
}