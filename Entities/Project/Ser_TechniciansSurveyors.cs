using System.ComponentModel.DataAnnotations;

namespace AMESWEB.Entities.Project
{
    public class Ser_TechniciansSurveyors
    {
        [Key]
        public long TechniciansSurveyorsId { get; set; }

        public byte CompanyId { get; set; }
        public long JobOrderId { get; set; }
        public string JobOrderNo { get; set; } // Nullable
        public short TaskId { get; set; }
        public long? DebitNoteId { get; set; } // Nullable
        public string DebitNoteNo { get; set; } // Nullable
        public decimal TotAmt { get; set; } = 0m;
        public decimal GstAmt { get; set; } = 0m;
        public decimal TotAmtAftGst { get; set; } = 0m;

        public DateTime? ArrivalDatetime { get; set; } // Nullable
        public DateTime? DepartureDatetime { get; set; } // Nullable
        public string RepresentativeName { get; set; } // Not nullable
        public short GLId { get; set; }
        public short ChargeId { get; set; }
        public int OffshorePass { get; set; }
        public string CompanyInfo { get; set; } // Not nullable
        public string NatureOfAttendance { get; set; } // Not nullable
        public short StatusId { get; set; }
        public decimal? Quantity { get; set; } // Nullable
        public short? UomId { get; set; } // Nullable
        public DateTime? Embarked { get; set; } // Nullable
        public DateTime? Disembarked { get; set; } // Nullable
        public string Remarks { get; set; } = string.Empty; // Default empty
        public short CreateById { get; set; }
        public DateTime CreateDate { get; set; } = DateTime.Now; // Default current date
        public short? EditById { get; set; } // Nullable
        public DateTime? EditDate { get; set; } // Nullable
        public byte EditVersion { get; set; }
    }
}