using System.ComponentModel.DataAnnotations;

namespace AMESWEB.Entities.Project
{
    public class Ser_CrewSignOn
    {
        [Key]
        public long CrewSignOnId { get; set; }

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
        public string CrewName { get; set; } // Not nullable
        public DateTime SignOnDate { get; set; }
        public string Position { get; set; } // Nullable
        public short GLId { get; set; }
        public short? GenderId { get; set; } // Nullable
        public string Nationality { get; set; } // Not nullable
        public short? VisaTypeId { get; set; } // Nullable
        public string TicketNo { get; set; } // Nullable
        public string Clearing { get; set; } // Nullable
        public string TransportName { get; set; } // Nullable
        public string HotelName { get; set; } // Not nullable
        public short StatusId { get; set; }
        public string FlightDetails { get; set; } // Not nullable
        public string RankId { get; set; } // Not nullable
        public int? ChargeId { get; set; } // Nullable
    }
}