namespace AMESWEB.Areas.Project.Models
{
    public class SaveOtherServiceViewModel
    {
        public OtherServiceViewModel otherService { get; set; }
        public string? companyId { get; set; }
    }

    public class OtherServiceViewModelCount
    {
        public Int16 responseCode { get; set; }
        public string? responseMessage { get; set; }
        public Int64 totalRecords { get; set; }
        public List<OtherServiceViewModel> data { get; set; }
    }

    public class OtherServiceViewModel
    {
        public long OtherServiceId { get; set; }
        public DateTime OtherServiceDate { get; set; }
        public byte CompanyId { get; set; }
        public long JobOrderId { get; set; }
        public string JobOrderNo { get; set; } = string.Empty; // Default value
        public short TaskId { get; set; }
        public short ChargeId { get; set; }
        public string? ChargeName { get; set; }
        public short GLId { get; set; }
        public string? GLName { get; set; }
        public string ServiceProvider { get; set; } = string.Empty;
        public decimal Quantity { get; set; } = 0; // Default value
        public decimal Amount { get; set; }
        public short UomId { get; set; }
        public short StatusId { get; set; }
        public long? DebitNoteId { get; set; } // Nullable
        public string? DebitNoteNo { get; set; } // Nullable
        public decimal TotAmt { get; set; } = 0; // Default value
        public decimal GstAmt { get; set; } = 0; // Default value
        public decimal TotAmtAftGst { get; set; } = 0; // Default value
        public string Remarks { get; set; } = string.Empty; // Default value
        public short CreateById { get; set; }
        public DateTime CreateDate { get; set; } = DateTime.Now; // Default value
        public short? EditById { get; set; } // Nullable
        public DateTime? EditDate { get; set; } // Nullable
        public byte EditVersion { get; set; }
        public string? CreateBy { get; set; } = string.Empty;
        public string? EditBy { get; set; } = string.Empty;
    }
}