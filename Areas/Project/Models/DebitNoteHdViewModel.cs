namespace AMESWEB.Areas.Project.Models
{
    public class SaveDebitNoteHdViewModel
    {
        public DebitNoteHdViewModel debitNoteHd { get; set; }
        public string? companyId { get; set; }
    }

    public class DebitNoteHdViewModelCount
    {
        public Int16 responseCode { get; set; }
        public string? responseMessage { get; set; }
        public Int64 totalRecords { get; set; }
        public List<DebitNoteHdViewModel> data { get; set; }
    }

    public class DebitNoteHdViewModel
    {
        public byte CompanyId { get; set; }
        public long DebitNoteId { get; set; }
        public string DebitNoteNo { get; set; } = string.Empty;
        public DateTime DebitNoteDate { get; set; }
        public long JobOrderId { get; set; }
        public string JobOrderNo { get; set; } = string.Empty;
        public short TaskId { get; set; }
        public string? TaskName { get; set; } = string.Empty;
        public long ServiceId { get; set; }
        public string? ServiceName { get; set; } = string.Empty;
        public short ChargeId { get; set; }
        public string? ChargeName { get; set; } = string.Empty;
        public byte CurrencyId { get; set; }
        public string? CurrencyName { get; set; } = string.Empty;
        public decimal ExhRate { get; set; } = 0M;
        public decimal TotAmt { get; set; } = 0M;
        public decimal GstAmt { get; set; } = 0M;
        public decimal TotAftGstAmt { get; set; } = 0M;
        public short GLId { get; set; }
        public string? GlName { get; set; } = string.Empty;
        public decimal TaxableAmt { get; set; } = 0M;
        public decimal NonTaxableAmt { get; set; } = 0M;
        public byte EditVersion { get; set; } = 0;
        
        // Additional UI display fields
        public List<DebitNoteDtViewModel>? DebitNoteDetails { get; set; }
    }
}