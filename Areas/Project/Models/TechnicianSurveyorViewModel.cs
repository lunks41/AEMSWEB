namespace AMESWEB.Areas.Project.Models
{
    public class SaveTechnicianSurveyorViewModel
    {
        public TechnicianSurveyorViewModel technicianSurveyor { get; set; }
        public string? companyId { get; set; }
    }

    public class TechnicianSurveyorViewModelCount
    {
        public Int16 responseCode { get; set; }
        public string? responseMessage { get; set; }
        public Int64 totalRecords { get; set; }
        public List<TechnicianSurveyorViewModel> data { get; set; }
    }

    public class TechnicianSurveyorViewModel
    {
        public long TechnicianSurveyorId { get; set; }
        public byte CompanyId { get; set; }
        public long JobOrderId { get; set; }
        public string JobOrderNo { get; set; } = string.Empty; // Default value as per SQL
        public short TaskId { get; set; }
        public short GLId { get; set; }
        public short ChargeId { get; set; }
        public string Name { get; set; } = string.Empty; // Not Nullable
        public decimal Quantity { get; set; } // Not Nullable
        public short UomId { get; set; }
        public string NatureOfAttendance { get; set; } = string.Empty; // Not Nullable
        public string CompanyInfo { get; set; } = string.Empty; // Not Nullable
        public short PassTypeId { get; set; }
        public DateTime? Embarked { get; set; } // Nullable
        public DateTime? Disembarked { get; set; } // Nullable
        public string? PortRequestNo { get; set; } // Nullable
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
        public string? CreateBy { get; set; } = string.Empty;

        public string? EditBy { get; set; } = string.Empty;
    }
}