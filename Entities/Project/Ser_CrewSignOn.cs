using System.ComponentModel.DataAnnotations;

namespace AMESWEB.Entities.Project
{
    public class Ser_CrewSignOn
    {
        [Key]
        public long CrewSignOnId { get; set; }
        public DateTime Date { get; set; }
        public byte CompanyId { get; set; }
        public long JobOrderId { get; set; }
        public string JobOrderNo { get; set; }
        public short TaskId { get; set; }
        public short ChargeId { get; set; }
        public short GLId { get; set; }
        public short VisaTypeId { get; set; }
        public string CrewName { get; set; }
        public short? GenderId { get; set; }
        public string Nationality { get; set; }
        public short RankId { get; set; }
        public string FlightDetails { get; set; }
        public string HotelName { get; set; }
        public string? TicketNo { get; set; }
        public string? TransportName { get; set; }
        public string? Clearing { get; set; }
        public short StatusId { get; set; }
        public long? DebitNoteId { get; set; }
        public string? DebitNoteNo { get; set; }
        public decimal TotAmt { get; set; }
        public decimal GstAmt { get; set; }
        public decimal TotAmtAftGst { get; set; }
        public string Remarks { get; set; } = string.Empty;
        public short CreateById { get; set; }
        public DateTime CreateDate { get; set; } = DateTime.Now;
        public short? EditById { get; set; }
        public DateTime? EditDate { get; set; }
        public byte EditVersion { get; set; }
    }
}