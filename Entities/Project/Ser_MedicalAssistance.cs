using System.ComponentModel.DataAnnotations;

namespace AMESWEB.Entities.Project
{
    public class Ser_MedicalAssistance
    {
        [Key]
        public long MedicalAssistanceId { get; set; }

        public byte CompanyId { get; set; }
        public long JobOrderId { get; set; }
        public string JobOrderNo { get; set; } // Nullable
        public short TaskId { get; set; }
        public long? DebitNoteId { get; set; } // Nullable
        public string DebitNoteNo { get; set; } // Nullable
        public decimal TotAmt { get; set; } = 0m;
        public decimal GstAmt { get; set; } = 0m;
        public decimal TotAmtAftGst { get; set; } = 0m;
        public string Remarks { get; set; } = string.Empty; // Default value
        public short CreateById { get; set; }
        public DateTime CreateDate { get; set; } = DateTime.Now; // Default value
        public short? EditById { get; set; } // Nullable
        public DateTime? EditDate { get; set; } // Nullable
        public byte EditVersion { get; set; }
        public DateTime MedicalAssistanceDate { get; set; }
        public string Rank { get; set; } // Not nullable
        public string PersonName { get; set; } // Not nullable
        public string DoctorName { get; set; } // Nullable
        public string ClinicName { get; set; } // Not nullable
        public string MobileNumber { get; set; } // Nullable
        public short GLId { get; set; }
        public short ChargeId { get; set; }
        public string Nationality { get; set; } // Not nullable
        public string Reason { get; set; } // Nullable
        public DateTime MedicalAssistanceDateInGmt { get; set; }
        public short StatusId { get; set; }
        public short VisaId { get; set; }
        public decimal? Quantity { get; set; } // Nullable
        public short? UomId { get; set; } // Nullable
        public DateTime? AdmittedDate { get; set; } // Nullable
        public DateTime? DischargedDate { get; set; } // Nullable
    }
}