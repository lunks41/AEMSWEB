namespace AMESWEB.Areas.Project.Models
{
    public class SaveLaunchServicesViewModel
    {
        public LaunchServicesViewModel launchService { get; set; }
        public string? companyId { get; set; }
    }

    public class LaunchServicesViewModelCount
    {
        public Int16 responseCode { get; set; }
        public string? responseMessage { get; set; }
        public Int64 totalRecords { get; set; }
        public List<LaunchServicesViewModel> data { get; set; }
    }

    public class LaunchServicesViewModel
    {
        public long LaunchServiceId { get; set; }
        public string LaunchServiceDate { get; set; }
        public byte CompanyId { get; set; }
        public long JobOrderId { get; set; }
        public string JobOrderNo { get; set; }
        public short TaskId { get; set; } = 2; // Default value
        public string? TaskName { get; set; }
        public short GLId { get; set; }
        public string? GLName { get; set; }
        public short ChargeId { get; set; }
        public string? ChargeName { get; set; }
        public short UomId { get; set; }
        public string? UomName { get; set; }
        public string AmeTally { get; set; }
        public string BoatopTally { get; set; }
        public decimal? DistanceFromJetty { get; set; }
        public string? LoadingTime { get; set; }
        public string? LeftJetty { get; set; }
        public string? AlongsideVessel { get; set; }
        public string? DepartedFromVessel { get; set; }
        public string? ArrivedAtJetty { get; set; }
        public decimal? LaunchWaitingTime { get; set; }
        public decimal? TimeDiff { get; set; }
        public decimal DistanceFromJettyToVessel { get; set; } = 0.00m; // Default value
        public decimal WeightOfCargoDelivered { get; set; } = 0.00m; // Default value
        public decimal WeightOfCargoLanded { get; set; } = 0.00m; // Default value
        public string BoatOperator { get; set; }
        public string Annexure { get; set; }
        public string InvoiceNo { get; set; }
        public short? PortId { get; set; }
        public short? BargeId { get; set; }
        public string? BargeName { get; set; }
        public short StatusId { get; set; }
        public string? StatusName { get; set; }
        public long? DebitNoteId { get; set; }
        public string DebitNoteNo { get; set; }
        public decimal TotAmt { get; set; } = 0.00m; // Default value
        public decimal GstAmt { get; set; } = 0.00m; // Default value
        public decimal TotAmtAftGst { get; set; } = 0.00m; // Default value
        public string Remarks { get; set; } = string.Empty; // Default value
        public short CreateById { get; set; }
        public DateTime CreateDate { get; set; } = DateTime.Now; // Default value
        public short? EditById { get; set; }
        public DateTime? EditDate { get; set; }
        public string? CreateBy { get; set; } = string.Empty;
        public string? EditBy { get; set; } = string.Empty;
        public byte EditVersion { get; set; }
    }
}