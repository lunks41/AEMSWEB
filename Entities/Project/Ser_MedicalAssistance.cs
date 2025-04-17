using System.ComponentModel.DataAnnotations;

namespace AMESWEB.Entities.Project
{
    public class Ser_MedicalAssistance
    {
        [Key]

        public long MedicalAssistanceId { get; set; }
        public DateTime Date { get; set; }
        public byte CompanyId { get; set; }
        public long JobOrderId { get; set; }
        public string JobOrderNo { get; set; }
        public short TaskId { get; set; }
        public short ChargeId { get; set; }
        public short GLId { get; set; }
        public short GenderId { get; set; }
        public string CrewName { get; set; }
        public string ClinicName { get; set; }
        public string? DoctorName { get; set; } // Nullable
        public string? MobileNumber { get; set; } // Nullable
        public string Nationality { get; set; }
        public short RankId { get; set; }
        public string? Reason { get; set; } // Nullable
        public DateTime? AdmittedDate { get; set; } // Nullable
        public DateTime? DischargedDate { get; set; } // Nullable
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

    }
}