namespace AMESWEB.Areas.Project.Models
{
    public class SaveThirdPartyViewModel
    {
        public ThirdPartyViewModel thirdParty { get; set; }
        public string? companyId { get; set; }
    }

    public class ThirdPartyViewModelCount
    {
        public Int16 responseCode { get; set; }
        public string? responseMessage { get; set; }
        public Int64 totalRecords { get; set; }
        public List<ThirdPartyViewModel> data { get; set; }
    }

    public class ThirdPartyViewModel
    {
        public long ThirdPartyId { get; set; }
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
        
        // Third party supply specific fields
        public decimal Quantity { get; set; } = 0M;
        public string? SupplierName { get; set; }
        public short GLId { get; set; }
        public string? GlName { get; set; } = string.Empty;
        public short ChargeId { get; set; }
        public string? ChargeName { get; set; } = string.Empty;
        public short StatusId { get; set; }
        public string? StatusName { get; set; } = string.Empty;
        public int SupplierId { get; set; }
        public string? SupplierMobileNumber { get; set; }
        public short UomId { get; set; }
        public string? UomName { get; set; } = string.Empty;
        public DateTime? DateDelivered { get; set; }
        
        // Additional fields for UI display
        public string? CreateBy { get; set; } = string.Empty;
        public string? EditBy { get; set; } = string.Empty;
    }
}