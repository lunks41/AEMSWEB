namespace AMESWEB.Areas.Project.Models
{
    public class SaveCrewMiscellaneousViewModel
    {
        public CrewMiscellaneousViewModel crewMiscellaneous { get; set; }
        public string? companyId { get; set; }
    }

    public class CrewMiscellaneousViewModelCount
    {
        public Int16 responseCode { get; set; }
        public string? responseMessage { get; set; }
        public Int64 totalRecords { get; set; }
        public List<CrewMiscellaneousViewModel> data { get; set; }
    }

    public class CrewMiscellaneousViewModel
    {
        public long CrewMiscellaneousId { get; set; }
        public byte CompanyId { get; set; }
        public long JobOrderId { get; set; }
        public string JobOrderNo { get; set; } = string.Empty;
        public short TaskId { get; set; } = 1;
        public long? DebitNoteId { get; set; }
        public string? DebitNoteNo { get; set; } = string.Empty;
        public decimal TotAmt { get; set; } = 0M;
        public decimal GstAmt { get; set; } = 0M;
        public decimal TotAmtAftGst { get; set; } = 0M;
        public string MiscDescription { get; set; } = string.Empty;
        public decimal MiscAmount { get; set; } = 0M;
        public short GLId { get; set; }
        public string? GlName { get; set; } = string.Empty;
        public decimal Quantity { get; set; } = 0M;
        public int? ChargeId { get; set; }
        public string? ChargeName { get; set; } = string.Empty;
        public string Remarks { get; set; } = string.Empty;
        public short CreateById { get; set; }
        public DateTime CreateDate { get; set; } = DateTime.Now;
        public short? EditById { get; set; }
        public DateTime? EditDate { get; set; }
        public string? CreateBy { get; set; } = string.Empty;
        public string? EditBy { get; set; } = string.Empty;
        public byte EditVersion { get; set; } = 0;
    }
}