namespace AMESWEB.Areas.Project.Models
{
    public class SaveMedicalAssistanceViewModel
    {
        public MedicalAssistanceViewModel medicalAssistance { get; set; }
        public string? companyId { get; set; }
    }

    public class MedicalAssistanceViewModelCount
    {
        public Int16 responseCode { get; set; }
        public string? responseMessage { get; set; }
        public Int64 totalRecords { get; set; }
        public List<MedicalAssistanceViewModel> data { get; set; }
    }

    public class MedicalAssistanceViewModel
    {
        public long MedicalAssistanceId { get; set; }
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
        
        // Medical assistance specific fields
        public DateTime MedicalAssistanceDate { get; set; }
        public DateTime MedicalAssistanceDateInGmt { get; set; }
        public string Rank { get; set; } = string.Empty;
        public string PersonName { get; set; } = string.Empty;
        public string? DoctorName { get; set; }
        public string ClinicName { get; set; } = string.Empty;
        public string? MobileNumber { get; set; }
        public short GLId { get; set; }
        public string? GlName { get; set; } = string.Empty;
        public short ChargeId { get; set; }
        public string? ChargeName { get; set; } = string.Empty;
        public string Nationality { get; set; } = string.Empty;
        public string? Reason { get; set; }
        public short StatusId { get; set; }
        public string? StatusName { get; set; } = string.Empty;
        public short VisaId { get; set; }
        public string? VisaName { get; set; } = string.Empty;
        public decimal? Quantity { get; set; }
        public short? UomId { get; set; }
        public string? UomName { get; set; } = string.Empty;
        public DateTime? AdmittedDate { get; set; }
        public DateTime? DischargedDate { get; set; }
        
        // Additional fields for UI display
        public string? CreateBy { get; set; } = string.Empty;
        public string? EditBy { get; set; } = string.Empty;
    }
}