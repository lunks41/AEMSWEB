namespace AMESWEB.Areas.Project.Models
{
    public class SaveCrewSignOffViewModel
    {
        public CrewSignOffViewModel crewSignOff { get; set; }
        public string? companyId { get; set; }
    }

    public class CrewSignOffViewModelCount
    {
        public Int16 responseCode { get; set; }
        public string? responseMessage { get; set; }
        public Int64 totalRecords { get; set; }
        public List<CrewSignOffViewModel> data { get; set; }
    }

    public class CrewSignOffViewModel
    {
        public long CrewSignOffId { get; set; }
        public DateTime Date { get; set; }
        public byte CompanyId { get; set; }
        public long JobOrderId { get; set; }
        public string JobOrderNo { get; set; }
        public short TaskId { get; set; }
        public short ChargeId { get; set; }
        public string? ChargeName { get; set; } = string.Empty;
        public short GLId { get; set; }
        public string? GlName { get; set; } = string.Empty;
        public short VisaTypeId { get; set; }
        public string CrewName { get; set; }
        public short? GenderId { get; set; }
        public string Nationality { get; set; }
        public short RankId { get; set; }
        public string? RankName { get; set; } = string.Empty;
        public string FlightDetails { get; set; }
        public string HotelName { get; set; }
        public string? TicketNo { get; set; }
        public string? TransportName { get; set; }
        public string? Clearing { get; set; }
        public short StatusId { get; set; }
        public string? StatusName { get; set; } = string.Empty;
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
        public string? CreateBy { get; set; } = string.Empty;
        public string? EditBy { get; set; } = string.Empty;
    }
}