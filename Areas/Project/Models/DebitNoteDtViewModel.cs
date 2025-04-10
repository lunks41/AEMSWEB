namespace AMESWEB.Areas.Project.Models
{
    public class SaveDebitNoteDtViewModel
    {
        public DebitNoteDtViewModel debitNoteDt { get; set; }
        public string? companyId { get; set; }
    }

    public class DebitNoteDtViewModelCount
    {
        public Int16 responseCode { get; set; }
        public string? responseMessage { get; set; }
        public Int64 totalRecords { get; set; }
        public List<DebitNoteDtViewModel> data { get; set; }
    }

    public class DebitNoteDtViewModel
    {
        public byte CompanyId { get; set; }
        public long DebitNoteId { get; set; }
        public string DebitNoteNo { get; set; } = string.Empty;
        public short ItemNo { get; set; }
        public short TaskId { get; set; }
        public string? TaskName { get; set; } = string.Empty;
        public short ChargeId { get; set; }
        public string? ChargeName { get; set; } = string.Empty;
        public short GlId { get; set; }
        public string? GlName { get; set; } = string.Empty;
        public decimal Qty { get; set; } = 0M;
        public decimal UnitPrice { get; set; } = 0M;
        public decimal Amt { get; set; } = 0M;
        public decimal TotAmt { get; set; } = 0M;
        public byte GstId { get; set; }
        public string? GstName { get; set; } = string.Empty;
        public decimal GstPercentage { get; set; } = 0M;
        public decimal GstAmt { get; set; } = 0M;
        public decimal TotAftGstAmt { get; set; } = 0M;
        public string Remarks { get; set; } = string.Empty;
        public byte EditVersion { get; set; } = 0;
    }
}