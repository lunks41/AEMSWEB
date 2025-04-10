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
        public byte CompanyId { get; set; }
        public long JobOrderId { get; set; }
        public string JobOrderNo { get; set; } = string.Empty;
        public short TaskId { get; set; }
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

        // Crew specific fields
        public string CrewName { get; set; } = string.Empty;
        public DateTime SignOffDate { get; set; }
        public string? Position { get; set; }
        public short GLId { get; set; }
        public string? GlName { get; set; } = string.Empty;
        public short? GenderId { get; set; }
        public string? GenderName { get; set; } = string.Empty;
        public string Nationality { get; set; } = string.Empty;
        public short? VisaTypeId { get; set; }
        public string? VisaTypeName { get; set; } = string.Empty;
        public string? TicketNo { get; set; }
        public string? Clearing { get; set; }
        public string? TransportName { get; set; }
        public string HotelName { get; set; } = string.Empty;
        public short StatusId { get; set; }
        public string? StatusName { get; set; } = string.Empty;
        public string FlightDetails { get; set; } = string.Empty;
        public string RankId { get; set; } = string.Empty;
        public string? RankName { get; set; } = string.Empty;
        public int? ChargeId { get; set; }
        public string? ChargeName { get; set; } = string.Empty;

        // Additional fields specific to sign-off
        public DateTime? SignOnDate { get; set; }
        public long? CrewSignOnId { get; set; }
        public string? ReasonForSignOff { get; set; }
        public string? ReplacementCrewName { get; set; }
        public string? DepartureDetails { get; set; }

        // Additional fields for UI display
        public string? CreateBy { get; set; } = string.Empty;
        public string? EditBy { get; set; } = string.Empty;
    }
}